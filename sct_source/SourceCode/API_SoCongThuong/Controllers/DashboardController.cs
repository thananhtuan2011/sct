using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.DashboardRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private DashboardRepo _repo;
        public DashboardController(SoHoa_SoCongThuongContext context)
        {
            _repo = new DashboardRepo(context);
        }

        #region Data thủ tục hành chính
        [Route("getDataStatus")]
        [HttpGet]
        public object LoadAdministrativeProcedures()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var result = from p in _repo._context.AdministrativeProcedures
                             where !p.IsDel
                             group p by p.Status into g
                             select new { g.Key, Count = g.Count() };
                return result;
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

        #region Data phòng thương mại - Quản lý chợ
        [Route("getDataMarket")]
        [HttpGet]
        public object getDataMarket()
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
                var result = from c in _repo._context.CommercialManagements
                             join t in _repo._context.Categories
                               on c.Type equals t.CategoryId
                             where (t.CategoryName == "Chợ" && !c.IsDel)
                             group c by c.DistrictId into g
                             select new { g.Key, Count = g.Count() };
                return result;
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

        #region Data huyện
        [Route("getDistrict")]
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
                IQueryable<DistrictMarketModel> _data = _repo._context.Districts.Where(x => !x.IsDel).Select(x => new DistrictMarketModel
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
        #endregion

        #region Data kinh phí khuyến nông
        [Route("getDataFunding")]
        [HttpGet]
        public object getDataFunding()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var sumIndustrialPromotionFunding = _repo._context.IndustrialPromotionProjects.Where(x => !x.IsDel).Sum(x => x.IndustrialPromotionFunding);
                var sumReciprocalEnterpriseFunding = _repo._context.IndustrialPromotionProjects.Where(x => !x.IsDel).Sum(x => x.ReciprocalEnterpriseFunding);
                var result = new { sumIndustrialPromotionFunding, sumReciprocalEnterpriseFunding };
                return result;
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

        #region Data tình hình bán lẻ rượu
        [Route("getDataAlcoholProduct")]
        [HttpGet]
        public object getDataAlcoholProduct()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var craftAncol = _repo._context.CateReportProduceCrafttAncolForEconomics.Where(x => !x.IsDel).Sum(x => x.Quantity);
                var industryAncol = _repo._context.CateReportProduceIndustlAncols.Where(x => !x.IsDel).Sum(x => x.QuantityProduction);
                var result = new { craftAncol, industryAncol };
                return result;
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

        #region Data phòng thương mại - Quản lý xăng dầu
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
                                    join b in _repo._context.Businesses
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
        #endregion

        #region Data số lượng doanh nghiệp
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

                var query = from b in _repo._context.Businesses
                            where !b.IsDel
                            group b by b.DistrictId into g
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
        #endregion

        #region Data phòng thương mại - Quản lý bán buôn rượu
        [Route("getAlcohol")]
        [HttpGet]
        public object getAlcohol()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var data = from p in _repo._context.AlcoholBusinesses
                           join b in _repo._context.Businesses
                            on p.AlcoholBusinessName equals b.BusinessId
                           where !p.IsDel
                           group b by b.DistrictId into g
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
        #endregion

        #region Data phòng thương mại - Quản lý bán buôn thuốc lá
        [Route("getDataCigarette")]
        [HttpGet]
        public object getDataCigarette()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var CigaretteData = from p in _repo._context.CigaretteBusinesses
                                    join b in _repo._context.Businesses
                                    on p.CigaretteBusinessName equals b.BusinessId
                                    where !p.IsDel
                                    group b by b.DistrictId into g
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
        #endregion

        #region Data xuất nhập khẩu
        [Route("getDataVolumeExportImport")]
        [HttpGet]
        public object getDataVolumeExportImport()
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

                var volumeExport = from ex in _repo._context.ExportGoods
                                   where !ex.IsDel
                                   group ex by ex.ExportGoodsName into g
                                   select new
                                   {
                                       ExportGoodsName = g.Key,
                                       Amount = g.Sum(x => x.Amount)
                                   };
                var volumeImport = from im in _repo._context.ImportGoods
                                   where !im.IsDel
                                   group im by im.ImportGoodsName into g
                                   select new
                                   {
                                       ImportGoodsName = g.Key,
                                       Amount = g.Sum(x => x.Amount)
                                   };

                var _data = new { volumeExport, volumeImport };
                return _data;
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

        #region Data tên doanh nghiệp
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

                var import_business_name = from i in _repo._context.ImportGoods
                                           where !i.IsDel
                                           select new
                                           {
                                               i.BusinessId
                                           };

                var export_business_name = from i in _repo._context.ExportGoods
                                           where !i.IsDel
                                           select new
                                           {
                                               i.BusinessId
                                           };

                var list_business_id = import_business_name.ToList().Union(export_business_name.ToList()).ToList().Select(x => x.BusinessId);

                var query = from b in _repo._context.Businesses
                          where !b.IsDel && list_business_id.Contains(b.BusinessId)
                          select new
                          {
                              BusinessId = b.BusinessId,
                              BusinessName = b.BusinessNameVi,
                          };

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
        #endregion

        #region Data quản lý đề án xúc tiến thương mại
        [Route("getDataTradePromotionProjectManagement")]
        [HttpGet]
        public object getDataTradePromotionProjectManagement()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var Data = new
                {
                    count0 = _repo._context.TradePromotionProjectManagements.Where(x => !x.IsDel && x.ImplementationResults == 0).Count(),
                    count1 = _repo._context.TradePromotionProjectManagements.Where(x => !x.IsDel && x.ImplementationResults == 1).Count(),
                };

                return Data;
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

        #region Data xuất nhập khẩu theo doanh nghiệp
        [Route("getExImpordDataById/{id}")]
        [HttpGet]
        public object getExImpordDataById(Guid id)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var volumeExport = from ex in _repo._context.ExportGoods
                                   where !ex.IsDel && ex.BusinessId == id
                                   group ex by ex.ExportGoodsName into g
                                   select new
                                   {
                                       ExportGoodsName = g.Key,
                                       Amount = g.Sum(x => x.Amount)
                                   };
                var volumeImport = from im in _repo._context.ImportGoods
                                   where !im.IsDel && im.BusinessId == id
                                   group im by im.ImportGoodsName into g
                                   select new
                                   {
                                       ImportGoodsName = g.Key,
                                       Amount = g.Sum(x => x.Amount)
                                   };

                var Data = new { volumeExport, volumeImport };

                return Data;
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
