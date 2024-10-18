
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.TypeOfProfessionRepository;
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
    public class TypeOfProfessionController : ControllerBase
    {
        private TypeOfProfessionRepo _repoTypeOfProfession;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public TypeOfProfessionController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoTypeOfProfession = new TypeOfProfessionRepo(context);
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

            BaseModels<TypeOfProfessionModel> model = new BaseModels<TypeOfProfessionModel>();
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

                Func<TypeOfProfessionModel, object> _orderByExpression = x => x.TypeOfProfessionCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TypeOfProfessionModel, object>> _sortableFields = new Dictionary<string, Func<TypeOfProfessionModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "TypeOfProfessionCode", x => x.TypeOfProfessionCode },
                        { "TypeOfProfessionName", x => x.TypeOfProfessionName }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<TypeOfProfessionModel> _data = _repoTypeOfProfession._context.TypeOfProfessions.Select(x => new TypeOfProfessionModel
                {
                    TypeOfProfessionId = x.TypeOfProfessionId,
                    TypeOfProfessionCode = x.TypeOfProfessionCode ?? "",
                    TypeOfProfessionName = x.TypeOfProfessionName ?? "",
                    IsDel = x.IsDel
                });
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
                       x.TypeOfProfessionName.ToLower().Contains(_keywordSearch) || x.TypeOfProfessionCode.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.TypeOfProfessionId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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
            BaseModels<TypeOfProfession> model = new BaseModels<TypeOfProfession>();
            try
            {
                var result = _repoTypeOfProfession.FindById(id);
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
        public async Task<IActionResult> Update(TypeOfProfession data)
        {
            BaseModels<TypeOfProfession> model = new BaseModels<TypeOfProfession>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                TypeOfProfession? SaveData = _repoTypeOfProfession._context.TypeOfProfessions.Where(x => x.TypeOfProfessionId == data.TypeOfProfessionId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    var typeOfProfession = _repoTypeOfProfession.findByTypeOfProfessionCode(data.TypeOfProfessionCode, Guid.Parse(data.TypeOfProfessionId.ToString()));

                    if (typeOfProfession)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã loại ngành nghề đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TYPE_OF_PROFESSION, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }
                    SaveData.TypeOfProfessionId = Guid.Parse(data.TypeOfProfessionId.ToString());
                    SaveData.TypeOfProfessionCode = data.TypeOfProfessionCode;
                    SaveData.TypeOfProfessionName = data.TypeOfProfessionName;
                    
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoTypeOfProfession.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.TYPE_OF_PROFESSION, Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(TypeOfProfession data)
        {
            BaseModels<TypeOfProfession> model = new BaseModels<TypeOfProfession>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var typeOfProfession = _repoTypeOfProfession.findByTypeOfProfessionCode(data.TypeOfProfessionCode, null);

                if (typeOfProfession)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã loại ngành nghề đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TYPE_OF_PROFESSION, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                TypeOfProfession SaveData = new TypeOfProfession();
                SaveData.TypeOfProfessionCode = data.TypeOfProfessionCode;
                SaveData.TypeOfProfessionName = data.TypeOfProfessionName;

                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoTypeOfProfession.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.TYPE_OF_PROFESSION, Action_Status.SUCCESS);
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

        [HttpPut("deleteTypeOfProfession/{id}")]
        public async Task<IActionResult> deleteTypeOfProfession(Guid id)
        {
            BaseModels<TypeOfProfession> model = new BaseModels<TypeOfProfession>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                TypeOfProfession DeleteData = new TypeOfProfession();
                DeleteData.TypeOfProfessionId = id;
                DeleteData.IsDel = true;
                await _repoTypeOfProfession.DeleteTypeOfProfession(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.TYPE_OF_PROFESSION, Action_Status.SUCCESS);
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

        [Route("deleteTypeOfProfessions")]
        [HttpPut()]
        public async Task<IActionResult> deleteTypeOfProfessions(removeListTypeOfProfessionItems data)
        {
            BaseModels<TypeOfProfession> model = new BaseModels<TypeOfProfession>();
            try
            {
                foreach (Guid id in data.TypeOfProfessionIds)
                {
                    TypeOfProfession DeleteData = new TypeOfProfession();
                    DeleteData.TypeOfProfessionId = id;
                    DeleteData.IsDel = true;
                    await _repoTypeOfProfession.DeleteTypeOfProfession(DeleteData);
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
