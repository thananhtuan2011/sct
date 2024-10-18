using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BusinessIndustryRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.IndustryRepository;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
using API_SoCongThuong.Reponsitories.TypeOfProfessionRepository;
using API_SoCongThuong.Reponsitories.PetroleumBusinessRepository;
using API_SoCongThuong.Reponsitories.CigaretteBusinessRepository;
using API_SoCongThuong.Reponsitories.AlcoholBusinessRepository;
using API_SoCongThuong.Reponsitories.InterCommerceRepository;
using API_SoCongThuong.Reponsitories.ExportGoodsRepository;
using API_SoCongThuong.Reponsitories.ImportGoodsRepository;
using API_SoCongThuong.Reponsitories.TradePromotionProjectManagementRepository;
using API_SoCongThuong.Reponsitories.MultiLevelSalesManagementRepository;
using API_SoCongThuong.Reponsitories;
using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Globalization;
using static API_SoCongThuong.Classes.Ulities;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using static API_SoCongThuong.Classes.ErrMsg_Const;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private BusinessRepo _repoBusiness;
        private IndustryRepo _repoIndustry;
        private TypeOfBusinessRepo _repoTypeOfBusiness;
        private TypeOfProfessionRepo _repoTypeOfProfession;
        private BusinessIndustryRepo _repoBusinessIndustry;
        private CigaretteBusinessRepo _repoCigaretteBusiness;
        private AlcoholBusinessRepo _repoAlcoholBusiness;
        private InterCommerceRepo _repoInterCommerce;
        private ExportGoodsRepo _repoExportGoods;
        private ImportGoodsRepo _repoImportGoods;
        private TradePromotionProjectManagementRepo _repoTradePromotionProjectManagement;
        private CateRPSAncolForFactoryRepo _repoCateRPSAncolForFactory;
        private CateRPPCrafttAncolForEconomicRepo _repoCateRPPCrafttAncolForEconomic;
        private CateRPSoldAncolRepo _repoCateRPSoldAncol;
        private CateRPProduceIndustlAncolRepo _repoCateRPProduceIndustlAncol;
        private CateRPTurnOverIndustAncolRepo _repoCateRPTurnOverIndustAncol;
        private CateManageAncolLocalBussinesRepo _repoCateManageAncolLocalBussines;
        private MultiLevelSalesManagementRepo _repoMultiLevelSalesManagement;
        private IndustrialPromotionProjectRepo _repoIndustrialPromotionProject;
        private ReportPromotionCommerceRepo _repoReportPromotionCommerce;

        private IConfiguration _config;
        //        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public BusinessController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoBusiness = new BusinessRepo(context);
            _repoIndustry = new IndustryRepo(context);
            _repoTypeOfBusiness = new TypeOfBusinessRepo(context);
            _repoTypeOfProfession = new TypeOfProfessionRepo(context);
            _repoBusinessIndustry = new BusinessIndustryRepo(context);
            _repoCigaretteBusiness = new CigaretteBusinessRepo(context);
            _repoAlcoholBusiness = new AlcoholBusinessRepo(context);
            _repoInterCommerce = new InterCommerceRepo(context);
            _repoExportGoods = new ExportGoodsRepo(context);
            _repoImportGoods = new ImportGoodsRepo(context);
            _repoTradePromotionProjectManagement = new TradePromotionProjectManagementRepo(context);
            _repoCateRPSAncolForFactory = new CateRPSAncolForFactoryRepo(context);
            _repoCateRPPCrafttAncolForEconomic = new CateRPPCrafttAncolForEconomicRepo(context);
            _repoCateRPSoldAncol = new CateRPSoldAncolRepo(context);
            _repoCateRPProduceIndustlAncol = new CateRPProduceIndustlAncolRepo(context);
            _repoCateRPTurnOverIndustAncol = new CateRPTurnOverIndustAncolRepo(context);
            _repoCateManageAncolLocalBussines = new CateManageAncolLocalBussinesRepo(context);
            _repoMultiLevelSalesManagement = new MultiLevelSalesManagementRepo(context);
            _repoIndustrialPromotionProject = new IndustrialPromotionProjectRepo(context);
            _repoReportPromotionCommerce = new ReportPromotionCommerceRepo(context);

            _config = configuration;
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
        }

        [Route("loadindustries")]
        [HttpGet]
        public IActionResult LoadIndustries()
        {
            BaseModels<IndustryModel> model = new BaseModels<IndustryModel>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<IndustryModel> _data = _repoIndustry._context.Industries.Select(x => new IndustryModel
                {
                    IndustryId = x.IndustryId,
                    IndustryCode = x.IndustryCode,
                    IndustryName = x.IndustryName ?? "",
                    ParentIndustryId = x.ParentIndustryId ?? Guid.Empty,
                    IsDel = x.IsDel,
                }).Where(n => !n.IsDel);

                model.status = 1;
                model.items = _data.ToList();

                var lstData = _repoIndustry._context.Industries
                    .Where(c => !c.IsDel)
                    .ToList();

                for (int i = 0; i < _data.Count(); i++)
                {
                    model.items[i].IndustryLevel = countLevel(model.items[i].IndustryId, lstData, 1);
                }

                var level = model.items.ToList();

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

        // Funtion tính cấp
        private int countLevel(Guid id, List<Industry> lstData, int curLevel)
        {
            if (lstData != null && lstData.Count() > 0)
            {
                var data = lstData.Where(x => x.IndustryId == id).FirstOrDefault();
                if (data == null)
                {
                    return 0;
                }
                else
                {
                    if (data.ParentIndustryId != null && data.ParentIndustryId.HasValue) // && data.ParentIndustryId != Guid.Empty
                    {
                        int nextLevel = curLevel + 1;
                        return countLevel(data.ParentIndustryId.Value, lstData, nextLevel);
                    }
                    else
                    {
                        return curLevel;
                    }
                }
            }
            else
            {
                return 0;
            }
        }

        [Route("loadtypeofbusinesses")]
        [HttpGet]
        public IActionResult LoadTypeOfBusiness()
        {
            BaseModels<TypeOfBusiness> model = new BaseModels<TypeOfBusiness>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<TypeOfBusiness> _data = _repoTypeOfBusiness.FindAll().Where(x => !x.IsDel);

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

        [Route("loadtypeofprofession")]
        [HttpGet]
        public IActionResult LoadTypeOfProfession()
        {
            BaseModels<TypeOfProfession> model = new BaseModels<TypeOfProfession>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<TypeOfProfession> _data = _repoTypeOfProfession.FindAll().Where(x => !x.IsDel);

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

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<BusinessModel> model = new BaseModels<BusinessModel>();
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

                Func<BusinessModel, object> _orderByExpression = x => x.BusinessCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<BusinessModel, object>> _sortableFields = new Dictionary<string, Func<BusinessModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessCode", x => x.BusinessCode },
                        { "TenGiaoDich", x => x.TenGiaoDich },
                        { "BusinessNameEn", x => x.BusinessNameEn },
                        { "DiaChiTruSo", x => x.DiaChiTruSo },
                        { "NgayCapPhep", x => x.NgayCapPhep },
                        { "MaSoThue", x => x.MaSoThue },
                        { "BusinessNameVi", x => x.BusinessNameVi },
                        { "LoaiHinhDoanhNghiep", x => x.LoaiHinhDoanhNghiep },
                        { "LoaiNganhNghe", x => x.LoaiNganhNghe },
                        { "NgayHoatDong", x => x.NgayHoatDong },
                        { "NguoiDaiDien", x => x.NguoiDaiDien },
                        { "SoDienThoai", x => x.SoDienThoai },
                        { "DiaChi", x => x.DiaChi },
                        { "GiamDoc", x => x.GiamDoc },
                        { "Email", x => x.Email },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity
                IQueryable<BusinessModel> _data = _repoBusiness._context.Businesses.Select(x => new BusinessModel
                {
                    BusinessId = x.BusinessId,
                    BusinessCode = x.BusinessCode ?? "",
                    TenGiaoDich = x.TenGiaoDich ?? "",
                    BusinessNameEn = x.BusinessNameEn ?? "",
                    DiaChiTruSo = x.DiaChiTruSo ?? "",
                    NgayCapPhep = x.NgayCapPhep,
                    MaSoThue = x.MaSoThue ?? "",
                    BusinessNameVi = x.BusinessNameVi ?? "",
                    LoaiHinhDoanhNghiep = x.LoaiHinhDoanhNghiep,
                    LoaiNganhNghe = x.LoaiNganhNghe ?? Guid.Empty,
                    NgayHoatDong = x.NgayHoatDong,
                    NguoiDaiDien = x.NguoiDaiDien ?? "",
                    SoDienThoai = x.SoDienThoai ?? "",
                    DiaChi = x.DiaChi ?? "",
                    GiamDoc = x.GiamDoc ?? "",
                    Email = x.Email ?? "",
                    IsDel = x.IsDel,
                    GiayPhepSanXuat = x.GiayPhepSanXuat,
                    LinkFileDisplay = _repoBusiness._context.BusinessLogos.Where(img => img.BusinessId == x.BusinessId).FirstOrDefault() == null ? "" :
                        (_config.GetValue<string>("MinioConfig:Protocol") + _config.GetValue<string>("MinioConfig:MinioServer") +
                        _repoBusiness._context.BusinessLogos.Where(img => img.BusinessId == x.BusinessId).FirstOrDefault().LinkFile)
                });
                _data = _data.Where(x => !x.IsDel);

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.BusinessNameVi.ToLower().Contains(_keywordSearch)
                       || x.BusinessCode.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                //if (query.Filter != null && query.Filter.ContainsKey("idGroupParent") && !string.IsNullOrEmpty(query.Filter["idGroupParent"]))
                //{
                //    _data = _data.Where(x => x.BusinessId.ToString().Contains(string.Join("", query.Filter["idGroupParent"])));
                //}
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
            BaseModels<GetBusinessModel> model = new BaseModels<GetBusinessModel>();
            try
            {
                var businessinfo = _repoBusiness.FindById(id).FirstOrDefault();

                if (businessinfo != null)
                {
                    var industrylistid = _repoBusinessIndustry.FindByBusinessId(id).Select(x => x.IndustryId).ToList();
                    var logoImage = _repoBusiness._context.BusinessLogos.Where(x => x.BusinessId == id).Select(x => new BusinessLogoModel
                    {
                        LogoId = x.LogoId,
                        BusinessId = x.BusinessId,
                        LinkFile = _config.GetValue<string>("MinioConfig:Protocol") + _config.GetValue<string>("MinioConfig:MinioServer") + x.LinkFile,
                    }).ToList();

                    //Tạo list
                    List<GetBusinessModel> lst = new List<GetBusinessModel>();

                    //Tạo model
                    GetBusinessModel data = new GetBusinessModel();
                    data.BusinessId = id;
                    data.BusinessCode = businessinfo.BusinessCode;
                    data.TenGiaoDich = businessinfo.TenGiaoDich;
                    data.BusinessNameEn = businessinfo.BusinessNameEn ?? "";
                    data.DistrictId = businessinfo.DistrictId ?? Guid.Empty;
                    data.CommuneId = businessinfo.CommuneId ?? Guid.Empty;
                    data.DiaChiTruSo = businessinfo.DiaChiTruSo ?? "";
                    data.NgayCapPhep = businessinfo.NgayCapPhep.HasValue ? businessinfo.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : null;
                    data.MaSoThue = businessinfo.MaSoThue ?? "";
                    data.BusinessNameVi = businessinfo.BusinessNameVi;
                    data.LoaiHinhDoanhNghiep = businessinfo.LoaiHinhDoanhNghiep;
                    data.LoaiNganhNghe = businessinfo.LoaiNganhNghe ?? Guid.Empty;
                    data.NgayHoatDong = businessinfo.NgayHoatDong.HasValue ? businessinfo.NgayHoatDong.Value.ToString("dd'/'MM'/'yyyy") : null;
                    data.NguoiDaiDien = businessinfo.NguoiDaiDien ?? "";
                    data.SoDienThoai = businessinfo.SoDienThoai ?? "";
                    data.NgaySinh = businessinfo.NgaySinh.HasValue ? businessinfo.NgaySinh.Value.ToString("dd'/'MM'/'yyyy") : null;
                    data.Cccd = businessinfo.Cccd ?? "";
                    data.NgayCap = businessinfo.NgayCap.HasValue ? businessinfo.NgayCap.Value.ToString("dd'/'MM'/'yyyy") : null;
                    data.NoiCap = businessinfo.NoiCap ?? "";
                    data.DiaChi = businessinfo.DiaChi ?? "";
                    data.GiamDoc = businessinfo.GiamDoc ?? "";
                    data.Email = businessinfo.Email ?? "";
                    data.IndustryId = industrylistid;
                    data.GiayPhepSanXuat = businessinfo.GiayPhepSanXuat ?? "";
                    data.Details = logoImage;

                    //Chuyển model về list
                    lst.Add(data);

                    //Set data cho base model
                    model.status = 1;
                    model.items = lst;
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

        [HttpGet("User/{id}")]
        public IActionResult getUserInfoById(Guid id)
        {
            BaseModels<viewdata> model = new BaseModels<viewdata>();
            try
            {
                var businessinfo = _repoBusiness.FindById(id).FirstOrDefault();

                if (businessinfo == null)
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
                    var industrylistid = _repoBusinessIndustry.FindByBusinessId(id)
                                          .Select(x => x.IndustryId)
                                          .ToList();

                    var industrylist = _repoIndustry._context.Industries.Where(x => industrylistid.Contains(x.IndustryId)).Select(x => new listindustry
                    {
                        IndustryCode = x.IndustryCode,
                        IndustryName = x.IndustryName,
                    }
                    ).ToList();

                    var typeofbusinessdata = _repoTypeOfBusiness._context.TypeOfBusinesses
                                             .Where(x => x.TypeOfBusinessId == businessinfo.LoaiHinhDoanhNghiep)
                                             .Select(x => x.TypeOfBusinessName).FirstOrDefault();

                    var typeofprofessiondata = _repoTypeOfProfession._context.TypeOfProfessions
                                               .Where(x => x.TypeOfProfessionId == businessinfo.LoaiNganhNghe)
                                               .Select(x => x.TypeOfProfessionName)
                                               .FirstOrDefault();

                    viewdata result = new viewdata();

                    result.databusiness = businessinfo;
                    result.dataindustry = industrylist;
                    result.datatypeofbusiness = typeofbusinessdata;
                    result.datatypeofprofession = typeofprofessiondata;

                    model.status = 1;
                    model.data = result;
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
        public async Task<IActionResult> Update([FromForm] GetBusinessModel data)
        {
            BaseModels<BusinessModel> model = new BaseModels<BusinessModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                Business? SaveData = _repoBusiness._context.Businesses.Where(x => x.BusinessId == data.BusinessId && !x.IsDel).FirstOrDefault();

                if (SaveData != null)
                {
                    var business = _repoBusiness.findByBusinessCode(data.BusinessCode, Guid.Parse(data.BusinessId.ToString()));

                    if (business)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Mã số doanh nghiệp đã tồn tại"
                        };
                        datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.BUSINESS, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }

                    var util = new Ulities();
                    data = util.TrimModel(data);

                    SaveData.BusinessId = Guid.Parse(data.BusinessId.ToString());
                    SaveData.BusinessCode = data.BusinessCode;
                    SaveData.TenGiaoDich = data.TenGiaoDich;
                    SaveData.BusinessNameEn = data.BusinessNameEn;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.DiaChiTruSo = data.DiaChiTruSo;
                    SaveData.NgayCapPhep = data.NgayCapPhep != null ? DateTime.ParseExact(data.NgayCapPhep, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    SaveData.MaSoThue = data.MaSoThue;
                    SaveData.BusinessNameVi = data.BusinessNameVi;
                    SaveData.LoaiHinhDoanhNghiep = data.LoaiHinhDoanhNghiep;
                    SaveData.LoaiNganhNghe = data.LoaiNganhNghe;
                    SaveData.NgayHoatDong = data.NgayHoatDong != null ? DateTime.ParseExact(data.NgayHoatDong, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    SaveData.NguoiDaiDien = data.NguoiDaiDien;
                    SaveData.SoDienThoai = data.SoDienThoai;
                    SaveData.NgaySinh = data.NgaySinh != null ? DateTime.ParseExact(data.NgaySinh, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    SaveData.Cccd = data.Cccd;
                    SaveData.NgayCap = data.NgayCap != null ? DateTime.ParseExact(data.NgayCap, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                    SaveData.NoiCap = data.NoiCap;
                    SaveData.DiaChi = data.DiaChi;
                    SaveData.GiamDoc = data.GiamDoc;
                    SaveData.Email = data.Email;
                    SaveData.GiayPhepSanXuat = data.GiayPhepSanXuat;
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoBusiness.Update(SaveData);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.BUSINESS, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));

                    List<BusinessIndustry> BusinessIndustryOld = _repoBusinessIndustry._context.BusinessIndustries.Where(x => x.BusinessId == data.BusinessId).ToList();
                    if (BusinessIndustryOld != null && BusinessIndustryOld.Count > 0)
                    {
                        await _repoBusinessIndustry.DeleteOld(BusinessIndustryOld);
                    }

                    if (!String.IsNullOrEmpty(data.IndustryIdString))
                    {
                        data.IndustryId = JsonConvert.DeserializeObject<List<Guid>>(data.IndustryIdString);
                        if (data.IndustryId != null && data.IndustryId.Count > 0)
                        {
                            List<BusinessIndustry> businessIndustryNew = new List<BusinessIndustry>();
                            foreach (var industry in data.IndustryId)
                            {
                                BusinessIndustry BusinessIndustryItem = new BusinessIndustry();
                                BusinessIndustryItem.Id = Guid.Empty;
                                BusinessIndustryItem.BusinessId = data.BusinessId;
                                BusinessIndustryItem.IndustryId = industry;
                                businessIndustryNew.Add(BusinessIndustryItem);
                            }
                            await _repoBusinessIndustry.InsertNew(businessIndustryNew);
                        }
                    }

                    //Nếu có thay đổi logo thì Files.Any() == true, không thì Files.Any() == false;
                    var Files = Request.Form.Files;
                    if (Files.Any())
                    {
                        #region Xóa Logo cũ
                        var del = _repoBusiness._context.BusinessLogos.Where(d => d.BusinessId == data.BusinessId).ToList();
                        foreach (var fdel in del)
                        {
                            string linkdel = fdel.LinkFile;
                            var result = Ulities.RemoveFileMinio(linkdel, _config);
                        }
                        _repoBusiness._context.BusinessLogos.RemoveRange(del);
                        await _repoBusiness._context.SaveChangesAsync();
                        #endregion

                        #region Update Logo mới
                        var LstFile = new List<BusinessLogo>();
                        foreach (var f in Files)
                        {
                            if (f.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    f.CopyTo(ms);
                                    upLoadFileModel up = new upLoadFileModel()
                                    {
                                        bs = ms.ToArray(),
                                        FileName = f.FileName.Replace(" ", ""),
                                        Linkfile = "BusinessLogo"
                                    };
                                    var result = Ulities.UploadFile(up, _config);

                                    BusinessLogo fileSave = new BusinessLogo();
                                    fileSave.LinkFile = result.link;
                                    fileSave.BusinessId = data.BusinessId;
                                    LstFile.Add(fileSave);
                                }
                            }
                        }
                        await _repoBusiness.InsertLogo(LstFile);
                        #endregion
                    }

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
        public async Task<IActionResult> Create([FromForm] GetBusinessModel data)
        {
            BaseModels<Business> model = new BaseModels<Business>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var business = _repoBusiness.findByBusinessCode(data.BusinessCode, null);
                SystemLog datalog = new SystemLog();
                if (business)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Mã số doanh nghiệp đã tồn tại"
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.BUSINESS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                var util = new Ulities();
                data = util.TrimModel(data);

                Business businessData = new Business();

                businessData.BusinessCode = data.BusinessCode;
                businessData.TenGiaoDich = data.TenGiaoDich;
                businessData.BusinessNameEn = data.BusinessNameEn;
                businessData.DistrictId = data.DistrictId == Guid.Empty ? null : data.DistrictId;
                businessData.CommuneId = data.CommuneId == Guid.Empty ? null : data.CommuneId;
                businessData.DiaChiTruSo = data.DiaChiTruSo;
                businessData.NgayCapPhep = data.NgayCapPhep != null ? DateTime.ParseExact(data.NgayCapPhep, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                businessData.MaSoThue = data.MaSoThue;
                businessData.BusinessNameVi = data.BusinessNameVi;
                businessData.LoaiHinhDoanhNghiep = data.LoaiHinhDoanhNghiep;
                businessData.LoaiNganhNghe = data.LoaiNganhNghe == Guid.Empty ? null : data.LoaiNganhNghe;
                businessData.NgayHoatDong = data.NgayHoatDong != null ? DateTime.ParseExact(data.NgayHoatDong, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                businessData.NguoiDaiDien = data.NguoiDaiDien;
                businessData.SoDienThoai = data.SoDienThoai;
                businessData.NgaySinh = data.NgaySinh != null ? DateTime.ParseExact(data.NgaySinh, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                businessData.Cccd = data.Cccd;
                businessData.NgayCap = data.NgayCap != null ? DateTime.ParseExact(data.NgayCap, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                businessData.NoiCap = data.NoiCap;
                businessData.DiaChi = data.DiaChi;
                businessData.GiamDoc = data.GiamDoc;
                businessData.Email = data.Email;
                businessData.CreateUserId = loginData.Userid;
                businessData.CreateTime = DateTime.Now;
                businessData.GiayPhepSanXuat = data.GiayPhepSanXuat;

                await _repoBusiness.Insert(businessData);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.BUSINESS, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));

                if (!String.IsNullOrEmpty(data.IndustryIdString))
                {
                    data.IndustryId = JsonConvert.DeserializeObject<List<Guid>>(data.IndustryIdString);
                    if (data.IndustryId != null && data.IndustryId.Count() > 0)
                    {
                        List<BusinessIndustry> businessIndustryNew = new List<BusinessIndustry>();
                        foreach (var industry in data.IndustryId)
                        {
                            BusinessIndustry BusinessIndustryItem = new BusinessIndustry();
                            BusinessIndustryItem.Id = Guid.Empty;
                            BusinessIndustryItem.BusinessId = businessData.BusinessId;
                            BusinessIndustryItem.IndustryId = industry;
                            businessIndustryNew.Add(BusinessIndustryItem);
                        }
                        await _repoBusinessIndustry.InsertNew(businessIndustryNew);
                    }
                }

                var Files = Request.Form.Files;
                if (Files.Any())
                {
                    var LstFile = new List<BusinessLogo>();
                    foreach (var f in Files)
                    {
                        if (f.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                f.CopyTo(ms);
                                upLoadFileModel up = new upLoadFileModel()
                                {
                                    bs = ms.ToArray(),
                                    FileName = f.FileName.Replace(" ", ""),
                                    Linkfile = "BusinessLogo"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                BusinessLogo fileSave = new BusinessLogo();
                                fileSave.LinkFile = result.link;
                                fileSave.BusinessId = businessData.BusinessId;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    await _repoBusiness.InsertLogo(LstFile);
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

        [HttpPut("deleteBusiness/{id}")]
        public async Task<IActionResult> deleteBusiness(Guid id)
        {
            BaseModels<Business> model = new BaseModels<Business>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var countBusinessIndustry = _repoBusinessIndustry._context.PetroleumBusinesses.Where(x => x.PetroleumBusinessName == id && !x.IsDel).Count();
                var countCigaretteBusiness = _repoCigaretteBusiness._context.CigaretteBusinesses.Where(x => x.CigaretteBusinessName == id && !x.IsDel).Count();
                var countAlcoholBusiness = _repoAlcoholBusiness._context.AlcoholBusinesses.Where(x => x.AlcoholBusinessName == id && !x.IsDel).Count();
                var countInterCommerce = _repoInterCommerce._context.InternationalCommerces.Where(x => x.InternationalCommerceName == id && !x.IsDel).Count();
                var countExportGoods = _repoExportGoods._context.ExportGoods.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countImportGoods = _repoImportGoods._context.ImportGoods.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countTradePromotionProjectManagement = _repoTradePromotionProjectManagement._context.TradePromotionProjectManagementBussinesses.Where(x => x.BusinessId == id).Count();
                var countCateRPSAncolForFactory = _repoCateRPSAncolForFactory._context.CateReportSoldAncolForFactoryLicenses.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countCateRPPCrafttAncolForEconomic = _repoCateRPPCrafttAncolForEconomic._context.CateReportProduceCrafttAncolForEconomics.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countCateRPSoldAncol = _repoCateRPSoldAncol._context.CateReportSoldAncols.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countCateRPProduceIndustlAncol = _repoCateRPProduceIndustlAncol._context.CateReportProduceIndustlAncols.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countCateRPTurnOverIndustAncol = _repoCateRPTurnOverIndustAncol._context.CateReportTurnOverIndustAncols.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countCateManageAncolLocalBussines = _repoCateManageAncolLocalBussines._context.CateManageAncolLocalBussines.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countMultiLevelSalesManagement = _repoMultiLevelSalesManagement._context.MultiLevelSalesManagements.Where(x => x.BusinessId == id && !x.IsDel).Count();
                var countIndustrialPromotionProject = _repoIndustrialPromotionProject._context.IndustrialPromotionProjectDetails.Where(x => x.BusinessId == id).Count();
                var countReportPromotionCommerce = _repoReportPromotionCommerce._context.ReportPromotionCommerces.Where(x => x.Business == id && !x.IsDel).Count();
                var countParticipateSupportFairDetail = _repoBusiness._context.ParticipateSupportFairDetails.Where(x => x.BusinessId == id).Count();

                if (countBusinessIndustry > 0 || countCigaretteBusiness > 0 || countAlcoholBusiness > 0 || countInterCommerce > 0 || countExportGoods > 0 || countImportGoods > 0 || countTradePromotionProjectManagement > 0 || countCateRPSAncolForFactory > 0 || countCateRPPCrafttAncolForEconomic > 0 || countCateRPSoldAncol > 0 || countCateRPProduceIndustlAncol > 0 || countCateRPTurnOverIndustAncol > 0 || countCateManageAncolLocalBussines > 0 || countMultiLevelSalesManagement > 0 || countIndustrialPromotionProject > 0 || countReportPromotionCommerce > 0 || countParticipateSupportFairDetail > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Dữ liệu đang được sử dụng ở trang khác"
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.BUSINESS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                Business DeleteData = new Business();
                DeleteData.BusinessId = id;
                DeleteData.IsDel = true;
                await _repoBusiness.DeleteBusiness(DeleteData);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.BUSINESS, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                List<BusinessIndustry> BusinessIndustryOld = _repoBusinessIndustry._context.BusinessIndustries.Where(x => x.BusinessId == id).ToList();
                if (BusinessIndustryOld != null && BusinessIndustryOld.Count > 0)
                {
                    await _repoBusinessIndustry.DeleteOld(BusinessIndustryOld);
                }

                await _repoBusiness.DeleteLogo(id);

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

        [Route("getDataBusiness")]
        [HttpGet]
        public object getDataBusiness()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var query = from b in _repoBusiness._context.Businesses
                            where !b.IsDel
                            group b by b.DistrictId
                            into g
                            select new { g.Key, Count = g.Count() };

                return query;
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

        [Route("getBusinessName")]
        [HttpGet]
        public object getBusinessName()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var query = from b in _repoBusiness._context.Businesses
                            where !b.IsDel
                            select new { b.BusinessNameVi };

                return query;
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
            var data = _repoBusiness.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhmucdoanhnghiep.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 3;
                    int row = 1;

                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi.ToUpper().Trim();
                            worksheet.Cell(index, 3).Value = item.NgayCapPhep.HasValue ? item.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : "";
                            worksheet.Cell(index, 4).Value = item.NgayHoatDong.HasValue ? item.NgayHoatDong.Value.ToString("dd'/'MM'/'yyyy") : "";
                            worksheet.Cell(index, 5).Value = item.NguoiDaiDien?.Trim();
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi.ToUpper().Trim();
                            worksheet.Cell(index, 3).Value = item.NgayCapPhep.HasValue ? item.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : "";
                            worksheet.Cell(index, 4).Value = item.NgayHoatDong.HasValue ? item.NgayHoatDong.Value.ToString("dd'/'MM'/'yyyy") : "";
                            worksheet.Cell(index, 5).Value = item.NguoiDaiDien?.Trim();
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
