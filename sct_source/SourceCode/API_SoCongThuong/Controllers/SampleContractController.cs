
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.SampleContractRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleContractController : ControllerBase
    {
        private SampleContractRepo _repoSampleContract;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public SampleContractController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoSampleContract = new SampleContractRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<SampleContractModel> model = new BaseModels<SampleContractModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<SampleContractModel, object> _orderByExpression = x => x.ProfileNumber; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<SampleContractModel, object>> _sortableFields = new Dictionary<string, Func<SampleContractModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProfileNumber", x => x.ProfileNumber },
                        { "RegistrantName", x => x.RegistrantName },
                        { "PhoneNumber", x => x.PhoneNumber },
                        { "BusinessName", x => x.BusinessName },
                        { "TaxCode", x => x.TaxCode },
                        { "BusinessPhoneNumber", x => x.BusinessPhoneNumber },
                        { "Address", x => x.Address },

                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<SampleContractModel> _data = _repoSampleContract._context.SampleContracts.Select(x => new SampleContractModel
                {
                    SampleContractId = x.SampleContractId,
                    SampleContractField = x.SampleContractField,
                    RegistrationTime = x.RegistrationTime,
                    ProfileNumber = x.ProfileNumber,
                    RegistrantName = x.RegistrantName,
                    PhoneNumber = x.PhoneNumber,
                    BusinessName = x.BusinessName,
                    TaxCode = x.TaxCode,
                    BusinessPhoneNumber = x.BusinessPhoneNumber,
                    Address = x.Address,
                    IsDel = x.IsDel,
                });
                _data = _data.Where(x => !x.IsDel);


                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>

                       x.ProfileNumber.ToLower().Contains(_keywordSearch) 
                       || x.RegistrantName.ToLower().Contains(_keywordSearch)
                       || x.PhoneNumber.Contains(_keywordSearch)
                       || x.BusinessName.ToLower().Contains(_keywordSearch)
                       || x.TaxCode.ToLower().Contains(_keywordSearch)
                       || x.BusinessPhoneNumber.Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                   ); 
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                     && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x =>
                                (x.RegistrationTime) >=
                                DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                    && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x =>
                               x.RegistrationTime <=
                                DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
                }
                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    //model.error = new ErrorModel()
                    //{
                    //    Code = ErrCode_Const.EXCEPTION_API,
                    //    Msg = "Không có dữ liệu"
                    //};
                    //return BadRequest(model);
                    return NotFound("Không có dữ liệu");
                }
                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else //Sắp xếp dữ liệu theo desc
                {
                    model.items = _data
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                model.status = 1;
                model.total = _countRows;
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<SampleContractModel> model = new BaseModels<SampleContractModel>();
            try
            {
                var result = _repoSampleContract.FindById(id);
                if (result != null)
                {
                    model.status = 1;
                    model.items = result.ToList();
                    return Ok(model);
                }

                else
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(model);
                }
            }
            catch (Exception ex)
            {

                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(SampleContract data)
        {
            BaseModels<SampleContract> model = new BaseModels<SampleContract>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                SampleContract? SaveData = _repoSampleContract._context.SampleContracts.Where(x => x.SampleContractId == data.SampleContractId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var Code = _repoSampleContract.findByProfileNumber(data.ProfileNumber, Guid.Parse(data.SampleContractId.ToString()));

                    if (Code)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Số hồ sơ đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.SAMPLE_CONTRACT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.SampleContractId = Guid.Parse(data.SampleContractId.ToString());
                    SaveData.SampleContractField = data.SampleContractField;
                    SaveData.RegistrationTime = data.RegistrationTime;
                    SaveData.ProfileNumber = data.ProfileNumber;
                    SaveData.RegistrantName = data.RegistrantName;
                    SaveData.PhoneNumber = data.PhoneNumber;
                    SaveData.BusinessName = data.BusinessName;
                    SaveData.TaxCode = data.TaxCode;
                    SaveData.BusinessPhoneNumber = data.BusinessPhoneNumber;
                    SaveData.Address = data.Address;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoSampleContract.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.SAMPLE_CONTRACT, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    model.status = 1;
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.SAMPLE_CONTRACT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return NotFound(model);
                }
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> create(SampleContract data)
        {
            BaseModels<SampleContract> model = new BaseModels<SampleContract>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var Code = _repoSampleContract.findByProfileNumber(data.ProfileNumber, null);
                SystemLog datalog = new SystemLog();
                if (Code)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Số hồ sơ đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.SAMPLE_CONTRACT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                SampleContract SaveData = new SampleContract();
                SaveData.SampleContractId = Guid.Parse(data.SampleContractId.ToString());
                SaveData.SampleContractField = data.SampleContractField;
                SaveData.RegistrationTime = data.RegistrationTime;
                SaveData.ProfileNumber = data.ProfileNumber;
                SaveData.RegistrantName = data.RegistrantName;
                SaveData.PhoneNumber = data.PhoneNumber;
                SaveData.BusinessName = data.BusinessName;
                SaveData.TaxCode = data.TaxCode;
                SaveData.BusinessPhoneNumber = data.BusinessPhoneNumber;
                SaveData.Address = data.Address;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoSampleContract.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.SAMPLE_CONTRACT, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                model.status = 1;
                return Ok(model);
            }
            catch (Exception ex)
            {

                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<SampleContract> model = new BaseModels<SampleContract>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                SampleContract DeleteData = new SampleContract();
                DeleteData.SampleContractId = id;
                DeleteData.IsDel = true;
                await _repoSampleContract.Delete(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.SAMPLE_CONTRACT, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                model.status = 1;
                return Ok(model);
            }
            catch (Exception ex)
            {

                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [Route("loadfield")]
        [HttpGet]
        public IActionResult LoadField()
        {
            BaseModels<Category> model = new BaseModels<Category>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<Category> _data = _repoSampleContract._context.Categories.Where(x => x.CategoryTypeCode == "ADMINISTRATIVE_PROCEDURE_FIELD" && x.IsAction == true);

                model.status = 1;
                model.items = _data.ToList();
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        private List<SampleContractModel> FindData([FromBody] QueryRequestBody query)
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            Func<SampleContractModel, object> _orderByExpression = x => x.ProfileNumber; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<SampleContractModel, object>> _sortableFields = new Dictionary<string, Func<SampleContractModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProfileNumber", x => x.ProfileNumber },
                        { "RegistrantName", x => x.RegistrantName },
                        { "PhoneNumber", x => x.PhoneNumber },
                        { "BusinessName", x => x.BusinessName },
                        { "TaxCode", x => x.TaxCode },
                        { "BusinessPhoneNumber", x => x.BusinessPhoneNumber },
                        { "Address", x => x.Address },

                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            //Cách 1 dùng entity
            IQueryable<SampleContractModel> _data = _repoSampleContract._context.SampleContracts.Select(x => new SampleContractModel
            {
                SampleContractId = x.SampleContractId,
                SampleContractField = x.SampleContractField,
                RegistrationTime = x.RegistrationTime,
                ProfileNumber = x.ProfileNumber,
                RegistrantName = x.RegistrantName,
                PhoneNumber = x.PhoneNumber,
                BusinessName = x.BusinessName,
                TaxCode = x.TaxCode,
                BusinessPhoneNumber = x.BusinessPhoneNumber,
                Address = x.Address,
                IsDel = x.IsDel,
            });
            _data = _data.Where(x => !x.IsDel);


            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>

                   x.ProfileNumber.ToLower().Contains(_keywordSearch)
                   || x.RegistrantName.ToLower().Contains(_keywordSearch)
                   || x.PhoneNumber.Contains(_keywordSearch)
                   || x.BusinessName.ToLower().Contains(_keywordSearch)
                   || x.TaxCode.ToLower().Contains(_keywordSearch)
                   || x.BusinessPhoneNumber.Contains(_keywordSearch)
                   || x.Address.ToLower().Contains(_keywordSearch)
               );
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                 && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                _data = _data.Where(x =>
                            (x.RegistrationTime) >=
                            DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                _data = _data.Where(x =>
                           x.RegistrationTime <=
                            DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
            }

            return _data.ToList();
        }

        [HttpPost("export")]
        public IActionResult ExportFile([FromBody] QueryRequestBody query)
        {
            //Query data
            var data = FindData(query);

            if (data == null || data.Count == 0)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachthongtinhopdongmau.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 8;
                    int row = 1;

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProfileNumber;
                            worksheet.Cell(index, 3).Value = item.RegistrantName;
                            worksheet.Cell(index, 4).Value = item.PhoneNumber;
                            worksheet.Cell(index, 5).Value = item.BusinessName;
                            worksheet.Cell(index, 6).Value = item.Address;
                            worksheet.Cell(index, 7).Value = item.TaxCode;
                            worksheet.Cell(index, 8).Value = item.BusinessPhoneNumber;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProfileNumber;
                            worksheet.Cell(index, 3).Value = item.RegistrantName;
                            worksheet.Cell(index, 4).Value = item.PhoneNumber;
                            worksheet.Cell(index, 5).Value = item.BusinessName;
                            worksheet.Cell(index, 6).Value = item.Address;
                            worksheet.Cell(index, 7).Value = item.TaxCode;
                            worksheet.Cell(index, 8).Value = item.BusinessPhoneNumber;
                            worksheet.Cell(index, 8).Value = item.PhoneNumber;
                            index++;
                            row++;
                        }
                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
