using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.StatisticalMultiLevelSalesRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalMultiLevelSalesController : ControllerBase
    {
        private StatisticalMultiLevelSalesRepo _repo;

        public StatisticalMultiLevelSalesController(SoHoa_SoCongThuongContext context)
        {
            _repo = new StatisticalMultiLevelSalesRepo(context);
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems([FromBody] QueryRequestBody query)//query truyền lên
        {
            BaseModels<StatisticalMultiLevelSalesModel> model = new BaseModels<StatisticalMultiLevelSalesModel>();
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

                Func<StatisticalMultiLevelSalesModel, object> _orderByExpression = x => x.BusinessName; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<StatisticalMultiLevelSalesModel, object>> _sortableFields = new Dictionary<string, Func<StatisticalMultiLevelSalesModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "BusinessName", x => x.BusinessName },
                        { "YearReport", x => x.YearReport },
                        { "DistrictName", x => x.DistrictName },
                        { "Revenue", x => x.Revenue },
                        { "Scale", x => x.Scale },
                        { "Representative", x => x.Representative },
                        { "PhoneNumber", x => x.PhoneNumber },
                    };

                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }

                IQueryable<StatisticalMultiLevelSalesModel> _data = from m in _repo._context.MultiLevelSalesManagements.Where(x => !x.IsDel)
                                                                    join bm in _repo._context.BusinessMultiLevels.Where(x => !x.IsDel) on m.BusinessId equals bm.BusinessMultiLevelId into mbm
                                                                    from bm in mbm.DefaultIfEmpty()
                                                                    join b in _repo._context.Businesses.Where(x => !x.IsDel) on bm.BusinessId equals b.BusinessId into bu
                                                                    from bus in bu.DefaultIfEmpty()
                                                                    join d in _repo._context.Districts.Where(x => !x.IsDel) on bus.DistrictId equals d.DistrictId into res
                                                                    from r in res.DefaultIfEmpty()
                                                                    select new StatisticalMultiLevelSalesModel
                                                                    {
                                                                        BusinessMultiLevelId = m.MultiLevelSalesManagementId,
                                                                        BusinessId = bus.BusinessId.ToString(),
                                                                        BusinessName = bus.BusinessNameVi,
                                                                        YearReport = m.YearReport,
                                                                        DistrictId = r.DistrictId.ToString(),
                                                                        DistrictName = r.DistrictName,
                                                                        Revenue = m.Turnover,
                                                                        Scale = m.Participants + m.NewParticipants,
                                                                        Representative = m.ContactPersonName,
                                                                        PhoneNumber = m.ContactPersonPhoneNumber,
                                                                    };

                //Search
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                        x.BusinessName.ToLower().Contains(_keywordSearch) ||
                        x.YearReport.ToString().Contains(_keywordSearch) ||
                        x.DistrictName.ToLower().Contains(_keywordSearch) ||
                        x.Revenue.ToString().Contains(_keywordSearch) ||
                        x.Scale.ToString().Contains(_keywordSearch) ||
                        x.Representative.ToLower().Contains(_keywordSearch) ||
                        x.PhoneNumber.Contains(_keywordSearch)
                   );
                }

                //Filter
                if (query.Filter != null && query.Filter.ContainsKey("BusinessId")
                    && !string.IsNullOrEmpty(query.Filter["BusinessId"]))
                {
                    _data = _data.Where(x => x.BusinessId == query.Filter["BusinessId"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("YearReport")
                    && !string.IsNullOrEmpty(query.Filter["YearReport"]))
                {
                    _data = _data.Where(x => x.YearReport.ToString() == query.Filter["YearReport"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("DistrictId")
                    && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
                {
                    _data = _data.Where(x => x.DistrictId == query.Filter["DistrictId"]);
                }

                //Check Count
                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }

                var total = new TotalStatisticalMultiLevelSalesModel()
                {
                    totalRevenue = _data.Sum(x => x.Revenue),
                    totalScale = _data.Sum(x => x.Scale),
                };


                //Phân trang
                if (query.Panigator.More) //query more = true
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

                //model.data = total;
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
    }
}
