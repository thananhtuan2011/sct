using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ExportGoodsRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.CountryRepository;
using API_SoCongThuong.Reponsitories.UnitRepository;
using API_SoCongThuong.Reponsitories.ImportGoodsRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportGoodsController : ControllerBase
    {
        private ExportGoodsRepo _repoExportGoods;
        private CategoryRepo _repoCategory;
        private BusinessRepo _repoBusiness;
        private CountryRepo _repoCountry;
        private UnitRepo _repoUnit;
        private ImportGoodsRepo _repoImportGoods;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ExportGoodsController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoExportGoods = new ExportGoodsRepo(context);
            _repoCategory = new CategoryRepo(context);
            _repoBusiness = new BusinessRepo(context);
            _repoCountry = new CountryRepo(context);
            _repoUnit = new UnitRepo(context);
            _repoImportGoods = new ImportGoodsRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("loaditemgroup")]
        [HttpGet]
        public IActionResult LoadItemGroup()
        {
            BaseModels<ItemGroup> model = new BaseModels<ItemGroup>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<ItemGroup> _data = _repoCategory.FindAll().Where(x => x.IsAction == true && x.CategoryTypeCode.Contains("ITEM_GROUP")).Select(x => new ItemGroup
                {
                    ItemGroupId = x.CategoryId,
                    ItemGroupName = x.CategoryName,
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

        [Route("loadtypeofeconomic")]
        [HttpGet]
        public IActionResult LoadTypeOfEconomic()
        {
            BaseModels<TypeOfEconomic> model = new BaseModels<TypeOfEconomic>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<TypeOfEconomic> _data = _repoCategory.FindAll().Where(x => x.IsAction == true && x.CategoryTypeCode == "TYPE_OF_ECONOMIC").Select(x => new TypeOfEconomic
                {
                    TypeOfEconomicId = x.CategoryId,
                    TypeOfEconomicName = x.CategoryName,
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

        [Route("loadcountry")]
        [HttpGet]
        public IActionResult LoadCountry()
        {
            BaseModels<CountryView> model = new BaseModels<CountryView>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
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

        [Route("loadbusiness")]
        [HttpGet]
        public IActionResult LoadBusiness()
        {
            BaseModels<BusinessView> model = new BaseModels<BusinessView>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
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

        [Route("loadunits")]
        [HttpGet]
        public IActionResult LoadUnits()
        {
            BaseModels<UnitsView> model = new BaseModels<UnitsView>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Query lấy data
                IQueryable<UnitsView> _data = _repoUnit.FindAll().Where(x => !x.IsDel).Select(x => new UnitsView
                {
                    UnitId = x.UnitId,
                    UnitName = x.UnitName,
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

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ExportGoodsView> model = new BaseModels<ExportGoodsView>();
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

                Func<ExportGoodsView, object> _orderByExpression = x => x.ExportGoodsName;
                Dictionary<string, Func<ExportGoodsView, object>> _sortableFields = new Dictionary<string, Func<ExportGoodsView, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ExportGoodsName", x => x.ExportGoodsName },
                        { "ItemGroupName", x => x.ItemGroupName },
                        { "TypeOfEconomicName", x => x.TypeOfEconomicName },
                        //{ "BusinessName", x => x.BusinessName },
                        { "CountryName", x => x.CountryName },
                        { "Amount", x => x.Amount },
                        { "Price", x => x.Price },
                        { "ExportTime", x => x.ExportTime },
                };

                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                var _data = from I in _repoExportGoods._context.ExportGoods.Where(x => !x.IsDel)
                            join C_ITEM in _repoCategory._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                                on I.ItemGroupId equals C_ITEM.CategoryId
                            join C_ECO in _repoCategory._context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                                on I.TypeOfEconomicId equals C_ECO.CategoryId
                            join C in _repoCountry._context.Countries
                                on I.CountryId equals C.CountryId
                            join U in _repoUnit._context.Units.Select(x => new { x.UnitId, x.UnitName })
                                on I.AmountUnit equals U.UnitId
                            //join B in _repoBusiness._context.Businesses.Where(x => !x.IsDel)
                            //    on I.BusinessId equals B.BusinessId
                            select (new ExportGoodsView
                            {
                                ExportGoodsId = I.ExportGoodsId,
                                ExportGoodsName = I.ExportGoodsName,
                                ItemGroupName = C_ITEM.CategoryName,
                                TypeOfEconomicName = C_ECO.CategoryName,
                                CountryName = C.CountryName,
                                Amount = I.Amount.ToString() + " " + U.UnitName,
                                Price = I.Price,
                                ExportTime = I.ExportTime.ToString("dd'/'MM'/'yyyy"),
                                ExportTimeDate = I.ExportTime,
                                //BusinessName = B.BusinessNameVi,
                            });

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.ExportGoodsName.ToLower().Contains(_keywordSearch) || x.ItemGroupName.ToLower().Contains(_keywordSearch)
                       || x.TypeOfEconomicName.ToLower().Contains(_keywordSearch) || x.CountryName.ToLower().Contains(_keywordSearch)
                       || x.Amount.ToLower().Contains(_keywordSearch) || x.Price.ToString().ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Month") && !string.IsNullOrEmpty(query.Filter["Month"]))
                {
                    _data = _data.Where(x => x.ExportTimeDate.HasValue && x.ExportTimeDate.Value.Month.ToString() == query.Filter["Month"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    _data = _data.Where(x => x.ExportTimeDate.HasValue && x.ExportTimeDate.Value.Year.ToString() == query.Filter["Year"]);
                }

                int _countRows = _data.Count();
                if (_countRows == 0)
                {
                    return NotFound("Không có dữ liệu");
                }


                if (_orderBy_ASC)
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else
                {
                    model.items = _data
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }

                if (query.Panigator.More)
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
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
            BaseModels<GetExportGoodsModel> model = new BaseModels<GetExportGoodsModel>();
            try
            {
                GetExportGoodsModel? result = _repoExportGoods._context.ExportGoods
                                            .Where(x => x.ExportGoodsId == id)
                                            .Select(x => new GetExportGoodsModel
                                            {
                                                ExportGoodsId = x.ExportGoodsId,
                                                ExportGoodsName = x.ExportGoodsName,
                                                ItemGroupId = x.ItemGroupId,
                                                TypeOfEconomicId = x.TypeOfEconomicId,
                                                BusinessId = x.BusinessId,
                                                CountryId = x.CountryId,
                                                Amount = x.Amount,
                                                AmountUnitId = x.AmountUnit,
                                                Price = x.Price,
                                                ExportTime = x.ExportTime.ToString("dd'/'MM'/'yyyy")
                                            })
                                            .FirstOrDefault();
                if (result != null)
                {
                    List<GetExportGoodsModel> lst = new List<GetExportGoodsModel>();
                    lst.Add(result);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(GetExportGoodsModel data)
        {
            BaseModels<ExportGood> model = new BaseModels<ExportGood>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ExportGood? SaveData = _repoExportGoods._context.ExportGoods.Where(x => x.ExportGoodsId == data.ExportGoodsId).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.ExportGoodsId = Guid.Parse(data.ExportGoodsId.ToString());
                    SaveData.ExportGoodsName = data.ExportGoodsName;
                    SaveData.ItemGroupId = data.ItemGroupId;
                    SaveData.TypeOfEconomicId = data.TypeOfEconomicId;
                    SaveData.BusinessId = data.BusinessId;
                    SaveData.CountryId = data.CountryId;
                    SaveData.Amount = data.Amount;
                    SaveData.AmountUnit = data.AmountUnitId;
                    SaveData.Price = data.Price;
                    SaveData.ExportTime = DateTime.ParseExact(data.ExportTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    SaveData.UpdateUserId = loginData.Userid;
                    SaveData.UpdateTime = DateTime.Now;

                    await _repoExportGoods.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.EXPORT_GOODS, Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.EXPORT_GOODS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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
        public async Task<IActionResult> create(GetExportGoodsModel data)
        {
            BaseModels<ExportGood> model = new BaseModels<ExportGood>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ExportGood SaveData = new ExportGood();
                SaveData.ExportGoodsName = data.ExportGoodsName;
                SaveData.ItemGroupId = data.ItemGroupId;
                SaveData.TypeOfEconomicId = data.TypeOfEconomicId;
                SaveData.BusinessId = data.BusinessId;
                SaveData.CountryId = data.CountryId;
                SaveData.Amount = data.Amount;
                SaveData.AmountUnit = data.AmountUnitId;
                SaveData.Price = data.Price;
                SaveData.ExportTime = DateTime.ParseExact(data.ExportTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;

                await _repoExportGoods.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.EXPORT_GOODS, Action_Status.SUCCESS);
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

        [HttpPut("deleteExportGood/{id}")]
        public async Task<IActionResult> deleteExportGood(Guid id)
        {
            BaseModels<ExportGood> model = new BaseModels<ExportGood>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ExportGood DeleteData = new ExportGood();
                DeleteData.ExportGoodsId = id;
                DeleteData.IsDel = true;
                await _repoExportGoods.DeleteExportGoods(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.EXPORT_GOODS, Action_Status.SUCCESS);
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

        [Route("deleteExportGoods")]
        [HttpPut()]
        public async Task<IActionResult> deleteExportGoods(removeListExportGoodsItems data)
        {
            BaseModels<ExportGood> model = new BaseModels<ExportGood>();
            try
            {
                foreach (Guid id in data.ExportGoodsIds)
                {
                    ExportGood DeleteData = new ExportGood();
                    DeleteData.CountryId = id;
                    DeleteData.IsDel = true;
                    await _repoExportGoods.DeleteExportGoods(DeleteData);
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

                var volumeExport = from ex in _repoExportGoods._context.ExportGoods
                                   where !ex.IsDel
                                   group ex by ex.ExportGoodsName
                                                    into g
                                   select new
                                   {
                                       ExportGoodsName = g.Key,
                                       Amount = g.Sum(x => x.Amount)
                                   };
                var volumeImport = from im in _repoImportGoods._context.ImportGoods
                                   where !im.IsDel
                                   group im by im.ImportGoodsName
                                                   into g
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

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = _repoExportGoods.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            if (query.Filter != null && query.Filter.ContainsKey("Month") && !string.IsNullOrEmpty(query.Filter["Month"]))
            {
                currentMonth = int.Parse(query.Filter["Month"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                currentYear = int.Parse(query.Filter["Year"]);
            }

            var GroupTypeOfEconomic = data.GroupBy(x => x.TypeOfEconomicName);
            var GroupItemData = data.GroupBy(x => x.ItemGroupName);
            var AllGroup = new List<string>() { "can kiem soat", "can nhap khau", "hang hoa khac" };

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyXuatKhau.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(2, 1).Value = $"THÁNG {currentMonth} NĂM {currentYear}";

                    if (GroupTypeOfEconomic.Any())
                    {
                        for (var i = 9; i < 14; i++)
                        {
                            var GroupItem = GroupTypeOfEconomic.FirstOrDefault(x => x.Key == worksheet.Cell(i, 2).Value.ToString());
                            if (GroupItem != null && GroupItem.Any())
                            {
                                worksheet.Cell(i, 3).Value = GroupItem.Sum(x => Convert.ToDouble(x.Amount)).ToString("#,##0.##") + " Tấn";
                                worksheet.Cell(i, 4).Value = GroupItem.Sum(x => x.Price).ToString("#,##0.##");
                            }
                        }
                    }

                    if (GroupItemData.Any())
                    {
                        foreach (var Group in GroupItemData)
                        {
                            string searchKey = Ulities.RemoveUnicode(Group.Key.ToLower());
                            AllGroup.Remove(searchKey);
                            var cell = worksheet.Cells("B15:B5000").FirstOrDefault(x => Ulities.RemoveUnicode(x.Value.ToString()).Contains(searchKey));
                            if (cell != null)
                            {
                                int index = cell.Address.RowNumber + 1;
                                int row = 1;

                                foreach (var item in Group)
                                {
                                    if (row == 1)
                                    {
                                        worksheet.Cell(index, 1).Value = row;
                                        worksheet.Cell(index, 2).Value = item.ExportGoodsName;
                                        worksheet.Cell(index, 3).Value = (Convert.ToDouble(item.Amount)).ToString("#,##0.##") + " Tấn";
                                        worksheet.Cell(index, 4).Value = item.Price.ToString("N0");

                                        index++;
                                        row++;
                                    }
                                    else
                                    {
                                        var addrow = worksheet.Row(index - 1);
                                        addrow.InsertRowsBelow(1);

                                        worksheet.Cell(index, 1).Value = row;
                                        worksheet.Cell(index, 2).Value = item.ExportGoodsName;
                                        worksheet.Cell(index, 3).Value = (Convert.ToDouble(item.Amount)).ToString("#,##0.##") + " Tấn";
                                        worksheet.Cell(index, 4).Value = item.Price.ToString("N0");

                                        index++;
                                        row++;
                                    }
                                }

                                var delRow = worksheet.Row(index);
                                delRow.Delete();
                            }
                        }

                        if (AllGroup.Any())
                        {
                            foreach (var Group in AllGroup)
                            {
                                var cell = worksheet.Cells("B15:B5000").FirstOrDefault(x => Ulities.RemoveUnicode(x.Value.ToString()).Contains(Group));
                                if (cell != null)
                                {
                                    int index = cell.Address.RowNumber + 1;
                                    var delRow = worksheet.Row(index);
                                    delRow.Delete();
                                }
                            }
                        }
                    }

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
