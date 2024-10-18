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
    public class IndustrialPromotionProjectController : ControllerBase
    {
        private IndustrialPromotionProjectRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        //private BusinessRepo _repoBusi;
        public IndustrialPromotionProjectController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new IndustrialPromotionProjectRepo(context);
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

            BaseModels<IndustrialPromotionProjectModel> model = new BaseModels<IndustrialPromotionProjectModel>();
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

                Func<IndustrialPromotionProjectModel, object> _orderByExpression = x => x.IndustrialPromotionProjectId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<IndustrialPromotionProjectModel, object>> _sortableFields = new Dictionary<string, Func<IndustrialPromotionProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "ProjectName", x => x.ProjectName },
                    { "StartDate", x => x.StartDateTime },
                    { "EndDate", x => x.EndDateTime },
                    { "CapitalName", x => x.Capital },
                    { "Funding", x => x.Funding },
                    { "IndustrialPromotionFunding", x => x.IndustrialPromotionFunding },
                    { "ReciprocalEnterpriseFunding", x => x.ReciprocalEnterpriseFunding },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }


                //var lstData = _repoBusi._context.Businesses
                //                   .Where(c => !c.IsDel)
                //                   .ToList();

                IQueryable<IndustrialPromotionProjectModel> _data = _repo._context.IndustrialPromotionProjects.Where(x => !x.IsDel)
                    .Select(info => new IndustrialPromotionProjectModel
                    {
                        IndustrialPromotionProjectId = info.IndustrialPromotionProjectId,
                        ProjectName = info.ProjectName,
                        Funding = info.Funding,
                        ReciprocalEnterpriseFunding = info.ReciprocalEnterpriseFunding,
                        IndustrialPromotionFunding = info.IndustrialPromotionFunding,
                        CapitalName = info.Capital == 1 ? "Trung ương" : "Địa phương",
                        StartDate = info.StartDate.ToString("dd'/'MM'/'yyyy"),
                        EndDate = info.EndDate.ToString("dd'/'MM'/'yyyy"),
                        StartYear = info.StartDate.Year,
                        StartDateTime = info.StartDate,
                        EndDateTime = info.EndDate,
                        CreateTime = info.CreateTime,
                    }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.Funding.ToString().Contains(_keywordSearch)
                    || x.ProjectName.Contains(_keywordSearch)
                    || x.Funding.ToString().Contains(_keywordSearch)
                    || x.ReciprocalEnterpriseFunding.ToString().Contains(_keywordSearch)
                    || x.IndustrialPromotionFunding.ToString().Contains(_keywordSearch)
                    || x.CapitalName.Contains(_keywordSearch)
                    //|| x.LicenseDateDisplay.ToString().Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
                }


                if (query.Filter != null && query.Filter.ContainsKey("Capital"))
                {
                    _data = _data.Where(x => x.CapitalName.ToLower().Equals(string.Join("", query.Filter["Capital"]).ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                    && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                (x.StartDateTime) >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.StartDateTime <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
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
        #endregion

        [HttpPost()]
        public async Task<IActionResult> Create(IndustrialPromotionProjectModel data)
        {
            BaseModels<IndustrialPromotionProjectModel> model = new BaseModels<IndustrialPromotionProjectModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (string.IsNullOrEmpty(data.ProjectName))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INDUSTRIAL_PROMOTION_PROJECT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                data = new Ulities().TrimModel(data);
                data.StartDateTime = DateTime.ParseExact(data.StartDate, "dd'/'MM'/'yyyy", null);
                data.EndDateTime = DateTime.ParseExact(data.EndDate, "dd'/'MM'/'yyyy", null);

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INDUSTRIAL_PROMOTION_PROJECT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(IndustrialPromotionProjectModel data)
        {
            BaseModels<IndustrialPromotionProjectModel> model = new BaseModels<IndustrialPromotionProjectModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.IndustrialPromotionProjectId);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_PROJECT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.ProjectName))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_PROJECT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                    data = new Ulities().TrimModel(data);
                    data.StartDateTime = DateTime.ParseExact(data.StartDate, "dd'/'MM'/'yyyy", null);
                    data.EndDateTime = DateTime.ParseExact(data.EndDate, "dd'/'MM'/'yyyy", null);

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_PROMOTION_PROJECT, Action_Status.SUCCESS);
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
            BaseModels<IndustrialPromotionProjectModel> model = new BaseModels<IndustrialPromotionProjectModel>();
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.INDUSTRIAL_PROMOTION_PROJECT, Action_Status.SUCCESS);
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
        [Route("getDataFunding")]
        [HttpGet]

        public object getDataFunding()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var sumIndustrialPromotionFunding = _repo._context.IndustrialPromotionProjects.Where(x => !x.IsDel).Sum(x => x.IndustrialPromotionFunding);
                var sumReciprocalEnterpriseFunding = _repo._context.IndustrialPromotionProjects.Where(x => !x.IsDel).Sum(x => x.ReciprocalEnterpriseFunding);

                var result = new { sumIndustrialPromotionFunding, sumReciprocalEnterpriseFunding };
                return result;
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

        private List<IndustrialPromotionProjectModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<IndustrialPromotionProjectModel, object> _orderByExpression = x => x.IndustrialPromotionProjectId; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<IndustrialPromotionProjectModel, object>> _sortableFields = new Dictionary<string, Func<IndustrialPromotionProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "ProjectName", x => x.ProjectName },
                    { "StartDate", x => x.StartDateTime },
                    { "EndDate", x => x.EndDateTime },
                    { "CapitalName", x => x.Capital },
                    { "Funding", x => x.Funding },
                    { "IndustrialPromotionFunding", x => x.IndustrialPromotionFunding },
                    { "ReciprocalEnterpriseFunding", x => x.ReciprocalEnterpriseFunding },
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                _orderByExpression = _sortableFields[query.Sort.ColumnName];
            }

            IQueryable<IndustrialPromotionProjectModel> _data = _repo._context.IndustrialPromotionProjects.Where(x => !x.IsDel)
                .Select(info => new IndustrialPromotionProjectModel
                {
                    IndustrialPromotionProjectId = info.IndustrialPromotionProjectId,
                    ProjectName = info.ProjectName,
                    CapitalName = info.Capital == 1 ? "Trung ương" : "Địa phương",
                    Capital = info.Capital,
                    StartDate = info.StartDate.ToString("dd'/'MM'/'yyyy"),
                    EndDate = info.StartDate.ToString("dd'/'MM'/'yyyy"),
                    StartDateTime = info.StartDate,
                    EndDateTime = info.EndDate,
                    StartYear = info.StartDate.Year,

                    Funding = info.Funding,
                    ReciprocalEnterpriseFunding = info.ReciprocalEnterpriseFunding,
                    IndustrialPromotionFunding = info.IndustrialPromotionFunding,
                    CreateTime = info.CreateTime,
                });

            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.Funding.ToString().Contains(_keywordSearch)
                || x.ProjectName.Contains(_keywordSearch)
                || x.Funding.ToString().Contains(_keywordSearch)
                || x.ReciprocalEnterpriseFunding.ToString().Contains(_keywordSearch)
                || x.IndustrialPromotionFunding.ToString().Contains(_keywordSearch)
                || x.CapitalName.Contains(_keywordSearch)
                );
            }

            if (query.Filter != null && query.Filter.ContainsKey("Capital"))
            {
                _data = _data.Where(x => x.CapitalName.ToLower().Equals(string.Join("", query.Filter["Capital"]).ToLower()));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                && !string.IsNullOrEmpty(query.Filter["MinTime"]))
            {
                _data = _data.Where(x =>
                            (x.StartDateTime) >=
                            DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
            {
                _data = _data.Where(x =>
                           x.StartDateTime <=
                            DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
            }

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);
            var dataLocal = data.Where(x => x.Capital == 2).ToList();
            var dataCentral = data.Where(x => x.Capital == 1).ToList();

            //Địa phương
            var listLocalId = dataLocal.Select(x => x.IndustrialPromotionProjectId).ToList();
            var localBusinessName = (from l in _context.IndustrialPromotionProjectDetails.Where(x => listLocalId.Contains(x.IndustrialPromotionProjectId)).ToList()
                                     group l by l.IndustrialPromotionProjectId into g
                                     select new
                                     {
                                         IndustrialPromotionProjectId = g.Key,
                                         AllBusiness = String.Join(", ", g.Select(x => x.BusinessNameVi.Trim() + (String.IsNullOrEmpty(x.DiaChi) ? "" : " (" + x.DiaChi.Trim() + ")")).ToList())
                                     }).ToList();
                
            //Trung ương
            var listCentralId = dataLocal.Select(x => x.IndustrialPromotionProjectId).ToList();
            var centralBusinessName = (from l in _context.IndustrialPromotionProjectDetails.Where(x => listCentralId.Contains(x.IndustrialPromotionProjectId)).ToList()
                                       group l by l.IndustrialPromotionProjectId into g
                                       select new
                                       {
                                           IndustrialPromotionProjectId = g.Key,
                                           AllBusiness = String.Join(", ", g.Select(x => x.BusinessNameVi.Trim() + (String.IsNullOrEmpty(x.DiaChi) ? "" : " (" + x.DiaChi.Trim() + ")")).ToList())
                                       }).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanlyBaoCaoDuAnKhuyenCong.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int row = 1;
                    int index = 4;
                    foreach (var item in dataLocal)
                    {
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.ProjectName;
                        worksheet.Cell(index, 3).Value = item.StartYear;
                        worksheet.Cell(index, 4).Value = item.Funding;
                        worksheet.Cell(index, 5).Value = centralBusinessName.Where(x => x.IndustrialPromotionProjectId == item.IndustrialPromotionProjectId).Select(x => x.AllBusiness).FirstOrDefault();
                        if (row != dataLocal.Count())
                        {
                            worksheet.Row(index).CopyTo(worksheet.Row(index+1));
                            row++;
                            index++;
                        }
                    }

                    worksheet = workbook.Worksheets.Worksheet(2);
                    row = 1;
                    index = 4;
                    foreach (var item in dataCentral)
                    {
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.ProjectName;
                        worksheet.Cell(index, 3).Value = item.StartYear;
                        worksheet.Cell(index, 4).Value = item.Funding;
                        worksheet.Cell(index, 5).Value = centralBusinessName.Where(x => x.IndustrialPromotionProjectId == item.IndustrialPromotionProjectId).Select(x => x.AllBusiness).FirstOrDefault();
                        if (row != dataLocal.Count())
                        {
                            worksheet.Row(index).CopyTo(worksheet.Row(index + 1));
                            row++;
                            index++;
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
