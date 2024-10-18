using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CommuneRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using API_SoCongThuong.Reponsitories.CommercialManagementRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;
using static API_SoCongThuong.Classes.ErrMsg_Const;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommuneController : ControllerBase
    {
        private DistrictRepository _repoDistrict;
        private CommuneRepo _repoCommune;
        private CommercialManagementRepo _repoCommercialManagement;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;


        public CommuneController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoDistrict = new DistrictRepository(context);
            _repoCommune = new CommuneRepo(context);
            _repoCommercialManagement = new CommercialManagementRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        // Load data cho ô chọn huyện
        #region Danh sách huyện
        [Route("loaddistrict")]
        [HttpGet]
        public IActionResult LoadDistrict()
        {
            BaseModels<District> model = new BaseModels<District>();
            
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<District> _data = _repoDistrict.FindAll().Where(x => !x.IsDel);

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
        #endregion 

        // Lấy danh sách danh mục xã
        #region Danh mục xã
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<CommuneModel> model = new BaseModels<CommuneModel>();
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

                Func<CommuneModel, object> _orderByExpression = x => x.CommuneCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CommuneModel, object>> _sortableFields = new Dictionary<string, Func<CommuneModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "CommuneCode", x => x.CommuneCode },
                    { "CommuneName", x => x.CommuneName },
                    { "DistrictName", x => x.DistrictName }
                };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<CommuneModel> _data = _repoCommune._context.Communes.Join(
                    _repoDistrict._context.Districts,
                    cc => cc.DistrictId,
                    cd => cd.DistrictId,
                    (cc, cd) => new CommuneModel
                    {
                        CommuneId = cc.CommuneId,
                        CommuneCode = cc.CommuneCode,
                        CommuneName = cc.CommuneName,
                        IsDel = cc.IsDel,
                        DistrictId = cd.DistrictId,
                        DistrictName = cd.DistrictName
                    }
                    );

                _data = _data.Where(x => !x.IsDel);


                //Cách 2 dùng linq
                //IQueryable<TestModel> _dataLinq = (from t in _repo._context.Districts
                //                                       //where
                //                                   select new TestModel
                //                                   {
                //                                       Id = t.Id,
                //                                       Name = t.Name,
                //                                   });


                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.CommuneName.ToLower().Contains(_keywordSearch) || x.CommuneCode.ToLower().Contains(_keywordSearch) || x.DistrictName.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.CommuneId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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


        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<Commune> model = new BaseModels<Commune>();
            try
            {
                var result = _repoCommune.FindById(id);

                if (result != null && result.Count() != 0)
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
        public async Task<IActionResult> Update(CommuneModel data)
        {
            BaseModels<Commune> model = new BaseModels<Commune>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();
                Commune? SaveData = _repoCommune._context.Communes.Where(x => x.CommuneId == data.CommuneId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var communeCode = _repoCommune.findByCommuneCode(data.CommuneCode, Guid.Parse(data.CommuneId.ToString()));

                    if (communeCode) 
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã xã đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COMMUNE, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }
                    SaveData.CommuneId = Guid.Parse(data.CommuneId.ToString());
                    SaveData.CommuneCode = data.CommuneCode;
                    SaveData.CommuneName = data.CommuneName;
                    SaveData.DistrictId = data.DistrictId;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoCommune.Update(SaveData);
                    model.status = 1;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COMMUNE, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                else
                {
                    model.status = 0;
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COMMUNE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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
        public async Task<IActionResult> create(CommuneModel data)
        {
            BaseModels<Commune> model = new BaseModels<Commune>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var communeCode =  _repoCommune.findByCommuneCode(data.CommuneCode, null);

                if (communeCode)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã xã đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.COMMUNE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }

                Commune SaveData = new Commune();
                SaveData.CommuneCode = data.CommuneCode;
                SaveData.CommuneName = data.CommuneName;
                SaveData.DistrictId = data.DistrictId;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoCommune.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.COMMUNE, Action_Status.SUCCESS);
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

        [HttpPut("deleteCommune/{id}")]
        public async Task<IActionResult> DeleteCommune(Guid id)
        {
            BaseModels<Commune> model = new BaseModels<Commune>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var countCommercialManagement = _repoCommercialManagement._context.CommercialManagements.Where(x => x.CommuneId == id && !x.IsDel).Count();
                if (countCommercialManagement > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Dữ liệu đang được sử dụng ở trang khác"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.COMMUNE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                Commune DeleteData = new Commune();
                DeleteData.CommuneId = id;
                DeleteData.IsDel = true;
                await _repoCommune.DeleteCommune(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.COMMUNE, Action_Status.SUCCESS);
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

        [Route("deleteCommunes")]
        [HttpPut()]
        public async Task<IActionResult> deleteItems(removeListCommuneItems data)
        {
            BaseModels<Commune> model = new BaseModels<Commune>();
            try
            {
                foreach (Guid id in data.CommuneIds)
                {
                    Commune DeleteData = new Commune();
                    DeleteData.CommuneId = id;
                    DeleteData.IsDel = true;
                    await _repoCommune.DeleteCommune(DeleteData);
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

