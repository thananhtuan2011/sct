using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CommonRepository;
using DocumentFormat.OpenXml.Office2010.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private CommonRepo _repo;
        public CommonController(SoHoa_SoCongThuongContext context)
        {
            _repo = new CommonRepo(context);
        }


        #region Danh sách chức vụ
        [Route("list-chuc-vu")]
        [HttpGet]
        public IActionResult LoadDanhSachChucVu()
        {
            BaseModels<StateTitle> model = new BaseModels<StateTitle>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<StateTitle> _data = _repo._context.StateTitles.Where(x => !x.IsDel).Select(d => new StateTitle()
                {
                    StateTitlesId = d.StateTitlesId,
                    StateTitlesCode = d.StateTitlesCode,
                    StateTitlesName = d.StateTitlesName,
                    IsDel = d.IsDel,
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
        #region Danh sách đơn vị
        [Route("list-don-vi")]
        [HttpGet]
        public IActionResult LoadDanhSachDonVi()
        {
            BaseModels<StateUnit> model = new BaseModels<StateUnit>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<StateUnit> _data = _repo._context.StateUnits.Where(x => !x.IsDel).Select(d => new StateUnit()
                {
                    StateUnitsId = d.StateUnitsId,
                    StateUnitsCode = d.StateUnitsCode,
                    StateUnitsName = d.StateUnitsName,
                    IsDel = d.IsDel,
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
        #region Danh sách User
        [Route("list-user")]
        [HttpGet]
        public IActionResult LoadDanhSachUser()
        {
            BaseModels<User> model = new BaseModels<User>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<User> _data = _repo._context.Users.Where(x => x.IsDel == false).Select(d => new User()
                {
                    UserId = d.UserId,
                    UserName = d.UserName,
                    FullName = d.FullName,
                    IsDel = d.IsDel,
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
        #region Danh sách doanh nghiệp
        [Route("list-doanh-nghiep")]
        [HttpGet]
        public IActionResult LoadDanhSachDoanhNghiep()
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

                IQueryable<Business> _data = _repo._context.Businesses.Where(x => !x.IsDel).Select(d => new Business()
                {
                    BusinessId = d.BusinessId,
                    BusinessCode = d.BusinessCode,
                    BusinessNameVi = d.BusinessNameVi.ToUpper().Trim(),
                    DiaChiTruSo = d.DiaChiTruSo,
                    DiaChi = d.DiaChi,
                    NguoiDaiDien = d.NguoiDaiDien,
                    LoaiNganhNghe = d.LoaiNganhNghe,
                    SoDienThoai = d.SoDienThoai,
                    GiayPhepSanXuat = d.GiayPhepSanXuat,
                    NgayCapPhep = d.NgayCapPhep,
                    GiamDoc = d.GiamDoc,
                    DistrictId = d.DistrictId,
                    CommuneId = d.CommuneId,
                    IsDel = d.IsDel,
                    BusinessNameEn = d.BusinessNameEn,
                    MaSoThue = d.MaSoThue,
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
        #region Danh sách xã 
        [Route("list-xa")]
        [HttpGet]
        public IActionResult LoadDanhSachXa()
        {
            BaseModels<Commune> model = new BaseModels<Commune>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<Commune> _data = _repo._context.Communes.Where(x => !x.IsDel).Select(d => new Commune()
                {
                    CommuneId = d.CommuneId,
                    CommuneCode = d.CommuneCode,
                    CommuneName = d.CommuneName,
                    DistrictId = d.DistrictId,
                    IsDel = d.IsDel,
                }).OrderBy(x => x.CommuneName);

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
        #region Danh sách quốc gia xuất khẩu, nhập khẩu
        [Route("list-country")]
        [HttpGet]
        public IActionResult LoadDanhSachCountry()
        {
            BaseModels<Country> model = new BaseModels<Country>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<Country> _data = _repo._context.Countries.Where(x => !x.IsDel).Select(d => new Country()
                {
                    CountryId = d.CountryId,
                    CountryCode = d.CountryCode,
                    CountryName = d.CountryName,
                    IsDel = d.IsDel,
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
        #region Danh sách loại hình chợ
        [Route("list-typeofmarket")]
        [HttpGet]
        public IActionResult LoadDanhSachTypeofMarket()
        {
            BaseModels<TypeOfMarket> model = new BaseModels<TypeOfMarket>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<TypeOfMarket> _data = _repo._context.TypeOfMarkets.Where(x => !x.IsDel).Select(d => new TypeOfMarket()
                {
                    TypeOfMarketId = d.TypeOfMarketId,
                    TypeOfMarketName = d.TypeOfMarketName,
                    TypeOfMarketCode = d.TypeOfMarketCode,
                    IsDel = d.IsDel,
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
        #region Danh sách loại ngành nghề
        [Route("list-typeofprofession")]
        [HttpGet]
        public IActionResult LoadDanhSachTypeofProfession()
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
                IQueryable<TypeOfProfession> _data = _repo._context.TypeOfProfessions.Where(x => !x.IsDel).Select(d => new TypeOfProfession()
                {
                    TypeOfProfessionId = d.TypeOfProfessionId,
                    TypeOfProfessionName = d.TypeOfProfessionName,
                    TypeOfProfessionCode = d.TypeOfProfessionCode,
                    IsDel = d.IsDel,
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
        #region Danh sách loại hình năng lượng
        [Route("list-typeofenergy")]
        [HttpGet]
        public IActionResult LoadDanhSachTypeofEnergy()
        {
            BaseModels<TypeOfEnergy> model = new BaseModels<TypeOfEnergy>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<TypeOfEnergy> _data = _repo._context.TypeOfEnergies.Where(x => !x.IsDel).Select(d => new TypeOfEnergy()
                {
                    TypeOfEnergyId = d.TypeOfEnergyId,
                    TypeOfEnergyName = d.TypeOfEnergyName,
                    TypeOfEnergyCode = d.TypeOfEnergyCode,
                    IsDel = d.IsDel,
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
        #region Danh sách hoạt động ngành năng lượng
        [Route("list-energyindustry")]
        [HttpGet]
        public IActionResult LoadDanhSachEnergyIndustry()
        {
            BaseModels<EnergyIndustry> model = new BaseModels<EnergyIndustry>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<EnergyIndustry> _data = _repo._context.EnergyIndustries.Where(x => !x.IsDel).Select(d => new EnergyIndustry()
                {
                    EnergyIndustryId = d.EnergyIndustryId,
                    EnergyIndustryName = d.EnergyIndustryName,
                    EnergyIndustryCode = d.EnergyIndustryCode,
                    IsDel = d.IsDel,
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
        #region Danh sách loại hình xúc tiến thương mại
        [Route("list-typeoftradepromotion")]
        [HttpGet]
        public IActionResult LoadDanhSachTypeoftradePromotion()
        {
            BaseModels<TypeOfTradePromotion> model = new BaseModels<TypeOfTradePromotion>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<TypeOfTradePromotion> _data = _repo._context.TypeOfTradePromotions.Where(x => !x.IsDel).Select(d => new TypeOfTradePromotion()
                {
                    TypeOfTradePromotionId = d.TypeOfTradePromotionId,
                    TypeOfTradePromotionName = d.TypeOfTradePromotionName,
                    TypeOfTradePromotionCode = d.TypeOfTradePromotionCode,
                    IsDel = d.IsDel,
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
        #region Danh sách đề án xúc tiến thương mại  
        [Route("list-tradepromotionproject")]
        [HttpGet]
        public IActionResult LoadDanhSachTradePromotionProject()
        {
            BaseModels<TradePromotionProject> model = new BaseModels<TradePromotionProject>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<TradePromotionProject> _data = _repo._context.TradePromotionProjects.Where(x => !x.IsDel).Select(d => new TradePromotionProject()
                {
                    TradePromotionProjectId = d.TradePromotionProjectId,
                    TradePromotionProjectCode = d.TradePromotionProjectCode,
                    TradePromotionProjectName = d.TradePromotionProjectName,
                    IsDel = d.IsDel,
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
        #region Danh sách loại hình doanh nghiệp 
        [Route("list-typeofbusiness")]
        [HttpGet]
        public IActionResult LoadDanhSachTypeOfBusiness()
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
                IQueryable<TypeOfBusiness> _data = _repo._context.TypeOfBusinesses.Where(x => !x.IsDel).Select(d => new TypeOfBusiness()
                {
                    TypeOfBusinessId = d.TypeOfBusinessId,
                    TypeOfBusinessCode = d.TypeOfBusinessCode,
                    TypeOfBusinessName = d.TypeOfBusinessName,
                    IsDel = d.IsDel,
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
        #region Danh sách ngành hàng 
        [Route("list-industry")]
        [HttpGet]
        public IActionResult LoadDanhSachIndustry()
        {
            BaseModels<Industry> model = new BaseModels<Industry>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<Industry> _data = _repo._context.Industries.Where(x => !x.IsDel).Select(d => new Industry()
                {
                    IndustryId = d.IndustryId,
                    IndustryCode = d.IndustryCode,
                    IndustryName = d.IndustryName,
                    IsDel = d.IsDel,
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
        #region Danh sách huyện  
        [Route("list-district")]
        [HttpGet]
        public IActionResult LoadDanhSachDistrict()
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
                IQueryable<District> _data = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictCode = d.DistrictCode,
                    DistrictName = d.DistrictName,
                    IsDel = d.IsDel,
                }).OrderBy(x => x.DistrictName);

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
        #region Danh sách đơn vị  
        [Route("list-stateunits")]
        [HttpGet]
        public IActionResult LoadDanhSachStateUnit()
        {
            BaseModels<StateUnit> model = new BaseModels<StateUnit>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<StateUnit> _data = _repo._context.StateUnits.Where(x => !x.IsDel).Select(d => new StateUnit()
                {
                    StateUnitsId = d.StateUnitsId,
                    StateUnitsCode = d.StateUnitsCode,
                    StateUnitsName = d.StateUnitsName,
                    IsDel = d.IsDel,
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
        #region Danh sách chức vụ
        [Route("list-statetitles")]
        [HttpGet]
        public IActionResult LoadDanhSachStateTitle()
        {
            BaseModels<StateTitle> model = new BaseModels<StateTitle>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<StateTitle> _data = _repo._context.StateTitles.Where(x => !x.IsDel).Select(d => new StateTitle()
                {
                    StateTitlesId = d.StateTitlesId,
                    StateTitlesCode = d.StateTitlesCode,
                    StateTitlesName = d.StateTitlesName,
                    IsDel = d.IsDel,
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
        #region Danh sách thủ tục hành chính
        [Route("list-adminformalities")]
        [HttpGet]
        public IActionResult LoadDanhSachAdministrativeFormality()
        {
            BaseModels<AdministrativeFormality> model = new BaseModels<AdministrativeFormality>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<AdministrativeFormality> _data = _repo._context.AdministrativeFormalities.Where(x => !x.IsDel).Select(d => new AdministrativeFormality()
                {
                    AdminFormalitiesId = d.AdminFormalitiesId,
                    AdminFormalitiesName = d.AdminFormalitiesName,
                    AdminFormalitiesCode = d.AdminFormalitiesCode,
                    IsDel = d.IsDel,
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
        #region Danh sách chỉ tiêu
        [Route("list-cri")]
        [HttpGet]
        public IActionResult LoadDanhSachCri()
        {
            BaseModels<CateCriterion> model = new BaseModels<CateCriterion>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<CateCriterion> _data = _repo._context.CateCriteria.Where(x => !x.IsDel).Select(d => new CateCriterion()
                {
                    CateCriteriaId = d.CateCriteriaId,
                    CateCriteriaName = d.CateCriteriaName,
                    IsDel = d.IsDel,
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
        #region Danh sách ngành hàng
        [Route("list-businessline")]
        [HttpGet]
        public IActionResult LoadDanhSachNganhHang()
        {
            BaseModels<BusinessLine> model = new BaseModels<BusinessLine>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<BusinessLine> _data = _repo._context.BusinessLines.Where(x => !x.IsDel).Select(d => new BusinessLine()
                {
                    BusinessLineId = d.BusinessLineId,
                    BusinessLineName = d.BusinessLineName,
                    IsDel = d.IsDel,
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
        #region Danh sách chợ, siêu thị, trung tâm thương mại
        [Route("list-commercial")]
        [HttpGet]
        public IActionResult LoadDanhSachChoSieuThi()
        {
            BaseModels<CommercialManagement> model = new BaseModels<CommercialManagement>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                IQueryable<CommercialManagement> _data = _repo._context.CommercialManagements.Where(x => !x.IsDel).Select(d => new CommercialManagement()
                {
                    CommercialId = d.CommercialId,
                    Name = d.Name,
                    DistrictId = d.DistrictId,
                    CommuneId = d.CommuneId
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

        [Route("GetConfig")]
        [HttpPost]
        public IActionResult GetConfig([FromBody] string Code)
        {
            BaseModels<ConfigModel> model = new BaseModels<ConfigModel>();
            try
            {
                var errorModel = new ErrorModel();
                if (string.IsNullOrEmpty(Code))
                {
                    errorModel = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(new { status = 0, error = errorModel });
                }

                var data = _repo._context.Categories
                           .Where(x => x.CategoryTypeCode == Code && !x.IsDel && x.IsAction == true)
                           .Select(f => new ConfigModel()
                           {
                               CategoryId = f.CategoryId,
                               CategoryTypeCode = f.CategoryTypeCode,
                               CategoryCode = f.CategoryCode,
                               CategoryName = f.CategoryName,
                               Priority = f.Piority,
                               IsAction = f.IsAction ?? false,
                           })
                           .OrderBy(x => x.Priority);

                if (data.Any())
                {
                    var result = new ListConfigModel();
                    result.ListConfig = data.ToList();
                    return Ok(new { status = 1, items = result });
                }
                else
                {
                    var result = new ListConfigModel();
                    return Ok(new { status = 1, items = result });
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

        #region Danh sách cấp độ điện áp
        [Route("ListVoltageLevel")]
        [HttpGet]
        public IActionResult loadListVoltageLevel()
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
                IQueryable<Category> _data = _repo._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "VOLTAGE_LEVEL").Select(d => new Category()
                {
                    CategoryId = d.CategoryId,
                    CategoryCode = d.CategoryCode,
                    CategoryName = d.CategoryName
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

        #region Danh sách loại công trình
        [Route("ListTypeOfConstruction")]
        [HttpGet]
        public IActionResult loadListTypeOfConstruction()
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
                IQueryable<Category> _data = _repo._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "TYPE_OF_CONSTRUCTION").Select(d => new Category()
                {
                    CategoryId = d.CategoryId,
                    CategoryCode = d.CategoryCode,
                    CategoryName = d.CategoryName
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

        #region Danh sách công suất lắp đặt
        [Route("ListInstallCapacity")]
        [HttpGet]
        public IActionResult loadListInstallCapacity()
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
                IQueryable<Category> _data = _repo._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "INSTALL_CAPACITY").Select(d => new Category()
                {
                    CategoryId = d.CategoryId,
                    CategoryCode = d.CategoryCode,
                    CategoryName = d.CategoryName
                }).OrderBy( x => x.CategoryCode);

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

        #region Danh sách loại giấy xác nhận đại lý xăng dầu
        [Route("ListCertificateType")]
        [HttpGet]
        public IActionResult ListCertificateType()
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
                IQueryable<Category> _data = _repo._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "CERTIFICATE_TYPE").Select(d => new Category()
                {
                    CategoryId = d.CategoryId,
                    CategoryCode = d.CategoryCode,
                    CategoryName = d.CategoryName
                }).OrderBy(x => x.CategoryCode);

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

        #region Danh sách hình thức ký hợp đồng mua bán xăng dầu 
        [Route("ListContractForm")]
        [HttpGet]
        public IActionResult ListContractForm()
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
                IQueryable<Category> _data = _repo._context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "CONTRACT_FORM").Select(d => new Category()
                {
                    CategoryId = d.CategoryId,
                    CategoryCode = d.CategoryCode,
                    CategoryName = d.CategoryName
                }).OrderBy(x => x.CategoryCode);

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

        #region Danh sách chỉ tiêu Báo cáo tình hình hoạt động của dự án đầu tư trong cụm công nghiệp
        [Route("ListTargetCCN1")]
        [HttpGet]
        public IActionResult ListTargetCCN1()
        {
            BaseModels<IndustrialManagementTarget> model = new BaseModels<IndustrialManagementTarget>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var data = (from cate in _repo._context.Categories where !cate.IsDel && cate.CategoryCode == "QLCN_TARGET_CCN1"
                           join imt in _repo._context.IndustrialManagementTargets on cate.CategoryId equals imt.ParentTargetId
                           where !imt.IsDel
                           select new IndustrialManagementTarget
                           {
                               IndustrialManagementTargetId = imt.IndustrialManagementTargetId,
                               Name = imt.Name,
                               Unit = imt.Unit
                           }).OrderBy( x => x.Name).ToList();

                model.status = 1;
                model.items = data;
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

        #region Danh sách chỉ tiêu Tình hình hoạt động của dự án đầu tư xây dựng hạ tầng kỹ thuật cụm công nghiệp
        [Route("ListTargetCCN2")]
        [HttpGet]
        public IActionResult ListTargetCCN2()
        {
            BaseModels<IndustrialManagementTarget> model = new BaseModels<IndustrialManagementTarget>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var data = (from cate in _repo._context.Categories
                            where !cate.IsDel && cate.CategoryCode == "QLCN_TARGET_CCN2"
                            join imt in _repo._context.IndustrialManagementTargets on cate.CategoryId equals imt.ParentTargetId
                            where !imt.IsDel
                            select new IndustrialManagementTarget
                            {
                                IndustrialManagementTargetId = imt.IndustrialManagementTargetId,
                                Name = imt.Name,
                                Unit = imt.Unit
                            }).OrderBy(x => x.Name).ToList();

                model.status = 1;
                model.items = data;
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

        #region Danh sách chỉ tiêu Tổng hợp tình hình cụm công nghiệp trên địa bàn cấp huyện
        [Route("ListTargetCCN3")]
        [HttpGet]
        public IActionResult ListTargetCCN3()
        {
            BaseModels<IndustrialManagementTarget> model = new BaseModels<IndustrialManagementTarget>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var data = (from cate in _repo._context.Categories
                            where !cate.IsDel && cate.CategoryCode == "QLCN_TARGET_CCN3"
                            join imt in _repo._context.IndustrialManagementTargets on cate.CategoryId equals imt.ParentTargetId
                            where !imt.IsDel
                            select new IndustrialManagementTarget
                            {
                                IndustrialManagementTargetId = imt.IndustrialManagementTargetId,
                                Name = imt.Name,
                                Unit = imt.Unit,
                                GroupTargetId = imt.GroupTargetId
                            }).OrderBy(x => x.Name).ToList();

                model.status = 1;
                model.items = data;
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

        #region danh sách giai đoạn
        [Route("LoadStage")]
        [HttpGet]
        public IActionResult LoadStage()
        {
            BaseModels<Stage> model = new BaseModels<Stage>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<Stage> _data = _repo._context.Stages
                    .Where(x => !x.IsDel
                        && ((x.StartYear % 10 == 1 && x.EndYear % 10 == 5)
                        || (x.StartYear % 10 == 6 && x.EndYear % 10 == 0)))
                    .Select(x => new Stage
                    {
                        StageId = x.StageId,
                        StageName = x.StageName,
                        StartYear = x.StartYear,
                        EndYear = x.EndYear,
                    })
                    .OrderBy(x => x.StartYear)
                    .ThenBy(x => x.EndYear);

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
