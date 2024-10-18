using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.BusinessIndustryRepository;
using API_SoCongThuong.Reponsitories.BusinessRepository;
using API_SoCongThuong.Reponsitories.IndustryRepository;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
using API_SoCongThuong.Reponsitories.TypeOfProfessionRepository;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using API_SoCongThuong.Reponsitories.DistrictRepository;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API_SoCongThuong.Reponsitories;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using static API_SoCongThuong.Classes.Ulities;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using System.Net;
using API_SoCongThuong.Logger;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOcopController : ControllerBase
    {
        private ProductOcopRepo _repo;
        private IConfiguration _config;
        //private BusinessRepo _repoBusi;

        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public ProductOcopController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new ProductOcopRepo(context);
            _config = configuration;
            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);

        }
        // Lấy danh sách 
        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ProductOcopModel> model = new BaseModels<ProductOcopModel>();
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

                Func<ProductOcopModel, object> _orderByExpression = x => x.ProductOcopid; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ProductOcopModel, object>> _sortableFields = new Dictionary<string, Func<ProductOcopModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "ProductName", x => x.ProductName },
                    { "ProductOwner", x => x.ProductOwner },
                    { "Ratings", x => x.Ratings },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<ProductOcopModel> _data = (from p in _repo._context.ProductOcops
                                                      join d in _repo._context.Districts on p.DistrictId equals d.DistrictId
                                                      where !p.IsDel && !d.IsDel
                                                      select new ProductOcopModel
                                                      {
                                                          ProductOcopid = p.ProductOcopid,
                                                          ProductName = p.ProductName,
                                                          ProductOwner = p.ProductOwner,
                                                          PhoneNumber = p.PhoneNumber,
                                                          Ratings = p.Ratings,
                                                          DistrictId = p.DistrictId,
                                                          DistrictName = d.DistrictName,
                                                          CreateTime = p.CreateTime,
                                                          LinkFileDisplay = _repo._context.ProductOcopAttachFiles
                                                                            .Where(img => img.ProductOcopid == p.ProductOcopid && img.Type == 0)
                                                                            .Select(img => _config.GetValue<string>("MinioConfig:Protocol") + _config.GetValue<string>("MinioConfig:MinioServer") + img.LinkFile)
                                                                            .FirstOrDefault() ?? ""
                                                      }
                                                     ).ToList().AsQueryable();

                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.ProductName.ToLower().Contains(_keywordSearch)
                    || x.ProductOwner.Contains(_keywordSearch)
                    );
                }

                if (query.Filter != null && query.Filter.ContainsKey("Rating"))
                {
                    _data = _data.Where(x => x.Ratings == int.Parse(query.Filter["Rating"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                    && !string.IsNullOrEmpty(query.Filter["MinTime"]))
                {
                    _data = _data.Where(x =>
                                (x.CreateTime) >=
                                DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                    && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                {
                    _data = _data.Where(x =>
                               x.CreateTime <=
                                DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("DistrictId")
                    && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
                {
                    _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]);
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
        public async Task<IActionResult> Create([FromForm] ProductOcopModel data)
        {
            BaseModels<ProductOcopModel> model = new BaseModels<ProductOcopModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                #region gắn hàm upload file
                var Files = Request.Form.Files;
                var LstFile = new List<ProductOcopAttachFileModel>();
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
                                Linkfile = "ProductOCOP"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            ProductOcopAttachFileModel fileSave = new ProductOcopAttachFileModel();
                            fileSave.LinkFile = result.link;
                            fileSave.Type = f.Name.Contains("HinhAnh") ? 0 : 1;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                data.Details = LstFile;
                #endregion
                var util = new Ulities();
                data = util.TrimModel(data);

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.PRODUCT_OCOP, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update([FromForm] ProductOcopModel data)
        {
            BaseModels<ProductOcopModel> model = new BaseModels<ProductOcopModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo.FindById(data.ProductOcopid, _config);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PRODUCT_OCOP, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    #region gắn hàm upload file
                    var Files = Request.Form.Files;
                    var LstFile = new List<ProductOcopAttachFileModel>();
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
                                    Linkfile = "ProductOCOP"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                ProductOcopAttachFileModel fileSave = new ProductOcopAttachFileModel();
                                fileSave.LinkFile = result.link;
                                fileSave.Type = f.Name.Contains("HinhAnh") ? 0 : 1;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    data.Details = LstFile;
                    #endregion
                    var util = new Ulities();
                    data = util.TrimModel(data);

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.PRODUCT_OCOP, Action_Status.SUCCESS);
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
        public IActionResult getItemById(Guid id)
        {
            BaseModels<ProductOcopModel> model = new BaseModels<ProductOcopModel>();
            try
            {
                var info = _repo.FindById(id, _config);
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
                await _repo.Delete(id, _config);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.PRODUCT_OCOP, Action_Status.SUCCESS);
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

        private List<ProductOcopModel> FindData(QueryRequestBody query)
        {
            bool _orderBy_ASC = true;
            string _keywordSearch = "";
            Func<ProductOcopModel, object> _orderByExpression = x => x.ProductOcopid;
            Dictionary<string, Func<ProductOcopModel, object>> _sortableFields = new Dictionary<string, Func<ProductOcopModel, object>>
                    {
                    { "ProductName", x => x.ProductName },
                    { "ProductOwner", x => x.ProductOwner },
                    { "Ratings", x => x.Ratings },
                    };

            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                _orderByExpression = _sortableFields[query.Sort.ColumnName];
            }

            IQueryable<ProductOcopModel> _data = (from p in _repo._context.ProductOcops
                                                  join d in _repo._context.Districts on p.DistrictId equals d.DistrictId
                                                  where !p.IsDel && !d.IsDel
                                                  select new ProductOcopModel
                                                  {
                                                      ProductOcopid = p.ProductOcopid,
                                                      ProductName = p.ProductName,
                                                      ProductOwner = p.ProductOwner,
                                                      PhoneNumber = p.PhoneNumber,
                                                      Address = p.Address,
                                                      Ratings = p.Ratings,
                                                      DistrictId = p.DistrictId,
                                                      DistrictName = d.DistrictName,
                                                      CreateTime = p.CreateTime,
                                                      ApprovalDecision = p.ApprovalDecision,
                                                      LinkFileDisplay = _repo._context.ProductOcopAttachFiles
                                                                        .Where(img => img.ProductOcopid == p.ProductOcopid && img.Type == 0)
                                                                        .Select(img => _config.GetValue<string>("MinioConfig:Protocol") + _config.GetValue<string>("MinioConfig:MinioServer") + img.LinkFile)
                                                                        .FirstOrDefault() ?? ""
                                                  }
                                                 ).ToList().AsQueryable();

            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.ProductName.ToLower().Contains(_keywordSearch)
                || x.ProductOwner.Contains(_keywordSearch)
                );
            }

            if (query.Filter != null && query.Filter.ContainsKey("Rating"))
            {
                _data = _data.Where(x => x.Ratings == int.Parse(query.Filter["Rating"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                && !string.IsNullOrEmpty(query.Filter["MinTime"]))
            {
                _data = _data.Where(x =>
                            (x.CreateTime) >=
                            DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
            {
                _data = _data.Where(x =>
                           x.CreateTime <=
                            DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("DistrictId")
                && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
            {
                _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]);
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/DanhsachsanphamdatOCOP.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 3;
                    int row = 1;

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProductName;
                            worksheet.Cell(index, 3).Value = item.ProductOwner;
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.PhoneNumber;
                            worksheet.Cell(index, 6).Value = item.Ratings;
                            worksheet.Cell(index, 7).Value = item.ApprovalDecision;
                            worksheet.Cell(index, 8).Value = item.LinkFileDisplay;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.ProductName;
                            worksheet.Cell(index, 3).Value = item.ProductOwner;
                            worksheet.Cell(index, 4).Value = item.Address;
                            worksheet.Cell(index, 5).Value = item.PhoneNumber;
                            worksheet.Cell(index, 6).Value = item.Ratings;
                            worksheet.Cell(index, 7).Value = item.ApprovalDecision;
                            worksheet.Cell(index, 8).Value = item.LinkFileDisplay;
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
