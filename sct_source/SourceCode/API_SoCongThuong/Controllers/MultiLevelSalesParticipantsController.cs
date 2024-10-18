
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.MultiLevelSalesParticipantsRepository;
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
    public class MultiLevelSalesParticipantsController : ControllerBase
    {
        private MultiLevelSalesParticipantsRepo _repoMultiLevelSalesParticipants;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public MultiLevelSalesParticipantsController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoMultiLevelSalesParticipants = new MultiLevelSalesParticipantsRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<MultiLevelSalesParticipantsModel> model = new BaseModels<MultiLevelSalesParticipantsModel>();
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

                Func<MultiLevelSalesParticipantsModel, object> _orderByExpression = x => x.MultiLevelSalesParticipantsCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<MultiLevelSalesParticipantsModel, object>> _sortableFields = new Dictionary<string, Func<MultiLevelSalesParticipantsModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "MultiLevelSalesParticipantsCode", x => x.MultiLevelSalesParticipantsCode },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<MultiLevelSalesParticipantsModel> _data = _repoMultiLevelSalesParticipants._context.MultiLevelSalesParticipants.Select(x => new MultiLevelSalesParticipantsModel
                {
                    MultiLevelSalesParticipantsId = x.MultiLevelSalesParticipantsId,
                    MultiLevelSalesParticipantsCode = x.MultiLevelSalesParticipantsCode ?? "",
                    ParticipantsName = x.ParticipantsName,
                    Birthday = x.Birthday,
                    BirthdayDisplay = x.Birthday.ToString("dd'/'MM'/'yyyy"),
                    PhoneNumber = x.PhoneNumber,
                    IdentityCardNumber = x.IdentityCardNumber,
                    DateOfIssuance = x.DateOfIssuance,
                    DateOfIssuanceDisplay = x.DateOfIssuance.ToString("dd'/'MM'/'yyyy"),
                    PlaceOfIssue = x.PlaceOfIssue,
                    Gender = x.Gender,
                    GenderDisplay = x.Gender == 1 ? "Nam" : "Nữ",
                    JoinDate = x.JoinDate,
                    JoinDateDisplay = x.JoinDate.ToString("dd'/'MM'/'yyyy"),
                    Province = x.Province,
                    Address = x.Address,
                    IsDel = x.IsDel
                });
                _data = _data.Where(x => !x.IsDel);


                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>

                       x.ParticipantsName.ToLower().Contains(_keywordSearch) 
                       || x.MultiLevelSalesParticipantsCode.ToLower().Contains(_keywordSearch)
                       || x.PhoneNumber.Contains(_keywordSearch)
                       || x.IdentityCardNumber.Contains(_keywordSearch)
                       || x.PlaceOfIssue.ToLower().Contains(_keywordSearch)
                       || x.GenderDisplay.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                   ); 
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                     && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x =>
                                (x.JoinDate) >=
                                DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                    && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x =>
                               x.JoinDate <=
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
            BaseModels<MultiLevelSalesParticipant> model = new BaseModels<MultiLevelSalesParticipant>();
            try
            {
                var result = _repoMultiLevelSalesParticipants.FindById(id);
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
        public async Task<IActionResult> Update(MultiLevelSalesParticipant data)
        {
            BaseModels<MultiLevelSalesParticipant> model = new BaseModels<MultiLevelSalesParticipant>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MultiLevelSalesParticipant? SaveData = _repoMultiLevelSalesParticipants._context.MultiLevelSalesParticipants.Where(x => x.MultiLevelSalesParticipantsId == data.MultiLevelSalesParticipantsId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var Code = _repoMultiLevelSalesParticipants.findByMultiLevelSalesParticipantsCode(data.MultiLevelSalesParticipantsCode, Guid.Parse(data.MultiLevelSalesParticipantsId.ToString()));

                    if (Code)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã số đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MULTI_LEVEL_SALE_PARTICIPANT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.MultiLevelSalesParticipantsId = Guid.Parse(data.MultiLevelSalesParticipantsId.ToString());
                    SaveData.MultiLevelSalesParticipantsCode = data.MultiLevelSalesParticipantsCode;
                    SaveData.ParticipantsName = data.ParticipantsName;
                    SaveData.Birthday = data.Birthday;
                    SaveData.PhoneNumber = data.PhoneNumber;
                    SaveData.IdentityCardNumber = data.IdentityCardNumber;
                    SaveData.DateOfIssuance = data.DateOfIssuance;
                    SaveData.PlaceOfIssue = data.PlaceOfIssue;
                    SaveData.Gender = data.Gender;
                    SaveData.JoinDate = data.JoinDate;
                    SaveData.Province = data.Province;
                    SaveData.Address = data.Address;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoMultiLevelSalesParticipants.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MULTI_LEVEL_SALE_PARTICIPANT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(MultiLevelSalesParticipant data)
        {
            BaseModels<MultiLevelSalesParticipant> model = new BaseModels<MultiLevelSalesParticipant>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var Code = _repoMultiLevelSalesParticipants.findByMultiLevelSalesParticipantsCode(data.MultiLevelSalesParticipantsCode, null);
                SystemLog datalog = new SystemLog();
                if (Code)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã số đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MULTI_LEVEL_SALE_PARTICIPANT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                MultiLevelSalesParticipant SaveData = new MultiLevelSalesParticipant();
                SaveData.MultiLevelSalesParticipantsId = Guid.Parse(data.MultiLevelSalesParticipantsId.ToString());
                SaveData.MultiLevelSalesParticipantsCode = data.MultiLevelSalesParticipantsCode;
                SaveData.ParticipantsName = data.ParticipantsName;
                SaveData.Birthday = data.Birthday;
                SaveData.PhoneNumber = data.PhoneNumber;
                SaveData.IdentityCardNumber = data.IdentityCardNumber;
                SaveData.DateOfIssuance = data.DateOfIssuance;
                SaveData.PlaceOfIssue = data.PlaceOfIssue;
                SaveData.Gender = data.Gender;
                SaveData.JoinDate = data.JoinDate;
                SaveData.Province = data.Province;
                SaveData.Address = data.Address;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoMultiLevelSalesParticipants.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MULTI_LEVEL_SALE_PARTICIPANT, Action_Status.SUCCESS);
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
            BaseModels<MultiLevelSalesParticipant> model = new BaseModels<MultiLevelSalesParticipant>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MultiLevelSalesParticipant DeleteData = new MultiLevelSalesParticipant();
                DeleteData.MultiLevelSalesParticipantsId = id;
                DeleteData.IsDel = true;
                await _repoMultiLevelSalesParticipants.Delete(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MULTI_LEVEL_SALE_PARTICIPANT, Action_Status.SUCCESS);
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

        private List<MultiLevelSalesParticipantsModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<MultiLevelSalesParticipantsModel, object> _orderByExpression = x => x.MultiLevelSalesParticipantsCode; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<MultiLevelSalesParticipantsModel, object>> _sortableFields = new Dictionary<string, Func<MultiLevelSalesParticipantsModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "MultiLevelSalesParticipantsCode", x => x.MultiLevelSalesParticipantsCode },
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            //Cách 1 dùng entity
            IQueryable<MultiLevelSalesParticipantsModel> _data = _repoMultiLevelSalesParticipants._context.MultiLevelSalesParticipants.Select(x => new MultiLevelSalesParticipantsModel
            {
                MultiLevelSalesParticipantsId = x.MultiLevelSalesParticipantsId,
                MultiLevelSalesParticipantsCode = x.MultiLevelSalesParticipantsCode ?? "",
                ParticipantsName = x.ParticipantsName,
                Birthday = x.Birthday,
                BirthdayDisplay = x.Birthday.ToString("dd'/'MM'/'yyyy"),
                PhoneNumber = x.PhoneNumber,
                IdentityCardNumber = x.IdentityCardNumber,
                DateOfIssuance = x.DateOfIssuance,
                DateOfIssuanceDisplay = x.DateOfIssuance.ToString("dd'/'MM'/'yyyy"),
                PlaceOfIssue = x.PlaceOfIssue,
                Gender = x.Gender,
                GenderDisplay = x.Gender == 1 ? "Nam" : "Nữ",
                JoinDate = x.JoinDate,
                JoinDateDisplay = x.JoinDate.ToString("dd'/'MM'/'yyyy"),
                Province = x.Province,
                Address = x.Address,
                IsDel = x.IsDel
            });
            _data = _data.Where(x => !x.IsDel);


            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>

                   x.ParticipantsName.ToLower().Contains(_keywordSearch)
                   || x.MultiLevelSalesParticipantsCode.ToLower().Contains(_keywordSearch)
                   || x.PhoneNumber.Contains(_keywordSearch)
                   || x.IdentityCardNumber.Contains(_keywordSearch)
                   || x.PlaceOfIssue.ToLower().Contains(_keywordSearch)
                   || x.GenderDisplay.ToLower().Contains(_keywordSearch)
                   || x.Address.ToLower().Contains(_keywordSearch)
               );
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                 && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                _data = _data.Where(x =>
                            (x.JoinDate) >=
                            DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                _data = _data.Where(x =>
                           x.JoinDate <=
                            DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
            }
            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachnguoithamgiahoatdongbanhangdacap.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 9;
                    int row = 1;

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.MultiLevelSalesParticipantsCode;
                            worksheet.Cell(index, 3).Value = item.ParticipantsName;
                            worksheet.Cell(index, 4).Value = item.Birthday;
                            worksheet.Cell(index, 5).Value = item.IdentityCardNumber;
                            worksheet.Cell(index, 6).Value = item.DateOfIssuance;
                            worksheet.Cell(index, 7).Value = item.PlaceOfIssue;
                            worksheet.Cell(index, 8).Value = item.PhoneNumber;
                            worksheet.Cell(index, 9).Value = item.Gender == 1 ? "Nam" : "Nữ";
                            worksheet.Cell(index, 10).Value = item.Address;
                            worksheet.Cell(index, 11).Value = item.Province;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.MultiLevelSalesParticipantsCode;
                            worksheet.Cell(index, 3).Value = item.ParticipantsName;
                            worksheet.Cell(index, 4).Value = item.Birthday;
                            worksheet.Cell(index, 5).Value = item.IdentityCardNumber;
                            worksheet.Cell(index, 6).Value = item.DateOfIssuance;
                            worksheet.Cell(index, 7).Value = item.PlaceOfIssue;
                            worksheet.Cell(index, 8).Value = item.PhoneNumber;
                            worksheet.Cell(index, 9).Value = item.Gender == 1 ? "Nam" : "Nữ";
                            worksheet.Cell(index, 10).Value = item.Address;
                            worksheet.Cell(index, 11).Value = item.Province;
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
