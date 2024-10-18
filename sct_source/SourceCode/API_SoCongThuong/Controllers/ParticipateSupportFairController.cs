using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BusinessIndustryRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.IndustryRepository;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
using API_SoCongThuong.Reponsitories.TypeOfProfessionRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using Microsoft.EntityFrameworkCore.Internal;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipateSupportFairController : ControllerBase
    {
        private ParticipateSupportFairRepo _repo;
        //private BusinessRepo _repoBusi;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ParticipateSupportFairController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ParticipateSupportFairRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;

        }
        // Lấy danh sách 
        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ParticipateSupportFairModel> model = new BaseModels<ParticipateSupportFairModel>();
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

                Func<ParticipateSupportFairModel, object> _orderByExpression = x => x.ParticipateSupportFairId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ParticipateSupportFairModel, object>> _sortableFields = new Dictionary<string, Func<ParticipateSupportFairModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "ParticipateSupportFairName", x => x.ParticipateSupportFairName },
                    { "CountryName", x => x.Country },
                    { "EndTimeDisplay", x => x.EndTime },
                    { "StartTimeDisplay", x => x.StartTime },
                    { "PlanJoinName", x => x.PlanJoinName },
                    { "CreateName", x => x.CreateUserId },
                    { "CreateTimeDisplay", x => x.CreateTime },
                    { "NumberOfBusiness", x => x.NumberOfBusiness },
                    { "ImplementCost", x => x.ImplementCost },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<ParticipateSupportFairModel> _data =
                    from cc in _repo._context.ParticipateSupportFairs where !cc.IsDel
                    join u in _repo._context.Users on cc.CreateUserId equals u.UserId into uGroup
                    from us in uGroup.DefaultIfEmpty()
                    join coun in _repo._context.Countries on cc.Country equals coun.CountryId into counGroup
                    from coun in counGroup.DefaultIfEmpty()
                    let numberOfBusiness = _repo._context.ParticipateSupportFairDetails.Count(x => x.ParticipateSupportFairId == cc.ParticipateSupportFairId)
                    let numberOfBusinessInProvice = _repo._context.ParticipateSupportFairDetails.Count(x => x.ParticipateSupportFairId == cc.ParticipateSupportFairId && x.BusinessId != Guid.Empty)
                    let numberOfBusinessOutProvice = _repo._context.ParticipateSupportFairDetails.Count(x => x.ParticipateSupportFairId == cc.ParticipateSupportFairId && x.BusinessId == Guid.Empty)
                    select new ParticipateSupportFairModel()
                    {
                        ParticipateSupportFairId = cc.ParticipateSupportFairId,
                        ParticipateSupportFairName = cc.ParticipateSupportFairName,
                        PlanJoinName = cc.PlanJoin == 1 ? "Sở tham gia" : "Hỗ trợ doanh nghiệp tham gia",
                        EndTimeDisplay = string.Format("{0:dd/MM/yyyy HH:mm}", cc.EndTime),
                        StartTimeDisplay = cc.StartTime.ToString("dd/MM/yyyy HH:mm"),
                        Address = cc.Address,
                        CountryName = coun.CountryName,
                        ImplementCost = cc.ImplementCost,
                        NumberOfBusiness = numberOfBusiness,
                        Type = numberOfBusinessInProvice > 0 && numberOfBusinessOutProvice == 0 ? 1 : numberOfBusinessInProvice == 0 && numberOfBusinessOutProvice > 0 ? 2 : numberOfBusinessInProvice == 0 && numberOfBusinessOutProvice == 0 ? 3 : 0,
                    }; 

                //Kiểm tra điều kiện tìm kiếm
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();

                    //Lấy table đã select tìm kiếm theo keyword
                    _data = _data.Where(x => 
                    x.ParticipateSupportFairName.ToLower().Contains(_keywordSearch)
                    || x.PlanJoinName.ToLower().Contains(_keywordSearch)
                    || x.CountryName.ToLower().Contains(_keywordSearch)
                    || x.NumberOfBusiness.ToString().Contains(_keywordSearch)
                    || x.ImplementCost.ToString().Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("TypeBusiness") && !string.IsNullOrEmpty(query.Filter["TypeBusiness"]))
                {
                    _data = _data.Where(x => x.Type.ToString() == query.Filter["TypeBusiness"] || x.Type == 0);
                }

                //Đếm số dòng của table đã select được
                int _countRows = _data.Count();

                //nếu table = 0 thì trả về không có dữ liệu
                if (_countRows == 0)
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
        #endregion

        [HttpPost()]
        public async Task<IActionResult> Create(ParticipateSupportFairModel data)
        {
            BaseModels<ParticipateSupportFairModel> model = new BaseModels<ParticipateSupportFairModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (string.IsNullOrEmpty(data.ParticipateSupportFairName))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.PARTICIPANT_SUPPORT_FAIR, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                if (string.IsNullOrEmpty(data.Scale))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.PARTICIPANT_SUPPORT_FAIR, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                var util = new Ulities();
                data = util.TrimModel(data);

                data.StartTime = Ulities.ConvertTimeZone(HttpContext.Request.Headers, data.StartTime);
                data.EndTime = data.EndTime == null ? null : Ulities.ConvertTimeZone(HttpContext.Request.Headers, (DateTime)data.EndTime);
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.PARTICIPANT_SUPPORT_FAIR, Action_Status.SUCCESS);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(ParticipateSupportFairModel data)
        {
            BaseModels<ParticipateSupportFairModel> model = new BaseModels<ParticipateSupportFairModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo.FindById(data.ParticipateSupportFairId);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PARTICIPANT_SUPPORT_FAIR, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.ParticipateSupportFairName))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PARTICIPANT_SUPPORT_FAIR, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                    if (string.IsNullOrEmpty(data.Scale))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PARTICIPANT_SUPPORT_FAIR, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    var util = new Ulities();
                    data = util.TrimModel(data);

                    data.StartTime = Ulities.ConvertTimeZone(HttpContext.Request.Headers, data.StartTime);
                    data.EndTime = data.EndTime == null ? null : Ulities.ConvertTimeZone(HttpContext.Request.Headers, (DateTime)data.EndTime);
                    // data.StartTime = Ulities.ConvertTimeZone(HttpContext.Request.Headers, data.StartTime);
                    //data.EndTime = data.EndTime == null ? null : Ulities.ConvertTimeZone(HttpContext.Request.Headers, (DateTime)data.EndTime);
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PARTICIPANT_SUPPORT_FAIR, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                }
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<ParticipateSupportFairModel> model = new BaseModels<ParticipateSupportFairModel>();
            try
            {
                var info = _repo.FindById(id);
                //var storelist = _repo.FindStoreId(id).Select(x => x.CateCriterionId).ToList().ToString();
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                //Set data cho base model
                model.status = 1;
                model.data = info;
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
        //[Route("deletes")]
        //[HttpPut()]
        //public async Task<IActionResult> deletes(List<Guid> IdRemoves)
        //{
        //    BaseModels<object> model = new BaseModels<object>();
        //    try
        //    {
        //        await _repo.Deletes(IdRemoves);
        //        model.status = 1;
        //        return Ok(model);
        //    }
        //    catch (Exception ex)
        //    {

        //        model.status = 0;
        //        model.error = new ErrorModel()
        //        {
        //            Code = ErrCode_Const.EXCEPTION_API,
        //            Msg = ex.Message
        //        };
        //        return BadRequest(model);
        //    }
        //}
        [HttpPut("delete/{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.Delete(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.PARTICIPANT_SUPPORT_FAIR, Action_Status.SUCCESS);
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


        private List<ParticipateSupportFairModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<ParticipateSupportFairModel, object> _orderByExpression = x => x.ParticipateSupportFairId; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<ParticipateSupportFairModel, object>> _sortableFields = new Dictionary<string, Func<ParticipateSupportFairModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "ParticipateSupportFairName", x => x.ParticipateSupportFairName },
                    { "CountryName", x => x.Country },
                    { "EndTimeDisplay", x => x.EndTime },
                    { "StartTimeDisplay", x => x.StartTime },
                    { "PlanJoinName", x => x.PlanJoinName },
                    { "CreateName", x => x.CreateUserId },
                    { "CreateTimeDisplay", x => x.CreateTime },
                    { "NumberOfBusiness", x => x.NumberOfBusiness },
                    { "ImplementCost", x => x.ImplementCost },
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            IQueryable<ParticipateSupportFairModel> _data =
                from cc in _repo._context.ParticipateSupportFairs
                where !cc.IsDel
                join u in _repo._context.Users on cc.CreateUserId equals u.UserId into uGroup
                from us in uGroup.DefaultIfEmpty()
                join coun in _repo._context.Countries on cc.Country equals coun.CountryId into counGroup
                from coun in counGroup.DefaultIfEmpty()
                let numberOfBusiness = _repo._context.ParticipateSupportFairDetails.Count(x => x.ParticipateSupportFairId == cc.ParticipateSupportFairId)
                let numberOfBusinessInProvice = _repo._context.ParticipateSupportFairDetails.Count(x => x.ParticipateSupportFairId == cc.ParticipateSupportFairId && x.BusinessId != Guid.Empty)
                let numberOfBusinessOutProvice = _repo._context.ParticipateSupportFairDetails.Count(x => x.ParticipateSupportFairId == cc.ParticipateSupportFairId && x.BusinessId == Guid.Empty)
                select new ParticipateSupportFairModel()
                {
                    ParticipateSupportFairId = cc.ParticipateSupportFairId,
                    ParticipateSupportFairName = cc.ParticipateSupportFairName,
                    PlanJoin = cc.PlanJoin,
                    PlanJoinName = cc.PlanJoin == 1 ? "Sở tham gia" : "Hỗ trợ doanh nghiệp tham gia",
                    EndTimeDisplay = string.Format("{0:dd/MM/yyyy HH:mm}", cc.EndTime),
                    StartTimeDisplay = cc.StartTime.ToString("dd/MM/yyyy HH:mm"),
                    Address = cc.Address,
                    CountryName = coun.CountryName,
                    ImplementCost = cc.ImplementCost,
                    NumberOfBusiness = numberOfBusiness,
                    Type = numberOfBusinessInProvice > 0 && numberOfBusinessOutProvice == 0 ? 1 : numberOfBusinessInProvice == 0 && numberOfBusinessOutProvice > 0 ? 2 : numberOfBusinessInProvice == 0 && numberOfBusinessOutProvice == 0 ? 3 : 0,
                };

            //Kiểm tra điều kiện tìm kiếm
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();

                //Lấy table đã select tìm kiếm theo keyword
                _data = _data.Where(x =>
                x.ParticipateSupportFairName.ToLower().Contains(_keywordSearch)
                || x.PlanJoinName.ToLower().Contains(_keywordSearch)
                || x.CountryName.ToLower().Contains(_keywordSearch)
                || x.NumberOfBusiness.ToString().Contains(_keywordSearch)
                || x.ImplementCost.ToString().Contains(_keywordSearch)
                );
            }

            if (query.Filter != null && query.Filter.ContainsKey("TypeBusiness") && !string.IsNullOrEmpty(query.Filter["TypeBusiness"]))
            {
                _data = _data.Where(x => x.Type.ToString() == query.Filter["TypeBusiness"] || x.Type == 0);
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachthamgiahotrohoicho.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 6;
                    int row = 1;

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ParticipateSupportFairName;
                            worksheet.Cell(index, 3).Value = item.CountryName;
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.StartTimeDisplay;
                            worksheet.Cell(index, 6).Value = item.Scale;
                            worksheet.Cell(index, 7).Value = item.PlanJoin == 1 ? "X" : "";
                            worksheet.Cell(index, 8).Value = item.PlanJoin == 2 ? "X" : "";
                            worksheet.Cell(index, 9).Value = item.NumberOfBusiness;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ParticipateSupportFairName;
                            worksheet.Cell(index, 3).Value = item.CountryName;
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.StartTimeDisplay;
                            worksheet.Cell(index, 6).Value = item.Scale;
                            worksheet.Cell(index, 7).Value = item.PlanJoin == 1 ? "X" : "";
                            worksheet.Cell(index, 8).Value = item.PlanJoin == 2 ? "X" : "";
                            worksheet.Cell(index, 9).Value = item.NumberOfBusiness;
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

        [HttpGet("export")]
        public IActionResult ExportFile()
        {
            //Query data
            var data = _repo._context.ParticipateSupportFairs.Where(x => !x.IsDel).Select(p => new
            {
                ParticipateSupportFairName = p.ParticipateSupportFairName,
                Country = _repo._context.Countries.Where(x => x.CountryId == p.Country).Select(x => x.CountryName).FirstOrDefault(),
                Address = p.Address,
                StartTime = p.StartTime.ToString("dd/MM/yyyy HH:mm"),
                Scale = p.Scale,
                PlanJoin = p.PlanJoin,
                NumBusiness = _repo._context.ParticipateSupportFairDetails.Where(x => x.ParticipateSupportFairId == p.ParticipateSupportFairId).Count(),
            }).ToList();

            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachthamgiahotrohoicho.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 6;
                    int row = 1;

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ParticipateSupportFairName;
                            worksheet.Cell(index, 3).Value = item.Country;
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.StartTime;
                            worksheet.Cell(index, 6).Value = item.Scale;
                            worksheet.Cell(index, 7).Value = item.PlanJoin == 1 ? "X" : "";
                            worksheet.Cell(index, 8).Value = item.PlanJoin == 2 ? "X" : "";
                            worksheet.Cell(index, 9).Value = item.NumBusiness;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ParticipateSupportFairName;
                            worksheet.Cell(index, 3).Value = item.Country;
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.StartTime;
                            worksheet.Cell(index, 6).Value = item.Scale;
                            worksheet.Cell(index, 7).Value = item.PlanJoin == 1 ? "X" : "";
                            worksheet.Cell(index, 8).Value = item.PlanJoin == 2 ? "X" : "";
                            worksheet.Cell(index, 9).Value = item.NumBusiness;
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
