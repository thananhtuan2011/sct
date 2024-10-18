using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.AlcoholBusinessRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.InterCommerceRepository;
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
    public class InternationalCommerceController : ControllerBase
    {
        private InterCommerceRepo _repo;
        private BusinessRepo _repoBusi;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public InternationalCommerceController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new InterCommerceRepo(context);
            _repoBusi = new BusinessRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<InterCommerceModel> model = new BaseModels<InterCommerceModel>();
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

                Func<InterCommerceModel, object> _orderByExpression = x => x.BusinessNameVi; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<InterCommerceModel, object>> _sortableFields = new Dictionary<string, Func<InterCommerceModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessNameVi", x => x.BusinessNameVi },
                        { "Address", x => x.Address },
                        { "GiayPhepBanLe", x => x.GiayPhepBanLe },
                        { "NgayCapGiayPhepBanLe", x => x.NgayCapGiayPhepBanLe },
                        { "GiayPhepKinhDoanh" , x => x.GiayPhepKinhDoanh },

                        { "Representative", x => x.Representative },
                        { "PhoneNumber", x => x.PhoneNumber },
                        { "TenLoaiHinhCoSo", x => x.TenLoaiHinhCoSo },
                        { "LicensingActivity", x => x.LicensingActivity },
                        { "InvestorName", x => x.InvestorName },
                        { "QuyMoCoSoBanLe", x => x.DienTichSuDung },
                        { "GhiChu", x => x.GhiChu }

                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                var _data = from ic in _repo._context.InternationalCommerces
                            where !ic.IsDel
                            join b in _repo._context.Businesses on ic.InternationalCommerceName equals b.BusinessId
                            join c in _repo._context.Categories on ic.LoaiHinhCoSo equals c.CategoryId into icc
                            from c in icc.DefaultIfEmpty()

                            select new InterCommerceModel()
                            {
                                InternationalCommerceName = ic.InternationalCommerceName,
                                InternationalCommerceId = ic.InternationalCommerceId,
                                Address = b.DiaChiTruSo ?? "",
                                InvestorName = ic.InvestorName ?? "",
                                LicensingActivity = ic.LicensingActivity ?? "",
                                IsDel = ic.IsDel,
                                PhoneNumber = b.SoDienThoai,
                                Representative = b.NguoiDaiDien,
                                BusinessNameVi = b.BusinessNameVi,
                                TenCoSoBanLe = ic.TenCoSoBanLe,
                                GiayPhepKinhDoanh = b.GiayPhepSanXuat,
                                NgayCapGiayPhepKinhDoanh = b.NgayCapPhep,
                                TenLoaiHinhCoSo = c.CategoryName,
                                DienTichSuDung = ic.DienTichSuDung,
                                DienTichKinhDoanh = ic.DienTichKinhDoanh,
                                DienTichBanHang = ic.DienTichBanHang,
                                DienTichSan = ic.DienTichSan,
                                GhiChu = ic.GhiChu,
                                GiayPhepBanLe = ic.GiayPhepBanLe,
                                NgayCapGiayPhepBanLe = ic.NgayCapGiayPhepBanLe
                            };
                _data = _data.Where(x => !x.IsDel);

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.BusinessNameVi.ToLower().Contains(_keywordSearch) || x.InvestorName.ToLower().Contains(_keywordSearch)
                    || x.GiayPhepBanLe.ToLower().Contains(_keywordSearch)
                    || x.GiayPhepKinhDoanh.ToLower().Contains(_keywordSearch)
                    || x.Address.ToLower().Contains(_keywordSearch)
                    || x.InvestorName.ToLower().Contains(_keywordSearch)
                    || x.TenLoaiHinhCoSo.ToLower().Contains(_keywordSearch)
                    || x.LicensingActivity.ToLower().Contains(_keywordSearch)
                    || x.GhiChu.ToLower().Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
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
        public async Task<IActionResult> create(InterCommerceModel data)
        {
            BaseModels<InterCommerceModel> model = new BaseModels<InterCommerceModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                InternationalCommerce interCommerceData = new InternationalCommerce
                {
                    //insert thong tin moi
                    InternationalCommerceName = data.InternationalCommerceName,
                    InvestorName = data.InvestorName,
                    Address = data.Address,
                    LicensingActivity = data.LicensingActivity,
                    CreateUserId = loginData.Userid,
                    CreateTime = new DateTime(),
                    TenCoSoBanLe = data.TenCoSoBanLe,
                    DiaChiCoSoBanLe = data.DiaChiCoSoBanLe,
                    LoaiHinhCoSo = data.LoaiHinhCoSo,
                    GiayPhepKinhDoanh = data.GiayPhepKinhDoanh,
                    NgayCapGiayPhepKinhDoanh = data.NgayCapGiayPhepKinhDoanh,
                    GiayPhepBanLe = data.GiayPhepBanLe,
                    NgayCapGiayPhepBanLe = data.NgayCapGiayPhepBanLe,
                    NgayHetHanGiayPhepBanLe = data.NgayHetHanGiayPhepBanLe,
                    DienTichSuDung = data.DienTichSuDung,
                    DienTichSan = data.DienTichSan,
                    DienTichBanHang = data.DienTichBanHang,
                    DienTichKinhDoanh = data.DienTichKinhDoanh,
                    GhiChu = data.GhiChu
                };
                await _repo.Insert(interCommerceData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INTER_COMMERCE, Action_Status.SUCCESS);
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
        #endregion
        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<InterCommerceModel> model = new BaseModels<InterCommerceModel>();
            try
            {
                var interCommerce = _repo.FindByInternationalCommerceId(id);
                if (interCommerce == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));
                //Tạo list
                List<InterCommerceModel> lst = new List<InterCommerceModel>();
                //Tạo model
                InterCommerceModel data = new InterCommerceModel();
                data.InternationalCommerceName = interCommerce.InternationalCommerceName;
                data.InternationalCommerceId = interCommerce.InternationalCommerceId;
                data.InvestorName = interCommerce.InvestorName;
                data.LicensingActivity = interCommerce.LicensingActivity;
                data.Address = interCommerce.Address;
                data.TenCoSoBanLe = interCommerce.TenCoSoBanLe;
                data.DiaChiCoSoBanLe = interCommerce.DiaChiCoSoBanLe;
                data.LoaiHinhCoSo = interCommerce.LoaiHinhCoSo;
                data.GiayPhepKinhDoanh = interCommerce.GiayPhepKinhDoanh;
                data.NgayCapGiayPhepKinhDoanh = interCommerce.NgayCapGiayPhepKinhDoanh;
                data.GiayPhepBanLe = interCommerce.GiayPhepBanLe;
                data.GiayPhepBanLe = interCommerce.GiayPhepBanLe;
                data.NgayCapGiayPhepBanLe = interCommerce.NgayCapGiayPhepBanLe;
                data.NgayHetHanGiayPhepBanLe = interCommerce.NgayHetHanGiayPhepBanLe;
                data.DienTichSuDung = interCommerce.DienTichSuDung;
                data.DienTichSan = interCommerce.DienTichSan;
                data.DienTichBanHang = interCommerce.DienTichBanHang;
                data.DienTichKinhDoanh = interCommerce.DienTichKinhDoanh;
                data.GhiChu = interCommerce.GhiChu;
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
        public async Task<IActionResult> Update(InternationalCommerce data)
        {
            BaseModels<InternationalCommerce> model = new BaseModels<InternationalCommerce>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                InternationalCommerce SaveData = new InternationalCommerce();
                SaveData.InternationalCommerceName = Guid.Parse(data.InternationalCommerceName.ToString());
                SaveData.InternationalCommerceId = data.InternationalCommerceId;
                SaveData.InvestorName = data.InvestorName;
                SaveData.LicensingActivity = data.LicensingActivity;
                SaveData.Address = data.Address;
                SaveData.UpdateUserId = loginData.Userid;
                SaveData.UpdateTime = DateTime.Now;
                SaveData.TenCoSoBanLe = data.TenCoSoBanLe;
                SaveData.DiaChiCoSoBanLe = data.DiaChiCoSoBanLe;
                SaveData.LoaiHinhCoSo = data.LoaiHinhCoSo;
                SaveData.GiayPhepKinhDoanh = data.GiayPhepKinhDoanh;
                SaveData.NgayCapGiayPhepKinhDoanh = data.NgayCapGiayPhepKinhDoanh;
                SaveData.GiayPhepBanLe = data.GiayPhepBanLe;
                SaveData.NgayCapGiayPhepBanLe = data.NgayCapGiayPhepBanLe;
                SaveData.NgayHetHanGiayPhepBanLe = data.NgayHetHanGiayPhepBanLe;
                SaveData.DienTichSuDung = data.DienTichSuDung;
                SaveData.DienTichSan = data.DienTichSan;
                SaveData.DienTichBanHang = data.DienTichBanHang;
                SaveData.DienTichKinhDoanh = data.DienTichKinhDoanh;
                SaveData.GhiChu = data.GhiChu;
                await _repo.Update(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INTER_COMMERCE, Action_Status.SUCCESS);
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
        public async Task<IActionResult> delete(removeListInterCommerceItems data)
        {
            BaseModels<InternationalCommerce> model = new BaseModels<InternationalCommerce>();
            try
            {
                foreach (Guid id in data.InternationalCommerceIds)
                {
                    InternationalCommerce DeleteData = new InternationalCommerce();
                    DeleteData.InternationalCommerceId = id;
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
        public async Task<IActionResult> deleteById(Guid id)
        {
            BaseModels<InternationalCommerce> model = new BaseModels<InternationalCommerce>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                InternationalCommerce DeleteData = new InternationalCommerce();
                DeleteData.InternationalCommerceId = id;
                DeleteData.IsDel = true;
                await _repo.DeleteById(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.INTER_COMMERCE, Action_Status.SUCCESS);
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

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = _repo.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyDauTuNuocNgoai.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 4;
                    int row = 1;
                    worksheet.Cell(1, 1).Value = "Quản lý thương mại quốc tế";

                    foreach (var item in data)
                    {
                        var str = "";
                        if(item.DienTichSuDung != 0)
                        {
                            str += $"Tổng diện tích đất sử dụng cho dự án lập cơ sở bán lẻ: {item.DienTichSuDung} m2 \r\n";
                        }
                        if(item.DienTichSan != 0)
                        {
                            str += $"Tổng diện tích sàn xây dựng: {item.DienTichSan} m2 \r\n";
                        }
                        if(item.DienTichBanHang != 0)
                        {
                            str += $"Diện tích bán hàng: {item.DienTichBanHang} m2 \r\n";
                        }
                        if(item.DienTichKinhDoanh != 0)
                        {
                            str += $"Diện tích kinh doanh dịch vụ: {item.DienTichKinhDoanh} m2";
                        }
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi;
                            worksheet.Cell(index, 3).Value = item.Address;
                            worksheet.Cell(index, 4).Value = item.GiayPhepBanLe;
                            worksheet.Cell(index, 5).Value = item.NgayCapGiayPhepBanLe != null ? item.NgayCapGiayPhepBanLe?.ToString("dd/MM/yyyy") : "";
                            worksheet.Cell(index, 6).Value = item.GiayPhepKinhDoanh;
                            worksheet.Cell(index, 7).Value = item.NgayCapGiayPhepKinhDoanh != null ? item.NgayCapGiayPhepKinhDoanh?.ToString("dd/MM/yyyy") : "";
                            worksheet.Cell(index, 8).Value = item.Representative;
                            worksheet.Cell(index, 9).Value = item.PhoneNumber;
                            worksheet.Cell(index, 10).Value = item.TenLoaiHinhCoSo;
                            worksheet.Cell(index, 11).Value = str;
                            worksheet.Cell(index, 12).Value = item.LicensingActivity;
                            worksheet.Cell(index, 13).Value = item.InvestorName;
                            worksheet.Cell(index, 14).Value = item.GhiChu;

                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi;
                            worksheet.Cell(index, 3).Value = item.Address;
                            worksheet.Cell(index, 4).Value = item.GiayPhepBanLe;
                            worksheet.Cell(index, 5).Value = item.NgayCapGiayPhepBanLe != null ? item.NgayCapGiayPhepBanLe?.ToString("dd/MM/yyyy") : "";
                            worksheet.Cell(index, 6).Value = item.GiayPhepKinhDoanh;
                            worksheet.Cell(index, 7).Value = item.NgayCapGiayPhepKinhDoanh != null ? item.NgayCapGiayPhepKinhDoanh?.ToString("dd/MM/yyyy") : "";
                            worksheet.Cell(index, 8).Value = item.Representative;
                            worksheet.Cell(index, 9).Value = item.PhoneNumber;
                            worksheet.Cell(index, 10).Value = item.TenLoaiHinhCoSo;
                            worksheet.Cell(index, 11).Value = str;
                            worksheet.Cell(index, 12).Value = item.LicensingActivity;
                            worksheet.Cell(index, 13).Value = item.InvestorName;
                            worksheet.Cell(index, 14).Value = item.GhiChu;

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
