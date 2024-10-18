using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using static API_SoCongThuong.Classes.ErrMsg_Const;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateCriteriaController : ControllerBase
    {
        private CateCriteriaRepo _repo;
        private RPOSOfInvestmentProjectsRepo _repoRPOSOfInvestmentProjects;
        private RPOSOfConstructionInvestmentProjectsRepo _repoRPOSOfConstructionInvestmentProjects;
        private ReportIndustrialClustersRepo _repoReportIndustrialClusters;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public CateCriteriaController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateCriteriaRepo(context);
            _repoRPOSOfInvestmentProjects = new RPOSOfInvestmentProjectsRepo(context);
            _repoRPOSOfConstructionInvestmentProjects = new RPOSOfConstructionInvestmentProjectsRepo(context);
            _repoReportIndustrialClusters = new ReportIndustrialClustersRepo(context);

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

            BaseModels<CateCriteriaModel> model = new BaseModels<CateCriteriaModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<CateCriteriaModel, object> _orderByExpression = x => x.CateCriteriaName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CateCriteriaModel, object>> _sortableFields = new Dictionary<string, Func<CateCriteriaModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "CateCriteriaName", x => x.CateCriteriaName }
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

                IQueryable<CateCriteriaModel> _data = _repo._context.CateCriteria.Where(x => !x.IsDel).Select(
                     (cc) => new CateCriteriaModel
                     {
                         CateCriteriaId = cc.CateCriteriaId,
                         CateCriteriaName = cc.CateCriteriaName
                     });
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.CateCriteriaName.ToLower().Contains(_keywordSearch));  //Lấy table đã select tìm kiếm theo keyword
                }
                // model.items = _data.ToList();

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
                ////Đoạn này lấy total đã tối ưu việc call DB nhiều lần
                //var listId = model.items.Select(x => x.CateCriteriaId).ToList();
                //var listTotal = _repo._context.CateCriteria.Where(x => listId.Contains(x.CateCriteriaId)).Select(x =>
                // new CateCriteriaModel
                // {
                //     CateCriteriaId = x.CateCriteriaId
                // }).ToList();
                //for (int i = 0; i < model.items.Count(); i++)
                //{
                //    int tt = listTotal.Where(x => x.CateCriterionId == model.items[i].CateCriterionId).Select(x => x.TotalStore).FirstOrDefault(0);
                //    model.items[i].TotalStore = tt;
                //}
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
        public async Task<IActionResult> create(CateCriteriaModel data)
        {
            BaseModels<CateCriteriaModel> model = new BaseModels<CateCriteriaModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                CateCriterion saveData = new CateCriterion
                {
                    //insert thong tin
                    CateCriteriaName = data.CateCriteriaName,
                    CreateUserId = loginData.Userid,
                    CreateTime = new DateTime(),
                };
                await _repo.Insert(saveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_CRITERIA, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateCriteriaModel data)
        {
            BaseModels<CateCriterion> model = new BaseModels<CateCriterion>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                CateCriterion SaveData = _repo.FindById(data.CateCriteriaId);
                if (SaveData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_CRITERIA, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.CateCriteriaName))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_CRITERIA, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    SaveData.CateCriteriaName = data.CateCriteriaName;
                    SaveData.UpdateTime = DateTime.Now;
                    SaveData.UpdateUserId = loginData.Userid;
                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_CRITERIA, Action_Status.SUCCESS);
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
            BaseModels<CateCriteriaModel> model = new BaseModels<CateCriteriaModel>();
            try
            {
                var info = _repo.FindById(id);
                //var storelist = _repo.FindStoreId(id).Select(x => x.CateCriterionId).ToList().ToString();
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                //Tạo list
                List<CateCriteriaModel> lst = new List<CateCriteriaModel>();
                //Tạo model
                CateCriteriaModel data = new CateCriteriaModel();
                data.CateCriteriaId = info.CateCriteriaId;
                data.CateCriteriaName = info.CateCriteriaName;
                //Chuyển model về list
                lst.Add(data);

                //Set data cho base model
                model.status = 1;
                model.items = lst;
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
        [Route("deletes")]
        [HttpPut()]
        public async Task<IActionResult> deletes(List<Guid> IdRemoves)
        {
            BaseModels<CateCriterion> model = new BaseModels<CateCriterion>();
            try
            {
                await _repo.Deletes(IdRemoves);
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
            BaseModels<CateCriterion> model = new BaseModels<CateCriterion>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var countRPOSOfInvestmentProjects = _repoRPOSOfInvestmentProjects._context.ReportOperationalStatusOfInvestmentProjects.Where(x => x.Criteria == id && !x.IsDel).Count();
                var countRPOSOfConstructionInvestmentProjects = _repoRPOSOfConstructionInvestmentProjects._context.ReportOperationalStatusOfConstructionInvestmentProjects.Where(x => x.Criteria == id && !x.IsDel).Count();
                var countReportIndustrialClusters = _repoReportIndustrialClusters._context.ReportIndustrialClusters.Where(x => x.Criteria == id && !x.IsDel).Count();
                if (countRPOSOfInvestmentProjects > 0 || countRPOSOfConstructionInvestmentProjects > 0 || countReportIndustrialClusters > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Dữ liệu đang được sử dụng ở trang khác"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_CRITERIA, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                await _repo.Delete(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_CRITERIA, Action_Status.SUCCESS);
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

    }
}
