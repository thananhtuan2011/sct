using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ReportAdministrativeProceduresRepository;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportAdministrativeProceduresController : ControllerBase
    {
        private ReportAdministrativeProceduresRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ReportAdministrativeProceduresController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ReportAdministrativeProceduresRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<ReportAdministrativeProceduresModel> model = new BaseModels<ReportAdministrativeProceduresModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ReportAdministrativeProceduresModel, object> _orderByExpression = x => x.FieldName!;
                Dictionary<string, Func<ReportAdministrativeProceduresModel, object>> _sortableFields = new Dictionary<string, Func<ReportAdministrativeProceduresModel, object>>
                {
                    { "FieldName", x => x.FieldName! },
                };

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ReportAdministrativeProceduresModel> _data = (from r in _repo._context.ReportAdministrativeProcedures
                                                                         where !r.IsDel

                                                                         join c in _repo._context.Categories
                                                                         on r.AdministrativeProceduresField equals c.CategoryId

                                                                         join p in _repo._context.Categories
                                                                         on r.Period equals p.CategoryId
                                                                         select new ReportAdministrativeProceduresModel()
                                                                         {
                                                                             ReportId = r.ReportId,
                                                                             Period = r.Period,
                                                                             PeriodName = p.CategoryName,
                                                                             Year = r.Year,
                                                                             AdministrativeProceduresField = r.AdministrativeProceduresField,
                                                                             FieldName = c.CategoryName,
                                                                             OnlineInPeriod = r.OnlineInPeriod,
                                                                             OfflineInPeriod = r.OfflineInPeriod,
                                                                             FromPreviousPeriod = r.FromPreviousPeriod,
                                                                             TotalReceive = r.OnlineInPeriod + r.OfflineInPeriod + r.FromPreviousPeriod,
                                                                             OnTimeProcessed = r.OnTimeProcessed,
                                                                             OutOfDateProcessed = r.OutOfDateProcessed,
                                                                             BeforeDeadlineProcessed = r.BeforeDeadlineProcessed,
                                                                             TotalProcessed = r.OnTimeProcessed + r.OutOfDateProcessed + r.BeforeDeadlineProcessed,
                                                                             OnTimeProcessing = r.OnTimeProcessing,
                                                                             OutOfDateProcessing = r.OutOfDateProcessing,
                                                                             TotalProcessing = r.OnTimeProcessing + r.OutOfDateProcessing,
                                                                         }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.FieldName!.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    _data = _data.Where(x => x.Year.ToString().Contains(string.Join("", query.Filter["Year"])));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Period") && !string.IsNullOrEmpty(query.Filter["Period"]))
                {
                    _data = _data.Where(x => x.Period.ToString().Contains(string.Join("", query.Filter["Period"])));
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
        public async Task<IActionResult> Create(ReportAdministrativeProceduresModel data)
        {
            BaseModels<ReportAdministrativeProceduresModel> model = new BaseModels<ReportAdministrativeProceduresModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var check = (from r in _repo._context.ReportAdministrativeProcedures
                             where !r.IsDel && r.AdministrativeProceduresField == data.AdministrativeProceduresField && r.Period == data.Period && r.Year == data.Year
                             select r).Any();
                SystemLog datalog = new SystemLog();
                if (check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Báo cáo đã tồn tại."
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_ADMINISTRATIVE_PROCEDURE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                var util = new Ulities();
                data = util.TrimModel(data);
                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.REPORT_ADMINISTRATIVE_PROCEDURE, Action_Status.SUCCESS);
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                await _repo.Delete(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.REPORT_ADMINISTRATIVE_PROCEDURE, Action_Status.SUCCESS);
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
        public IActionResult GetItemById(Guid id)
        {
            BaseModels<ReportAdministrativeProceduresModel> model = new BaseModels<ReportAdministrativeProceduresModel>();
            try
            {
                var result = _repo.FindById(id);
                if (result != null)
                {
                    model.status = 1;
                    model.items = result.ToList();
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
        public async Task<IActionResult> Update(ReportAdministrativeProceduresModel data)
        {
            BaseModels<ReportAdministrativeProceduresModel> model = new BaseModels<ReportAdministrativeProceduresModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ReportAdministrativeProcedure? CheckData = _repo._context.ReportAdministrativeProcedures.Where(x => x.ReportId == data.ReportId && !x.IsDel).FirstOrDefault();
                if (CheckData != null)
                {
                    bool Check = _repo._context.ReportAdministrativeProcedures.Where(x => x.ReportId != data.ReportId && !x.IsDel && x.AdministrativeProceduresField == data.AdministrativeProceduresField && x.Period == data.Period && x.Year == data.Year).Any();
                    if (Check)
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.EXCEPTION_API,
                            Msg = "Báo cáo này đã tồn tại."
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_ADMINISTRATIVE_PROCEDURE, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return Ok(model);
                    }

                    var util = new Ulities();
                    data = util.TrimModel(data);
                    data.UpdateUserId = loginData.Userid;
                    data.UpdateTime = DateTime.Now;

                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.REPORT_ADMINISTRATIVE_PROCEDURE, Action_Status.SUCCESS);
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

        [Route("ReportAdministrativeProcedures")]
        [HttpPost]
        public IActionResult ReportAdministrativeProcedures([FromBody] QueryReportAdministrativeProceduresBody query)
        {
            BaseModels<ReturnReportAdministrativeProcedures> model = new BaseModels<ReturnReportAdministrativeProcedures>();
            try
            {
                //Get query body data:
                var filtertype = (query.Filter != null && query.Filter.ContainsKey("filterType")) ? int.Parse(query.Filter["filterType"]) : 1;
                var periods = (query.Filter != null && query.Filter.ContainsKey("periods")) ? int.Parse(query.Filter["periods"]) : 5;
                var year = (query.Filter != null && query.Filter.ContainsKey("year")) ? int.Parse(query.Filter["year"]) : DateTime.Now.Year;

                List<ResultReportAdministrativeProceduresModel> Data = _repo.FindData(year, periods);

                TotalReportAdministrativeProceduresModel Total = new TotalReportAdministrativeProceduresModel()
                {
                    //Tổng tiếp nhận
                    TotalReceive = Data.Sum(x => x.TotalReceive),
                    //Online trong kỳ
                    OnlineInPeriod = Data.Sum(x => x.OnlineInPeriod),
                    //Offline trong kỳ
                    OfflineInPeriod = Data.Sum(x => x.OfflineInPeriod),
                    //Từ kỳ trước
                    FromPreviousPeriod = Data.Sum(x => x.FromPreviousPeriod),
                    //Tổng - đã giải quyết
                    TotalProcessed = Data.Sum(x => x.TotalProcessed),
                    //Đúng hạn - đã giải quyết
                    OnTimeProcessed = Data.Sum(x => x.OnTimeProcessed),
                    //Quá hạn - đã giải quyết
                    OutOfDateProcessed = Data.Sum(x => x.OutOfDateProcessed),
                    //Trước hạn - đã giải quyết
                    BeforeDeadlineProcessed = Data.Sum(x => x.BeforeDeadlineProcessed),
                    //Tổng - đang xử lý
                    TotalProcessing = Data.Sum(x => x.TotalProcessing),
                    //Trong hạng - đang xử lý
                    OnTimeProcessing = Data.Sum(x => x.OnTimeProcessing),
                    //Quá hạn - đang xử lý
                    OutOfDateProcessing = Data.Sum(x => x.OutOfDateProcessing),
                };

                ReturnReportAdministrativeProcedures Result = new ReturnReportAdministrativeProcedures()
                {
                    Data = Data,
                    Total = Total
                };
                //Return Data:
                model.status = 1;
                model.data = Result;

                return Ok(model);
            }
            catch (Exception ex)
            {
                //Catch Error:
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }


        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel([FromBody] QueryReportAdministrativeProceduresBody query)
        {
            //Query data           
            var year = (query.Filter != null && query.Filter.ContainsKey("year")) ? int.Parse(query.Filter["year"]) : DateTime.Now.Year;

            var TextByPeriods = new Dictionary<int, Tuple<string, string, string>>()
            {
                {1, Tuple.Create("Quý 1." + year.ToString(), "Kỳ báo cáo: Quý I năm " + year.ToString() + "\r\n", "(Từ ngày 15/12/" + (year - 1).ToString() + " đến ngày 14/03/" + year.ToString() + ")")},
                {2, Tuple.Create("Quý 2." + year.ToString(), "Kỳ báo cáo: Quý II năm " + year.ToString() + "\r\n", "(Từ ngày 15/03/" + year.ToString() + " đến ngày 14/06/" + year.ToString() + ")")},
                {3, Tuple.Create("Quý 3." + year.ToString(), "Kỳ báo cáo: Quý III năm " + year.ToString() + "\r\n", "(Từ ngày 15/06/" + year.ToString() + " đến ngày 14/09/" + year.ToString() + ")")},
                {4, Tuple.Create("Quý 4." + year.ToString(), "Kỳ báo cáo: Quý IV năm " + year.ToString() + "\r\n", "(Từ ngày 15/09/" + year.ToString() + " đến ngày 14/12/" + year.ToString() + ")")},
                //{5, Tuple.Create("Năm " + year.ToString(), "Kỳ báo cáo: Năm " + year.ToString() + "\r\n", "(Từ ngày 15/12/" + (year - 1).ToString() + " đến ngày 14/12/" + year.ToString() + ")")}
            };

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (var workbook = new XLWorkbook(@"Upload/Templates/Baocaotinhhinhgiaiquyetthutuchanhchinh.xlsx"))
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            //Lấy data
                            var periods = i;

                            List<ResultReportAdministrativeProceduresModel> data = _repo.FindData(year, periods);

                            if (data == null)
                            {
                                return BadRequest();
                            }

                            //Mở file excel
                            IXLWorksheet worksheet = workbook.Worksheets.Worksheet(i);

                            #region Code cũ
                            ////Đổi tên sheet
                            //var WorksheetName = i == 1 ? "Quý 1." + year.ToString() :
                            //                    i == 2 ? "Quý 2." + year.ToString() :
                            //                    i == 3 ? "Quý 3." + year.ToString() :
                            //                    i == 4 ? "Quý 4." + year.ToString() :
                            //                             "Năm " + year.ToString();

                            //worksheet.Name = WorksheetName;

                            ////Tên văn bản
                            //var TextPeriod = periods == 4 ? "Kỳ báo cáo: Quý IV năm " + year.ToString() + "\r\n" :
                            //                 periods == 3 ? "Kỳ báo cáo: Quý III năm " + year.ToString() + "\r\n" :
                            //                 periods == 2 ? "Kỳ báo cáo: Quý II năm " + year.ToString() + "\r\n" :
                            //                 periods == 1 ? "Kỳ báo cáo: Quý I năm " + year.ToString() + "\r\n" :
                            //                                "Kỳ báo cáo: Năm " + year.ToString() + "\r\n";

                            //var TextTime = periods == 4 ? "(Từ ngày 15/09/" + year.ToString() + " đến ngày 14/12/" + year.ToString() + ")" :
                            //               periods == 3 ? "(Từ ngày 15/06/" + year.ToString() + " đến ngày 14/09/" + year.ToString() + ")" :
                            //               periods == 2 ? "(Từ ngày 15/03/" + year.ToString() + " đến ngày 14/06/" + year.ToString() + ")" :
                            //               periods == 1 ? "(Từ ngày 15/12/" + (year - 1).ToString() + " đến ngày 14/03/" + year.ToString() + ")" :
                            //                              "(Từ ngày 15/12/" + (year - 1).ToString() + " đến ngày 14/12/" + year.ToString() + ")";
                            #endregion

                            var TextRange = TextByPeriods[periods];

                            //Đổi tên Sheet
                            worksheet.Name = TextRange.Item1;

                            //Tên văn bản
                            var TextPeriod = TextRange.Item2;
                            var TextTime = TextRange.Item3;
                            var Header = worksheet.Cell("C1");
                            var HeaderContent = Header.GetRichText().ClearText();

                            HeaderContent.AddText("TÌNH HÌNH, KẾT QUẢ GIẢI QUYẾT THỦ TỤC\r\n").SetBold()
                                         .AddText("HÀNH CHÍNH TẠI CƠ QUAN, ĐƠN VỊ TRỰC TIẾP GIẢI QUYẾT\r\n").SetBold()
                                         .AddText("THỦ TỤC HÀNH CHÍNH\r\n").SetBold()
                                         .AddText(TextPeriod)
                                         .AddText(TextTime).SetUnderline();

                            //Setup vị trí bắt đầu ghi dữ liệu
                            int index = 10;
                            int row = 1;

                            //Bắt đầu ghi dữ liệu vào file
                            foreach (ResultReportAdministrativeProceduresModel Item in data)
                            {
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = Item.FieldName;
                                worksheet.Cell(index, 3).Value = Item.TotalReceive;
                                worksheet.Cell(index, 4).Value = Item.OnlineInPeriod;
                                worksheet.Cell(index, 5).Value = Item.OfflineInPeriod;
                                worksheet.Cell(index, 6).Value = Item.FromPreviousPeriod;
                                worksheet.Cell(index, 7).Value = Item.TotalProcessed;
                                worksheet.Cell(index, 8).Value = Item.BeforeDeadlineProcessed;
                                worksheet.Cell(index, 9).Value = Item.OnTimeProcessed;
                                worksheet.Cell(index, 10).Value = Item.OutOfDateProcessed;
                                worksheet.Cell(index, 11).Value = Item.TotalProcessing;
                                worksheet.Cell(index, 12).Value = Item.OnTimeProcessing;
                                worksheet.Cell(index, 13).Value = Item.OutOfDateProcessing;

                                if (row < data.Count())
                                {
                                    worksheet.Row(index).InsertRowsBelow(1);
                                    index++;
                                    row++;
                                } 
                                else
                                {
                                    worksheet.Row(index +1).Delete();
                                }
                            }

                            workbook.SaveAs(stream);
                        }
                    }

                    stream.Flush();
                    stream.Position = 0;

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
