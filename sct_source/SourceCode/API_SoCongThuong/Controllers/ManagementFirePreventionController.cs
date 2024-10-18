
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ManagementFirePreventionRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementFirePreventionController : ControllerBase
    {
        private ManagementFirePreventionRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ManagementFirePreventionController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ManagementFirePreventionRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ManagementFirePreventionModel> model = new BaseModels<ManagementFirePreventionModel>();
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

                Func<ManagementFirePreventionModel, object> _orderByExpression = x => x.BusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ManagementFirePreventionModel, object>> _sortableFields = new Dictionary<string, Func<ManagementFirePreventionModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "Address", x => x.Address },
                        { "Reality", x => x.Reality }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<ManagementFirePreventionModel> _data = _repo._context.ManagementFirePreventions.Select(x => new ManagementFirePreventionModel
                {
                    ManagementFirePreventionId = x.ManagementFirePreventionId,
                    BusinessName = x.BusinessName,
                    Address = x.Address,
                    Reality = x.Reality,
                    RealityName = x.Reality == 0 ? "Không Đạt" : x.Reality == 1 ? "Trung bình" : "Tốt",
                    IsDel = x.IsDel
                });
                _data = _data.Where(x => !x.IsDel);


                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.BusinessName.ToLower().Contains(_keywordSearch)
                       || x.Address.ToLower().Contains(_keywordSearch)
                       || x.Reality.ToString().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }

                //Filter
                if (query.Filter != null && query.Filter.ContainsKey("reality") && !string.IsNullOrEmpty(query.Filter["reality"]))
                {
                    _data = _data.Where(x => x.Reality.ToString().Contains(string.Join("", query.Filter["reality"])));
                }

                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
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
            BaseModels<ManagementFirePrevention> model = new BaseModels<ManagementFirePrevention>();
            try
            {
                var result = _repo.FindById(id);
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
        public async Task<IActionResult> Update(ManagementFirePrevention data)
        {
            BaseModels<ManagementFirePrevention> model = new BaseModels<ManagementFirePrevention>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManagementFirePrevention? SaveData = _repo._context.ManagementFirePreventions.Where(x => x.ManagementFirePreventionId == data.ManagementFirePreventionId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.ManagementFirePreventionId = Guid.Parse(data.ManagementFirePreventionId.ToString());
                    SaveData.BusinessName = data.BusinessName;
                    SaveData.Address = data.Address;
                    SaveData.Reality = data.Reality;
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGEMENT_FIRE_PREVENTION, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGEMENT_FIRE_PREVENTION, Action_Status.FAIL);
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
        public async Task<IActionResult> create(ManagementFirePrevention data)
        {
            BaseModels<ManagementFirePrevention> model = new BaseModels<ManagementFirePrevention>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManagementFirePrevention SaveData = new ManagementFirePrevention();
                SaveData.BusinessName = data.BusinessName;
                SaveData.Address = data.Address;
                SaveData.Reality = data.Reality;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MANAGEMENT_FIRE_PREVENTION, Action_Status.SUCCESS);
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
            BaseModels<ManagementFirePrevention> model = new BaseModels<ManagementFirePrevention>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManagementFirePrevention DeleteData = new ManagementFirePrevention();
                DeleteData.ManagementFirePreventionId = id;
                DeleteData.IsDel = true;
                await _repo.DeleteManagementFirePrevention(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MANAGEMENT_FIRE_PREVENTION, Action_Status.SUCCESS);
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
        private List<ManagementFirePreventionModel> FindData([FromBody] QueryRequestBody query)
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            Func<ManagementFirePreventionModel, object> _orderByExpression = x => x.BusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<ManagementFirePreventionModel, object>> _sortableFields = new Dictionary<string, Func<ManagementFirePreventionModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "Address", x => x.Address },
                        { "Reality", x => x.Reality }
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            //Cách 1 dùng entity
            IQueryable<ManagementFirePreventionModel> _data = _repo._context.ManagementFirePreventions.Select(x => new ManagementFirePreventionModel
            {
                ManagementFirePreventionId = x.ManagementFirePreventionId,
                BusinessName = x.BusinessName,
                Address = x.Address,
                Reality = x.Reality,
                RealityName = x.Reality == 0 ? "Không Đạt" : x.Reality == 1 ? "Trung bình" : "Tốt",
                IsDel = x.IsDel
            });
            _data = _data.Where(x => !x.IsDel);


            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                    || */
                   x.BusinessName.ToLower().Contains(_keywordSearch)
                   || x.Address.ToLower().Contains(_keywordSearch)
                   || x.Reality.ToString().Contains(_keywordSearch)
               );  //Lấy table đã select tìm kiếm theo keyword
            }

            //Filter
            if (query.Filter != null && query.Filter.ContainsKey("reality") && !string.IsNullOrEmpty(query.Filter["reality"]))
            {
                _data = _data.Where(x => x.Reality.ToString().Contains(string.Join("", query.Filter["reality"])));
            }

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);

            if (!data.Any() || data.Count == 0)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/ThucTrangPCCCCuaCacChoTrenDiaBanTinh.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 5;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index - 1);
                        addrow.InsertRowsBelow(1);

                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.BusinessName;
                        worksheet.Cell(index, 3).Value = item.Address;
                        worksheet.Cell(index, 4).Value = item.RealityName;                  
                        index++;
                        row++;
                    }

                    var delrow = worksheet.Row(index);
                    delrow.Delete();
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
