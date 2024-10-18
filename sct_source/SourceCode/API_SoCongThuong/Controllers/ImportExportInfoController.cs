
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ImportGoodsRepository;
using API_SoCongThuong.Reponsitories.ExportGoodsRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.CountryRepository;
using API_SoCongThuong.Reponsitories.UnitRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExportInfoController : ControllerBase
    {
        private ImportGoodsRepo _repoImportGoods;
        private ExportGoodsRepo _repoExportGoods;
        private BusinessRepo _repoBusiness;
        private CountryRepo _repoCountry;
        private UnitRepo _repoUnit;
        public ImportExportInfoController(SoHoa_SoCongThuongContext context)
        {
            _repoImportGoods = new ImportGoodsRepo(context);
            _repoExportGoods = new ExportGoodsRepo(context);
            _repoBusiness = new BusinessRepo(context);
            _repoCountry = new CountryRepo(context);
            _repoUnit = new UnitRepo(context);
        }

        [Route("loadcountry")]
        [HttpGet]
        public IActionResult LoadCountry()
        {
            BaseModels<CountryView> model = new BaseModels<CountryView>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<CountryView> _data = _repoCountry.FindAll().Where(x => !x.IsDel).Select(x => new CountryView
                {
                    CountryId = x.CountryId,
                    CountryName = x.CountryName,
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

        [Route("loaditems")]
        [HttpGet]
        public IActionResult LoadItems()
        {
            BaseModels<ItemsModel> model = new BaseModels<ItemsModel>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<ItemsModel> _data = _repoImportGoods._context.ImportGoods.Where(x => !x.IsDel)
                                               .Select(x => new ItemsModel { ItemName = x.ImportGoodsName })
                                                   .Union(
                                               _repoExportGoods._context.ExportGoods.Where(x => !x.IsDel)
                                               .Select(x => new ItemsModel { ItemName = x.ExportGoodsName })
                                               );
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

        [Route("loadbusiness")]
        [HttpGet]
        public IActionResult LoadBusiness()
        {
            BaseModels<BusinessView> model = new BaseModels<BusinessView>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<BusinessView> _data = _repoBusiness.FindAll().Where(x => !x.IsDel).Select(x => new BusinessView
                {
                    BusinessId = x.BusinessId,
                    BusinessName = x.BusinessNameVi,
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

        [Route("loadcriteria")]
        [HttpGet]
        public IActionResult LoadCriteria()
        {
            BaseModels<CriteriaModel> model = new BaseModels<CriteriaModel>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                IQueryable<CriteriaModel> import_data = from I in _repoImportGoods.FindAll().Where(x => !x.IsDel)
                                                        join U in _repoUnit._context.Units.Where(x => !x.IsDel)
                                                            on I.AmountUnit equals U.UnitId
                                                        select (new CriteriaModel
                                                        {
                                                            Amount = I.Amount.ToString() + " " + U.UnitName,
                                                            Price = I.Price.ToString(),
                                                        });

                IQueryable<CriteriaModel> export_data = from E in _repoExportGoods.FindAll().Where(x => !x.IsDel)
                                                        join U in _repoUnit._context.Units.Where(x => !x.IsDel)
                                                            on E.AmountUnit equals U.UnitId
                                                        select (new CriteriaModel
                                                        {
                                                            Amount = E.Amount.ToString() + " " + U.UnitName,
                                                            Price = E.Price.ToString(),
                                                        });

                var _data = import_data.Union(export_data);
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
            BaseModels<ImportExportInfoModel> model = new BaseModels<ImportExportInfoModel>();
            //string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ImportExportInfoModel, object> _orderByExpression = x => x.GoodsName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ImportExportInfoModel, object>> _sortableFields = new Dictionary<string, Func<ImportExportInfoModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "GoodsName", x => x.GoodsName },
                        { "BusinessName", x => x.BusinessName },
                        { "CountryName", x => x.CountryName },
                        { "Amount", x => x.Amount },
                        { "Price", x => x.Price },
                        { "Time", x => x.Time },
                        { "Method", x => x.Method },
                };

                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                var import_data = from I in _repoImportGoods._context.ImportGoods.Where(x => !x.IsDel)
                                  join U in _repoUnit._context.Units.Where(x => !x.IsDel)
                                    on I.AmountUnit equals U.UnitId
                                  join C in _repoCountry._context.Countries.Where(x => !x.IsDel)
                                    on I.CountryId equals C.CountryId
                                  join B in _repoBusiness._context.Businesses.Where(x => !x.IsDel)
                                    on I.BusinessId equals B.BusinessId
                                  select (new ImportExportInfoModel
                                  {
                                      ItemId = I.ImportGoodsId,
                                      GoodsName = I.ImportGoodsName,
                                      BusinessName = B.BusinessNameVi,
                                      CountryName = C.CountryName,
                                      Amount = I.Amount.ToString() + " " + U.UnitName,
                                      Price = I.Price.ToString(),
                                      Time = I.ImportTime.ToString("dd/MM/yyyy"),
                                      Method = "Nhập khẩu"
                                  });

                var export_data = from E in _repoExportGoods._context.ExportGoods.Where(x => !x.IsDel)
                                  join U in _repoUnit._context.Units.Where(x => !x.IsDel)
                                    on E.AmountUnit equals U.UnitId
                                  join C in _repoCountry._context.Countries.Where(x => !x.IsDel)
                                    on E.CountryId equals C.CountryId
                                  join B in _repoBusiness._context.Businesses.Where(x => !x.IsDel)
                                    on E.BusinessId equals B.BusinessId
                                  select (new ImportExportInfoModel
                                  {
                                      ItemId = E.ExportGoodsId,
                                      GoodsName = E.ExportGoodsName,
                                      BusinessName = B.BusinessNameVi,
                                      CountryName = C.CountryName,
                                      Amount = E.Amount.ToString() + " " + U.UnitName,
                                      Price = E.Price.ToString(),
                                      Time = E.ExportTime.ToString("dd/MM/yyyy"),
                                      Method = "Xuất khẩu"
                                  });

                var _data = import_data.ToList().Concat(export_data.ToList());

                //Filter
                if (query.Filter != null && query.Filter.ContainsKey("GoodsName") && !string.IsNullOrEmpty(query.Filter["GoodsName"]))
                {
                    _data = _data.Where(x => x.GoodsName.ToString().ToLower().Contains(string.Join("", query.Filter["GoodsName"]).ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("BusinessName") && !string.IsNullOrEmpty(query.Filter["BusinessName"]))
                {
                    _data = _data.Where(x => x.BusinessName.ToString().ToLower().Contains(string.Join("", query.Filter["BusinessName"]).ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("CountryName") && !string.IsNullOrEmpty(query.Filter["CountryName"]))
                {
                    _data = _data.Where(x => x.CountryName.ToString().ToLower().Contains(string.Join("", query.Filter["CountryName"]).ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Amount") && !string.IsNullOrEmpty(query.Filter["Amount"]))
                {
                    _data = _data.Where(x => x.Amount.ToString().ToLower().Contains(string.Join("", query.Filter["Amount"]).ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Price") && !string.IsNullOrEmpty(query.Filter["Price"].ToString()))
                {
                    _data = _data.Where(x => x.Price.ToString().ToLower().Equals(string.Join("", query.Filter["Price"]).ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Method") && !string.IsNullOrEmpty(query.Filter["Method"].ToString()))
                {
                    _data = _data.Where(x => x.Method.ToString().ToLower().Contains(string.Join("", query.Filter["Method"]).ToLower()));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                    && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                DateTime.ParseExact(x.Time, "dd/MM/yyyy", null) >= 
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                                DateTime.ParseExact(x.Time, "dd/MM/yyyy", null) <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
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
    }

    //    [HttpGet("{id}")]
    //    public IActionResult getItemById(Guid id)
    //    {
    //        BaseModels<GetImportGoodsModel> model = new BaseModels<GetImportGoodsModel>();
    //        try
    //        {
    //            GetImportGoodsModel? result = _repoImportGoods._context.ImportGoods
    //                                        .Where(x => x.ImportGoodsId == id)
    //                                        .Select(x => new GetImportGoodsModel
    //                                        {
    //                                            ImportGoodsId = x.ImportGoodsId,
    //                                            ImportGoodsName = x.ImportGoodsName,
    //                                            ItemGroupId = x.ItemGroupId,
    //                                            TypeOfEconomicId = x.TypeOfEconomicId,
    //                                            BusinessId = x.BusinessId,
    //                                            CountryId = x.CountryId,
    //                                            Amount = x.Amount,
    //                                            AmountUnitId = x.AmountUnit,
    //                                            Price = x.Price,
    //                                            ImportTime = x.ImportTime.ToString("dd'/'MM'/'yyyy")
    //                                        })
    //                                        .FirstOrDefault();
    //            if (result != null)
    //            {
    //                List<GetImportGoodsModel> lst = new List<GetImportGoodsModel>();
    //                lst.Add(result);
    //                model.status = 1;
    //                model.items = lst;
    //                return Ok(model);
    //            }
    //            else
    //            {
    //                model.status = 0;
    //                model.error = new ErrorModel()
    //                {
    //                    Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
    //                    Msg = "Không có dữ liệu này trên DB",
    //                };
    //                return NotFound(model);
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //            model.status = 0;
    //            model.error = new ErrorModel()
    //            {
    //                Code = ErrCode_Const.EXCEPTION_API,
    //                Msg = ex.Message
    //            };
    //            return BadRequest(model);
    //        }
    //    }

    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> Update(GetImportGoodsModel data)
    //    {
    //        BaseModels<ImportGood> model = new BaseModels<ImportGood>();
    //        try
    //        {

    //            UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
    //            if (loginData == null)
    //            {
    //                return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
    //            }

    //            ImportGood? SaveData = _repoImportGoods._context.ImportGoods.Where(x => x.ImportGoodsId == data.ImportGoodsId).FirstOrDefault();
    //            if (SaveData != null)
    //            {
    //                SaveData.ImportGoodsId = Guid.Parse(data.ImportGoodsId.ToString());
    //                SaveData.ImportGoodsName = data.ImportGoodsName;
    //                SaveData.ItemGroupId = data.ItemGroupId;
    //                SaveData.TypeOfEconomicId = data.TypeOfEconomicId;
    //                SaveData.BusinessId = data.BusinessId;
    //                SaveData.CountryId = data.CountryId;
    //                SaveData.Amount = data.Amount;
    //                SaveData.AmountUnit = data.AmountUnitId;
    //                SaveData.Price = data.Price;
    //                SaveData.ImportTime = DateTime.ParseExact(data.ImportTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
    //                SaveData.UpdateUserId = loginData.Userid;
    //                SaveData.UpdateTime = DateTime.Now;

    //                await _repoImportGoods.Update(SaveData);
    //                model.status = 1;
    //                return Ok(model);
    //            }
    //            else
    //            {
    //                model.status = 0;
    //                model.error = new ErrorModel()
    //                {
    //                    Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
    //                    Msg = "Không có dữ liệu này trên DB",
    //                };
    //                return NotFound(model);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            model.status = 0;
    //            model.error = new ErrorModel()
    //            {
    //                Code = ErrCode_Const.EXCEPTION_API,
    //                Msg = ex.Message
    //            };
    //            return BadRequest(model);
    //        }
    //    }

    //    [HttpPost()]
    //    public async Task<IActionResult> create(GetImportGoodsModel data)
    //    {
    //        BaseModels<ImportGood> model = new BaseModels<ImportGood>();
    //        try
    //        {
    //            UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
    //            if (loginData == null)
    //            {
    //                return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
    //            }

    //            ImportGood SaveData = new ImportGood();
    //            SaveData.ImportGoodsName = data.ImportGoodsName;
    //            SaveData.ItemGroupId = data.ItemGroupId;
    //            SaveData.TypeOfEconomicId = data.TypeOfEconomicId;
    //            SaveData.BusinessId = data.BusinessId;
    //            SaveData.CountryId = data.CountryId;
    //            SaveData.Amount = data.Amount;
    //            SaveData.AmountUnit = data.AmountUnitId;
    //            SaveData.Price = data.Price;
    //            SaveData.ImportTime = DateTime.ParseExact(data.ImportTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
    //            SaveData.CreateUserId = loginData.Userid;
    //            SaveData.CreateTime = DateTime.Now;

    //            await _repoImportGoods.Insert(SaveData);
    //            model.status = 1;
    //            return Ok(model);
    //        }
    //        catch (Exception ex)
    //        {
    //            model.status = 0;
    //            model.error = new ErrorModel()
    //            {
    //                Code = ErrCode_Const.EXCEPTION_API,
    //                Msg = ex.Message
    //            };
    //            return BadRequest(model);
    //        }
    //    }

    //    [HttpPut("deleteImportGood/{id}")]
    //    public async Task<IActionResult> deleteImportGood(Guid id)
    //    {
    //        BaseModels<ImportGood> model = new BaseModels<ImportGood>();
    //        try
    //        {
    //            ImportGood DeleteData = new ImportGood();
    //            DeleteData.ImportGoodsId = id;
    //            DeleteData.IsDel = true;
    //            await _repoImportGoods.DeleteImportGoods(DeleteData);

    //            model.status = 1;
    //            return Ok(model);
    //        }
    //        catch (Exception ex)
    //        {

    //            model.status = 0;
    //            model.error = new ErrorModel()
    //            {
    //                Code = ErrCode_Const.EXCEPTION_API,
    //                Msg = ex.Message
    //            };
    //            return BadRequest(model);
    //        }
    //    }

    //    [Route("deleteImportGoods")]
    //    [HttpPut()]
    //    public async Task<IActionResult> deleteImportGoods(removeListImportGoodsItems data)
    //    {
    //        BaseModels<ImportGood> model = new BaseModels<ImportGood>();
    //        try
    //        {
    //            foreach (Guid id in data.ImportGoodsIds)
    //            {
    //                ImportGood DeleteData = new ImportGood();
    //                DeleteData.CountryId = id;
    //                DeleteData.IsDel = true;
    //                await _repoImportGoods.DeleteImportGoods(DeleteData);
    //            }
    //            model.status = 1;
    //            return Ok(model);
    //        }
    //        catch (Exception ex)
    //        {

    //            model.status = 0;
    //            model.error = new ErrorModel()
    //            {
    //                Code = ErrCode_Const.EXCEPTION_API,
    //                Msg = ex.Message
    //            };
    //            return BadRequest(model);
    //        }
    //    }
    //}
}
