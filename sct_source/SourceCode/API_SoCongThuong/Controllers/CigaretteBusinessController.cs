using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.CigaretteBusinessRepository;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CigaretteBusinessController : ControllerBase
    {
        private CigaretteBusinessRepo _repo;
        private BusinessRepo _repoBusi;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public CigaretteBusinessController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CigaretteBusinessRepo(context);
            _repoBusi = new BusinessRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        // Lấy danh sách danh mục kinh doanh ciga
        #region Danh mục kinh doanh ciga
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<CigaretteBusinessModel> model = new BaseModels<CigaretteBusinessModel>();
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

                Func<CigaretteBusinessModel, object> _orderByExpression = x => x.CigaretteBusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CigaretteBusinessModel, object>> _sortableFields = new Dictionary<string, Func<CigaretteBusinessModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessNameVi", x => x.BusinessNameVi },
                        { "Supplier", x => x.Supplier }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<CigaretteBusinessModel> _data = _repo._context.CigaretteBusinesses.Join(
                    _repoBusi._context.Businesses,
                    cc => cc.CigaretteBusinessName, cd => cd.BusinessId,
                     (cc, cd) => new CigaretteBusinessModel
                     {
                         CigaretteBusinessId = cc.CigaretteBusinessId,
                         CigaretteBusinessName = cc.CigaretteBusinessName,
                         Address = cc.Address ?? "",
                         PhoneNumber = cd.SoDienThoai ?? "",
                         Supplier = cc.Supplier,
                         Representative = cd.NguoiDaiDien ?? "",
                         IsDel = cc.IsDel,
                         BusinessNameVi = cd.BusinessNameVi,
                     }).ToList().AsQueryable();
                _data = _data.Where(x => !x.IsDel).DistinctBy(x => x.CigaretteBusinessName);

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.BusinessNameVi.ToLower().Contains(_keywordSearch) || x.Representative.ToLower().Contains(_keywordSearch));  //Lấy table đã select tìm kiếm theo keyword
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
        // Load data cho loại hình doanh nghiệp
        #region Danh sách loại hình doanh nghiệp
        [Route("loadbusinesses")]
        [HttpGet]
        public IActionResult LoadBusiness()
        {
            BaseModels<Business> model = new BaseModels<Business>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<Business> _data = _repoBusi.FindAll().Where(x => !x.IsDel);

                //Trả data về model
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
        [HttpPost()]
        public async Task<IActionResult> create(CigaretteBusinessModel data)
        {
            BaseModels<CigaretteBusinessModel> model = new BaseModels<CigaretteBusinessModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                //if (_repo.IsExistCigaretteBusiness(data.CigaretteBusinessName, data.CigaretteBusinessId))
                //{
                //    model.status = 0;
                //    model.error = new ErrorModel()
                //    {
                //        Code = ErrCode_Const.EXCEPTION_API,
                //        Msg = "Doanh nghiệp đã tồn tại"
                //    };
                //    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CIGARETTE, Action_Status.FAIL);
                //    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                //    return Ok(model);
                //}
                data.CreateUserId = loginData.Userid;
                data.CreateTime = new DateTime();
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CIGARETTE, Action_Status.SUCCESS);
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<CigaretteBusinessModel> model = new BaseModels<CigaretteBusinessModel>();
            try
            {
                var cigaretteInfo = _repo.FindByCigaretteBusinessId(id);
                if (cigaretteInfo == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));
                //Tạo list
                List<CigaretteBusinessModel> lst = new List<CigaretteBusinessModel>();
                //Tạo model
                CigaretteBusinessModel data = new CigaretteBusinessModel();
                data.CigaretteBusinessId = cigaretteInfo.CigaretteBusinessId;
                data.CigaretteBusinessName = cigaretteInfo.CigaretteBusinessName;
                data.Address = cigaretteInfo.Address;
                data.Supplier = cigaretteInfo.Supplier;
                data.PhoneNumber = cigaretteInfo.PhoneNumber;
                data.Representative = cigaretteInfo.Representative;
                data.CigaretteBusinessDetail = cigaretteInfo.CigaretteBusinessDetail;
                data.BusinessNameVi = cigaretteInfo.BusinessNameVi;
                data.GiayDangKyKinhDoanh = cigaretteInfo.GiayDangKyKinhDoanh;
                data.NgayCap = cigaretteInfo.NgayCap;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CigaretteBusinessModel data)
        {
            BaseModels<CigaretteBusinessModel> model = new BaseModels<CigaretteBusinessModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (_repo.IsExistCigaretteBusiness(data.CigaretteBusinessName, data.CigaretteBusinessId))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Doanh nghiệp đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CIGARETTE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }

                data.UpdateUserId = loginData.Userid;
                data.UpdateTime = DateTime.Now;
                await _repo.Update(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CIGARETTE, Action_Status.SUCCESS);
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

        [Route("delete")]
        [HttpPut()]
        public async Task<IActionResult> deleteBusinesses(removeListCigaretteBusinessItems data)
        {
            BaseModels<CigaretteBusiness> model = new BaseModels<CigaretteBusiness>();
            try
            {
                foreach (Guid id in data.CigaretteBusinessIds)
                {
                    CigaretteBusiness DeleteData = new CigaretteBusiness();
                    DeleteData.CigaretteBusinessId = id;
                    DeleteData.IsDel = true;
                    await _repo.Delete(DeleteData);
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
        [HttpPut("delete/{id}")]
        public async Task<IActionResult> deleteCigaBusiness(Guid id)
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
                await _repo.DeleteById(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CIGARETTE, Action_Status.SUCCESS);
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

        [Route("getDataCigarette")]
        [HttpGet]
        public object getDataCigarette()
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
                var CigaretteData = from p in _repo._context.CigaretteBusinesses
                                    join b in _repoBusi._context.Businesses
                                    on p.CigaretteBusinessName equals b.BusinessId
                                    where !p.IsDel
                                    group b by b.DistrictId
                                    into g
                                    select new { g.Key, Count = g.Count() };
                return CigaretteData;
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

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = _repo.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            var listId = data.Select(x => x.CigaretteBusinessId).ToList();
            var listSupplier = _repo._context.CigaretteBusinesses.Where(x => listId.Contains(x.CigaretteBusinessId)).Select(x =>
            new
            {
                CigaretteBusinessId = x.CigaretteBusinessId,
                Supplier = _repo._context.CigaretteBusinessStores.Where(z => z.CigaretteBusinessId == x.CigaretteBusinessId).FirstOrDefault().DonViCungCap,
            }).ToList();

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyDonViKinhDoanhThuocLa.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 4;
                    int row = 1;

                    foreach (var item in data)
                    {
                        var supplier = listSupplier.Where(x => x.CigaretteBusinessId == item.CigaretteBusinessId).FirstOrDefault();
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi.Trim();
                            worksheet.Cell(index, 3).Value = item.Representative.Trim();
                            worksheet.Cell(index, 4).Value = item.PhoneNumber.Trim();
                            worksheet.Cell(index, 5).Value = supplier == null ? "" : supplier.Supplier;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi.Trim();
                            worksheet.Cell(index, 3).Value = item.Representative.Trim();
                            worksheet.Cell(index, 4).Value = item.PhoneNumber.Trim();
                            worksheet.Cell(index, 5).Value = supplier == null ? "" : supplier.Supplier;
                            index++;
                            row++;
                        }
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
