using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Models.TestModel;
using API_SoCongThuong.Reponsitories.TestRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Reflection.Metadata;
using System.Security.Principal;
using static API_SoCongThuong.Classes.Ulities;

namespace API_SoCongThuong.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private TestRepo _repo;
        private IConfiguration _config;
        private readonly IConnectionMultiplexer _redisCache;
        public TestController(SoHoa_SoCongThuongContext context, IConfiguration configuration, IConnectionMultiplexer redisCache)
        {
            _repo = new TestRepo(context);
            _config = configuration;
            _redisCache = redisCache;
        }

        //[Route("find_old")]
        //[HttpPost]
        //public IActionResult ListItems([FromBody] QueryRequestBody query)
        //{
        //    UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
        //    if (loginData == null)
        //    {
        //        return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
        //    }
        //    BaseModels<TblTest> model = new BaseModels<TblTest>();
        //    int total = 0;
        //    try
        //    {
        //        var result =_repo._context.TblTests.Select(dt => new TblTest
        //        {
        //            Id = dt.Id,
        //            Name = dt.Name,
        //        });
                
        //        //filter request have to right and enough , same in code
        //        Func<TblTest, bool> _filterByExpression = (dt => dt.Name.ToString().Contains(""));
        //        Dictionary<string, Func<TblTest, bool>> v_dic_keyFilter = new Dictionary<string, Func<TblTest, bool>>
        //                        {
        //                            { "Id", dt => dt.Id.ToString().Contains(query.Filter["Id"]) },
        //                            { "Name", dt => dt.Name.ToString().Contains(query.Filter["Name"]) }
        //                        };
        //        var a = _repo._context.TblTests.Any(x => x.Id == 10);
        //        var b = _repo._context.TblTests.Where(x => x.Id == 1).FirstOrDefault();
        //        if (b != null)
        //        {

        //        }
        //        var c = _repo._context.TblTests.Where(x => x.Id == 1).ToArray();
        //        if (query.Filter != null)
        //        {
        //            if (query.Filter.Count > 0)
        //            {
        //                //kiểm tra key filter có tồn tại trong list filter key ko?
        //                var listKeySearch = query.Filter.Where(x => !v_dic_keyFilter.ContainsKey(x.Key)).Select(q => q.Key).ToList();
        //                if (listKeySearch != null)
        //                    if (listKeySearch.Count > 0)
        //                    {
        //                        model.status = 0;
        //                        model.items = null;
        //                        model.error = new ErrorModel() { Code = ErrCode_Const.KEY_FILTER_NOT_EXISTS, Msg = ErrMsg_Const.GetMsg(ErrCode_Const.KEY_FILTER_NOT_EXISTS) };
        //                        return BadRequest(model);
        //                    }
        //                foreach (string _filter in query.Filter.Keys)
        //                {
        //                    _filterByExpression = v_dic_keyFilter[_filter];
        //                    if (!string.IsNullOrEmpty(query.Filter[_filter]) && v_dic_keyFilter[_filter] != null)
        //                    {
        //                        result = result.Where(_filterByExpression).ToList();
        //                    }
        //                }
        //            }
        //        }

        //        //search
        //        if (query.SearchValue != null && query.SearchValue != "")
        //        {
        //            result = result.Where(x =>
        //               x.Id.ToString().ToLower().Contains(query.SearchValue) || x.Name.ToLower().Contains(query.SearchValue)
        //           ).ToList();
        //        }
        //        total = result.Count();
        //        //sort column
        //        Func<TblTest, object> _orderByExpression = (dt => dt.Name); //Khởi tạo mặc định sắp xếp dữ liệu
        //        Dictionary<string, Func<TblTest, object>> _sortableFields = new Dictionary<string, Func<TblTest, object>>   //Khởi tạo các trường để sắp xếp
        //        {
        //            { "Id", dt => dt.Id },
        //            { "Name", dt => dt.Name }
        //        };
        //        if (query.Sort != null)
        //        {
        //            if (!string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
        //            {
        //                _orderByExpression = _sortableFields[query.Sort.ColumnName];
        //                if (query.Sort.Direction.ToLower().Equals("asc")) //Sắp xếp dữ liệu theo acs
        //                {
        //                    result = result.OrderBy(_orderByExpression).ToList();
        //                }
        //                else if (query.Sort.Direction.ToLower().Equals("desc")) //Sắp xếp dữ liệu theo desc
        //                {
        //                    result = result.OrderByDescending(_orderByExpression).ToList();
        //                }
        //            }
        //        }

        //        // PANIGATOR
        //        if (!query.Panigator.More)
        //        {
        //            result = result.AsEnumerable().Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize).Take(query.Panigator.PageSize).ToList();
        //        }

        //        //retrun not found
        //        if (result == null)
        //        {
        //            return NotFound();
        //        }
        //        //retrun data success
        //        model.status = 1;
        //        model.items = result.ToList();
        //        model.total = total;
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
        #region Danh sách test
        /// <summary>
        /// Lấy danh sách thông tin test
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("find")]
        [HttpPost]
        [CusAuthorize(Roles = "12, 23")]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {
           
            BaseModels<TestModel> model = new BaseModels<TestModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
               
                Func<TestModel, object> _orderByExpression = x => x.id; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<TestModel, object>> _sortableFields = new Dictionary<string, Func<TestModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "Id", x => x.id },
                    { "Name", x => x.name }
                };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                //Cách 1 dùng entity
                IQueryable<TestModel> _data = _repo._context.TblTests.Select(x => new TestModel
                {
                    id = x.Id,
                    name = x.Name??"",
                });

                //Cách 2 dùng linq
                //IQueryable<TestModel> _dataLinq = (from t in _repo._context.TblTests
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
                       x.id.ToString().ToLower().Contains(_keywordSearch)
                       || x.name.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("idGroupParent")  && !string.IsNullOrEmpty( query.Filter["idGroupParent"]))
                {
                    _data = _data.Where(x => x.id.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
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
                    model.items  = _data
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
        public IActionResult getItemById(long id)
        {
            BaseModels<TblTest> model = new BaseModels<TblTest>();
            try
            {
                var result = _repo.FindById(id);
                if (result == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                model.status = 1;
                model.items = result.ToList();
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
        public async Task<IActionResult> Update(TestModel data)
        {
            BaseModels<TblTest> model = new BaseModels<TblTest>();
            try
            {
                TblTest test = new TblTest();
                test.Id = long.Parse(data.id.ToString());
                test.Name = data.name;
                await _repo.Update(test);
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
        [HttpPost()]
        public async Task<IActionResult> create(TestModel data)
        {
            BaseModels<TblTest> model = new BaseModels<TblTest>();
            try
            {
                TblTest test = new TblTest();
                test.Name = data.name;
                await _repo.Insert(test);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> delete(long id)
        {
            BaseModels<TblTest> model = new BaseModels<TblTest>();
            try
            {
                await _repo.Delete(id);

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

        [Route("deleteItems")]
        [HttpPut()]
        public async Task<IActionResult> deleteItems(removeListItems data)
        {
            BaseModels<TblTest> model = new BaseModels<TblTest>();
            try
            {
                foreach (long id in data.ids)
                {
                    await _repo.Delete(id);
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

        [HttpGet]
        [Route("UploadFileAllTypeMinio")]
        public UploadResult UploadFileAllTypeMinio()
        {
            upLoadFileModel up = new upLoadFileModel()
            {
                bs = Convert.FromBase64String("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"),
                FileName = "a3ebda890753c80d9142.jpg",
                Linkfile = "FileAll" //Tên folder upload
            };
            return Ulities.UploadFile(up, _config);
        }

        [HttpGet]
        [Route("setCache")]
        public void setCache(string key, string value)
        {
            var db = _redisCache.GetDatabase();
            db.StringSet(key, value);
        }

        [HttpGet]
        [Route("getCache")]
        public string getCache(string key)
        {
            var db = _redisCache.GetDatabase();
            return db.StringGet(key);
        }

        [HttpGet]
        [Route("removeCache")]
        public bool removeCache(string key)
        {
            var db = _redisCache.GetDatabase();
            return db.KeyDelete(key);
        }

    }
}
