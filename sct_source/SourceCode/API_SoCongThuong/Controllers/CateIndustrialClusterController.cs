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

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateIndustrialClusterController : ControllerBase
    {
        private CateIndustrialClusterRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public CateIndustrialClusterController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateIndustrialClusterRepo(context);
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

            BaseModels<CateIndustrialClusterModel> model = new BaseModels<CateIndustrialClusterModel>();
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

                Func<CateIndustrialClusterModel, object> _orderByExpression = x => x.CateIndustrialClustersId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CateIndustrialClusterModel, object>> _sortableFields = new Dictionary<string, Func<CateIndustrialClusterModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "IndustrialClustersName", x => x.IndustrialClustersName },
                        { "OriginalArea", x => x.OriginalArea },
                        { "DetailedArea", x => x.DetailedArea },
                        { "ExpandedArea", x => x.ExpandedArea },
                        { "IndustrialArea", x => x.IndustrialArea },
                        { "RentedArea", x => x.RentedArea },
                        { "Occupancy", x => x.Occupancy },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<CateIndustrialClusterModel> _data = _repo._context.CateIndustrialClusters.Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users,
                    cc => cc.CreateUserId,
                    u => u.UserId,
                     (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us) 
                     => new CateIndustrialClusterModel
                     {
                         CateIndustrialClustersId = info.cc.CateIndustrialClustersId,
                         IndustrialClustersName= info.cc.IndustrialClustersName,
                         Occupancy=info.cc.Occupancy,
                         OriginalArea=info.cc.OriginalArea,
                         RentedArea=info.cc.RentedArea,
                         DetailedArea=info.cc.DetailedArea,
                         ExpandedArea=info.cc.ExpandedArea,
                         IndustrialArea=info.cc.IndustrialArea,
                         CreateName = us.FullName,
                         CreateTimeDisplay = info.cc.CreateTime.ToString("dd/MM/yyyy hh:mm"),
                         District = info.cc.District
                     });
                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.District == Guid.Parse(query.Filter["District"]));
                }

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x => x.IndustrialClustersName.ToLower().Contains(_keywordSearch)
                    || x.Occupancy.ToString().Contains(_keywordSearch)
                    || x.OriginalArea.ToString().Contains(_keywordSearch)
                    || x.RentedArea.ToString().Contains(_keywordSearch)
                    || x.DetailedArea.ToString().Contains(_keywordSearch)
                    || x.ExpandedArea.ToString().Contains(_keywordSearch)
                    || x.IndustrialArea.ToString().Contains(_keywordSearch)
                    //|| x.LicenseDateDisplay.ToString().Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
                }
                // model.items = _data.ToList();

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
                ////Đoạn này lấy total đã tối ưu việc call DB nhiều lần
                //var listId = model.items.Select(x => x.CateCriteriaId).ToList();
                //var listTotal = _repo._context.CateCriteria.Where(x => listId.Contains(x.CateCriteriaId)).Select(x =>
                // new CateCriteriaModel
                // {
                //     CateCriteriaId = x.CateCriteriaId
                // }).ToList();
                //for (int i = 0; i < model.items.Count(); i++)
                //{
                //    int tt = listTotal.Where(x => x.CateCriterionId == model.items[i].CateCriterionId).Select(x => x.TotalStore).FirstOrDefault(0);
                //    model.items[i].TotalStore = tt;
                //}
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
        public async Task<IActionResult> create(CateIndustrialClusterModel data)
        {
            BaseModels<CateIndustrialClusterModel> model = new BaseModels<CateIndustrialClusterModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (string.IsNullOrEmpty(data.IndustrialClustersName))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_INDUSTRIAL_CLUSTER, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.CATE_INDUSTRIAL_CLUSTER, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateIndustrialClusterModel data)
        {
            BaseModels<CateIndustrialClusterModel> model = new BaseModels<CateIndustrialClusterModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                var CheckData = _repo.FindById(data.CateIndustrialClustersId);
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_INDUSTRIAL_CLUSTER, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.IndustrialClustersName))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_INDUSTRIAL_CLUSTER, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.CATE_INDUSTRIAL_CLUSTER, Action_Status.SUCCESS);
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
            BaseModels<CateIndustrialClusterModel> model = new BaseModels<CateIndustrialClusterModel>();
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
                await _repo.Delete(id);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.CATE_INDUSTRIAL_CLUSTER, Action_Status.SUCCESS);
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

        private List<CateIndustrialClusterModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<CateIndustrialClusterModel, object> _orderByExpression = x => x.CateIndustrialClustersId; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<CateIndustrialClusterModel, object>> _sortableFields = new Dictionary<string, Func<CateIndustrialClusterModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "IndustrialClustersName", x => x.IndustrialClustersName },
                        { "OriginalArea", x => x.OriginalArea },
                        { "DetailedArea", x => x.DetailedArea },
                        { "ExpandedArea", x => x.ExpandedArea },
                        { "IndustrialArea", x => x.IndustrialArea },
                        { "RentedArea", x => x.RentedArea },
                        { "Occupancy", x => x.Occupancy },
                        { "CreateName", x => x.CreateUserId },
                        { "CreateTimeDisplay", x => x.CreateTime },
                        { "IsAction", x => x.IsAction },
                    };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }

            IQueryable<CateIndustrialClusterModel> _data = _repo._context.CateIndustrialClusters.Where(x => !x.IsDel).Select(x => new CateIndustrialClusterModel
            {
                IndustrialClustersName = x.IndustrialClustersName,
                OriginalArea = x.OriginalArea,
                EstablishCode = x.EstablishCode,
                ExpandedArea = x.ExpandedArea,
                DecisionExpandCode = x.DecisionExpandCode,
                DetailedArea = x.DetailedArea,
                ApprovalDecision = x.ApprovalDecision,
                IndustrialArea = x.IndustrialArea,
                RentedArea = x.RentedArea,
                Occupancy = x.Occupancy,
                Note = x.Note,
                District = x.District
            });

            if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
            {
                _data = _data.Where(x => x.District == Guid.Parse(query.Filter["District"]));
            }

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.IndustrialClustersName.ToLower().Contains(_keywordSearch)
                || x.Occupancy.ToString().Contains(_keywordSearch)
                || x.OriginalArea.ToString().Contains(_keywordSearch)
                || x.RentedArea.ToString().Contains(_keywordSearch)
                || x.DetailedArea.ToString().Contains(_keywordSearch)
                || x.ExpandedArea.ToString().Contains(_keywordSearch)
                || x.IndustrialArea.ToString().Contains(_keywordSearch)
                //|| x.LicenseDateDisplay.ToString().Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
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
                string districtName = "";
                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    string districtId = query.Filter["District"];
                    districtName = _repo._context.Districts.Where(x => !x.IsDel && x.DistrictId == Guid.Parse(districtId)).Select(x => x.DistrictName).FirstOrDefault();
                }
               
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyCCN.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    if (districtName != "")
                    {
                        worksheet.Cell(1, 1).Value = $"TÌNH HÌNH CÁC CỤM CÔNG NGHIỆP ĐÃ THÀNH LẬP TRÊN ĐỊA BÀN {districtName.ToUpper()}";
                        worksheet.Cell(2, 1).Value = "Đơn vị báo cáo: Phòng quản lý chuyên môn công thương thuộc Ủy ban nhân dân cấp huyện";

                    }
                    else
                    {
                        worksheet.Cell(1, 1).Value = "TÌNH HÌNH CÁC CỤM CÔNG NGHIỆP ĐÃ THÀNH LẬP TRÊN ĐỊA BÀN TỈNH";
                        worksheet.Cell(2, 1).Value = "Đơn vị báo cáo: Sở Công Thương";

                    }
                    int index = 9;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index);
                        addrow.InsertRowsBelow(1);

                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.IndustrialClustersName;
                        worksheet.Cell(index, 3).Value = item.OriginalArea;
                        worksheet.Cell(index, 4).Value = item.EstablishCode;
                        worksheet.Cell(index, 5).Value = item.ExpandedArea;
                        worksheet.Cell(index, 6).Value = item.DecisionExpandCode;
                        worksheet.Cell(index, 7).Value = item.DetailedArea;
                        worksheet.Cell(index, 8).Value = item.ApprovalDecision;
                        worksheet.Cell(index, 9).Value = item.IndustrialArea;
                        worksheet.Cell(index, 10).Value = item.RentedArea;
                        worksheet.Cell(index, 11).Value = item.Occupancy;
                        worksheet.Cell(index, 12).Value = item.Note;

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
