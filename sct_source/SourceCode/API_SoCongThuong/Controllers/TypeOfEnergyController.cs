
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.TypeOfEnergyRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using static API_SoCongThuong.Classes.ErrMsg_Const;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeOfEnergyController : ControllerBase
    {
        private TypeOfEnergyRepo _repoTypeOfEnergy;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public TypeOfEnergyController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoTypeOfEnergy = new TypeOfEnergyRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        // Lấy danh sách danh mục loại hình chợ
        #region Danh mục loại hình chợ
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<TypeOfEnergyModel> model = new BaseModels<TypeOfEnergyModel>();
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

                Func<TypeOfEnergyModel, object> _orderByExpression = x => x.TypeOfEnergyCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TypeOfEnergyModel, object>> _sortableFields = new Dictionary<string, Func<TypeOfEnergyModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "TypeOfEnergyCode", x => x.TypeOfEnergyCode },
                        { "TypeOfEnergyName", x => x.TypeOfEnergyName }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<TypeOfEnergyModel> _data = _repoTypeOfEnergy._context.TypeOfEnergies.Select(x => new TypeOfEnergyModel
                {
                    TypeOfEnergyId = x.TypeOfEnergyId,
                    TypeOfEnergyCode = x.TypeOfEnergyCode ?? "",
                    TypeOfEnergyName = x.TypeOfEnergyName ?? "",
                    IsDel = x.IsDel
                });
                _data = _data.Where(x => !x.IsDel);

                //Kiểm tra điều kiện tìm kiếm
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();

                    //Lấy table đã select tìm kiếm theo keyword
                    _data = _data.Where(x =>
                       x.TypeOfEnergyName.ToLower().Contains(_keywordSearch) || x.TypeOfEnergyCode.ToLower().Contains(_keywordSearch)
                   );
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.TypeOfEnergyId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
                }

                //Đếm số dòng của table đã select được
                int _countRows = _data.Count();

                //nếu table = 0 thì trả về không có dữ liệu
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }

                //query more = true
                if (query.Panigator.More)
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }

                //Sắp xếp dữ liệu theo acs
                if (_orderBy_ASC)
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }

                //Sắp xếp dữ liệu theo desc
                else
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
            BaseModels<TypeOfEnergy> model = new BaseModels<TypeOfEnergy>();
            try
            {
                var result = _repoTypeOfEnergy.FindById(id);
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
        public async Task<IActionResult> Update(TypeOfEnergy data)
        {
            BaseModels<TypeOfEnergy> model = new BaseModels<TypeOfEnergy>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();

                TypeOfEnergy? SaveData = _repoTypeOfEnergy._context.TypeOfEnergies.Where(x => x.TypeOfEnergyId == data.TypeOfEnergyId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var typeOfEnergy = _repoTypeOfEnergy.findByTypeOfEnergyCode(data.TypeOfEnergyCode, Guid.Parse(data.TypeOfEnergyId.ToString()));

                    if (typeOfEnergy)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã loại hình năng lượng đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TYPE_OF_ENERGY, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.TypeOfEnergyId = Guid.Parse(data.TypeOfEnergyId.ToString());
                    SaveData.TypeOfEnergyCode = data.TypeOfEnergyCode;
                    SaveData.TypeOfEnergyName = data.TypeOfEnergyName;

                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoTypeOfEnergy.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TYPE_OF_ENERGY, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(TypeOfEnergy data)
        {
            BaseModels<TypeOfEnergy> model = new BaseModels<TypeOfEnergy>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var typeOfEnergy = _repoTypeOfEnergy.findByTypeOfEnergyCode(data.TypeOfEnergyCode, null);

                if (typeOfEnergy)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã loại hình năng lượng đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TYPE_OF_ENERGY, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                TypeOfEnergy SaveData = new TypeOfEnergy();
                SaveData.TypeOfEnergyCode = data.TypeOfEnergyCode;
                SaveData.TypeOfEnergyName = data.TypeOfEnergyName;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoTypeOfEnergy.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TYPE_OF_ENERGY, Action_Status.SUCCESS);
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

        [HttpPut("deleteTypeOfEnergy/{id}")]
        public async Task<IActionResult> deleteTypeOfEnergy(Guid id)
        {
            BaseModels<TypeOfEnergy> model = new BaseModels<TypeOfEnergy>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                TypeOfEnergy DeleteData = new TypeOfEnergy();
                DeleteData.TypeOfEnergyId = id;
                DeleteData.IsDel = true;
                await _repoTypeOfEnergy.DeleteTypeOfEnergy(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TYPE_OF_ENERGY, Action_Status.SUCCESS);
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

        [Route("deleteTypeOfEnergies")]
        [HttpPut()]
        public async Task<IActionResult> deleteTypeOfEnergies(removeListTypeOfEnergyItems data)
        {
            BaseModels<TypeOfEnergy> model = new BaseModels<TypeOfEnergy>();
            try
            {
                foreach (Guid id in data.TypeOfEnergyIds)
                {
                    TypeOfEnergy DeleteData = new TypeOfEnergy();
                    DeleteData.TypeOfEnergyId = id;
                    DeleteData.IsDel = true;
                    await _repoTypeOfEnergy.DeleteTypeOfEnergy(DeleteData);
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
