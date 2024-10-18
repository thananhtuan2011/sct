using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.PetroleumBusinessRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using API_SoCongThuong.Reponsitories.CommuneRepository;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetroleumBusinessController : ControllerBase
    {
        private PetroleumRepo _repo;
        private BusinessRepo _repoBusi;
        private DistrictRepository _repoDistrict;
        private CommuneRepo _repoCommune;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public PetroleumBusinessController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new PetroleumRepo(context);
            _repoBusi = new BusinessRepo(context);
            _repoDistrict = new DistrictRepository(context);
            _repoCommune = new CommuneRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }
        // Lấy danh sách danh mục kinh doanh xăng dầu
        #region Danh mục kinh doanh xăng dầu
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<PetroleumBusinessModel> model = new BaseModels<PetroleumBusinessModel>();
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

                Func<PetroleumBusinessModel, object> _orderByExpression = x => x.PetroleumBusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<PetroleumBusinessModel, object>> _sortableFields = new Dictionary<string, Func<PetroleumBusinessModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessNameVi", x => x.BusinessNameVi },
                        { "Supplier", x => x.Supplier },
                        //{ "Representative", x => x.Representative }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }


                //var lstData = _repoBusi._context.Businesses
                //                   .Where(c => !c.IsDel )
                //                   .ToList();

                IQueryable < PetroleumBusinessModel > _data = (_repo._context.PetroleumBusinesses.Join(
                    _repoBusi._context.Businesses,
                    cc => cc.PetroleumBusinessName, cd => cd.BusinessId,
                     (cc, cd) => new PetroleumBusinessModel

                     {
                         PetroleumBusinessId = cc.PetroleumBusinessId,
                         PetroleumBusinessName = cc.PetroleumBusinessName,
                         Address = cd.DiaChiTruSo ?? "",
                         PhoneNumber = cd.SoDienThoai ?? "",
                         Supplier = cc.Supplier,
                         Representative = cd.NguoiDaiDien ?? "",
                         IsDel = cc.IsDel,
                         BusinessNameVi = cd.BusinessNameVi,
                     })).ToList().AsQueryable();

                _data = _data.Where(x => !x.IsDel);

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.BusinessNameVi.ToLower().Contains(_keywordSearch) || x.Representative.ToLower().Contains(_keywordSearch) || x.Address.ToLower().Contains(_keywordSearch));  //Lấy table đã select tìm kiếm theo keyword
                }
                // model.items = _data.ToList();

                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }

               

                //Đoạn này lấy total đã tối ưu việc call DB nhiều lần

                var count_store = (from p in _repo._context.PetroleumBusinesses where !p.IsDel
                                   join ps in _repo._context.PetroleumBusinessStores
                                   on p.PetroleumBusinessId equals ps.PetroleumBusinessId
                                   select new
                                   {
                                       PetroleumBusinessName = p.PetroleumBusinessName,
                                       PetroleumDetailId = ps.PetroleumDetailId,
                                   }).GroupBy(x => x.PetroleumBusinessName).Select(x => new
                                   {
                                       PetroleumBusinessName = x.Key,
                                       TotalStore = x.ToList().Count(),
                                   }).ToList();
                model.items = _data.DistinctBy(x => x.PetroleumBusinessName).ToList();

                for (int i = 0; i < model.items.Count(); i++)
                {
                    model.items[i].TotalStore = count_store.Where(x => x.PetroleumBusinessName == model.items[i].PetroleumBusinessName).Select(x => x.TotalStore).FirstOrDefault(0);
                }
                if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                {
                    model.items = model.items
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else //Sắp xếp dữ liệu theo desc
                {
                    model.items = model.items
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }

                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.total = _countRows;
                    return Ok(model);
                }
                model.status = 1;
                model.total = model.items.Count();
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
                IQueryable<Business> _data = _repo.FindAll().Where(x => !x.IsDel);

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

        // tong cua hang
        private int totalStore(Guid id)
        {
            int tong = 0;
            var lstData = _repo._context.PetroleumBusinessStores
                               .Where(c => c.PetroleumBusinessId == id)
                               .ToList();
            lstData.ForEach(x =>
            {
                tong++;
            });

            return tong;
        }

        [HttpPost()]
        public async Task<IActionResult> create(PetroleumBusinessModel data)
        {
            BaseModels<PetroleumBusinessModel> model = new BaseModels<PetroleumBusinessModel>();
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.PETROLEUM, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(PetroleumBusinessModel data)
        {
            BaseModels<PetroleumBusiness> model = new BaseModels<PetroleumBusiness>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (_repo.IsExistPetroleumBusiness(data.PetroleumBusinessName, data.PetroleumBusinessId))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Doanh nghiệp đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PETROLEUM, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }

                data.UpdateUserId = loginData.Userid;
                data.UpdateTime = DateTime.Now;
                await _repo.Update(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PETROLEUM, Action_Status.SUCCESS);
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
            BaseModels<PetroleumBusinessModel> model = new BaseModels<PetroleumBusinessModel>();
            try
            {
                var petroinfo = _repo.FindByPetroleumBusinessId(id);
                if (petroinfo == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                //Tạo list
                List<PetroleumBusinessModel> lst = new List<PetroleumBusinessModel>();

                ////Tạo model
                PetroleumBusinessModel data = new PetroleumBusinessModel();
                data.PetroleumBusinessId = petroinfo.PetroleumBusinessId;
                data.PetroleumBusinessName = petroinfo.PetroleumBusinessName;
                data.Address = petroinfo.Address;
                data.Supplier = petroinfo.Supplier;
                data.PhoneNumber = petroinfo.PhoneNumber;
                data.Representative = petroinfo.Representative;
                data.PetroleumBusinessDetail = petroinfo.PetroleumBusinessDetail;
                data.BusinessNameVi = petroinfo.BusinessNameVi;
                data.GiayDangKyKinhDoanh = petroinfo.GiayDangKyKinhDoanh;
                data.NgayCap = petroinfo.NgayCap;
                ////Chuyển model về list
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
        [Route("delete")]
        [HttpPut()]
        public async Task<IActionResult> deleteBusinesses(removeListPetroleumBusinessItems data)
        {
            BaseModels<PetroleumBusiness> model = new BaseModels<PetroleumBusiness>();
            try
            {
                foreach (Guid id in data.PetroleumBusinessIds)
                {
                    PetroleumBusiness DeleteData = new PetroleumBusiness();
                    DeleteData.PetroleumBusinessId = id;
                    DeleteData.IsDel = true;
                    await _repo.DeletePetroleumBu(DeleteData);
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
            BaseModels<PetroleumBusiness> model = new BaseModels<PetroleumBusiness>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.DeleteById(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.PETROLEUM, Action_Status.SUCCESS);
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

        [Route("getDataPetroleum")]
        [HttpGet]
        public object getDataPetroleum()
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
                var petroleumData = from p in _repo._context.PetroleumBusinesses
                                    join b in _repoBusi._context.Businesses
                                    on p.PetroleumBusinessName equals b.BusinessId
                                    where !p.IsDel
                                    group b by b.DistrictId
                            into g
                                    select new { g.Key, Count = g.Count() };
                return petroleumData;
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
        // Load data cho dropdown hình thức
        #region
        [Route("loadformality")]
        [HttpGet]
        public IActionResult loadFormality()
        {
            BaseModels<Category> model = new BaseModels<Category>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<Category> _data = _repo._context.Categories.Where(x => x.CategoryTypeCode == "BUSINESS_FORMALITY");

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

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = _repo.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            var listId = data.Select(x => x.PetroleumBusinessId).ToList();
            var listSupplier = _repo._context.PetroleumBusinesses.Where(x => listId.Contains(x.PetroleumBusinessId)).Select(x =>
            new {
                PetroleumBusinessId = x.PetroleumBusinessId,
                Supplier = _repo._context.PetroleumBusinessStores.Where(z => z.PetroleumBusinessId == x.PetroleumBusinessId).FirstOrDefault().DonViCungCap,
            }).ToList();

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyCuaHangXangDau.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 4;
                    int row = 1;

                    foreach (var item in data)
                    {
                        var supplier = listSupplier.Where(x => x.PetroleumBusinessId == item.PetroleumBusinessId).FirstOrDefault();
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi.Trim();
                            worksheet.Cell(index, 3).Value = item.Address.Trim();
                            worksheet.Cell(index, 4).Value = item.Representative.Trim();
                            worksheet.Cell(index, 5).Value = item.PhoneNumber.Trim();
                            worksheet.Cell(index, 6).Value = supplier == null ? "" : supplier.Supplier;
                            worksheet.Cell(index, 7).Value = $"{item.TotalStore} cửa hàng";
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi.Trim();
                            worksheet.Cell(index, 3).Value = item.Address.Trim();
                            worksheet.Cell(index, 4).Value = item.Representative.Trim();
                            worksheet.Cell(index, 5).Value = item.PhoneNumber.Trim();
                            worksheet.Cell(index, 6).Value = supplier == null ? "" : supplier.Supplier;
                            worksheet.Cell(index, 7).Value = $"{item.TotalStore} cửa hàng";
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

        [HttpGet("loadDetail/{id}")]
        public IActionResult LoadDetail(Guid id)
        {
            BaseModels<PetroleumBusinessDetailModel> model = new BaseModels<PetroleumBusinessDetailModel>();
            try
            {
                var detail = (from pb in _context.PetroleumBusinessStores
                              where pb.PetroleumDetailId == id
                              join d in _context.Districts on pb.Huyen equals d.DistrictId
                              join c in _context.Communes on pb.Xa equals c.CommuneId
                              join cate in _context.Categories on pb.HinhThuc equals cate.CategoryId
                              join cate1 in _context.Categories on pb.LoaiGiayXacNhan equals cate1.CategoryId into pbcate1
                              from cate1 in pbcate1.DefaultIfEmpty()
                              join cate2 in _context.Categories on pb.HinhThucHopDong equals cate2.CategoryId into pbcate2
                              from cate2 in pbcate2.DefaultIfEmpty()
                              select new PetroleumBusinessDetailModel()
                              {
                                  TenCuaHang = pb.TenCuaHang,
                                  NguoiDaiDien = pb.NguoiDaiDien,
                                  SoDienThoai = pb.SoDienThoai,
                                  Huyen = pb.Huyen,
                                  TenHuyen = d.DistrictName,
                                  Xa = pb.Xa,
                                  TenXa = c.CommuneName,
                                  HinhThuc = pb.HinhThuc,
                                  TenHinhThuc = cate.CategoryName,
                                  DiaChi = pb.DiaChi,
                                  GiayPhepKinhDoanh = pb.GiayPhepKinhDoanh,
                                  NgayHetHan = pb.NgayHetHan,
                                  DonViCungCap = pb.DonViCungCap,
                                  ThoiHan5Nam = pb.ThoiHan5Nam,
                                  NguoiQuanLy = pb.NguoiQuanLy,
                                  NgayCapPhep = pb.NgayCapPhep,
                                  SoBeChua = pb.SoBeChua,
                                  SoCotBomOil = pb.SoCotBomOil,
                                  SoCotBomE5 = pb.SoCotBomE5,
                                  SoCotBomA95 = pb.SoCotBomA95,
                                  DienTichXayDung = pb.DienTichXayDung,
                                  ThoiGianBanHang = pb.ThoiGianBanHang,
                                  TongDungTich = pb.TongDungTich,
                                  TuyenPhucVu = pb.TuyenPhucVu,
                                  ThoiHan1Nam = pb.ThoiHan1Nam,
                                  NguoiLienHeDonViCungCap = pb.NguoiLienHeDonViCungCap,
                                  DiaChiDonViCungCap = pb.DiaChiDonViCungCap,
                                  SoDienThoaiDonViCungCap = pb.SoDienThoaiDonViCungCap,
                                  NgayCapPhepXayDung = pb.NgayCapPhepXayDung,
                                  GhiChu = pb.GhiChu,
                                  TenLoaiGiayXacNhan = cate1.CategoryName,
                                  TenHinhThucHopDong = cate2.CategoryName
                              }).ToList();

                //Set data cho base model
                model.status = 1;
                model.items = detail;
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
