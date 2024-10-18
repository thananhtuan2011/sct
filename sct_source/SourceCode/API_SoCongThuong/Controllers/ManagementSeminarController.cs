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
using Microsoft.EntityFrameworkCore.Internal;
using System.Net;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;
using DocumentFormat.OpenXml.EMMA;
using System;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementSeminarController : ControllerBase
    {
        private ManagementSeminarRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ManagementSeminarController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ManagementSeminarRepo(context);
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;

        }
        // Lấy danh sách 
        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ManagementSeminarModel> model = new BaseModels<ManagementSeminarModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                ////Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<ManagementSeminarModel, object> _orderByExpression = x => x.ProfileCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ManagementSeminarModel, object>> _sortableFields = new Dictionary<string, Func<ManagementSeminarModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "ProfileCode", x => x.ProfileCode },
                        { "BusinessName", x => x.BusinessName },
                        { "Title", x => x.Title },
                        { "Address", x => x.Address },
                        { "Contact", x => x.Contact },
                        { "PhoneNumber", x => x.PhoneNumber },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<ManagementSeminarModel> _data = (from ms in _repo._context.ManagementSeminars where !ms.IsDel
                                                           join b in _repo._context.Businesses on ms.BusinessId equals b.BusinessId
                                                           select new ManagementSeminarModel()
                                                           {
                                                               ManagementSeminarId = ms.ManagementSeminarId,
                                                               ProfileCode = ms.ProfileCode,
                                                               BusinessId = ms.BusinessId,
                                                               BusinessName = b.BusinessNameVi,
                                                               Title = ms.Title,
                                                               Address = ms.Address,
                                                               Contact = ms.Contact,
                                                               PhoneNumber = ms.PhoneNumber,
                                                               DistrictId = ms.DistrictId
                                                           }).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.ProfileCode.ToLower().Contains(_keywordSearch)
                    || x.BusinessName.ToLower().Contains(_keywordSearch)
                    || x.Title.ToLower().Contains(_keywordSearch)
                    || x.Address.ToLower().Contains(_keywordSearch)
                    || x.Contact.ToLower().Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
                }

                var listTime = _repo._context.TimeManagementSeminars.Where(x => !x.IsDel).ToList().AsQueryable();
                // model.items = _data.ToList();
                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["District"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate"))
                {
                    var listId = listTime.Where(x => x.StartTime >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null)).Select(x => x.ManagementSeminarId).Distinct().ToList();
                    _data = (from data in _data where listId.Contains(data.ManagementSeminarId) select data).ToList().AsQueryable();
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate"))
                {
                    var listId = listTime.Where(x => x.StartTime <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null)).Select(x => x.ManagementSeminarId).Distinct().ToList();
                    _data = (from data in _data where listId.Contains(data.ManagementSeminarId) select data).ToList().AsQueryable();
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
        #endregion

        [HttpPost()]
        public async Task<IActionResult> create(ManagementSeminarModel data)
        {
            BaseModels<ManagementSeminarModel> model = new BaseModels<ManagementSeminarModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
               

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, "Quản lý hồ sơ hội nghị, hội thảo, đào tạo kiến thức bán hàng đa cấp", Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(ManagementSeminarModel data)
        {
            BaseModels<ManagementSeminarModel> model = new BaseModels<ManagementSeminarModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                data.UpdateTime = DateTime.Now;
                data.UpdateUserId = loginData.Userid;
                await _repo.Update(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, "Quản lý hồ sơ hội nghị, hội thảo, đào tạo kiến thức bán hàng đa cấp", Action_Status.SUCCESS);
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
            BaseModels<ManagementSeminarModel> model = new BaseModels<ManagementSeminarModel>();
            try
            {
                var info = _repo.FindById(id);
                //var storelist = _repo.FindStoreId(id).Select(x => x.CateCriterionId).ToList().ToString();
                if (info == null)
                    return NotFound(ErrMsg_Const.GetMsg(ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY));

                //Set data cho base model
                model.status = 1;
                model.data = info;
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, "Quản lý hồ sơ hội nghị, hội thảo, đào tạo kiến thức bán hàng đa cấp", Action_Status.SUCCESS);
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

        private List<ManagementSeminarModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<ManagementSeminarModel, object> _orderByExpression = x => x.ProfileCode; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<ManagementSeminarModel, object>> _sortableFields = new Dictionary<string, Func<ManagementSeminarModel, object>>   //Khởi tạo các trường để sắp xếp
                {
                    { "ProfileCode", x => x.ProfileCode },
                    { "BusinessName", x => x.BusinessName },
                    { "Title", x => x.Title },
                    { "Address", x => x.Address },
                    { "Contact", x => x.Contact },
                    { "PhoneNumber", x => x.PhoneNumber },
                };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }
            var listTime = _repo._context.TimeManagementSeminars.Where(x => !x.IsDel).ToList().AsQueryable();

            IQueryable<ManagementSeminarModel> _data = (from ms in _repo._context.ManagementSeminars
                                                        where !ms.IsDel
                                                        join b in _repo._context.Businesses on ms.BusinessId equals b.BusinessId
                                                        select new ManagementSeminarModel()
                                                        {
                                                            ManagementSeminarId = ms.ManagementSeminarId,
                                                            ProfileCode = ms.ProfileCode,
                                                            BusinessId = ms.BusinessId,
                                                            BusinessName = b.BusinessNameVi,
                                                            Title = ms.Title,
                                                            Address = ms.Address,
                                                            Contact = ms.Contact,
                                                            PhoneNumber = ms.PhoneNumber,
                                                            DistrictId = ms.DistrictId,
                                                            NumberParticipant = ms.NumberParticipant,
                                                            Note = ms.Note,
                                                        }).ToList().AsQueryable();

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.ProfileCode.ToLower().Contains(_keywordSearch)
                || x.BusinessName.ToLower().Contains(_keywordSearch)
                || x.Title.ToLower().Contains(_keywordSearch)
                || x.Address.ToLower().Contains(_keywordSearch)
                || x.Contact.ToLower().Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
            }
            // model.items = _data.ToList();
            if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
            {
                _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["District"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinDate"))
            {
                var listId = listTime.Where(x => x.StartTime >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null)).Select(x => x.ManagementSeminarId).Distinct().ToList();
                _data = (from data in _data where listId.Contains(data.ManagementSeminarId) select data).ToList().AsQueryable();
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate"))
            {
                var listId = listTime.Where(x => x.StartTime <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null)).Select(x => x.ManagementSeminarId).Distinct().ToList();
                _data = (from data in _data where listId.Contains(data.ManagementSeminarId) select data).ToList().AsQueryable();
            }

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);

            if (!data.Any() || data.Count == 0)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/HoiNghiHoiThaoBHDC.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 5;
                    int row = 1;
                    var listTime = _repo._context.TimeManagementSeminars.Where(x => !x.IsDel).ToList().AsQueryable();

                    foreach (var item in data)
                    {
                            var addrow = worksheet.Row(index);
                            addrow.InsertRowsBelow(1);
                            var _data = listTime.Where(x => x.ManagementSeminarId == item.ManagementSeminarId).ToList();
                            string time = "";
                            foreach(var _item in _data)
                            {
                                time += $"{_item.StartTime.ToString("hh:mm dd/MM/yyyy")} - {_item.EndTime.ToString("hh:mm dd/MM/yyyy")}\r\t";
                            }
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProfileCode;
                            worksheet.Cell(index, 3).Value = item.BusinessName;
                            worksheet.Cell(index, 4).Value = time;
                            worksheet.Cell(index, 5).Value = item.Address;
                            worksheet.Cell(index, 6).Value = item.Title;
                            worksheet.Cell(index, 7).Value = $"{item.Contact} \r\t {item.PhoneNumber}" ;
                            worksheet.Cell(index, 8).Value = item.NumberParticipant;
                            worksheet.Cell(index, 9).Value = item.Note;
                            index++;
                            row++;  
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

