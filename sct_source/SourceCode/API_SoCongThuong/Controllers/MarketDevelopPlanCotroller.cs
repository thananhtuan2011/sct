using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketDevelopPlanController : ControllerBase
    {
        private MarketDevelopPlanRepo _repo;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public MarketDevelopPlanController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new MarketDevelopPlanRepo(context);

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

            BaseModels<MarketDevelopPlanModel> model = new BaseModels<MarketDevelopPlanModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = false;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<MarketDevelopPlanModel, object> _orderByExpression = x => x.MarketName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<MarketDevelopPlanModel, object>> _sortableFields = new Dictionary<string, Func<MarketDevelopPlanModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "MarketName", x => x.MarketName },
                        { "DistrictName", x => x.DistrictName },
                        { "CommuneName", x => x.CommuneName },
                        { "RankName", x => x.RankName},
                        { "TypeOfPlanMarketName", x => x.TypeOfPlanMarketName },
                        { "StageName", x => x.StageName },
                        { "Note", x => x.Note },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity

                var _data = from c in _repo._context.MarketDevelopPlans
                            join d in _repo._context.Districts on c.DistrictId equals d.DistrictId
                            join com in _repo._context.Communes on c.CommuneId equals com.CommuneId
                            join stage in _repo._context.Stages on c.Stage equals stage.StageId
                            join cate in _repo._context.Categories on c.RankId equals cate.CategoryId into g
                            from cate in g.DefaultIfEmpty()
                            join cate1 in _repo._context.Categories on c.TypeOfPlanMarket equals cate1.CategoryId
                            where !c.IsDel
                            select new MarketDevelopPlanModel
                            {
                                MarketDevelopPlanId = c.MarketDevelopPlanId,
                                MarketName = c.MarketName,
                                DistrictName = d.DistrictName,
                                DistrictId = c.DistrictId,
                                CommuneId = c.CommuneId,
                                CommuneName = com.CommuneName,
                                StageName = stage.StageName,
                                RankName = cate.CategoryName ?? "",
                                Capital = c.Capital,
                                TypeOfPlanMarketName = cate1.CategoryName,
                                TypeOfPlanMarket = c.TypeOfPlanMarket,
                                Stage = c.Stage,
                                Note = c.Note
                            };

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.MarketName.ToLower().Contains(_keywordSearch)
                       || x.DistrictName.ToLower().Contains(_keywordSearch)
                       || x.CommuneName.ToLower().Contains(_keywordSearch)
                       || x.TypeOfPlanMarketName.ToLower().Contains(_keywordSearch)
                       || x.Note.ToLower().Contains(_keywordSearch)

                   );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
                {
                    _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["District"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Commune") && !string.IsNullOrEmpty(query.Filter["Commune"]))
                {
                    _data = _data.Where(x => x.CommuneId == Guid.Parse(query.Filter["Commune"]));
                }

                if (query.Filter != null && query.Filter.ContainsKey("Stage") && !string.IsNullOrEmpty(query.Filter["Stage"]))
                {
                    _data = _data.Where(x => x.Stage.ToString() == query.Filter["Stage"]);
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

        [HttpGet("{id}")]
        public IActionResult getItemById(Guid id)
        {
            BaseModels<MarketDevelopPlan> model = new BaseModels<MarketDevelopPlan>();
            try
            {
                var result = _repo.FindById(id);
                if (result == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(model);
                }
                else
                {
                    model.status = 1;
                    model.items = result.ToList();
                    return Ok(model);
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
        public async Task<IActionResult> Update(MarketDevelopPlan data)
        {
            BaseModels<MarketDevelopPlan> model = new BaseModels<MarketDevelopPlan>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MarketDevelopPlan? SaveData = _repo._context.MarketDevelopPlans.Where(x => x.MarketDevelopPlanId == data.MarketDevelopPlanId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.MarketName = data.MarketName;
                    SaveData.RankId = data.RankId;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.Address = data.Address;
                    SaveData.Stage = data.Stage;
                    SaveData.TypeOfPlanMarket = data.TypeOfPlanMarket;
                    SaveData.ExistLandArea = data.ExistLandArea;
                    SaveData.NewLandArea = data.NewLandArea;
                    SaveData.AddLandArea = data.AddLandArea;
                    SaveData.Capital = data.Capital;
                    SaveData.Note = data.Note;
                    SaveData.UpdateTime = DateTime.Now;
                    SaveData.UpdateUserId = loginData.Userid;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, "Quản lý thông tin chợ quy hoạch phát triển chợ", Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, "Quản lý thông tin chợ quy hoạch phát triển chợ", Action_Status.FAIL);
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

        [HttpPost()]
        public async Task<IActionResult> create(MarketDevelopPlan data)
        {
            BaseModels<MarketDevelopPlan> model = new BaseModels<MarketDevelopPlan>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MarketDevelopPlan SaveData = new MarketDevelopPlan();
                SaveData.MarketName = data.MarketName;
                SaveData.RankId = data.RankId;
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.Address = data.Address;
                SaveData.Stage = data.Stage;
                SaveData.TypeOfPlanMarket = data.TypeOfPlanMarket;
                SaveData.ExistLandArea = data.ExistLandArea;
                SaveData.NewLandArea = data.NewLandArea;
                SaveData.AddLandArea = data.AddLandArea;
                SaveData.Capital = data.Capital;
                SaveData.Note = data.Note;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = new DateTime();

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, "Quản lý thông tin chợ quy hoạch phát triển chợ", Action_Status.SUCCESS);
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
            BaseModels<MarketDevelopPlan> model = new BaseModels<MarketDevelopPlan>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                MarketDevelopPlan DeleteData = new MarketDevelopPlan();
                DeleteData.MarketDevelopPlanId = id;
                DeleteData.IsDel = true;
                await _repo.Delete(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, "Quản lý thông tin chợ quy hoạch phát triển chợ", Action_Status.SUCCESS);
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

        private List<MarketDevelopPlanModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            string _keywordSearch = "";
            //Cách 1 dùng entity

            var _data = from c in _repo._context.MarketDevelopPlans
                        join d in _repo._context.Districts on c.DistrictId equals d.DistrictId
                        join com in _repo._context.Communes on c.CommuneId equals com.CommuneId
                        join stage in _repo._context.Stages on c.Stage equals stage.StageId
                        join cate in _repo._context.Categories on c.RankId equals cate.CategoryId into g
                        from cate in g.DefaultIfEmpty()
                        join cate1 in _repo._context.Categories on c.TypeOfPlanMarket equals cate1.CategoryId
                        where !c.IsDel
                        select new MarketDevelopPlanModel
                        {
                            MarketDevelopPlanId = c.MarketDevelopPlanId,
                            MarketName = c.MarketName,
                            DistrictName = d.DistrictName,
                            DistrictId = c.DistrictId,
                            CommuneId = c.CommuneId,
                            CommuneName = com.CommuneName,
                            StageName = stage.StageName,
                            RankName = cate.CategoryName ?? "",
                            Capital = c.Capital,
                            TypeOfPlanMarketName = cate1.CategoryName,
                            TypeOfPlanMarket = c.TypeOfPlanMarket,
                            Stage = c.Stage,
                            Note = c.Note,
                            ExistLandArea = c.ExistLandArea,
                            NewLandArea = c.NewLandArea,
                            AddLandArea = c.AddLandArea,
                            TypeOfPlanMarketCode = cate1.CategoryCode
                        };

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   x.MarketName.ToLower().Contains(_keywordSearch)
                   || x.DistrictName.ToLower().Contains(_keywordSearch)
                   || x.CommuneName.ToLower().Contains(_keywordSearch)
                   || x.TypeOfPlanMarketName.ToLower().Contains(_keywordSearch)
                   || x.Note.ToLower().Contains(_keywordSearch)

               );  //Lấy table đã select tìm kiếm theo keyword
            }
            if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
            {
                _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["District"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("Commune") && !string.IsNullOrEmpty(query.Filter["Commune"]))
            {
                _data = _data.Where(x => x.CommuneId == Guid.Parse(query.Filter["Commune"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("Stage") && !string.IsNullOrEmpty(query.Filter["Stage"]))
            {
                _data = _data.Where(x => x.Stage.ToString() == query.Filter["Stage"]);
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuyHoachPhatTrienCho.xlsx"))
                {
                    var listStage  = (_repo._context.Stages
                    .Where(x => !x.IsDel
                        && ((x.StartYear % 10 == 1 && x.EndYear % 10 == 5)
                        || (x.StartYear % 10 == 6 && x.EndYear % 10 == 0)))
                    .Select(x => new Stage
                    {
                        StageId = x.StageId,
                        StageName = x.StageName,
                        StartYear = x.StartYear,
                        EndYear = x.EndYear,
                    })
                    .OrderBy(x => x.StartYear)
                    .ThenBy(x => x.EndYear)).ToList();
                    var _data = (from d in data
                                 group d by d.Stage into g
                                 select new
                                 {
                                     StageId = g.Key,
                                     StageName = g.Select(x => x.StageName).FirstOrDefault(),
                                     Value = g.OrderBy(x => x.StageName).Select(x => x).ToList()
                                 }).ToList();
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 5;
                    int row = 1;
                    foreach (var stage in listStage)
                    {
                        foreach (var _item in _data)
                        {
                            if (stage.StageId == _item.StageId)
                            {
                                var addrow = worksheet.Row(index);
                                addrow.InsertRowsBelow(1);
                                worksheet.Cell(index, 2).Value = $"Giai đoạn {stage.StageName}";
                                worksheet.Cell(index, 2).Style.Font.FontColor = XLColor.Red;

                                int stt = 1;
                                index++;
                                row++;

                                foreach (var item in _item.Value)
                                {
                                    var addrow1 = worksheet.Row(index);
                                    addrow1.InsertRowsBelow(1);
                                    worksheet.Cell(index, 1).Value = stt;

                                    worksheet.Cell(index, 2).Value = item.MarketName;
                                    worksheet.Cell(index, 3).Value = item.RankName;
                                    worksheet.Cell(index, 4).Value = item.ExistLandArea;
                                    worksheet.Cell(index, 5).Value = item.NewLandArea;
                                    worksheet.Cell(index, 6).Value = item.AddLandArea;
                                    worksheet.Cell(index, 7).Value = item.Capital;

                                    switch (item.TypeOfPlanMarketCode)
                                    {
                                        case "XMTNC":
                                            worksheet.Cell(index, 8).Value = "x";
                                            break;
                                        case "NC":
                                            worksheet.Cell(index, 9).Value = "x";
                                            break;
                                        case "DDXM":
                                            worksheet.Cell(index, 10).Value = "x";
                                            break;
                                        case "GT":
                                            worksheet.Cell(index, 11).Value = "x";
                                            break;
                                        case "XM":
                                            worksheet.Cell(index, 12).Value = "x";
                                            break;
                                    };
                                    stt++;
                                    index++;
                                    row++;
                                }
                               
                               
                            }
                        }

                    }

                    worksheet.Row(index).Delete();



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