using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateRPProduceIndustlAncolController : ControllerBase
    {
        private CateRPProduceIndustlAncolRepo _repo;
        //private BusinessRepo _repoBusi;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public CateRPProduceIndustlAncolController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateRPProduceIndustlAncolRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;

        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<CateReportProduceIndustlAncolModel> model = new BaseModels<CateReportProduceIndustlAncolModel>();
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

                Func<CateReportProduceIndustlAncolModel, object> _orderByExpression = x => x.CateReportProduceIndustlAncolId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CateReportProduceIndustlAncolModel, object>> _sortableFields = new Dictionary<string, Func<CateReportProduceIndustlAncolModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "AlcoholBusinessName", x => x.AlcoholBusinessName },
                        { "Address", x => x.Address },
                        { "PhoneNumber", x => x.PhoneNumber },
                        { "LicenseCode", x => x.LicenseCode },
                        { "LicenseDateDisplay", x => x.LicenseDateDisplay },
                        { "QuantityConsume", x => x.QuantityConsume },
                        { "QuantityProduction", x => x.QuantityProduction },
                        { "DesignCapacity", x => x.DesignCapacity },
                        { "Investment", x => x.Investment },
                        { "DistrictId", x=> x.DistrictId},
                        { "Representative", x => x.Representative },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }


                //IQueryable<CateReportProduceIndustlAncolModel> _data = _repo._context.CateReportProduceIndustlAncols.Where(x => !x.IsDel)
                //    //_repo._context.Users,
                //    //cc => cc.CreateUserId,
                //    //u => u.UserId,
                //    // (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us)
                //    .GroupJoin(_repo._context.Districts,
                //         x => x.DistrictId,
                //         d => d.DistrictId,
                //      (x, d) => new { x, d }).SelectMany(r => r.d.DefaultIfEmpty(), (cc, u) => new { cc, u })
                //    .GroupJoin(_repo._context.Users,
                //        x1 => x1.u.CreateUserId,
                //        u => u.UserId,
                //     (x1, u) => new { x1, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us)
                //     => new CateReportProduceIndustlAncolModel
                //     {
                //         CateReportProduceIndustlAncolId = info.x1.cc.x.CateReportProduceIndustlAncolId,
                //         AlcoholBusinessName = info.x1.cc.x.AlcoholBusinessName.ToUpper(),
                //         PhoneNumber = info.x1.cc.x.PhoneNumber,
                //         Address = info.x1.cc.x.Address,
                //         DistrictId = info.x1.cc.x.DistrictId,
                //         DistrictName = info.x1.u.DistrictName,
                //         Representative = info.x1.cc.x.Representative,
                //         LicenseCode = info.x1.cc.x.LicenseCode,
                //         LicenseDateDisplay = info.x1.cc.x.LicenseDate.ToString("dd/MM/yyyy hh:mm"),
                //         DesignCapacity = info.x1.cc.x.DesignCapacity,
                //         ProductionForm = info.x1.cc.x.ProductionForm,
                //         Investment = info.x1.cc.x.Investment,
                //         QuantityConsume = info.x1.cc.x.QuantityConsume,
                //         QuantityProduction = info.x1.cc.x.QuantityProduction,
                //         TypeofWine = info.x1.cc.x.TypeofWine,
                //         CreateName = us.FullName,
                //         CreateTimeDisplay = info.x1.cc.x.CreateTime.ToString("dd/MM/yyyy hh:mm"),
                //         BusinessId = info.x1.cc.x.BusinessId,
                //         YearReport = info.x1.cc.x.YearReport
                //     }).ToList().AsQueryable();

                //Linq Query Syntax
                IQueryable<CateReportProduceIndustlAncolModel> _data = (from crp in _repo._context.CateReportProduceIndustlAncols
                                                                        where !crp.IsDel
                                                                        join b in _repo._context.Businesses on crp.BusinessId equals b.BusinessId into JoinBu
                                                                        from bu in JoinBu.DefaultIfEmpty()
                                                                        join d in _repo._context.Districts on bu.DistrictId equals d.DistrictId into JoinDis
                                                                        from dis in JoinDis.DefaultIfEmpty()
                                                                        select new CateReportProduceIndustlAncolModel
                                                                        {
                                                                            CateReportProduceIndustlAncolId = crp.CateReportProduceIndustlAncolId,

                                                                            AlcoholBusinessName = bu.BusinessNameVi,
                                                                            Representative = bu.NguoiDaiDien,
                                                                            DistrictName = dis.DistrictName,

                                                                            DesignCapacity = crp.DesignCapacity,
                                                                            QuantityProduction = crp.QuantityProduction,
                                                                            QuantityConsume = crp.QuantityConsume,
                                                                            Investment = crp.Investment,
                                                                            YearReport = crp.YearReport,
                                                                        }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                    x.AlcoholBusinessName.ToLower().Contains(_keywordSearch)
                    || x.Representative.ToLower().Contains(_keywordSearch)
                    || x.DistrictName.ToLower().Contains(_keywordSearch)
                    || x.DesignCapacity.ToLower().Contains(_keywordSearch)
                    || x.QuantityConsume.ToString().Contains(_keywordSearch)
                    || x.QuantityProduction.ToString().Contains(_keywordSearch)
                    || x.Investment.ToString().Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("YearReport"))
                {
                    _data = _data.Where(x => x.YearReport.ToString().Equals(string.Join("", query.Filter["YearReport"])));
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

        [HttpPost()]
        public async Task<IActionResult> Create(CateReportProduceIndustlAncolModel data)
        {
            BaseModels<CateReportSoldAncolForFactoryLicenseModel> model = new BaseModels<CateReportSoldAncolForFactoryLicenseModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var util = new Ulities();
                data = util.TrimModel(data);
                SystemLog datalog = new SystemLog();

                if (string.IsNullOrEmpty(data.TypeofWine))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_PRODUCE_INDUST_ANCOL, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;

                await _repo.Insert(data);

                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_RP_PRODUCE_INDUST_ANCOL, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateReportProduceIndustlAncolModel data)
        {
            BaseModels<CateReportProduceIndustlAncol> model = new BaseModels<CateReportProduceIndustlAncol>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var util = new Ulities();
                data = util.TrimModel(data);

                var CheckData = _repo.FindById(data.CateReportProduceIndustlAncolId);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_PRODUCE_INDUST_ANCOL, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.TypeofWine))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_PRODUCE_INDUST_ANCOL, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);

                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_RP_PRODUCE_INDUST_ANCOL, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
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

        [HttpGet("{id}")]
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<CateReportProduceIndustlAncolModel> model = new BaseModels<CateReportProduceIndustlAncolModel>();
            try
            {
                var info = _repo.FindById(id);

                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                CateReportProduceIndustlAncolModel result = new CateReportProduceIndustlAncolModel()
                {
                    CateReportProduceIndustlAncolId = info.CateReportProduceIndustlAncolId,
                    BusinessId = info.BusinessId,
                    DesignCapacity = info.DesignCapacity,
                    Investment = info.Investment,
                    QuantityConsume = info.QuantityConsume,
                    QuantityProduction = info.QuantityProduction,
                    TypeofWine = info.TypeofWine,
                    YearReport = info.YearReport
                };

                //Set data cho base model
                model.status = 1;
                model.data = result;
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
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<CateReportProduceIndustlAncol> model = new BaseModels<CateReportProduceIndustlAncol>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.Delete(id);

                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_RP_PRODUCE_INDUST_ANCOL, Action_Status.SUCCESS);
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

            int currentYear = DateTime.Now.Year;

            if (query.Filter != null && query.Filter.ContainsKey("YearReport") && !string.IsNullOrEmpty(query.Filter["YearReport"]))
            {
                currentYear = int.Parse(query.Filter["YearReport"]);
            };

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaosanxuatruoucongnghiep.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = $"Báo cáo tình hình sản xuất kinh doanh rượu công nghiệp (quy mô dưới 3 triệu lít/ năm) trên địa bàn năm {currentYear}";

                    int row = 1;
                    int index = 16;

                    //Sản xuất rượu thủ công để bán cho doanh nghiệp có giấy phép sản xuất rượu công nghiệp
                    var SoldAncolForFactoryByYear = _repo._context.CateReportSoldAncolForFactoryLicenses
                        .Where(x => !x.IsDel && x.YearReport == currentYear);
                    worksheet.Cell(10, 3).Value = SoldAncolForFactoryByYear.Count();
                    worksheet.Cell(10, 4).Value = SoldAncolForFactoryByYear.Sum(i => i.Quantity);

                    //Sản xuất rượu thủ công nhằm mục đích kinh doanh
                    var CrafttAncolForEconomicsByYear = _repo._context.CateReportProduceCrafttAncolForEconomics
                        .Where(x => !x.IsDel && x.YearReport == currentYear);
                    worksheet.Cell(11, 3).Value = CrafttAncolForEconomicsByYear.Count();
                    worksheet.Cell(11, 4).Value = CrafttAncolForEconomicsByYear.Sum(i => i.QuantityConsume);

                    foreach (var item in data)
                    {
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.AlcoholBusinessName.ToUpper();
                        worksheet.Cell(index, 3).Value = item.Address;
                        worksheet.Cell(index, 4).Value = item.PhoneNumber;
                        worksheet.Cell(index, 5).Value = item.LicenseCode;
                        worksheet.Cell(index, 6).Value = item.LicenseDate;
                        worksheet.Cell(index, 7).Value = item.TypeofWine;
                        worksheet.Cell(index, 8).Value = item.DesignCapacity;
                        worksheet.Cell(index, 9).Value = item.QuantityProduction;
                        worksheet.Cell(index, 10).Value = item.QuantityConsume;
                        worksheet.Cell(index, 11).Value = item.Investment;

                        if (row < data.Count)
                        {
                            worksheet.Row(index).InsertRowsBelow(1);
                            index++;
                            row++;
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
