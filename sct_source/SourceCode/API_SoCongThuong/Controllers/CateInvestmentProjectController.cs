using DpsLibs.Web;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.TypeOfBusinessRepository;
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
    public class CateInvestmentProjectController : ControllerBase
    {
        private CateInvestmentProjectRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;
        public CateInvestmentProjectController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CateInvestmentProjectRepo(context);
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

            BaseModels<CateInvestmentProjectModel> model = new BaseModels<CateInvestmentProjectModel>();
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

                Func<CateInvestmentProjectModel, object> _orderByExpression = x => x.CateInvestmentProjectId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CateInvestmentProjectModel, object>> _sortableFields = new Dictionary<string, Func<CateInvestmentProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "Investment", x => x.Investment },
                        { "Produce", x => x.Produce },
                        { "Quantity", x => x.Quantity },
                        { "ProductValue", x => x.ProductValue },
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

                IQueryable<CateInvestmentProjectModel> _data = _repo._context.CateInvestmentProjects.Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users,
                    cc => cc.CreateUserId,
                    u => u.UserId,
                     (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us)
                     => new CateInvestmentProjectModel
                     {
                         CateInvestmentProjectId = info.cc.CateInvestmentProjectId,
                         BusinessName = info.cc.BusinessName,
                         Investment = info.cc.Investment,
                         Produce = info.cc.Produce,
                         ProductValue = info.cc.ProductValue,
                         Quantity = info.cc.Quantity,
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
                    _data = _data.Where(x => x.BusinessName.ToLower().Contains(_keywordSearch)
                    || x.Investment.ToString().Contains(_keywordSearch)
                    || x.Produce.ToString().Contains(_keywordSearch)
                    || x.ProductValue.ToString().Contains(_keywordSearch)
                    || x.Quantity.ToString().Contains(_keywordSearch)
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
        public async Task<IActionResult> create(CateInvestmentProjectModel data)
        {
            BaseModels<CateInvestmentProjectModel> model = new BaseModels<CateInvestmentProjectModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                if (string.IsNullOrEmpty(data.BusinessName))
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INDUSTRIAL_CLUSTER_INFO_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }

                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.INDUSTRIAL_CLUSTER_INFO_MANAGEMENT, Action_Status.SUCCESS);
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
        public async Task<IActionResult> Update(CateInvestmentProjectModel data)
        {
            BaseModels<CateInvestmentProjectModel> model = new BaseModels<CateInvestmentProjectModel>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var CheckData = _repo.FindById(data.CateInvestmentProjectId);
                SystemLog datalog = new SystemLog();
                if (CheckData == null)
                {
                    //chỗ này không tồn tại id
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                    };
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_CLUSTER_INFO_MANAGEMENT, Action_Status.FAIL);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                    return BadRequest(model);
                }
                else
                {
                    if (string.IsNullOrEmpty(data.BusinessName))
                    {
                        model.status = 0;
                        model.error = new ErrorModel()
                        {
                            Code = ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY
                        };
                        datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_CLUSTER_INFO_MANAGEMENT, Action_Status.FAIL);
                        _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                        return BadRequest(model);
                    }

                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.INDUSTRIAL_CLUSTER_INFO_MANAGEMENT, Action_Status.SUCCESS);
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
            BaseModels<CateInvestmentProjectModel> model = new BaseModels<CateInvestmentProjectModel>();
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
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.INDUSTRIAL_CLUSTER_INFO_MANAGEMENT, Action_Status.SUCCESS);
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

        private List<CateInvestmentProjectModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<CateInvestmentProjectModel, object> _orderByExpression = x => x.CateInvestmentProjectId; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<CateInvestmentProjectModel, object>> _sortableFields = new Dictionary<string, Func<CateInvestmentProjectModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "Investment", x => x.Investment },
                        { "Produce", x => x.Produce },
                        { "Quantity", x => x.Quantity },
                        { "ProductValue", x => x.ProductValue },
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

            IQueryable<CateInvestmentProjectModel> _data = _repo._context.CateInvestmentProjects.Where(x => !x.IsDel).Select(x => new CateInvestmentProjectModel
            {
                BusinessName = x.BusinessName,
                InvestmentType = x.InvestmentType,
                Investment = x.Investment,
                Owner = x.Owner,
                Career = x.Career,
                PhoneNumber = x.PhoneNumber,
                ProjectArea = x.ProjectArea,
                ProductValue = x.ProductValue,
                Produce = x.Produce,
                Quantity = x.Quantity,
                NumberOfWorker = x.NumberOfWorker,
                Reality = x.Reality,
                District = x.District
            });

            if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
            {
                _data = _data.Where(x => x.District == Guid.Parse(query.Filter["District"]));
            }

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.BusinessName.ToLower().Contains(_keywordSearch)
                || x.Investment.ToString().Contains(_keywordSearch)
                || x.Produce.ToString().Contains(_keywordSearch)
                || x.ProductValue.ToString().Contains(_keywordSearch)
                || x.Quantity.ToString().Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
            }
            // model.items = _data.ToList();
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

                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyDuAnDauTu.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    if (districtName != "")
                    {
                        worksheet.Cell(2, 1).Value = $"THÔNG TIN CÁC DỰ ÁN ĐẦU TƯ VÀO CỤM CÔNG NGHIỆP TRÊN ĐỊA BÀN {districtName.ToUpper()}";
                        worksheet.Cell(3, 1).Value = "Đơn vị báo cáo: Phòng quản lý chuyên môn công thương thuộc Ủy ban nhân dân cấp huyện";

                    }
                    else
                    {
                        worksheet.Cell(2, 1).Value = "THÔNG TIN CÁC DỰ ÁN ĐẦU TƯ VÀO CỤM CÔNG NGHIỆP TRÊN ĐỊA BÀN TỈNH";
                        worksheet.Cell(3, 1).Value = "Đơn vị báo cáo: Sở Công Thương";

                    }
                    int index = 11;
                    int row = 1;
                    foreach (var item in data)
                    {
                        var addrow = worksheet.Row(index - 1);
                        addrow.InsertRowsBelow(1);
                        
                        worksheet.Cell(index, 1).Value = row;
                        worksheet.Cell(index, 2).Value = item.BusinessName;
                        if(item.InvestmentType == 1)
                        {
                            worksheet.Cell(index, 3).Value = item.Investment;
                        }
                        else
                        {
                            worksheet.Cell(index, 4).Value = item.Investment;
                        }
                        worksheet.Cell(index, 5).Value = item.Owner;
                        worksheet.Cell(index, 6).Value = item.Career;
                        worksheet.Cell(index, 7).Value = item.PhoneNumber;
                        worksheet.Cell(index, 8).Value = item.ProjectArea;
                        worksheet.Cell(index, 9).Value = item.Produce;
                        worksheet.Cell(index, 10).Value = item.Quantity;
                        worksheet.Cell(index, 11).Value = item.ProductValue;
                        worksheet.Cell(index, 12).Value = item.NumberOfWorker;
                        worksheet.Cell(index, 13).Value = item.Reality;

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

