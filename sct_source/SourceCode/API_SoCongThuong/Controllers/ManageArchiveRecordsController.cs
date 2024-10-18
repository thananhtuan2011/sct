using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2019.Drawing.Diagram11;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageArchiveRecordsController : ControllerBase
    {
        private ManageArchiveRecordsRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public ManageArchiveRecordsController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ManageArchiveRecordsRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)
        {
            BaseModels<ManageArchiveRecordsModel> model = new BaseModels<ManageArchiveRecordsModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ManageArchiveRecordsModel, object> _orderByExpression = x => x.CodeFile;
                Dictionary<string, Func<ManageArchiveRecordsModel, object>> _sortableFields = new Dictionary<string, Func<ManageArchiveRecordsModel, object>>
                {
                    { "CodeFile", x => x.CodeFile },
                    { "RecordsGroup" , x => x.RecordsFinancePlan },
                    { "Title", x => x.Title },
                    { "StorageTime" , x => x.StorageTime },
                };

                Dictionary<int, string> GroupName = new Dictionary<int, string>()
                {
                    {1, "An toàn thực phẩm"},
                    {2, "Bảo vệ môi trường"},
                    {3, "An toàn hóa chất"},
                    {4, "Công tác phòng chống cháy nổ"},
                    {5, "Lĩnh vực kinh doanh khí"}
                };

                IQueryable<ManageArchiveRecordsModel> _data = _repo._context.ManageArchiveRecords.Where(x => !x.IsDel).Select(x => new ManageArchiveRecordsModel
                {
                    ManageArchiveRecordsId = x.ManageArchiveRecordsId,
                    RecordsFinancePlanId = x.RecordsFinancePlanId,
                    CodeFile = x.CodeFile,
                    Title = x.Title,
                    ReceptionTime = x.ReceptionTime,
                    StorageTime = x.StorageTime,
                    Location = x.Location,
                    StoreDocumentsAt = x.StoreDocumentsAt,
                    StoreFilesAt = x.StoreFilesAt,
                    Creator = x.Creator,
                    Note = x.Note,
                    RecordsFinancePlan = GroupName[x.RecordsFinancePlanId]
                }).ToList().AsQueryable();

                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.CodeFile.ToLower().Contains(_keywordSearch)
                       || x.RecordsFinancePlan.ToLower().Contains(_keywordSearch)
                       || x.Title.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("RecordsFinancePlan") && !string.IsNullOrEmpty(query.Filter["RecordsFinancePlan"]))
                {
                    _data = _data.Where(x => x.RecordsFinancePlanId.ToString() == query.Filter["RecordsFinancePlan"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
                {
                    _data = _data.Where(x => x.ReceptionTime.Year.ToString() == query.Filter["Year"]);
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
        public async Task<IActionResult> Create(ManageArchiveRecordsModel data)
        {
            BaseModels<ManageArchiveRecordsModel> model = new BaseModels<ManageArchiveRecordsModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManageArchiveRecord SaveData = new ManageArchiveRecord();
                SaveData.RecordsFinancePlanId = data.RecordsFinancePlanId;
                SaveData.CodeFile = data.CodeFile; ;
                SaveData.Title = data.Title; ;
                SaveData.ReceptionTime = data.ReceptionTime;
                SaveData.StorageTime = data.StorageTime;
                SaveData.Location = data.Location;
                SaveData.StoreDocumentsAt = data.StoreDocumentsAt;
                SaveData.StoreFilesAt = data.StoreFilesAt;
                SaveData.Creator = data.Creator; ;
                SaveData.Note = data.Note;
                SaveData.CreateTime = DateTime.Now;
                SaveData.CreateUserId = loginData.Userid;

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.MANAGE_ARCHIVE_RECORDS, Action_Status.SUCCESS);
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
            BaseModels<ManageArchiveRecordsModel> model = new BaseModels<ManageArchiveRecordsModel>();
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
        public async Task<IActionResult> Update(ManageArchiveRecordsModel data)
        {
            BaseModels<ManageArchiveRecordsModel> model = new BaseModels<ManageArchiveRecordsModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManageArchiveRecord? _data = _repo._context.ManageArchiveRecords.Where(x => !x.IsDel && x.ManageArchiveRecordsId == data.ManageArchiveRecordsId).FirstOrDefault();
                if (_data != null)
                {
                    _data.ManageArchiveRecordsId = data.ManageArchiveRecordsId;
                    _data.RecordsFinancePlanId = data.RecordsFinancePlanId;
                    _data.CodeFile = data.CodeFile; ;
                    _data.Title = data.Title; ;
                    _data.ReceptionTime = data.ReceptionTime;
                    _data.StorageTime = data.StorageTime;
                    _data.Location = data.Location;
                    _data.StoreDocumentsAt = data.StoreDocumentsAt;
                    _data.StoreFilesAt = data.StoreFilesAt;
                    _data.Creator = data.Creator; ;
                    _data.Note = data.Note;
                    _data.UpdateTime = DateTime.Now;
                    _data.UpdateUserId = loginData.Userid;

                    await _repo.Update(_data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.MANAGE_ARCHIVE_RECORDS, Action_Status.SUCCESS);
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

        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<ManageArchiveRecordsModel> model = new BaseModels<ManageArchiveRecordsModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                ManageArchiveRecord item = new ManageArchiveRecord();
                item.ManageArchiveRecordsId = id;
                item.IsDel = true;
                await _repo.Delete(item);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.MANAGE_ARCHIVE_RECORDS, Action_Status.SUCCESS);
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

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                currentYear = int.Parse(query.Filter["Year"]);
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyHoSoLuuTru.xlsx"))
                {
                    var group = data.GroupBy(x => x.RecordsFinancePlanId);
                    foreach (var record in group)
                    {
                        IXLWorksheet worksheet = workbook.Worksheets.Worksheet(record.Key);
                        worksheet.Cell(1, 1).Value = "QUẢN LÝ HỒ SƠ LƯU TRỮ NĂM " + currentYear;

                        int index = 4;
                        int row = 1;
                        foreach (var item in record)
                        {
                            if (row == 1)
                            {
                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = item.CodeFile;
                                worksheet.Cell(index, 3).Value = item.Title;
                                worksheet.Cell(index, 4).Value = item.Location;
                                worksheet.Cell(index, 5).Value = item.StoreDocumentsAt;
                                worksheet.Cell(index, 6).Value = item.StoreFilesAt;
                                worksheet.Cell(index, 7).Value = item.StorageTime;
                                worksheet.Cell(index, 8).Value = item.Note;

                                index++;
                                row++;
                            }
                            else
                            {
                                var addrow = worksheet.Row(index - 1);
                                addrow.InsertRowsBelow(1);

                                worksheet.Cell(index, 1).Value = row;
                                worksheet.Cell(index, 2).Value = item.CodeFile;
                                worksheet.Cell(index, 3).Value = item.Title;
                                worksheet.Cell(index, 4).Value = item.Location;
                                worksheet.Cell(index, 5).Value = item.StoreDocumentsAt;
                                worksheet.Cell(index, 6).Value = item.StoreFilesAt;
                                worksheet.Cell(index, 7).Value = item.StorageTime;
                                worksheet.Cell(index, 8).Value = item.Note;

                                index++;
                                row++;
                            }
                        }

                        var delrow = worksheet.Row(index);
                        delrow.Delete();
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
