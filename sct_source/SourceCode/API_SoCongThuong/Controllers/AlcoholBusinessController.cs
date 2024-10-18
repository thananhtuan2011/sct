using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.AlcoholBusinessRepository;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using API_SoCongThuong.Reponsitories.CigaretteBusinessRepository;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CommuneRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlcoholBusinessController : ControllerBase
    {
        private AlcoholBusinessRepo _repo;
        private BusinessRepo _repoBusi;
        private DistrictRepository _repoDistrict;
        private CommuneRepo _repoCommune;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public AlcoholBusinessController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new AlcoholBusinessRepo(context);
            _repoBusi = new BusinessRepo(context);
            _repoDistrict = new DistrictRepository(context);
            _repoCommune = new CommuneRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<AlcoholBusinessModel> model = new BaseModels<AlcoholBusinessModel>();
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

                Func<AlcoholBusinessModel, object> _orderByExpression = x => x.AlcoholBusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<AlcoholBusinessModel, object>> _sortableFields = new Dictionary<string, Func<AlcoholBusinessModel, object>>   //Khởi tạo các trường để sắp xếp
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

                IQueryable<AlcoholBusinessModel> _data = _repo._context.AlcoholBusinesses.Join(
                    _repoBusi._context.Businesses,
                    cc => cc.AlcoholBusinessName, cd => cd.BusinessId,
                     (cc, cd) => new AlcoholBusinessModel
                     {
                         AlcoholBusinessName = cc.AlcoholBusinessName,
                         AlcoholBusinessId = cc.AlcoholBusinessId,
                         Address = cc.Address ?? "",
                         PhoneNumber = cd.SoDienThoai ?? "",
                         Supplier = cc.Supplier,
                         Representative = cd.NguoiDaiDien ?? "",
                         IsDel = cc.IsDel,
                         BusinessNameVi = cd.BusinessNameVi,
                     }).ToList().AsQueryable(); ;
                _data = _data.Where(x => !x.IsDel).DistinctBy(x => x.AlcoholBusinessName);

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
        [HttpPost()]
        public async Task<IActionResult> create(AlcoholBusinessModel data)
        {
            BaseModels<AlcoholBusinessModel> model = new BaseModels<AlcoholBusinessModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                data.CreateUserId = loginData.Userid;
                data.CreateTime = new DateTime();
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ALCOHOL, Action_Status.SUCCESS);
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
        [Route("loaddistrict")]
        [HttpGet]
        public IActionResult LoadDistrict()
        {
            BaseModels<DistrictMarketModel> district_model = new BaseModels<DistrictMarketModel>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<DistrictMarketModel> _data = _repoDistrict._context.Districts.Where(x => !x.IsDel).Select(x => new DistrictMarketModel
                {
                    DistrictId = x.DistrictId,
                    DistrictCode = x.DistrictCode,
                    DistrictName = x.DistrictName,
                });

                district_model.status = 1;
                district_model.items = _data.ToList();
                return Ok(district_model);
            }
            catch (Exception ex)
            {
                district_model.status = 0;
                district_model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(district_model);
            }
        }

        [Route("loadcommune")]
        [HttpGet]
        public IActionResult LoadCommune()
        {
            BaseModels<CommuneMarketModel> Commune_model = new BaseModels<CommuneMarketModel>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<CommuneMarketModel> _data = _repoCommune._context.Communes.Where(x => !x.IsDel).Select(x => new CommuneMarketModel
                {
                    CommuneId = x.CommuneId,
                    CommuneCode = x.CommuneCode,
                    CommuneName = x.CommuneName,
                    DistrictId = x.DistrictId,
                });

                Commune_model.status = 1;
                Commune_model.items = _data.ToList();
                return Ok(Commune_model);
            }
            catch (Exception ex)
            {
                Commune_model.status = 0;
                Commune_model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(Commune_model);
            }
        }
        #endregion
        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<AlcoholBusinessModel> model = new BaseModels<AlcoholBusinessModel>();
            try
            {
                var alcoinfo = _repo.FindByAlcoholBusinessId(id);
                if (alcoinfo == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));
                //Tạo list
                List<AlcoholBusinessModel> lst = new List<AlcoholBusinessModel>();
                //Tạo model
                AlcoholBusinessModel data = new AlcoholBusinessModel();
                data.AlcoholBusinessId = alcoinfo.AlcoholBusinessId;
                data.AlcoholBusinessName = alcoinfo.AlcoholBusinessName;
                data.Address = alcoinfo.Address;
                data.Supplier = alcoinfo.Supplier;
                data.PhoneNumber = alcoinfo.PhoneNumber;
                data.Representative = alcoinfo.Representative;
                data.AlcoholBusinessDetail = alcoinfo.AlcoholBusinessDetail;
                data.BusinessNameVi = alcoinfo.BusinessNameVi;
                data.GiayDangKyKinhDoanh = alcoinfo.GiayDangKyKinhDoanh;
                data.NgayCapPhep = alcoinfo.NgayCapPhep;
                data.GiayPhepBanBuon = alcoinfo.GiayPhepBanBuon;
                data.NgayCapGiayPhepBanBuon = alcoinfo.NgayCapGiayPhepBanBuon;
                data.NgayHetHanGiayPhepBanBuon = alcoinfo.NgayHetHanGiayPhepBanBuon;
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
        public async Task<IActionResult> Update(AlcoholBusinessModel data)
        {
            BaseModels<AlcoholBusinessModel> model = new BaseModels<AlcoholBusinessModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (_repo.IsExistAlcoholBusiness(data.AlcoholBusinessName, data.AlcoholBusinessId))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Doanh nghiệp đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ALCOHOL, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }

                data.UpdateUserId = loginData.Userid;
                data.UpdateTime = DateTime.Now;
                await _repo.Update(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ALCOHOL, Action_Status.SUCCESS);
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
        public async Task<IActionResult> deleteBusinesses(removeListAlcoholBusinessItems data)
        {
            BaseModels<AlcoholBusiness> model = new BaseModels<AlcoholBusiness>();
            try
            {
                foreach (Guid id in data.AlcoholBusinessIds)
                {
                    AlcoholBusiness DeleteData = new AlcoholBusiness();
                    DeleteData.AlcoholBusinessId = id;
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
        public async Task<IActionResult> deletePetroleumBusiness(Guid id)
        {
            BaseModels<AlcoholBusiness> model = new BaseModels<AlcoholBusiness>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.DeleteById(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ALCOHOL, Action_Status.SUCCESS);
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

        [Route("getAlcohol")]
        [HttpGet]
        public object getAlcohol()
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
                var data = from p in _repo._context.AlcoholBusinesses
                           join b in _repoBusi._context.Businesses
                           on p.AlcoholBusinessName equals b.BusinessId
                           where !p.IsDel
                           group b by b.DistrictId
                                    into g
                           select new { g.Key, Count = g.Count() };
                return data;
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

            var listId = data.Select(x => x.AlcoholBusinessId).ToList();
            var listSupplier = _repo._context.AlcoholBusinesses.Where(x => listId.Contains(x.AlcoholBusinessId)).Select(x =>
            new
            {
                AlcoholBusinessId = x.AlcoholBusinessId,
                Supplier = _repo._context.AlcoholBussinessDetails.Where(z => z.AlcoholBusinessId == x.AlcoholBusinessId).FirstOrDefault().DonViCungCap,
            }).ToList();

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyDonViBuonBanRuou.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 4;
                    int row = 1;

                    foreach (var item in data)
                    {
                        var supplier = listSupplier.Where(x => x.AlcoholBusinessId == item.AlcoholBusinessId).FirstOrDefault();
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

        #region Danh sách giấy phép bán buôn rượu
        [Route("ListAlcoholWholesaleLicense")]
        [HttpGet]
        public IActionResult ListAlcoholWholesaleLicense()
        {
            BaseModels<AlcoholWholesaleLicense> model = new BaseModels<AlcoholWholesaleLicense>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<AlcoholWholesaleLicense> _data = _repo._context.AlcoholWholesaleLicenses.Where(x => !x.IsDel).Select(d => new AlcoholWholesaleLicense()
                {
                    BusinessId = d.BusinessId,
                    LicenseNumber = d.LicenseNumber,
                    LicenseDate = d.LicenseDate,
                    ExpirationDate = d.ExpirationDate
                });

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
    }
}
