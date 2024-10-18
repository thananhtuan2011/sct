using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using DocumentFormat.OpenXml.Office2019.Drawing.Diagram11;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GasBusinessController : ControllerBase
    {
        private GasBusinessRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public GasBusinessController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new GasBusinessRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<GasBusinessModel> model = new BaseModels<GasBusinessModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                Func<GasBusinessModel, object> _orderByExpression = x => x.BusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<GasBusinessModel, object>> _sortableFields = new Dictionary<string, Func<GasBusinessModel, object>>
                {
                    { "BusinessName", x => x.BusinessName },
                    { "BusinessCode" , x => x.BusinessCode },
                    { "PhoneNumber", x => x.PhoneNumber },
                    { "Address" , x => x.Address },
                    {"TypeBusiness", x => x.TypeBusiness },
                    {"NumDoc", x => x.NumDoc },
                    {"DateStart", x => x.DateStart },
                    {"ComplianceStatus", x => x.ComplianceStatus }
                };//Khởi tạo các trường để sắp xếp
                IQueryable<GasBusinessModel> _data = (from gb in _repo._context.GasBusinesses
                                                      where !gb.IsDel
                                                      join b in _repo._context.Businesses on gb.BusinessName equals b.BusinessId
                                                      join c in _repo._context.Categories on gb.ComplianceStatus equals c.CategoryId
                                                      select new GasBusinessModel()
                                                      {
                                                          GasBusinessId = gb.GasBusinessId,
                                                          BusinessId = gb.BusinessName,
                                                          GasBusiness = gb.GasId,
                                                          TypeBusiness = gb.TypeBusiness,
                                                          BusinessName = b.BusinessNameVi,
                                                          BusinessCode = b.BusinessCode,
                                                          ForeignTransactionName = b.BusinessNameEn,
                                                          Address = b.DiaChiTruSo,
                                                          PhoneNumber = b.SoDienThoai,
                                                          BusinessCertificate = b.GiayPhepSanXuat,
                                                          LicenseDate = b.NgayCapPhep,
                                                          TaxId = b.MaSoThue,
                                                          Fax = gb.Fax,
                                                          Licensors = gb.Licensors,
                                                          NumDoc = gb.NumDoc,
                                                          DateEnd = gb.DateEnd,
                                                          DateStart = gb.DateStart,
                                                          ComplianceStatus = gb.ComplianceStatus,
                                                          ComplianceStatusName = c.CategoryName,
                                                          District = b.DistrictId
                                                      }).ToList().AsQueryable();


                if (query.Sort != null
                   && !string.IsNullOrEmpty(query.Sort.ColumnName)
                   && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.BusinessName.ToLower().Contains(_keywordSearch) || x.BusinessCertificate.ToLower().Contains(_keywordSearch) || x.PhoneNumber.ToLower().Contains(_keywordSearch) || x.Address.ToLower().Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("TypeBusiness") && !string.IsNullOrEmpty(query.Filter["TypeBusiness"]))
                {
                    _data = _data.Where(x => x.TypeBusiness.ToString() == query.Filter["TypeBusiness"]);
                }
                if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"]))
                {
                    if (query.Filter["Status"] == "1")
                    {
                        _data = _data.Where(x => x.NumDoc != "");
                    }
                    else
                    {
                        _data = _data.Where(x => x.NumDoc == "");
                    }
                }

                if (query.Filter != null && query.Filter.ContainsKey("ComplianceStatus") && !string.IsNullOrEmpty(query.Filter["ComplianceStatus"]))
                {
                    _data = _data.Where(x => x.ComplianceStatus.ToString() == query.Filter["ComplianceStatus"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.District.ToString() == query.Filter["District"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("DateStart") && !string.IsNullOrEmpty(query.Filter["DateStart"]))
                {
                    _data = _data.Where(x => x.DateStart >= DateTime.ParseExact(query.Filter["DateStart"], "dd/MM/yyyy", null));
                }
                if (query.Filter != null && query.Filter.ContainsKey("DateEnd") && !string.IsNullOrEmpty(query.Filter["DateEnd"]))
                {
                    _data = _data.Where(x => x.DateStart <= DateTime.ParseExact(query.Filter["DateEnd"], "dd/MM/yyyy", null));
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(GasBusinessModel data)
        {
            BaseModels<GasBusinessModel> model = new BaseModels<GasBusinessModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                int count = _repo._context.GasBusinesses.Where(x => !x.IsDel && x.GasBusinessId != data.GasBusinessId && x.BusinessName == data.BusinessId).Count();
                if (count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Doanh nghiệp đã tồn tại!"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.GAS_BUSINESS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                GasBusiness? _data = _repo._context.GasBusinesses.Where(x => !x.IsDel && x.GasBusinessId == data.GasBusinessId).FirstOrDefault();
                if (_data != null)
                {
                    _data.TypeBusiness = data.TypeBusiness;
                    _data.BusinessName = data.BusinessId;
                    _data.GasId = data.GasBusiness;
                    _data.Licensors = data.Licensors;
                    _data.Fax = data.Fax;
                    _data.NumDoc = data.NumDoc;
                    _data.DateEnd = data.DateEnd;
                    _data.DateStart = data.DateStart;
                    _data.ComplianceStatus = data.ComplianceStatus;
                    _data.UpdateTime = DateTime.Now;
                    _data.UpdateUserId = loginData.Userid;
                    await _repo.Update(_data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.GAS_BUSINESS, Action_Status.SUCCESS);
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
        [HttpPost()]
        public async Task<IActionResult> create(GasBusinessModel data)
        {
            BaseModels<GasBusinessModel> model = new BaseModels<GasBusinessModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                int count = _repo._context.GasBusinesses.Where(x => !x.IsDel && x.BusinessName == data.BusinessId).Count();
                if (count > 0)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.EXCEPTION_API,
                        Msg = "Doanh nghiệp đã tồn tại!"
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.GAS_BUSINESS, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return Ok(model);
                }
                GasBusiness SaveData = new GasBusiness();
                SaveData.TypeBusiness = data.TypeBusiness;
                SaveData.BusinessName = data.BusinessId;
                SaveData.GasId = data.GasBusiness;
                SaveData.Licensors = data.Licensors;
                SaveData.Fax = data.Fax;
                SaveData.NumDoc = data.NumDoc;
                SaveData.DateEnd = data.DateEnd;
                SaveData.DateStart = data.DateStart;
                SaveData.ComplianceStatus = data.ComplianceStatus;
                SaveData.CreateTime = DateTime.Now;
                SaveData.CreateUserId = loginData.Userid;
                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.GAS_BUSINESS, Action_Status.SUCCESS);
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
            BaseModels<GasBusinessModel> model = new BaseModels<GasBusinessModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                GasBusiness item = new GasBusiness();
                item.GasBusinessId = id;
                item.IsDel = true;
                await _repo.Delete(item);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.GAS_BUSINESS, Action_Status.SUCCESS);
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

        private List<GasBusinessModel> FindData([FromBody] QueryRequestBody query)
        {

            List<GasBusinessModel> result = new List<GasBusinessModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;
            Func<GasBusinessModel, object> _orderByExpression = x => x.BusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<GasBusinessModel, object>> _sortableFields = new Dictionary<string, Func<GasBusinessModel, object>>
                {
                    { "BusinessName", x => x.BusinessName },
                    { "BusinessCode" , x => x.BusinessCode },
                    { "PhoneNumber", x => x.PhoneNumber },
                    { "Address" , x => x.Address },

                };//Khởi tạo các trường để sắp xếp
            IQueryable<GasBusinessModel> _data = (from gb in _repo._context.GasBusinesses
                                                  where !gb.IsDel
                                                  join b in _repo._context.Businesses on gb.BusinessName equals b.BusinessId
                                                  join c in _repo._context.Categories on gb.ComplianceStatus equals c.CategoryId
                                                  select new GasBusinessModel()
                                                  {
                                                      GasBusinessId = gb.GasBusinessId,
                                                      BusinessId = gb.BusinessName,
                                                      GasBusiness = gb.GasId,
                                                      TypeBusiness = gb.TypeBusiness,
                                                      BusinessName = b.BusinessNameVi,
                                                      BusinessCode = b.BusinessCode,
                                                      ForeignTransactionName = b.BusinessNameEn,
                                                      Address = b.DiaChiTruSo,
                                                      PhoneNumber = b.SoDienThoai,
                                                      BusinessCertificate = b.GiayPhepSanXuat,
                                                      LicenseDate = b.NgayCapPhep,
                                                      TaxId = b.MaSoThue,
                                                      Fax = gb.Fax,
                                                      Licensors = gb.Licensors,
                                                      NumDoc = gb.NumDoc,
                                                      DateEnd = gb.DateEnd,
                                                      DateStart = gb.DateStart,
                                                      ComplianceStatus = gb.ComplianceStatus,
                                                      ComplianceStatusName = c.CategoryName,
                                                      District = b.DistrictId
                                                  }).ToList().AsQueryable();
            if (query.Sort != null
                  && !string.IsNullOrEmpty(query.Sort.ColumnName)
                  && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }
            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                    || */
                   x.BusinessName.ToLower().Contains(_keywordSearch) || x.BusinessCertificate.ToLower().Contains(_keywordSearch) || x.PhoneNumber.ToLower().Contains(_keywordSearch) || x.Address.ToLower().Contains(_keywordSearch)
               );  //Lấy table đã select tìm kiếm theo keyword
            }
            if (query.Filter != null && query.Filter.ContainsKey("TypeBusiness") && !string.IsNullOrEmpty(query.Filter["TypeBusiness"]))
            {
                _data = _data.Where(x => x.TypeBusiness.ToString() == query.Filter["TypeBusiness"]);
            }
            if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"]))
            {
                if (query.Filter["Status"] == "1")
                {
                    _data = _data.Where(x => x.NumDoc != "");
                }
                else
                {
                    _data = _data.Where(x => x.NumDoc == "");
                }
            }
            if (query.Filter != null && query.Filter.ContainsKey("ComplianceStatus") && !string.IsNullOrEmpty(query.Filter["ComplianceStatus"]))
            {
                _data = _data.Where(x => x.ComplianceStatus.ToString() == query.Filter["ComplianceStatus"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
            {
                _data = _data.Where(x => x.District.ToString() == query.Filter["District"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("DateStart") && !string.IsNullOrEmpty(query.Filter["DateStart"]))
            {
                _data = _data.Where(x => x.DateStart >= DateTime.ParseExact(query.Filter["DateStart"], "dd/MM/yyyy", null));
            }
            if (query.Filter != null && query.Filter.ContainsKey("DateEnd") && !string.IsNullOrEmpty(query.Filter["DateEnd"]))
            {
                _data = _data.Where(x => x.DateStart <= DateTime.ParseExact(query.Filter["DateEnd"], "dd/MM/yyyy", null));
            }
            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            //Query data
            var data = FindData(query);
            // var _data = OkObjectResult(dat)
            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Quanlykinhdoanhkhi.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 5;
                    int row = 1;

                    worksheet.Cell(1, 1).Value = "QUẢN LÝ LĨNH VỰC KINH DOANH KHÍ";

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessName;
                            worksheet.Cell(index, 3).Value = item.BusinessCode;
                            worksheet.Cell(index, 4).Value = item.PhoneNumber;
                            worksheet.Cell(index, 5).Value = item.Address;
                            worksheet.Cell(index, 6).Value = item.TypeBusiness == 0 ? "Thương nhân kinh doanh" : "Cửa hàng bán lẻ";
                            worksheet.Cell(index, 8).Value = item.NumDoc;
                            worksheet.Cell(index, 7).Value = item.DateStart?.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 9).Value = item.ComplianceStatusName;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessName;
                            worksheet.Cell(index, 3).Value = item.BusinessCode;
                            worksheet.Cell(index, 4).Value = item.PhoneNumber;
                            worksheet.Cell(index, 5).Value = item.Address;
                            worksheet.Cell(index, 6).Value = item.TypeBusiness == 0 ? "Thương nhân kinh doanh" : "Cửa hàng bán lẻ";
                            worksheet.Cell(index, 8).Value = item.NumDoc;
                            worksheet.Cell(index, 7).Value = item.DateStart?.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 9).Value = item.ComplianceStatusName;
                            index++;
                            row++;
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
