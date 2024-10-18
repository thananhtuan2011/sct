using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.ChemicalSafetyCertificateRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Globalization;
using RestSharp.Extensions;
using Newtonsoft.Json;
using static API_SoCongThuong.Classes.Ulities;
using API_SoCongThuong.Logger;
using System.Data;
using ClosedXML.Excel;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChemicalSafetyCertificateController : ControllerBase
    {
        private ChemicalSafetyCertificateRepo _repo;
        private IConfiguration _config;
        private readonly ILogger<AsyncLogger> _logger;
        private AsyncLogger _asyncLogger;
        public SoHoa_SoCongThuongContext _context;

        public ChemicalSafetyCertificateController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ChemicalSafetyCertificateRepo(context);
            _config = configuration;
            _context = context;
            _logger = logger;
            _asyncLogger = new AsyncLogger(_logger, _context);
        }

        [Route("find")]
        [HttpPost]
        public IActionResult Find([FromBody] QueryRequestBody query)
        {
            BaseModels<ChemicalSafetyCertificateModel> model = new BaseModels<ChemicalSafetyCertificateModel>();
            string _keywordSearch = "";
            bool _orderBy_ASC = true;
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ChemicalSafetyCertificateModel, object> _orderByExpression = x => x.BusinessName;
                Dictionary<string, Func<ChemicalSafetyCertificateModel, object>> _sortableFields =
                    new Dictionary<string, Func<ChemicalSafetyCertificateModel, object>>
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "BusinessCode", x => x.BusinessCode },
                        { "Num", x => x.Num },
                        { "Address", x => x.Address },
                        { "Status", x => x.Status },
                    };
                if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                    _orderByExpression = _sortableFields[query.Sort.ColumnName];
                }

                IQueryable<ChemicalSafetyCertificateModel> _data = (from e in _repo._context.ChemicalSafetyCertificates
                                                                    where !e.IsDel
                                                                    join b in _repo._context.Businesses on e.BusinessId equals b.BusinessId
                                                                    select new ChemicalSafetyCertificateModel
                                                                    {
                                                                        ChemicalSafetyCertificateId = e.ChemicalSafetyCertificateId,
                                                                        BusinessId = e.BusinessId,
                                                                        BusinessName = b.BusinessNameVi ?? "",
                                                                        BusinessCode = e.BusinessCode,
                                                                        Num = e.Num ?? "",
                                                                        Address = e.Address,
                                                                        ValidTill = e.ValidTill.HasValue ? e.ValidTill.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                        Status = e.Status
                                                                    }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.BusinessName.ToLower().Contains(_keywordSearch)
                        || x.Address.ToLower().Contains(_keywordSearch)
                        || x.Num.ToLower().Contains(_keywordSearch)
                        || x.BusinessCode.ToLower().Contains(_keywordSearch)
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
        public async Task<IActionResult> Create([FromForm] ChemicalSafetyCertificateModel data)
        {
            BaseModels<ChemicalSafetyCertificateModel> model = new BaseModels<ChemicalSafetyCertificateModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var util = new Ulities();
                data = util.TrimModel(data);

                data.CreateUserId = loginData.Userid;
                data.ListChemical = JsonConvert.DeserializeObject<List<ChemicalInfoModel>>(data.JsonListChemical);

                SystemLog datalog = new SystemLog();

                var Files = Request.Form.Files;
                var LstFile = new List<ChemicalSafetyCertificateAttachFileModel>();
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
                                Linkfile = "ChemicalSafetyCertificate"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            ChemicalSafetyCertificateAttachFileModel fileSave = new ChemicalSafetyCertificateAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;

                await _repo.Insert(data);


                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CHEMICAL_SAFETY_CERTIFICATE, Action_Status.SUCCESS);
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
        public IActionResult getItemById(Guid id)
        {
            BaseModels<ChemicalSafetyCertificateModel> model = new BaseModels<ChemicalSafetyCertificateModel>();
            try
            {
                List<ChemicalSafetyCertificateModel> data = new List<ChemicalSafetyCertificateModel>();
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
        public async Task<IActionResult> Update([FromForm] ChemicalSafetyCertificateModel data)
        {
            BaseModels<ChemicalSafetyCertificate> model = new BaseModels<ChemicalSafetyCertificate>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var util = new Ulities();
                data = util.TrimModel(data);

                var check = _repo._context.ChemicalSafetyCertificates.Where(x => x.ChemicalSafetyCertificateId == data.ChemicalSafetyCertificateId && !x.IsDel).Any();

                SystemLog datalog = new SystemLog();

                if (!check)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CHEMICAL_SAFETY_CERTIFICATE, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return NotFound(model);
                }

                data.CreateUserId = loginData.Userid;
                data.ListChemical = JsonConvert.DeserializeObject<List<ChemicalInfoModel>>(data.JsonListChemical);

                var Files = Request.Form.Files;
                var LstFile = new List<ChemicalSafetyCertificateAttachFileModel>();
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
                                Linkfile = "ChemicalSafetyCertificate"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            ChemicalSafetyCertificateAttachFileModel fileSave = new ChemicalSafetyCertificateAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;
                data.UpdateUserId = loginData.Userid;

                await _repo.Update(data, _config);

                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CHEMICAL_SAFETY_CERTIFICATE, Action_Status.SUCCESS);
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
        public async Task<IActionResult> delete(Guid id)
        {
            BaseModels<ChemicalSafetyCertificate> model = new BaseModels<ChemicalSafetyCertificate>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                ChemicalSafetyCertificate? DeleteData = _repo._context.ChemicalSafetyCertificates.Where(x => x.ChemicalSafetyCertificateId == id && !x.IsDel).FirstOrDefault();
                if (DeleteData != null)
                {
                    DeleteData.IsDel = true;
                    await _repo.Delete(DeleteData);

                    SystemLog datalog = new SystemLog();
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CHEMICAL_SAFETY_CERTIFICATE, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));

                    List<ChemicalSafetyCertificateChemicalInfo> DelRange = _repo._context.ChemicalSafetyCertificateChemicalInfos.Where(x => x.ChemicalSafetyCertificateId == id).ToList();
                    if (DelRange.Count > 0)
                    {
                        _repo._context.ChemicalSafetyCertificateChemicalInfos.RemoveRange(DelRange);
                        _repo._context.SaveChanges();
                    }

                    var del = _repo._context.ChemicalSafetyCertificateAttachFiles.Where(d => d.ChemicalSafetyCertificateId == id).ToList();
                    foreach (var fdel in del)
                    {
                        string linkdel = fdel.LinkFile;
                        var result = Ulities.RemoveFileMinio(linkdel, _config);
                    }
                    _repo._context.ChemicalSafetyCertificateAttachFiles.RemoveRange(del);
                    await _repo._context.SaveChangesAsync();

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

        [Route("ExportCertificate/{id}")]
        [HttpPost]
        public async Task<IActionResult> ExportCertificate(Guid id)
        {
            try
            {
                /** Query and check data **/
                ChemicalSafetyCertificateModel? FormData = (from e in _repo._context.ChemicalSafetyCertificates
                                                            where !e.IsDel && e.ChemicalSafetyCertificateId == id
                                                            join b in _repo._context.Businesses on e.BusinessId equals b.BusinessId into be
                                                            from res in be.DefaultIfEmpty()
                                                            select new ChemicalSafetyCertificateModel
                                                            {
                                                                ChemicalSafetyCertificateId = e.ChemicalSafetyCertificateId,
                                                                BusinessId = e.BusinessId,
                                                                BusinessName = res.BusinessNameVi,
                                                                Address = e.Address,
                                                                PhoneNumber = e.PhoneNumber,
                                                                Fax = e.Fax,
                                                                BusinessAddress = e.BusinessAddress,
                                                                Num = e.Num,
                                                                Provider = e.Provider,
                                                                LicenseDate = e.LicenseDate.HasValue ? e.LicenseDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                ValidTill = e.ValidTill.HasValue ? e.ValidTill.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                            }).FirstOrDefault();

                List<ChemicalInfoModel>? TableData = _repo._context.ChemicalSafetyCertificateChemicalInfos
                    .Where(x => x.ChemicalSafetyCertificateId == id)
                    .Select(x => new ChemicalInfoModel
                    {
                        TradeName = x.TradeName,
                        NameOfChemical = x.NameOfChemical,
                        Cascode = x.Cascode,
                        ChemicalFormula = x.ChemicalFormula,
                        Content = x.Content,
                        Mass = x.Mass,
                    }).ToList();

                if (FormData == null || !TableData.Any())
                {
                    return BadRequest("No item in Database.");
                }
                else
                {
                    ChemicalSafetyCertificate? SaveData = _repo._context.ChemicalSafetyCertificates.Where(x => x.ChemicalSafetyCertificateId == FormData.ChemicalSafetyCertificateId && !x.IsDel).FirstOrDefault();
                    if (SaveData == null)
                    {
                        return BadRequest("No item in Database.");
                    }

                    SaveData.Status = 2;
                    await _repo.UpdateStatus(SaveData);
                }

                /** Clone file template **/
                string filePath = @"Upload/Templates/GCN_DDK_ATHC.docx";
                string clonePath = @"Upload/Templates/GCN_DDK_ATHC_Clone.docx";

                System.IO.File.Delete(clonePath);
                System.IO.File.Copy(filePath, clonePath);

                /** Open file and insert data **/
                using (FileStream fileStream = System.IO.File.Open(clonePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(fileStream, true))
                    {
                        MainDocumentPart? mainDocPart = doc.MainDocumentPart;

                        if (mainDocPart == null)
                        {
                            return BadRequest("Invalid template file.");
                        }

                        /** Insert Form **/
                        List<Text> mergeField = mainDocPart.Document.Body.Descendants<Text>().Where(x => x.Text.Contains("<<") && x.Text.Contains(">>")).ToList();
                        if (mergeField.Count == 10)
                        {

                            mergeField[0].Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FormData.BusinessName.ToLower());
                            mergeField[1].Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FormData.BusinessName.ToLower());
                            mergeField[2].Text = FormData.Address;
                            mergeField[3].Text = FormData.PhoneNumber;
                            mergeField[4].Text = FormData.Fax.HasValue() ? " " + FormData.Fax : "";
                            mergeField[5].Text = " " + FormData.BusinessAddress;
                            mergeField[6].Text = " " + FormData.Num + "do ";
                            mergeField[7].Text = FormData.Provider;
                            mergeField[8].Text = FormData.LicenseDate;
                            mergeField[9].Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FormData.BusinessName.ToLower());
                        }
                        else
                        {
                            return BadRequest("Invalid template file.");
                        }

                        /** Insert Table **/
                        var table = mainDocPart.Document.Body.Elements<Table>().ElementAt(1);

                        /** Add row before edit **/
                        // Number of copies = length of data table
                        var numberOfCopies = TableData.Count();

                        // Row original (Row 2)
                        var originalRow = table.Elements<TableRow>().ElementAt(2);

                        // Insertion position
                        var rowIndex = 4;

                        for (int i = 1; i < numberOfCopies; i++)
                        {
                            // Create a copy of the row using the CloneNode() method.
                            var newRow = (TableRow)originalRow.CloneNode(true);

                            // Insert the new row at a specific index.
                            table.InsertAt(newRow, rowIndex);
                        }

                        /** Insert data to Table **/
                        // CurrentRow
                        int CurrentRow = 0;

                        // Loop through the rows and cells of the table.
                        foreach (var row in table.Elements<TableRow>())
                        {
                            CurrentRow++;
                            if (CurrentRow > 2 && CurrentRow < numberOfCopies + 3)
                            {
                                var RowData = TableData[CurrentRow - 3];
                                var CellIndex = 0;
                                foreach (var cell in row.Elements<TableCell>())
                                {
                                    var paragraph = cell.Elements<Paragraph>().FirstOrDefault();

                                    if (paragraph != null)
                                    {
                                        //Delete Run in Paragraph of Cell
                                        paragraph.RemoveAllChildren<Run>();

                                        //Add Run to Paragraph
                                        var text = new Run();
                                        switch (CellIndex)
                                        {
                                            case 0:
                                                text = new Run(new Text((CurrentRow - 2).ToString()));
                                                break;
                                            case 1:
                                                text = new Run(new Text(RowData.TradeName));
                                                break;

                                            case 2:
                                                text = new Run(new Text(RowData.NameOfChemical));
                                                break;

                                            case 3:
                                                text = new Run(new Text(RowData.Cascode));
                                                break;

                                            case 4:
                                                text = new Run(new Text(RowData.ChemicalFormula));
                                                break;

                                            case 5:
                                                text = new Run(new Text(RowData.Content));
                                                break;

                                            case 6:
                                                text = new Run(new Text(RowData.Mass));
                                                break;
                                        }
                                        paragraph.Append(text);

                                        CellIndex++;
                                    }
                                }
                            }
                        }
                        doc.Save();
                        doc.Close();
                        //using (MemoryStream memoryStream = new MemoryStream())
                        //{
                        //    doc.Clone(memoryStream);
                        //    memoryStream.Flush();
                        //    memoryStream.Position = 0;
                        //    return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "certificate.docx");
                        //}
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
                IQueryable<ChemicalSafetyCertificateModel> _data = (from e in _repo._context.ChemicalSafetyCertificates
                                                                    where !e.IsDel
                                                                    join b in _repo._context.Businesses on e.BusinessId equals b.BusinessId
                                                                    select new ChemicalSafetyCertificateModel
                                                                    {
                                                                        ChemicalSafetyCertificateId = e.ChemicalSafetyCertificateId,
                                                                        BusinessId = e.BusinessId,
                                                                        BusinessName = b.BusinessNameVi,
                                                                        BusinessCode = e.BusinessCode,
                                                                        Num = e.Num,
                                                                        Address = e.Address,
                                                                        ValidTill = e.ValidTill.HasValue ? e.ValidTill.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                        Status = e.Status
                                                                    }).ToList().AsQueryable();

                string _keywordSearch = "";
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.BusinessName.ToLower().Contains(_keywordSearch)
                        || x.Address.ToLower().Contains(_keywordSearch)
                        || x.Num.ToLower().Contains(_keywordSearch)
                        || x.BusinessCode.ToLower().Contains(_keywordSearch)
                   );
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

                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachdoanhnghiepathc.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    worksheet.Cell(1, 1).Value = "DANH SÁCH DOANH NGHIỆP - " + StatusName.ToUpper() + " AN TOÀN HÓA CHẤT";

                    int index = 3;
                    int row = 1;
                    ChemicalSafetyCertificateModel LastItem = _data.Last();

                    foreach (var item in _data)
                    {
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.BusinessName;
                        worksheet.Cell(index, 3).Value = item.BusinessCode;
                        worksheet.Cell(index, 4).Value = item.Num;
                        worksheet.Cell(index, 5).Value = item.Address;
                        worksheet.Cell(index, 6).Value = LStatus[item.Status.ToString() ?? "0"];
                        if (item.Equals(LastItem))
                        {
                            break;
                        }
                        worksheet.Row(index).InsertRowsBelow(1);
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
