using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.FoodSafetyCertificateRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Globalization;
using RestSharp.Extensions;
using static API_SoCongThuong.Classes.Ulities;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodSafetyCertificateController : ControllerBase
    {
        private FoodSafetyCertificateRepo _repo;
        private IConfiguration _config;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public FoodSafetyCertificateController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new FoodSafetyCertificateRepo(context);
            _config = configuration;
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<FoodSafetyCertificateModel> model = new BaseModels<FoodSafetyCertificateModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<FoodSafetyCertificateModel, object> _orderByExpression = x => x.BusinessName;
                Dictionary<string, Func<FoodSafetyCertificateModel, object>> _sortableFields =
                    new Dictionary<string, Func<FoodSafetyCertificateModel, object>>
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "Address", x => x.Address },
                        { "Num", x => x.Num },
                        { "ManagerName", x => x.ManagerName },
                        { "Status", x => x.Status },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<FoodSafetyCertificateModel> _data = (from e in _repo._context.FoodSafetyCertificates
                                                                where !e.IsDel
                                                                join b in _repo._context.Businesses on e.BusinessId equals b.BusinessId
                                                                select new FoodSafetyCertificateModel
                                                                {
                                                                    FoodSafetyCertificateId = e.FoodSafetyCertificateId,
                                                                    BusinessId = e.BusinessId,
                                                                    BusinessName = b.BusinessNameVi ?? "",
                                                                    Address = e.Address,
                                                                    Num = e.Num ?? "",
                                                                    ManagerName = e.ManagerName,
                                                                    ValidTill = e.ValidTill.HasValue ? e.ValidTill.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                    LicenseDate = e.LicenseDate.HasValue ? e.LicenseDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                    Status = e.Status,
                                                                    Note = e.Note
                                                                }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.BusinessName.ToLower().Contains(_keywordSearch)
                        || x.Address.ToLower().Contains(_keywordSearch)
                        || x.Num.ToLower().Contains(_keywordSearch)
                        || x.ManagerName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"]))
                {
                    _data = _data.Where(x => x.Status.ToString() == query.Filter["Status"]);
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
        public async Task<IActionResult> Create([FromForm] FoodSafetyCertificateModel data)
        {
            BaseModels<FoodSafetyCertificate> model = new BaseModels<FoodSafetyCertificate>();
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

                var Files = Request.Form.Files;
                var LstFile = new List<FoodSafetyCertificateAttachFileModel>();
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
                                Linkfile = "FoodSafetyCertificate"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            FoodSafetyCertificateAttachFileModel fileSave = new FoodSafetyCertificateAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;
                data.CreateUserId = loginData.Userid;

                data.ProductionData = JsonConvert.DeserializeObject<List<FoodSafetyCertificateItemModel>>(data.ProductionDataString);

                await _repo.Insert(data);

                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.FOOD_SAFETY_CERTIFICATE, Action_Status.SUCCESS);
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
            BaseModels<FoodSafetyCertificateModel> model = new BaseModels<FoodSafetyCertificateModel>();
            try
            {
                List<FoodSafetyCertificateModel> data = new List<FoodSafetyCertificateModel>();
                var result = _repo.FindById(id, _config);
                if (result != null)
                {
                    data.Add(result);
                    model.status = 1;
                    model.items = data;
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
        public async Task<IActionResult> Update([FromForm] FoodSafetyCertificateModel data)
        {
            BaseModels<FoodSafetyCertificate> model = new BaseModels<FoodSafetyCertificate>();
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

                var check = _repo._context.FoodSafetyCertificates.Where(x => x.FoodSafetyCertificateId == data.FoodSafetyCertificateId && !x.IsDel).Any();
                if (!check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };

                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.FOOD_SAFETY_CERTIFICATE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return NotFound(model);
                }

                var Files = Request.Form.Files;
                var LstFile = new List<FoodSafetyCertificateAttachFileModel>();
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
                                Linkfile = "FoodSafetyCertificate"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            FoodSafetyCertificateAttachFileModel fileSave = new FoodSafetyCertificateAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;
                data.UpdateUserId = loginData.Userid;
                data.ProductionData = JsonConvert.DeserializeObject<List<FoodSafetyCertificateItemModel>>(data.ProductionDataString);

                await _repo.Update(data, _config);

                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.FOOD_SAFETY_CERTIFICATE, Action_Status.SUCCESS);
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
            BaseModels<FoodSafetyCertificate> model = new BaseModels<FoodSafetyCertificate>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                SystemLog datalog = new SystemLog();

                FoodSafetyCertificate? DeleteData = _repo._context.FoodSafetyCertificates.Where(x => x.FoodSafetyCertificateId == id && !x.IsDel).FirstOrDefault();
                if (DeleteData != null)
                {
                    DeleteData.IsDel = true;
                    await _repo.Delete(DeleteData, _config);

                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.FOOD_SAFETY_CERTIFICATE, Action_Status.SUCCESS);
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

                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.FOOD_SAFETY_CERTIFICATE, Action_Status.FAIL);
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

        [Route("ExportCertificate/{id}")]
        [HttpPost]
        public async Task<IActionResult> ExportCertificate(Guid id)
        {
            try
            {
                var data = (from e in _repo._context.FoodSafetyCertificates
                            where !e.IsDel && e.FoodSafetyCertificateId == id
                            join b in _repo._context.Businesses on e.BusinessId equals b.BusinessId into be
                            from res in be.DefaultIfEmpty()
                            select new FoodSafetyCertificateModel
                            {
                                FoodSafetyCertificateId = e.FoodSafetyCertificateId,
                                BusinessId = e.BusinessId,
                                BusinessName = res.BusinessNameVi,
                                Address = e.Address,
                                PhoneNumber = e.PhoneNumber,
                                Num = e.Num,
                                ManagerName = e.ManagerName,
                                ValidTill = e.ValidTill.HasValue ? e.ValidTill.Value.ToString("dd'/'MM'/'yyyy") : "",
                                LicenseDate = e.LicenseDate.HasValue ? e.LicenseDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                                Status = e.Status,
                            }).FirstOrDefault();

                var ListProduct = _repo._context.FoodSafetyCertificateItems
                    .Where(x => x.FoodSafetyCertificateId.Equals(id))
                    .Select(i => i.ProductName)
                    .ToList();  

                if (data == null)
                {
                    return BadRequest("No item in Database.");
                }
                else
                {
                    FoodSafetyCertificate? SaveData = _repo._context.FoodSafetyCertificates.Where(x => x.FoodSafetyCertificateId == data.FoodSafetyCertificateId && !x.IsDel).FirstOrDefault();
                    if (SaveData != null)
                    {
                        SaveData.Status = 2;
                        await _repo.UpdateStatus(SaveData);
                    }

                }

                string filePath = @"Upload/Templates/GCN_DDK_ATTTP.docx";
                string clonePath = @"Upload/Templates/GCN_DDK_ATTTP_Clone.docx";

                System.IO.File.Delete(clonePath);
                System.IO.File.Copy(filePath, clonePath);

                using (FileStream fileStream = System.IO.File.Open(clonePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(fileStream, true))
                    {
                        MainDocumentPart? mainDocPart = doc.MainDocumentPart;

                        if (mainDocPart == null)
                        {
                            return BadRequest("Invalid template file.");
                        }

                        List<Text> mergeField = mainDocPart.Document.Body.Descendants<Text>().Where(x => x.Text.Contains("<<") && x.Text.Contains(">>")).ToList();

                        if (mergeField.Count == 10)
                        {
                            mergeField[0].Text = data.BusinessName;
                            mergeField[1].Text = string.Join(", ", ListProduct);
                            mergeField[2].Text = data.ManagerName;
                            mergeField[3].Text = data.Address;
                            mergeField[4].Text = data.PhoneNumber;
                            mergeField[5].Text = data.LicenseDate.Length > 0 ? data.LicenseDate.Split("/")[0] : "";
                            mergeField[6].Text = data.LicenseDate.Length > 0 ? data.LicenseDate.Split("/")[1] : "";
                            mergeField[7].Text = data.LicenseDate.Length > 0 ? data.LicenseDate.Split("/")[2] : "";
                            mergeField[8].Text = data.Num;
                            mergeField[9].Text = data.ValidTill;
                        }
                        else
                        {
                            return BadRequest("Invalid template file.");
                        }

                        //using (MemoryStream memoryStream = new MemoryStream())
                        //{
                        //    doc.Clone(memoryStream);
                        //    memoryStream.Flush();
                        //    memoryStream.Position = 0;
                        //    return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "certificate.docx");
                        //}
                        doc.Save();
                        doc.Close();
                    }
                    fileStream.Close();
                    string apiEndpoint = _config.GetValue<string>("API_CONVERT_PDF");
                    using (HttpClient client = new HttpClient())
                    {
                        using (MultipartFormDataContent formData = new MultipartFormDataContent())
                        {

                            FileStream fileDocX = System.IO.File.Open(clonePath, FileMode.Open);
                            formData.Add(new StreamContent(fileDocX), "file", Path.GetFileName(filePath));

                            // make the API call
                            HttpResponseMessage response = await client.PostAsync(apiEndpoint, formData);

                            // handle the response
                            if (response.IsSuccessStatusCode)
                            {
                                // read the response content as a byte array
                                byte[] responseBytes = await response.Content.ReadAsByteArrayAsync();

                                // create a memory stream from the byte array
                                MemoryStream stream = new MemoryStream(responseBytes);

                                //Always catches me out
                                stream.Flush();

                                //Not sure if this is required
                                stream.Position = 0;

                                // return the memory stream as a file
                                return File(stream, "application/pdf", "Certificate.pdf");
                            }
                            else
                            {
                                // handle the error response
                                return BadRequest("Error message: " + await response.Content.ReadAsStringAsync() + ", " + "API call failed with status code " + response.StatusCode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("ExportExcel")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            try
            {
                IQueryable<FoodSafetyCertificateModel> _data = (from e in _repo._context.FoodSafetyCertificates
                                                                where !e.IsDel
                                                                join b in _repo._context.Businesses on e.BusinessId equals b.BusinessId
                                                                join d in _repo._context.Districts on e.DistrictId equals d.DistrictId
                                                                select new FoodSafetyCertificateModel
                                                                {
                                                                    FoodSafetyCertificateId = e.FoodSafetyCertificateId,
                                                                    ProfileCode = e.ProfileCode,
                                                                    ProfileName = e.ProfileName,
                                                                    BusinessId = e.BusinessId,
                                                                    BusinessName = b.BusinessNameVi ?? "",
                                                                    DistrictId = e.DistrictId,
                                                                    DistrictName = d.DistrictName,
                                                                    Address = e.Address,
                                                                    Num = e.Num ?? "",
                                                                    ManagerName = e.ManagerName,
                                                                    ValidTill = e.ValidTill.HasValue ? e.ValidTill.Value.ToString("yyyy") : "",
                                                                    LicenseDate = e.LicenseDate.HasValue ? e.LicenseDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                    Status = e.Status,
                                                                    Note = e.Note ?? "",
                                                                    ProductionData = _context.FoodSafetyCertificateItems
                                                                        .Where(x => x.FoodSafetyCertificateId == e.FoodSafetyCertificateId)
                                                                        .Select(model => new FoodSafetyCertificateItemModel
                                                                        {
                                                                            Type = model.Type,
                                                                            ProductName = model.ProductName,
                                                                        }).ToList(),
                                                                }).ToList().AsQueryable();

                string _keywordSearch = "";
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.BusinessName.ToLower().Contains(_keywordSearch)
                        || x.Address.ToLower().Contains(_keywordSearch)
                        || x.Num.ToLower().Contains(_keywordSearch)
                        || x.ManagerName.ToLower().Contains(_keywordSearch)
                   );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"]))
                {
                    _data = _data.Where(x => x.Status.ToString() == query.Filter["Status"]);
                }

                var LStatus = new Dictionary<string, string>()
                {
                    {"0", "Chưa đủ điều kiện"},
                    {"1", "Đủ điều kiện (Chưa xuất GCN)"},
                    {"2", "Đã xuất GCN"}
                };

                string StatusName = "";
                if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"]))
                {
                    _data = _data.Where(x => x.Status.ToString() == query.Filter["Status"]);
                    StatusName = LStatus[query.Filter["Status"]] + " - ";
                }

                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachdoanhnghiepattp.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = "DANH SÁCH DOANH NGHIỆP - " + StatusName.ToUpper() + " AN TOÀN THỰC PHẨM";

                    int index = 5;
                    int row = 1;

                    FoodSafetyCertificateModel LastItem = _data.Last();

                    foreach (var item in _data)
                    {
                        if (row < _data.Count())
                        {
                            worksheet.Row(index).CopyTo(worksheet.Row(index + 1));
                        }
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.LicenseDate.HasValue() ? item.LicenseDate.Split('/').ToList()[2] : "";
                        worksheet.Cell(index, 3).Value = item.ValidTill;
                        worksheet.Cell(index, 4).Value = item.ProfileCode;
                        worksheet.Cell(index, 5).Value = item.ProfileName;
                        worksheet.Cell(index, 6).Value = item.DistrictName;
                        worksheet.Cell(index, 7).Value = item.BusinessName;
                        worksheet.Cell(index, 8).Value = item.ManagerName;
                        worksheet.Cell(index, 9).Value = item.Address;
                        worksheet.Cell(index, 10).Value = string.Join(',', item.ProductionData.Where(x => x.Type == 1).Select(x => x.ProductName).ToList());
                        worksheet.Cell(index, 11).Value = string.Join(',', item.ProductionData.Where(x => x.Type == 2).Select(x => x.ProductName).ToList());
                        worksheet.Cell(index, 12).Value = item.Num;
                        worksheet.Cell(index, 13).Value = item.LicenseDate;
                        worksheet.Cell(index, 14).Value = item.Note;

                        index++;
                        row++;
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
