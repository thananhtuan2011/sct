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
    public class MarketInvestEnterpriseController : ControllerBase
    {
        private MarketInvestEnterpriseRepo _repo;

        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public MarketInvestEnterpriseController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new MarketInvestEnterpriseRepo(context);

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

            BaseModels<MarketInvestEnterpriseModel> model = new BaseModels<MarketInvestEnterpriseModel>();
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

                Func<MarketInvestEnterpriseModel, object> _orderByExpression = x => x.MarketName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<MarketInvestEnterpriseModel, object>> _sortableFields = new Dictionary<string, Func<MarketInvestEnterpriseModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "MarketName", x => x.MarketName },
                        { "DistrictName", x => x.DistrictName },
                        { "BusinessName", x => x.BusinessName },
                        { "InvestmentName", x => x.InvestmentName},
                        { "Capital", x => x.Capital },
                        { "Evaluate", x => x.Evaluate },
                    };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                //Cách 1 dùng entity

                var _data = from c in _repo._context.MarketInvestEnterprises
                            join d in _repo._context.Districts on c.DistrictId equals d.DistrictId
                            where !c.IsDel
                            select new MarketInvestEnterpriseModel
                            {
                                MarketInvestEnterpriseId = c.MarketInvestEnterpriseId,
                                MarketName = c.MarketName,
                                DistrictName = d.DistrictName,
                                DistrictId = c.DistrictId,
                                CommuneId = c.CommuneId,
                                InvestmentName = c.Investment == true ? "Doanh nghiệp" : "Hợp tác xã",
                                BusinessName = c.BusinessName,
                                Capital = c.Capital,
                                Evaluate = c.Evaluate,
                                Investment = c.Investment
                            };

                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       x.MarketName.ToLower().Contains(_keywordSearch)
                       || x.DistrictName.ToLower().Contains(_keywordSearch)
                       || x.InvestmentName.ToLower().Contains(_keywordSearch)
                       || x.BusinessName.ToLower().Contains(_keywordSearch)
                       || x.Capital.ToString().Contains(_keywordSearch)
                       || x.Evaluate.ToLower().Contains(_keywordSearch)

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

                if (query.Filter != null && query.Filter.ContainsKey("Investment") && !string.IsNullOrEmpty(query.Filter["Investment"]))
                {
                    bool condition = query.Filter["Investment"] == "0" ? false : true;
                    _data = _data.Where(x => x.Investment == condition);
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
            BaseModels<MarketInvestEnterprise> model = new BaseModels<MarketInvestEnterprise>();
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
        public async Task<IActionResult> Update(MarketInvestEnterprise data)
        {
            BaseModels<MarketInvestEnterprise> model = new BaseModels<MarketInvestEnterprise>();
            try
            {

                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MarketInvestEnterprise? SaveData = _repo._context.MarketInvestEnterprises.Where(x => x.MarketInvestEnterpriseId == data.MarketInvestEnterpriseId && !x.IsDel).FirstOrDefault();
                if (SaveData != null)
                {
                    SaveData.MarketName = data.MarketName;
                    SaveData.DistrictId = data.DistrictId;
                    SaveData.CommuneId = data.CommuneId;
                    SaveData.Address = data.Address;
                    SaveData.Investment = data.Investment;
                    SaveData.BusinessName = data.BusinessName;
                    SaveData.Capital = data.Capital;
                    SaveData.Evaluate = data.Evaluate;
                    SaveData.Note = data.Note;
                    SaveData.UpdateTime = DateTime.Now;
                    SaveData.UpdateUserId = loginData.Userid;

                    await _repo.Update(SaveData);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, "Quản lý chợ do doanh nghiệp/ HTX đầu tư xây dựng, khai thác và quản lý", Action_Status.SUCCESS);
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
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, "Quản lý chợ do doanh nghiệp/ HTX đầu tư xây dựng, khai thác và quản lý", Action_Status.FAIL);
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
        public async Task<IActionResult> create(MarketInvestEnterprise data)
        {
            BaseModels<MarketInvestEnterprise> model = new BaseModels<MarketInvestEnterprise>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                MarketInvestEnterprise SaveData = new MarketInvestEnterprise();
                SaveData.MarketName = data.MarketName;
                SaveData.DistrictId = data.DistrictId;
                SaveData.CommuneId = data.CommuneId;
                SaveData.Address = data.Address;
                SaveData.Investment = data.Investment;
                SaveData.BusinessName = data.BusinessName;
                SaveData.Capital = data.Capital;
                SaveData.Evaluate = data.Evaluate;
                SaveData.Note = data.Note;
                SaveData.CreateUserId = loginData.Userid;
                SaveData.CreateTime = new DateTime();

                await _repo.Insert(SaveData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, "Quản lý chợ do doanh nghiệp/ HTX đầu tư xây dựng, khai thác và quản lý", Action_Status.SUCCESS);
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
            BaseModels<MarketInvestEnterprise> model = new BaseModels<MarketInvestEnterprise>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();

                MarketInvestEnterprise DeleteData = new MarketInvestEnterprise();
                DeleteData.MarketInvestEnterpriseId = id;
                DeleteData.IsDel = true;
                await _repo.Delete(DeleteData);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, "Quản lý chợ do doanh nghiệp/ HTX đầu tư xây dựng, khai thác và quản lý", Action_Status.SUCCESS);
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

        private List<MarketInvestEnterpriseModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            string _keywordSearch = "";
            //Cách 1 dùng entity

            var _data = (from c in _repo._context.MarketInvestEnterprises
                         join d in _repo._context.Districts on c.DistrictId equals d.DistrictId
                         join com in _repo._context.Communes on c.CommuneId equals com.CommuneId
                         where !c.IsDel
                         select new MarketInvestEnterpriseModel
                         {
                             MarketInvestEnterpriseId = c.MarketInvestEnterpriseId,
                             MarketName = c.MarketName,
                             DistrictName = d.DistrictName,
                             DistrictId = c.DistrictId,
                             CommuneId = c.CommuneId,
                             InvestmentName = c.Investment == true ? "Doanh nghiệp" : "Hợp tác xã",
                             BusinessName = c.BusinessName,
                             Capital = c.Capital,
                             Evaluate = c.Evaluate,
                             Investment = c.Investment,
                             CommuneName = com.CommuneName
                         });

            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   x.MarketName.ToLower().Contains(_keywordSearch)
                   || x.DistrictName.ToLower().Contains(_keywordSearch)
                   || x.InvestmentName.ToLower().Contains(_keywordSearch)
                   || x.BusinessName.ToLower().Contains(_keywordSearch)
                   || x.Capital.ToString().Contains(_keywordSearch)
                   || x.Evaluate.ToLower().Contains(_keywordSearch)

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

            if (query.Filter != null && query.Filter.ContainsKey("Investment") && !string.IsNullOrEmpty(query.Filter["Investment"]))
            {
                _data = _data.Where(x => x.Investment.ToString() == query.Filter["Investment"]);
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/QuanLyChoDoDNHTXDauTu.xlsx"))
                {
                    var listDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(x => new
                    {
                        DistrictName = x.DistrictName,
                        DistrictId = x.DistrictId
                    }).OrderBy(x => x.DistrictName).ToList();
                    var _data = (from d in data
                                 group d by d.DistrictId into g
                                 select new
                                 {
                                     DistrictId = g.Key,
                                     Value = g.Select(x => x).ToList()
                                 }).ToList();
                    int currentSheet = 0;
                    foreach (var district in listDistrict)
                    {
                        foreach (var _item in _data)
                        {
                            if (district.DistrictId == _item.DistrictId)
                            {
                                IXLWorksheet worksheet = workbook.Worksheets.Worksheet(currentSheet + 1);

                                worksheet.Name = district.DistrictName;
                                currentSheet++;
                                worksheet.CopyTo(workbook, $"{district.DistrictName}{currentSheet}");

                                int index = 5;
                                int row = 1;

                                foreach (var item in _item.Value)
                                {
                                    var addrow = worksheet.Row(index);
                                    addrow.InsertRowsBelow(1);
                                    worksheet.Cell(index, 1).Value = row;
                                    worksheet.Cell(index, 2).Value = item.MarketName;
                                    worksheet.Cell(index, 3).Value = item.CommuneName;
                                    worksheet.Cell(index, 4).Value = item.DistrictName;
                                    worksheet.Cell(index, 5).Value = item.BusinessName;
                                    worksheet.Cell(index, 6).Value = item.InvestmentName;
                                    worksheet.Cell(index, 7).Value = item.Capital;
                                    worksheet.Cell(index, 8).Value = item.Evaluate;
                                    index++;
                                    row++;
                                }

                                worksheet.Row(index).Delete();
                                break;
                            }
                        }

                    }

                    IXLWorksheet worksheetT = workbook.Worksheets.Worksheet(currentSheet + 1);

                    worksheetT.Name = "Tổng hợp";
                    int indexT = 5;
                    int rowT = 1;
                    foreach (var item in data)
                    {


                        var addrow = worksheetT.Row(indexT);
                        addrow.InsertRowsBelow(1);
                        worksheetT.Cell(indexT, 1).Value = rowT;
                        worksheetT.Cell(indexT, 2).Value = item.MarketName;
                        worksheetT.Cell(indexT, 3).Value = item.CommuneName;
                        worksheetT.Cell(indexT, 4).Value = item.DistrictName;
                        worksheetT.Cell(indexT, 5).Value = item.BusinessName;
                        worksheetT.Cell(indexT, 6).Value = item.InvestmentName;
                        worksheetT.Cell(indexT, 7).Value = item.Capital;
                        worksheetT.Cell(indexT, 8).Value = item.Evaluate;
                        indexT++;
                        rowT++;



                    }
                    worksheetT.Row(indexT).Delete();
                    worksheetT.SetTabActive().SetTabSelected();

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