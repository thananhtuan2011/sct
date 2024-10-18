using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Models.TestModel;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using API_SoCongThuong.Reponsitories.CommuneRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;
using static API_SoCongThuong.Classes.ErrMsg_Const;

namespace API_SoCongThuong.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private DistrictRepository _repo;
        private CommuneRepo _repoCommune;
        private BusinessRepo _repoBusiness;
        private CateCriteriaNumberSevenRepo _repoCateCriteriaNumberSeven;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public DistrictController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new DistrictRepository(context);
            _repoCommune = new CommuneRepo(context);
            _repoBusiness = new BusinessRepo(context);
            _repoCateCriteriaNumberSeven = new CateCriteriaNumberSevenRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<DistrictModel> model = new BaseModels<DistrictModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<DistrictModel, object> _orderByExpression = x => x.DistrictCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<DistrictModel, object>> _sortableFields = new Dictionary<string, Func<DistrictModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "DistrictCode", x => x.DistrictCode },
                    { "DistrictName", x => x.DistrictName },
                    { "CommuneNumber", x => x.CommuneNumber }
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                //Cách 1 dùng entity
                IQueryable<DistrictModel> _data = _repo._context.Districts.Select(x => new DistrictModel
                {
                    DistrictId = x.DistrictId,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictName,
                    CommuneNumber = _repo._context.Communes.Where(c => c.DistrictId == x.DistrictId && !c.IsDel).Count(),
                    IsDel = x.IsDel
                });
                _data = _data.Where(x => !x.IsDel);

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.DistrictName.ToLower().Contains(_keywordSearch) || x.DistrictCode.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.DistrictId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                var result = _repo.FindById(id);
                if (result == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(model);
                }
                else
                {
                    model.status = 1;
                    model.items = result.ToList();
                    return Ok(model);
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
        public async Task<IActionResult> Update(DistrictModel data)
        {
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                District? test = _repo._context.Districts.Where(x => x.DistrictId == data.DistrictId && !x.IsDel).FirstOrDefault();
                if (test != null)
                {
                    var districtCode = _repo.findByDistrictCode(data.DistrictCode, Guid.Parse(data.DistrictId.ToString()));

                    if (districtCode)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã huyện đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.DISTRICT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                    test.DistrictId = Guid.Parse(data.DistrictId.ToString());
                    test.DistrictCode = data.DistrictCode;
                    test.DistrictName = data.DistrictName;
                    test.CommuneNumber = data.CommuneNumber;
                    test.UpdateUserId = loginData.Userid;
                    test.UpdateTime = DateTime.Now;

                    await _repo.Update(test);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.DISTRICT, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    model.status = 1;
                    return Ok(model);
                }
                else
                {
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.DISTRICT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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
        public async Task<IActionResult> create(DistrictModel data)
        {
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var districtCode = _repo.findByDistrictCode(data.DistrictCode, null);
                if (districtCode)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã huyện đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.DISTRICT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }

                District test = new District();
                test.DistrictCode = data.DistrictCode;
                test.DistrictName = data.DistrictName;
                test.CommuneNumber = data.CommuneNumber;
                test.CreateUserId = loginData.Userid;
                test.CreateTime = DateTime.Now;

                await _repo.Insert(test);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.DISTRICT, Action_Status.SUCCESS);
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

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                await _repo.Delete(id);
                *//* District test = new District();
                 test.DistrictId = id;
                 test.IsDel = ;
                 await _repo.Update(test);*//*

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
        }*/
        [HttpPut("deleteDistrict/{id}")]
        public async Task<IActionResult> DeleteDistrict(Guid id)
        {
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var countCommune = _repoCommune._context.Communes.Where(c => c.DistrictId == id && !c.IsDel).Count();
                var countBusiness = _repoBusiness._context.Businesses.Where(c => c.DistrictId == id && !c.IsDel).Count();
                var countCateCriteriaNumberSeven = _repoCateCriteriaNumberSeven._context.CateCriteriaNumberSevenDetails.Where(c => c.DistrictId == id).Count();
                if (countCommune > 0 || countBusiness > 0 || countCateCriteriaNumberSeven > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Dữ liệu đang được sử dụng ở trang khác"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.DISTRICT, Action_Status.FAIL );
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                District test = new District();
                test.DistrictId = id;
                test.IsDel = true;
                await _repo.DeleteDistrict(test);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.DISTRICT, Action_Status.SUCCESS);
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

        /*[HttpPut("deleteDistricts")]*/
        /*[Route("deleteDistricts")]
        [HttpPut()]
        public async Task<IActionResult> deleteItems(removeListDistrictItems data)
        {
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                foreach (Guid id in data.DistrictIds)
                {
                    District test = new District();
                    test.DistrictId = id;
                    test.IsDel = true;
                    await _repo.DeleteDistrict(test);
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
        }*/

        [Route("deleteItems")]
        [HttpPut()]
        public async Task<IActionResult> deleteItems(removeListDistrictItems data)
        {
            BaseModels<District> model = new BaseModels<District>();
            try
            {
                foreach (Guid id in data.DistrictIds)
                {
                    District test = new District();
                    test.DistrictId = id;
                    test.IsDel = true;
                    await _repo.DeleteDistrict(test);
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


    }
}
