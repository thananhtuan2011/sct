
using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BuildAndUpgradeRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using API_SoCongThuong.Reponsitories.CommercialManagementRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuildAndUpgradeController : ControllerBase
    {
        private BuildAndUpgradeRepo _repoBuildAndUpgrade;
        private CategoryRepo _repoCategory;
        private CommercialManagementRepo _repoCommercialManagement;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public BuildAndUpgradeController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repoBuildAndUpgrade = new BuildAndUpgradeRepo(context);
            _repoCategory = new CategoryRepo(context);
            _repoCommercialManagement = new CommercialManagementRepo(context);


            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<BuildAndUpgradeModel> model = new BaseModels<BuildAndUpgradeModel>();
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

                Func<BuildAndUpgradeModel, object> _orderByExpression = x => x.BuildAndUpgradeName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<BuildAndUpgradeModel, object>> _sortableFields = new Dictionary<string, Func<BuildAndUpgradeModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BuildAndUpgradeName", x => x.BuildAndUpgradeName },
                        { "Address", x => x.Address },
                        { "Name", x=> x.Name },
                        { "DistrictName", x=> x.DistrictsName},
                        { "CommuneName", x=> x.CommuneName},
                        { "TotalInvestment", x => x.TotalInvestment },
                        { "RealizedCapital", x => x.RealizedCapital },
                        { "BudgetCapital", x => x.BudgetCapital },
                        { "LandUseCapital", x => x.LandUseCapital },
                        { "Loans", x => x.Loans },
                        { "AnotherCapital", x => x.AnotherCapital },
                        { "IsBuild", x => x.IsBuild},
                        { "IsUpgrade", x=> x.IsUpgrade},
                    };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //IQueryable<BuildAndUpgradeModel> _data = _repoBuildAndUpgrade._context.BuildAndUpgradeMarkets.Where(x => !x.IsDel)
                //  .GroupJoin(_repoBuildAndUpgrade._context.CommercialManagements,
                //             x => x.CommercialId,
                //             cm => cm.CommercialId,
                //             (x, cm) => new { x, cm }).SelectMany(r => r.cm.DefaultIfEmpty(), (b, cmm) => new { b, cmm })
                //             .GroupJoin(_repoBuildAndUpgrade._context.Districts,
                //             x1 => x1.cmm.DistrictId,
                //             d => d.DistrictId,
                //             (x1, d) => new { x1, d }).SelectMany(r1 => r1.d.DefaultIfEmpty(), (r, di) => new { r, di })
                //             .GroupJoin(_repoBuildAndUpgrade._context.Communes,
                //             x2 => x2.r.x1.cmm.CommuneId,
                //             commune => commune.CommuneId,
                //             (x2, commune) => new { x2, commune }).SelectMany(res => res.commune.DefaultIfEmpty(), (res1, res2) => new BuildAndUpgradeModel
                //             {
                //                 BuildAndUpgradeId = res1.x2.r.x1.b.x.BuildAndUpgradeId,
                //                 BuildAndUpgradeName = res1.x2.r.x1.b.x.BuildAndUpgradeName,
                //                 Name = res1.x2.r.x1.cmm.Name,
                //                 DistrictsName = res1.x2.di.DistrictName,
                //                 CommuneName = res2.CommuneName,
                //                 Address = res1.x2.r.x1.b.x.Address,
                //                 TotalInvestment = res1.x2.r.x1.b.x.TotalInvestment,
                //                 TotalInvestmentUnit = res1.x2.r.x1.b.x.TotalInvestmentUnit,
                //                 RealizedCapital = res1.x2.r.x1.b.x.RealizedCapital,
                //                 RealizedCapitalUnit = res1.x2.r.x1.b.x.RealizedCapitalUnit,
                //                 BudgetCapital = res1.x2.r.x1.b.x.BudgetCapital,
                //                 BudgetCapitalUnit = res1.x2.r.x1.b.x.BudgetCapitalUnit,
                //                 LandUseCapital = res1.x2.r.x1.b.x.LandUseCapital,
                //                 LandUseCapitalUnit = res1.x2.r.x1.b.x.LandUseCapitalUnit,
                //                 Loans = res1.x2.r.x1.b.x.Loans,
                //                 LoansUnit = res1.x2.r.x1.b.x.LoansUnit,
                //                 AnotherCapital = res1.x2.r.x1.b.x.AnotherCapital,
                //                 AnotherCapitalUnit = res1.x2.r.x1.b.x.AnotherCapitalUnit,
                //                 IsBuild = res1.x2.r.x1.b.x.IsBuild,
                //                 IsUpgrade = res1.x2.r.x1.b.x.IsUpgrade,
                //                 Note = res1.x2.r.x1.b.x.Note,
                //                 IsDel = res1.x2.r.x1.b.x.IsDel
                //             });

                IQueryable<BuildAndUpgradeModel> _data = from b in _repoBuildAndUpgrade._context.BuildAndUpgradeMarkets
                                                         where !b.IsDel
                                                         join cm in _repoBuildAndUpgrade._context.CommercialManagements on b.CommercialId equals cm.CommercialId into cmGroup
                                                         from cmm in cmGroup.DefaultIfEmpty()
                                                         join d in _repoBuildAndUpgrade._context.Districts on cmm.DistrictId equals d.DistrictId into dGroup
                                                         from di in dGroup.DefaultIfEmpty()
                                                         join commune in _repoBuildAndUpgrade._context.Communes on cmm.CommuneId equals commune.CommuneId into communeGroup
                                                         from res2 in communeGroup.DefaultIfEmpty()
                                                         select new BuildAndUpgradeModel
                                                         {
                                                             BuildAndUpgradeId = b.BuildAndUpgradeId,
                                                             BuildAndUpgradeName = b.BuildAndUpgradeName,
                                                             Name = cmm.Name,
                                                             DistrictId = di.DistrictId,
                                                             DistrictsName = di.DistrictName,
                                                             CommuneName = res2.CommuneName,
                                                             Address = b.Address,
                                                             TotalInvestment = b.TotalInvestment,
                                                             TotalInvestmentUnit = b.TotalInvestmentUnit,
                                                             RealizedCapital = b.RealizedCapital,
                                                             RealizedCapitalUnit = b.RealizedCapitalUnit,
                                                             BudgetCapital = b.BudgetCapital,
                                                             BudgetCapitalUnit = b.BudgetCapitalUnit,
                                                             LandUseCapital = b.LandUseCapital,
                                                             LandUseCapitalUnit = b.LandUseCapitalUnit,
                                                             Loans = b.Loans,
                                                             LoansUnit = b.LoansUnit,
                                                             AnotherCapital = b.AnotherCapital,
                                                             AnotherCapitalUnit = b.AnotherCapitalUnit,
                                                             IsBuild = b.IsBuild,
                                                             IsUpgrade = b.IsUpgrade,
                                                             Note = b.Note,
                                                             IsDel = b.IsDel,
                                                             Year = b.Year,
                                                         };

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.Name.ToLower().Contains(_keywordSearch)
                       || x.TotalInvestment.ToString().Contains(_keywordSearch) || x.RealizedCapital.ToString().Contains(_keywordSearch)
                       || x.BudgetCapital.ToString().Contains(_keywordSearch) || x.LandUseCapital.ToString().Contains(_keywordSearch)
                       || x.Loans.ToString().Contains(_keywordSearch) || x.AnotherCapital.ToString().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    _data = _data.Where(x => x.Year.ToString() == query.Filter["Year"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["District"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
                {
                    if(query.Filter["Type"] == "isBuild")
                    {
                        _data = _data.Where(x => x.IsBuild == true);
                    }
                    else
                    {
                        _data = _data.Where(x => x.IsBuild == false);
                    }
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
            BaseModels<BuildAndUpgradeMarket> model = new BaseModels<BuildAndUpgradeMarket>();
            try
            {
                var result = _repoBuildAndUpgrade.FindById(id);
                if (result == null)
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
                    model.status = 1;
                    model.items = result.ToList();
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
        public async Task<IActionResult> Update(BuildAndUpgradeMarket data)
        {
            BaseModels<BuildAndUpgradeMarket> model = new BaseModels<BuildAndUpgradeMarket>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                BuildAndUpgradeMarket SaveData = _repoBuildAndUpgrade._context.BuildAndUpgradeMarkets.FirstOrDefault(x => x.BuildAndUpgradeId == data.BuildAndUpgradeId && !x.IsDel);
                SystemLog datalog = new SystemLog();
                if (SaveData == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.BUILD_AND_UPGRADE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return NotFound(model);
                }

                SaveData.BuildAndUpgradeName = data.BuildAndUpgradeName;
                SaveData.Address = data.Address;
                SaveData.CommercialId = data.CommercialId;
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.TotalInvestment = data.TotalInvestment;
                SaveData.TotalInvestmentUnit = data.TotalInvestmentUnit != "null" ? data.TotalInvestmentUnit : null;
                SaveData.RealizedCapital = data.RealizedCapital;
                SaveData.RealizedCapitalUnit = data.RealizedCapitalUnit != "null" ? data.RealizedCapitalUnit : null;
                SaveData.BudgetCapital = data.BudgetCapital;
                SaveData.BudgetCapitalUnit = data.BudgetCapitalUnit != "null" ? data.BudgetCapitalUnit : null;
                SaveData.LandUseCapital = data.LandUseCapital;
                SaveData.LandUseCapitalUnit = data.LandUseCapitalUnit != "null" ? data.LandUseCapitalUnit : null;
                SaveData.Loans = data.Loans;
                SaveData.LoansUnit = data.LoansUnit != "null" ? data.LoansUnit : null;
                SaveData.AnotherCapital = data.AnotherCapital;
                SaveData.AnotherCapitalUnit = data.AnotherCapitalUnit != "null" ? data.AnotherCapitalUnit : null;
                SaveData.IsBuild = data.IsBuild;
                SaveData.IsUpgrade = data.IsUpgrade;
                SaveData.Note = data.Note;
                SaveData.IsDel = data.IsDel;
                SaveData.UpdateUserId = loginData.Userid;
                SaveData.UpdateTime = DateTime.Now;
                SaveData.Year = data.Year;

                await _repoBuildAndUpgrade.Update(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.BUILD_AND_UPGRADE, Action_Status.SUCCESS);
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

        [HttpPost()]
        public async Task<IActionResult> create(BuildAndUpgradeMarket data)
        {
            BaseModels<BuildAndUpgradeMarket> model = new BaseModels<BuildAndUpgradeMarket>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                BuildAndUpgradeMarket SaveData = new BuildAndUpgradeMarket();
                SaveData.BuildAndUpgradeName = data.BuildAndUpgradeName;
                SaveData.Address = data.Address;
                SaveData.CommercialId = data.CommercialId;
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.TotalInvestment = data.TotalInvestment;
                SaveData.TotalInvestmentUnit = data.TotalInvestmentUnit != "null" ? data.TotalInvestmentUnit : null;
                SaveData.RealizedCapital = data.RealizedCapital;
                SaveData.RealizedCapitalUnit = data.RealizedCapitalUnit != "null" ? data.RealizedCapitalUnit : null;
                SaveData.BudgetCapital = data.BudgetCapital;
                SaveData.BudgetCapitalUnit = data.BudgetCapitalUnit != "null" ? data.BudgetCapitalUnit : null;
                SaveData.LandUseCapital = data.LandUseCapital;
                SaveData.LandUseCapitalUnit = data.LandUseCapitalUnit != "null" ? data.LandUseCapitalUnit : null;
                SaveData.Loans = data.Loans;
                SaveData.LoansUnit = data.LoansUnit != "null" ? data.LoansUnit : null;
                SaveData.AnotherCapital = data.AnotherCapital;
                SaveData.AnotherCapitalUnit = data.AnotherCapitalUnit != "null" ? data.AnotherCapitalUnit : null;
                SaveData.IsBuild = data.IsBuild;
                SaveData.IsUpgrade = data.IsUpgrade;
                SaveData.Note = data.Note;
                SaveData.IsDel = data.IsDel;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = DateTime.Now;
                SaveData.Year = data.Year;

                await _repoBuildAndUpgrade.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.BUILD_AND_UPGRADE, Action_Status.SUCCESS);
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

        [HttpPut("deleteBuildAndUpgrade/{id}")]
        public async Task<IActionResult> deleteBuildAndUpgrade(Guid id)
        {
            BaseModels<BuildAndUpgradeMarket> model = new BaseModels<BuildAndUpgradeMarket>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                BuildAndUpgradeMarket DeleteData = new BuildAndUpgradeMarket();
                DeleteData.BuildAndUpgradeId = id;
                DeleteData.IsDel = true;
                await _repoBuildAndUpgrade.DeleteBuildAndUpgrade(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.BUILD_AND_UPGRADE, Action_Status.SUCCESS);
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

        [Route("loadmarket")]
        [HttpGet]
        public IActionResult LoadMarket()
        {
            BaseModels<MarketBuildModel> Market_model = new BaseModels<MarketBuildModel>();

            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var _data = from c in _repoCommercialManagement._context.CommercialManagements
                            join t in _repoCategory._context.Categories
                                on c.Type equals t.CategoryId
                            join d in _repoCommercialManagement._context.Districts on c.DistrictId equals d.DistrictId
                            join com in _repoCommercialManagement._context.Communes on c.CommuneId equals com.CommuneId
                            where (t.CategoryTypeCode == "MARKET" && !c.IsDel)
                            select new MarketBuildModel
                            {
                                CommercialId = c.CommercialId,
                                CommercialName = c.Name,
                                DistrictId = c.DistrictId,
                                CommuneId = c.CommuneId,
                                DistrictName = d.DistrictName,
                                CommuneName = com.CommuneName,
                                Address = c.Address
                            };

                Market_model.status = 1;
                Market_model.items = _data.ToList();
                return Ok(Market_model);
            }
            catch (Exception ex)
            {
                Market_model.status = 0;
                Market_model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(Market_model);
            }
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = _repoBuildAndUpgrade.FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Quanlythongtinchosttttmxaydungnangcap.xlsx"))
                {
                    var listYear = data.Select(x => x.Year).Distinct().ToList();
                    
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 6;
                    int row = 1;
                    if(listYear.Count != 1)
                    {
                        worksheet.Cell(2, 1).Value = $"Giai đoạn {listYear.First()} - {listYear.Last()}";
                    }
                    else
                    {
                        worksheet.Cell(2, 1).Value = $"Năm {listYear.First()}";
                    }
                    for (int i = 0; i <listYear.Count; i++)
                    {
                        var addrow = worksheet.Row(index);
                        addrow.InsertRowsBelow(1);
                        int currentIndex = index;
                        
                        index++;
                        row++;
                        int stt = 1;
                        decimal sumTotalInvestment = 0;
                        decimal sumRealizedCapital = 0;
                        decimal sumBudgetCapital = 0;
                        decimal sumLandUseCapital = 0;
                        decimal sumLoans = 0;
                        decimal sumAnotherCapital = 0;
                        int sumBuild = 0;
                        int sumUpgrade = 0;
                        foreach (var item in data)
                        {
                            if(item.Year == listYear[i])
                            {
                                var addrow1 = worksheet.Row(index);
                                addrow1.InsertRowsBelow(1);
                                worksheet.Cell(index, 1).Value = stt;
                                worksheet.Cell(index, 2).Value = item.Name;
                                worksheet.Cell(index, 3).Value = $"{item.CommuneName} - {item.DistrictsName}";
                                worksheet.Cell(index, 4).Value = _repoBuildAndUpgrade.GetStringValueUnitBillion(item.TotalInvestment);
                                worksheet.Cell(index, 5).Value = _repoBuildAndUpgrade.GetStringValueUnitBillion(item.RealizedCapital);
                                worksheet.Cell(index, 6).Value = _repoBuildAndUpgrade.GetStringValueUnitBillion(item.BudgetCapital);
                                worksheet.Cell(index, 7).Value = _repoBuildAndUpgrade.GetStringValueUnitBillion(item.LandUseCapital);
                                worksheet.Cell(index, 8).Value = _repoBuildAndUpgrade.GetStringValueUnitBillion(item.Loans);
                                worksheet.Cell(index, 9).Value = _repoBuildAndUpgrade.GetStringValueUnitBillion(item.AnotherCapital);
                              
                                worksheet.Cell(index, 10).Value = item.IsBuild == true ? "x" : "";
                                worksheet.Cell(index, 11).Value = item.IsUpgrade == true ? "x" : "";
                                worksheet.Cell(index, 12).Value = item.Note;
                                index++;
                                row++;
                                stt++;

                                sumTotalInvestment += item.TotalInvestment ?? 0;
                                sumRealizedCapital += item.RealizedCapital?? 0;
                                sumBudgetCapital += item.BudgetCapital?? 0;
                                sumLandUseCapital += item.LandUseCapital?? 0;
                                sumLoans += item.Loans?? 0;
                                sumAnotherCapital += item.AnotherCapital?? 0;
                                if(item.IsBuild == true)
                                {
                                    sumBuild++;
                                }
                                else
                                {
                                    sumUpgrade++;
                                }
                            }
                        }
                        stt--;
                        worksheet.Cell(currentIndex, 2).Value = $"Năm {listYear[i]}: {stt}";
                        worksheet.Cell(currentIndex, 4).Value = $"{_repoBuildAndUpgrade.GetStringValueUnitBillion(sumTotalInvestment)}";
                        worksheet.Cell(currentIndex, 5).Value = $"{_repoBuildAndUpgrade.GetStringValueUnitBillion(sumRealizedCapital)}";
                        worksheet.Cell(currentIndex, 6).Value = $"{_repoBuildAndUpgrade.GetStringValueUnitBillion(sumBudgetCapital)}";
                        worksheet.Cell(currentIndex, 7).Value = $"{_repoBuildAndUpgrade.GetStringValueUnitBillion(sumLandUseCapital)}";
                        worksheet.Cell(currentIndex, 8).Value = $"{_repoBuildAndUpgrade.GetStringValueUnitBillion(sumLoans)}";
                        worksheet.Cell(currentIndex, 9).Value = $"{_repoBuildAndUpgrade.GetStringValueUnitBillion(sumAnotherCapital)}";
                        worksheet.Cell(currentIndex, 10).Value = sumBuild;
                        worksheet.Cell(currentIndex, 11).Value = sumUpgrade;
                        worksheet.Cell(currentIndex, 2).Style.Font.FontColor = XLColor.Red;
                        for (int j = 4; j<= 11; j++)
                        {
                            worksheet.Cell(currentIndex, j).Style.Font.FontColor = XLColor.Red;
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
