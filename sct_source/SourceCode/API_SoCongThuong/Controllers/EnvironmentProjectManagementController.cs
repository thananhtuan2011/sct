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
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Globalization;
using API_SoCongThuong.Logger;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentProjectManagementController : ControllerBase
    {
        private EnvironmentProjectManagementRepo _repo;
        private IConfiguration _config;
        //private BusinessRepo _repoBusi;

        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public EnvironmentProjectManagementController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new EnvironmentProjectManagementRepo(context);
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

            BaseModels<EnvironmentProjectManagementModel> model = new BaseModels<EnvironmentProjectManagementModel>();
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

                Func<EnvironmentProjectManagementModel, object> _orderByExpression = x => x.EnvironmentProjectManagementId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<EnvironmentProjectManagementModel, object>> _sortableFields = new Dictionary<string, Func<EnvironmentProjectManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "ProjectName", x => x.ProjectName },
                    { "ImplementationContent", x => x.ImplementationContent },
                    { "ApprovedFunding", x => x.ApprovedFunding },
                    { "ImplementationCost", x => x.ImplementationCost },
                    { "CoordinationUnit", x => x.CoordinationUnit },
                    { "YearOfImplementationFrom", x => x.YearOfImplementationFrom },
                    { "YearOfImplementationTo", x => x.YearOfImplementationTo },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<EnvironmentProjectManagementModel> _data =
                    from info in _repo._context.EnvironmentProjectManagements
                    where !info.IsDel
                    select new EnvironmentProjectManagementModel
                    {
                        EnvironmentProjectManagementId = info.EnvironmentProjectManagementId,
                        ProjectName = info.ProjectName,
                        ImplementationContent = info.ImplementationContent,
                        ApprovedFunding = info.ApprovedFunding,
                        ImplementationCost = info.ImplementationCost,
                        CoordinationUnit = info.CoordinationUnit,
                        YearOfImplementationFrom = info.YearOfImplementationFrom,
                        YearOfImplementationTo = info.YearOfImplementationTo
                    };

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();

                    _data = _data.Where(x => x.ProjectName.ToLower().Contains(_keywordSearch)
                    || x.ImplementationContent.ToLower().Contains(_keywordSearch)
                    || x.ApprovedFunding.ToLower().Contains(_keywordSearch)
                    || x.ImplementationCost.ToLower().Contains(_keywordSearch)
                    || x.CoordinationUnit.ToLower().Contains(_keywordSearch)
                    || x.YearOfImplementationFrom.ToString().Contains(_keywordSearch)
                    || x.YearOfImplementationTo.ToString().Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
                }

                if (query.Filter != null && query.Filter.ContainsKey("YearOfImplementationFrom")
                                         && !string.IsNullOrEmpty(query.Filter["YearOfImplementationFrom"]))
                {
                    _data = _data.Where(x =>
                        x.YearOfImplementationFrom >= int.Parse(query.Filter["YearOfImplementationFrom"])
                    );
                };
                if (query.Filter != null && query.Filter.ContainsKey("YearOfImplementationTo")
                         && !string.IsNullOrEmpty(query.Filter["YearOfImplementationTo"]))
                {
                    _data = _data.Where(x =>
                        x.YearOfImplementationTo <= int.Parse(query.Filter["YearOfImplementationTo"])
                    );
                };

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
        public async Task<IActionResult> create([FromForm] EnvironmentProjectManagementModel data)
        {
            BaseModels<EnvironmentProjectManagementModel> model = new BaseModels<EnvironmentProjectManagementModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                #region gắn hàm upload file
                var Files = Request.Form.Files;
                var LstFile = new List<EnvironmentProjectManagementAttachFileModel>();
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
                                Linkfile = "EnvironmentProjectManagement"
                            };
                            var result = Ulities.UploadFile(up, _config);

                            EnvironmentProjectManagementAttachFileModel fileSave = new EnvironmentProjectManagementAttachFileModel();
                            fileSave.LinkFile = result.link;
                            LstFile.Add(fileSave);
                        }
                    }
                }
                //DateTime date = DateTime.ParseExact(data.TrainingTimeDisplay, "dd/MM/yyyy", null);
                //data.TrainingTime = date;
                data.FileUpload = LstFile;
                #endregion
                SystemLog datalog = new SystemLog();
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.ENVIRONMENT_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update([FromForm] EnvironmentProjectManagementModel data)
        {
            BaseModels<EnvironmentProjectManagementModel> model = new BaseModels<EnvironmentProjectManagementModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.EnvironmentProjectManagementId, _config);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ENVIRONMENT_PROJECT_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    #region gắn hàm upload file
                    var Files = Request.Form.Files;
                    var LstFile = new List<EnvironmentProjectManagementAttachFileModel>();
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
                                    Linkfile = "EnvironmentProjectManagement"
                                };
                                var result = Ulities.UploadFile(up, _config);

                                EnvironmentProjectManagementAttachFileModel fileSave = new EnvironmentProjectManagementAttachFileModel();
                                fileSave.LinkFile = result.link;
                                LstFile.Add(fileSave);
                            }
                        }
                    }
                    data.FileUpload = LstFile;
                    #endregion

                    //DateTime date = DateTime.ParseExact(data.TrainingTimeDisplay, "dd/MM/yyyy", null);
                    //data.TrainingTime = date;
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data, _config);
                    datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.ENVIRONMENT_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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
            BaseModels<EnvironmentProjectManagementModel> model = new BaseModels<EnvironmentProjectManagementModel>();
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
        //[Route("deletes")]
        //[HttpPut()]
        //public async Task<IActionResult> deletes(List<Guid> IdRemoves)
        //{
        //    BaseModels<object> model = new BaseModels<object>();
        //    try
        //    {
        //        await _repo.Deletes(IdRemoves);
        //        model.status = 1;
        //        return Ok(model);
        //    }
        //    catch (Exception ex)
        //    {

        //        model.status = 0;
        //        model.error = new ErrorModel()
        //        {
        //            Code = ErrCode_Const.EXCEPTION_API,
        //            Msg = ex.Message
        //        };
        //        return BadRequest(model);
        //    }
        //}
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
                datalog = Ulities.WriteLog(_config, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.ENVIRONMENT_PROJECT_MANAGEMENT, Action_Status.SUCCESS);
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

        private List<EnvironmentProjectManagementModel> FindData([FromBody] QueryRequestBody query)
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            Func<EnvironmentProjectManagementModel, object> _orderByExpression = x => x.EnvironmentProjectManagementId; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<EnvironmentProjectManagementModel, object>> _sortableFields = new Dictionary<string, Func<EnvironmentProjectManagementModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                    { "ProjectName", x => x.ProjectName },
                    { "ImplementationContent", x => x.ImplementationContent },
                    { "ApprovedFunding", x => x.ApprovedFunding },
                    { "ImplementationCost", x => x.ImplementationCost },
                    { "CoordinationUnit", x => x.CoordinationUnit },
                    { "YearOfImplementationFrom", x => x.YearOfImplementationFrom },
                    { "YearOfImplementationTo", x => x.YearOfImplementationTo },
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            IQueryable<EnvironmentProjectManagementModel> _data =
                from info in _repo._context.EnvironmentProjectManagements
                where !info.IsDel
                select new EnvironmentProjectManagementModel
                {
                    EnvironmentProjectManagementId = info.EnvironmentProjectManagementId,
                    ProjectName = info.ProjectName,
                    ImplementationContent = info.ImplementationContent,
                    ApprovedFunding = info.ApprovedFunding,
                    ImplementationCost = info.ImplementationCost,
                    CoordinationUnit = info.CoordinationUnit,
                    YearOfImplementationFrom = info.YearOfImplementationFrom,
                    YearOfImplementationTo = info.YearOfImplementationTo,
                    FileUpload = _repo._context.EnvironmentProjectManagementAttachFiles.Where(d => d.EnvironmentProjectManagementId == info.EnvironmentProjectManagementId).Select(model => new EnvironmentProjectManagementAttachFileModel
                    {
                        EnvironmentProjectManagementAttachFileId = model.EnvironmentProjectManagementAttachFileId,
                        LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : _config.GetValue<string>("MinioConfig:Protocol") + _config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
                    }).ToList()

                };
            
            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();

                _data = _data.Where(x => x.ProjectName.ToLower().Contains(_keywordSearch)
                || x.ImplementationContent.ToLower().Contains(_keywordSearch)
                || x.ApprovedFunding.ToLower().Contains(_keywordSearch)
                || x.ImplementationCost.ToLower().Contains(_keywordSearch)
                || x.CoordinationUnit.ToLower().Contains(_keywordSearch)
                || x.YearOfImplementationFrom.ToString().Contains(_keywordSearch)
                || x.YearOfImplementationTo.ToString().Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
            }

            if (query.Filter != null && query.Filter.ContainsKey("YearOfImplementationFrom")
                                     && !string.IsNullOrEmpty(query.Filter["YearOfImplementationFrom"]))
            {
                _data = _data.Where(x =>
                    x.YearOfImplementationFrom >= int.Parse(query.Filter["YearOfImplementationFrom"])
                );
            };
            if (query.Filter != null && query.Filter.ContainsKey("YearOfImplementationTo")
                     && !string.IsNullOrEmpty(query.Filter["YearOfImplementationTo"]))
            {
                _data = _data.Where(x =>
                    x.YearOfImplementationTo <= int.Parse(query.Filter["YearOfImplementationTo"])
                );
            };

            int _countRows = _data.Count(); //Đếm số dòng của table đã select được

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
                using (var workbook = new XLWorkbook(@"Upload/Templates/ATKTMT_DuAnBVMT.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);

                    int index = 4;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index - 1);
                        addrow.InsertRowsBelow(1);
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.ProjectName;
                        worksheet.Cell(index, 3).Value = item.ImplementationContent;
                        worksheet.Cell(index, 4).Value = string.Format(new CultureInfo("vi-VN"), "{0:#,##0.00}", Int32.Parse(item.ApprovedFunding));
                        worksheet.Cell(index, 5).Value = string.Format(new CultureInfo("vi-VN"), "{0:#,##0.00}", Int32.Parse(item.ImplementationCost)); // item.ImplementationCost;
                        worksheet.Cell(index, 6).Value = item.CoordinationUnit;
                        worksheet.Cell(index, 7).Value = item.YearOfImplementationFrom;
                        if (item.FileUpload.Count > 0)
                        {
                            var str = "";
                            foreach (var it in item.FileUpload)
                            {
                                str += it.LinkFile;
                                str += "\r\n";
                            }
                            worksheet.Cell(index, 8).Value = str;
                        }
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
