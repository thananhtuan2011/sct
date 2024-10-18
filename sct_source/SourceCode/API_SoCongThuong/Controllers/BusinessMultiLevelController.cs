using API_SoCongThuong.Classes;
using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessMultiLevelController : ControllerBase
    {
        private BusinessMultiLevelRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public BusinessMultiLevelController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new BusinessMultiLevelRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<BusinessMultiLevelModel> model = new BaseModels<BusinessMultiLevelModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<BusinessMultiLevelModel, object> _orderByExpression = x => x.BusinessCode; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<BusinessMultiLevelModel, object>> _sortableFields = new Dictionary<string, Func<BusinessMultiLevelModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "BusinessCode", x => x.BusinessCode },
                        { "NumCert", x => x.NumCert },
                        { "CertDate", x => x.CertDate },
                        { "CertExp", x => x.CertExp },
                        { "Goods", x => x.Goods },
                        { "Address", x => x.Address },
                        { "StatusName", x => x.StatusName }
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<BusinessMultiLevelModel> _data = from m in _repo._context.BusinessMultiLevels.Where(x => !x.IsDel)
                                                                   join b in _repo._context.Businesses.Where(x => !x.IsDel)
                                                                   on m.BusinessId equals b.BusinessId into res
                                                                   from r in res.DefaultIfEmpty()
                                                                   join cate in _repo._context.Categories on m.Status equals cate.CategoryId
                                                                   select new BusinessMultiLevelModel()
                                                                   {
                                                                       BusinessMultiLevelId = m.BusinessMultiLevelId,
                                                                       BusinessName = r.BusinessNameVi,
                                                                       BusinessId = m.BusinessId,
                                                                       BusinessCode = r.BusinessCode,
                                                                       NumCert = m.NumCert,
                                                                       CertDate = m.CertDate,
                                                                       CertExp = m.CertExp,
                                                                       Goods = m.Goods,
                                                                       Address = m.Address,
                                                                       StatusName = cate.CategoryName,
                                                                       StartDate = m.StartDate,
                                                                       Status = m.Status,
                                                                       DistrictId = m.DistrictId
                                                                   };
                //_data = _data.Where(x => !x.IsDel);

                //Search
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.BusinessName.ToLower().Contains(_keywordSearch)
                        || x.BusinessCode.ToLower().Contains(_keywordSearch)
                        || x.NumCert.ToLower().Contains(_keywordSearch)
                        || x.StatusName.ToLower().Contains(_keywordSearch)
                        || x.Goods.ToLower().Contains(_keywordSearch)
                        || x.Address.ToLower().Contains(_keywordSearch)
                   );
                }

                //Filter
                if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"].ToString()))
                {
                    _data = _data.Where(x => x.Status == Guid.Parse(query.Filter["Status"]));
                }
                if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"].ToString()))
                {
                    _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["DistrictId"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                    && !string.IsNullOrEmpty(query.Filter["MinDate"]))
                {
                    _data = _data.Where(x =>
                                (x.StartDate) >=
                                DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                    && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
                {
                    _data = _data.Where(x =>
                               x.StartDate <=
                                DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<BusinessMultiLevelModel> model = new BaseModels<BusinessMultiLevelModel>();
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
        public async Task<IActionResult> Update(BusinessMultiLevelModel data)
        {
            BaseModels<BusinessMultiLevelModel> model = new BaseModels<BusinessMultiLevelModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                data.UpdateUserId = loginData.Userid;
                data.UpdateTime = DateTime.Now;

                await _repo.Update(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, "Quản lý cơ sở hoạt động bán hàng đa cấp", Action_Status.SUCCESS);
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
        public async Task<IActionResult> create(BusinessMultiLevelModel data)
        {
            BaseModels<BusinessMultiLevelModel> model = new BaseModels<BusinessMultiLevelModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.USER_NOT_FOUND));
                }
                SystemLog datalog = new SystemLog();

                data.CreateUserId = loginData.Userid;
                data.CreateTime = DateTime.Now;

                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, "Quản lý cơ sở hoạt động bán hàng đa cấp", Action_Status.SUCCESS);
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, "Quản lý cơ sở hoạt động bán hàng đa cấp", Action_Status.SUCCESS);
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

        private List<BusinessMultiLevelModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            string _keywordSearch = "";
            IQueryable<BusinessMultiLevelModel> _data = from m in _repo._context.BusinessMultiLevels.Where(x => !x.IsDel)
                                                        join b in _repo._context.Businesses.Where(x => !x.IsDel)
                                                        on m.BusinessId equals b.BusinessId into res
                                                        from r in res.DefaultIfEmpty()
                                                        join cate in _repo._context.Categories on m.Status equals cate.CategoryId
                                                        select new BusinessMultiLevelModel()
                                                        {
                                                            BusinessMultiLevelId = m.BusinessMultiLevelId,
                                                            BusinessName = r.BusinessNameVi,
                                                            BusinessId = m.BusinessId,
                                                            BusinessCode = r.BusinessCode,
                                                            NumCert = m.NumCert,
                                                            CertDate = m.CertDate,
                                                            CertExp = m.CertExp,
                                                            Goods = m.Goods,
                                                            Address = m.Address,
                                                            StatusName = cate.CategoryName,
                                                            StartDate = m.StartDate,
                                                            Status = m.Status,
                                                            DistrictId = m.DistrictId
                                                        };
 
            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                    x.BusinessName.ToLower().Contains(_keywordSearch)
                    || x.BusinessCode.ToLower().Contains(_keywordSearch)
                    || x.NumCert.ToLower().Contains(_keywordSearch)
                    || x.StatusName.ToLower().Contains(_keywordSearch)
                    || x.Goods.ToLower().Contains(_keywordSearch)
                    || x.Address.ToLower().Contains(_keywordSearch)
               );
            }

            //Filter
            if (query.Filter != null && query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"].ToString()))
            {
                _data = _data.Where(x => x.Status == Guid.Parse(query.Filter["Status"]));
            }
            if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"].ToString()))
            {
                _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["DistrictId"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinDate")
                && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                _data = _data.Where(x =>
                            (x.StartDate) >=
                            DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate")
                && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                _data = _data.Where(x =>
                           x.StartDate <=
                            DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
            }

            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            var data = FindData(query);

            if (!data.Any())
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Danhsachcosobanhangdacap.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 4;
                    int row = 1;

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessCode;
                            worksheet.Cell(index, 3).Value = item.BusinessName;
                            worksheet.Cell(index, 4).Value = item.NumCert;
                            worksheet.Cell(index, 5).Value = item.StartDate.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 6).Value = item.CertDate.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 7).Value = item.CertExp?.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 8).Value = item.Goods;
                            worksheet.Cell(index, 9).Value = item.Address;
                            worksheet.Cell(index, 10).Value = item.StatusName;
                            worksheet.Cell(index, 11).Value = item.Note;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessCode;
                            worksheet.Cell(index, 3).Value = item.BusinessName;
                            worksheet.Cell(index, 4).Value = item.NumCert;
                            worksheet.Cell(index, 5).Value = item.StartDate.ToString("dd/MM/yyy");
                            worksheet.Cell(index, 6).Value = item.CertDate.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 7).Value = item.CertExp?.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 8).Value = item.Goods;
                            worksheet.Cell(index, 9).Value = item.Address;
                            worksheet.Cell(index, 10).Value = item.StatusName;
                            worksheet.Cell(index, 11).Value = item.Note; 
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

