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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipateTradePromotionController : ControllerBase
    {
        private ParticipateTradePromotionRepository _repo;
        private IConfiguration _config;
        //private BusinessRepo _repoBusi;

        public ParticipateTradePromotionController(SoHoa_SoCongThuongContext context, IConfiguration configuration)
        {
            _repo = new ParticipateTradePromotionRepository(context);
            _config = configuration;

        }
        // Lấy danh sách 
        #region 
        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<ParticipateTradePromotionModel> model = new BaseModels<ParticipateTradePromotionModel>();
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

                Func<ParticipateTradePromotionModel, object> _orderByExpression = x => x.ParticipateSupportFairDetailId; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<ParticipateTradePromotionModel, object>> _sortableFields = new Dictionary<string, Func<ParticipateTradePromotionModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "CountryName", x => x.CountryName },
                        { "BusinessNameVi", x => x.BusinessNameVi },
                        { "TypeOfProfessionName", x => x.TypeOfProfessionName },
                     };
                if (query.Sort != null
                    && !string.IsNullOrEmpty(query.Sort.ColumnName)
                    && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                //IQueryable<ParticipateTradePromotionModel> _data = _repo._context.ParticipateSupportFairs.Where(x => !x.IsDel).GroupJoin(
                //    _repo._context.ParticipateSupportFairDetails.Where(x => (x.BusinessId.ToString() != "00000000-0000-0000-0000-000000000000")),
                //    cc => cc.ParticipateSupportFairId,
                //    u => u.ParticipateSupportFairId,
                //     (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us) => new { info, us }).GroupJoin(
                //    _repo._context.Businesses,
                //    query => query.us.BusinessId
                //    , buss => buss.BusinessId,
                //    (query, buss) => new { query, buss }).SelectMany(rs => rs.buss.DefaultIfEmpty(), (info1, bussi) => new { info1, bussi }).GroupJoin(
                //    _repo._context.Countries,
                //      query1 => query1.info1.query.info.cc.Country
                //    , country => country.CountryId,
                //     (query1, country) => new { query1, country }).SelectMany(result => result.country.DefaultIfEmpty(), (info2, country) => new { info2, country }).GroupJoin(
                //    _repo._context.TypeOfProfessions,
                //      query2 => query2.info2.query1.bussi.LoaiNganhNghe
                //    , prof => prof.TypeOfProfessionId,
                //     (query2, prof) => new { query2, prof }).SelectMany(result => result.prof.DefaultIfEmpty(), (info3, typeofpro)
                //     => new ParticipateTradePromotionModel
                //     {
                //         ParticipateSupportFairId = info3.query2.info2.query1.info1.query.info.cc.ParticipateSupportFairId,
                //         CountryName = info3.query2.country.CountryName,
                //         BusinessNameVi = info3.query2.info2.query1.bussi.BusinessNameVi,
                //         BusinessId = info3.query2.info2.query1.bussi.BusinessId,
                //         TypeOfProfessionName = typeofpro.TypeOfProfessionName,
                //     }).ToList().AsQueryable();

                IQueryable<ParticipateTradePromotionModel> _data = (from d in _repo._context.ParticipateSupportFairDetails.Where(x => x.BusinessId != Guid.Empty)
                                                                    join p in _repo._context.ParticipateSupportFairs.Where(x => !x.IsDel) on d.ParticipateSupportFairId equals p.ParticipateSupportFairId
                                                                    join b in _repo._context.Businesses.Where(x => !x.IsDel) on d.BusinessId equals b.BusinessId
                                                                    join c in _repo._context.Countries.Where(x => !x.IsDel) on p.Country equals c.CountryId
                                                                    join t in _repo._context.TypeOfProfessions.Where(x => !x.IsDel) on b.LoaiNganhNghe equals t.TypeOfProfessionId
                                                                    select new ParticipateTradePromotionModel
                                                                    {
                                                                        
                                                                        CountryName = c.CountryName,
                                                                        BusinessNameVi = b.BusinessNameVi,
                                                                        BusinessCode = b.BusinessCode,
                                                                        BusinessId = b.BusinessId,
                                                                        TypeOfProfessionName = t.TypeOfProfessionName,
                                                                    }).ToList().DistinctBy(x => x.BusinessId).AsQueryable();
                IQueryable<ParticipateTradePromotionModel> _dataOutProvice = (from d in _repo._context.ParticipateSupportFairDetails.Where(x => x.BusinessId == Guid.Empty)
                                                                    select new ParticipateTradePromotionModel
                                                                    {
                                                                        CountryName = "",
                                                                        BusinessNameVi = d.BusinessNameVi,
                                                                        BusinessId = d.BusinessId,
                                                                        BusinessCode = d.BusinessCode,
                                                                        TypeOfProfessionName = d.NganhNghe
                                                                    }).ToList().DistinctBy(x => x.BusinessCode).AsQueryable();
                IQueryable<ParticipateTradePromotionModel> _dataIndustrialPromotion = (from d in _repo._context.IndustrialPromotionProjectDetails.Where(x => x.BusinessId != Guid.Empty)
                                                                    join p in _repo._context.IndustrialPromotionProjects.Where(x => !x.IsDel) on d.IndustrialPromotionProjectId equals p.IndustrialPromotionProjectId
                                                                                       join b in _repo._context.Businesses.Where(x => !x.IsDel) on d.BusinessId equals b.BusinessId
                                                                    join t in _repo._context.TypeOfProfessions.Where(x => !x.IsDel) on b.LoaiNganhNghe equals t.TypeOfProfessionId
                                                                    select new ParticipateTradePromotionModel
                                                                    {
                                                                        CountryName = "",
                                                                        BusinessNameVi = b.BusinessNameVi,
                                                                        BusinessCode = b.BusinessCode,
                                                                        BusinessId = b.BusinessId,
                                                                        TypeOfProfessionName = t.TypeOfProfessionName,
                                                                    }).ToList().DistinctBy(x => x.BusinessId).AsQueryable();
                IQueryable<ParticipateTradePromotionModel> _dataIndustrialPromotionOutProvice = (from d in _repo._context.IndustrialPromotionProjectDetails.Where(x => x.BusinessId == Guid.Empty)
                                                                              select new ParticipateTradePromotionModel
                                                                              {
                                                                                  CountryName = "",
                                                                                  BusinessNameVi = d.BusinessNameVi,
                                                                                  BusinessId = d.BusinessId,
                                                                                  BusinessCode = d.BusinessCode,
                                                                                  TypeOfProfessionName = d.NganhNghe
                                                                              }).ToList().DistinctBy(x => x.BusinessCode).AsQueryable();
                var result = Enumerable.Concat(
                    _data.AsEnumerable(),
                    _dataOutProvice.AsEnumerable()
                    );
                result = Enumerable.Concat(result.AsEnumerable(), _dataIndustrialPromotion.AsEnumerable());
                result = Enumerable.Concat(result.AsEnumerable(), _dataIndustrialPromotionOutProvice.AsEnumerable());
                result = result.DistinctBy(x => x.BusinessCode);
                if (query.SearchValue != null && query.SearchValue != "")
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    result = result.Where(x => x.CountryName.ToLower().Contains(_keywordSearch)
                    || x.BusinessNameVi.ToLower().Contains(_keywordSearch)
                    || x.TypeOfProfessionName.ToLower().Contains(_keywordSearch)
                    );  //Lấy table đã select tìm kiếm theo keyword
                }
                if (query.Filter != null && query.Filter.ContainsKey("TypeBusiness") && !string.IsNullOrEmpty(query.Filter["TypeBusiness"]))
                {
                    if(query.Filter["TypeBusiness"] == "1")
                    {
                        result = result.Where(x => x.BusinessId != Guid.Empty);
                    }else if(query.Filter["TypeBusiness"] == "2")
                    {
                        result = result.Where(x => x.BusinessId == Guid.Empty);
                    }
                }
                //  if (query.Filter != null && query.Filter.ContainsKey("MinTime")
                //&& !string.IsNullOrEmpty(query.Filter["MinTime"]))
                //  {
                //      _data = _data.Where(x =>
                //                  (x.TradePromotionOtherTime) >=
                //                  DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
                //  }

                //  if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                //      && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
                //  {
                //      _data = _data.Where(x =>
                //                 x.TradePromotionOtherTime <=
                //                  DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
                //  }
                int _countRows = result.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.items = result.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                {
                    model.items = result
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else //Sắp xếp dữ liệu theo desc
                {
                    model.items = result
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

        [HttpGet("getItemByCode/{id}")]
        public IActionResult getItemById(string id)
        {
            BaseModels<ParticipateTradePromotionModel> model = new BaseModels<ParticipateTradePromotionModel>();
            try
            {
                var info = _repo.FindByCode(id, _config);
                //var info = new ParticipateTradePromotionModel();
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
        #endregion

        private List<ParticipateTradePromotionModel> FindData([FromBody] QueryRequestBody query)//query truyền lên
        {
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            string _keywordSearch = "";
            Func<ParticipateTradePromotionModel, object> _orderByExpression = x => x.ParticipateSupportFairDetailId; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<ParticipateTradePromotionModel, object>> _sortableFields = new Dictionary<string, Func<ParticipateTradePromotionModel, object>>   //Khởi tạo các trường để sắp xếp
                    {
                        { "CountryName", x => x.CountryName },
                        { "BusinessNameVi", x => x.BusinessNameVi },
                        { "TypeOfProfessionName", x => x.TypeOfProfessionName },
                     };
            if (query.Sort != null
                && !string.IsNullOrEmpty(query.Sort.ColumnName)
                && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }


            IQueryable<ParticipateTradePromotionModel> _data = (from d in _repo._context.ParticipateSupportFairDetails.Where(x => x.BusinessId != Guid.Empty)
                                                                join p in _repo._context.ParticipateSupportFairs.Where(x => !x.IsDel) on d.ParticipateSupportFairId equals p.ParticipateSupportFairId
                                                                join b in _repo._context.Businesses.Where(x => !x.IsDel) on d.BusinessId equals b.BusinessId
                                                                join c in _repo._context.Countries.Where(x => !x.IsDel) on p.Country equals c.CountryId
                                                                join t in _repo._context.TypeOfProfessions.Where(x => !x.IsDel) on b.LoaiNganhNghe equals t.TypeOfProfessionId
                                                                select new ParticipateTradePromotionModel
                                                                {

                                                                    BusinessId = b.BusinessId,
                                                                    BusinessNameVi = b.BusinessNameVi,
                                                                    DiaChi = b.DiaChi,
                                                                    DiaChiTruSo = b.DiaChiTruSo,
                                                                    NguoiDaiDien = b.NguoiDaiDien,
                                                                    SoDienThoai = b.SoDienThoai,
                                                                    Email = b.Email,
                                                                    TypeOfProfessionName = t.TypeOfProfessionName,
                                                                    CountryName = c.CountryName,
                                                                    BusinessCode = b.BusinessCode,

                                                                }).ToList().DistinctBy(x => x.BusinessId).AsQueryable();
            IQueryable<ParticipateTradePromotionModel> _dataOutProvice = (from d in _repo._context.ParticipateSupportFairDetails.Where(x => x.BusinessId == Guid.Empty)
                                                                          select new ParticipateTradePromotionModel
                                                                          {
                                                                              CountryName = "",
                                                                              BusinessNameVi = d.BusinessNameVi,
                                                                              DiaChi = d.DiaChi,
                                                                              DiaChiTruSo = d.DiaChi,
                                                                              NguoiDaiDien = d.NguoiDaiDien,
                                                                              SoDienThoai = "",
                                                                              BusinessId = d.BusinessId,
                                                                              BusinessCode = d.BusinessCode,
                                                                              TypeOfProfessionName = d.NganhNghe
                                                                          }).ToList().DistinctBy(x => x.BusinessCode).AsQueryable();
            IQueryable<ParticipateTradePromotionModel> _dataIndustrialPromotion = (from d in _repo._context.IndustrialPromotionProjectDetails.Where(x => x.BusinessId != Guid.Empty)
                                                                                   join p in _repo._context.IndustrialPromotionProjects.Where(x => !x.IsDel) on d.IndustrialPromotionProjectId equals p.IndustrialPromotionProjectId
                                                                                   join b in _repo._context.Businesses.Where(x => !x.IsDel) on d.BusinessId equals b.BusinessId
                                                                                   join t in _repo._context.TypeOfProfessions.Where(x => !x.IsDel) on b.LoaiNganhNghe equals t.TypeOfProfessionId
                                                                                   select new ParticipateTradePromotionModel
                                                                                   {
                                                                                       CountryName = "",
                                                                                       DiaChi = b.DiaChi,
                                                                                       NguoiDaiDien = b.NguoiDaiDien,
                                                                                       SoDienThoai = b.SoDienThoai,
                                                                                       BusinessNameVi = b.BusinessNameVi,
                                                                                       BusinessCode = b.BusinessCode,
                                                                                       BusinessId = b.BusinessId,
                                                                                       TypeOfProfessionName = t.TypeOfProfessionName,
                                                                                   }).ToList().DistinctBy(x => x.BusinessId).AsQueryable();
            IQueryable<ParticipateTradePromotionModel> _dataIndustrialPromotionOutProvice = (from d in _repo._context.IndustrialPromotionProjectDetails.Where(x => x.BusinessId == Guid.Empty)
                                                                                             select new ParticipateTradePromotionModel
                                                                                             {
                                                                                                 CountryName = "",
                                                                                                 DiaChi = d.DiaChi,
                                                                                                 DiaChiTruSo = d.DiaChi,
                                                                                                 NguoiDaiDien = d.NguoiDaiDien,
                                                                                                 BusinessNameVi = d.BusinessNameVi,
                                                                                                 BusinessId = d.BusinessId,
                                                                                                 BusinessCode = d.BusinessCode,
                                                                                                 TypeOfProfessionName = d.NganhNghe
                                                                                             }).ToList().DistinctBy(x => x.BusinessCode).AsQueryable();
            var result = Enumerable.Concat(
                _data.AsEnumerable(),
                _dataOutProvice.AsEnumerable()
                );
            result = Enumerable.Concat(result.AsEnumerable(), _dataIndustrialPromotion.AsEnumerable());
            result = Enumerable.Concat(result.AsEnumerable(), _dataIndustrialPromotionOutProvice.AsEnumerable());
            result = result.DistinctBy(x => x.BusinessCode);
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                result = result.Where(x => x.CountryName.ToLower().Contains(_keywordSearch)
                || x.BusinessNameVi.ToLower().Contains(_keywordSearch)
                || x.TypeOfProfessionName.ToLower().Contains(_keywordSearch)
                );  //Lấy table đã select tìm kiếm theo keyword
            }
            if (query.Filter != null && query.Filter.ContainsKey("TypeBusiness") && !string.IsNullOrEmpty(query.Filter["TypeBusiness"]))
            {
                if (query.Filter["TypeBusiness"] == "1")
                {
                    result = result.Where(x => x.BusinessId != Guid.Empty);
                }
                else if (query.Filter["TypeBusiness"] == "2")
                {
                    result = result.Where(x => x.BusinessId == Guid.Empty);
                }
            }

            List<Guid?> ListId = result.Where(x => x.BusinessId != Guid.Empty).Select(x => x.BusinessId).ToList();

            var Industrials = _repo._context.BusinessIndustries
                              .Where(x => ListId.Contains(x.BusinessId))
                              .GroupJoin(
                                _repo._context.Industries,
                                b => b.IndustryId,
                                i => i.IndustryId,
                                (b, i) => new { b, i })
                              .SelectMany(
                                res => res.i.DefaultIfEmpty(),
                                (b1, i1) => new
                                {
                                    BusinessId = b1.b.BusinessId,
                                    Industrial = i1.IndustryName
                                })
                              .GroupBy(x => x.BusinessId)
                              .Select(x => new
                              {
                                  BusinessId = x.Key,
                                  Industrial = string.Join(", ", x.ToList().Select(x => x.Industrial))
                              })
                              .ToList();

            result = result.GroupJoin(Industrials, d => d.BusinessId, i => i.BusinessId, (d, indust) => new ParticipateTradePromotionModel
            {
                BusinessNameVi = d.BusinessNameVi,
                DiaChi = d.DiaChi,
                DiaChiTruSo = d.DiaChi,
                NguoiDaiDien = d.NguoiDaiDien,
                SoDienThoai = "",
                BusinessId = d.BusinessId,
                BusinessCode = d.BusinessCode,
                TypeOfProfessionName = d.TypeOfProfessionName,
                Email = d.Email,
                CountryName = d.CountryName,
                Industrial = indust.Select(x => x.Industrial).FirstOrDefault(),
            }).ToList();

            return result.ToList();
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
                using (var workbook = new XLWorkbook(@"Upload/Templates/DanhsachdoanhnghiepthamgiachuongtrinhXTTM.xlsx"))
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
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi;
                            worksheet.Cell(index, 3).Value = item.DiaChiTruSo;
                            worksheet.Cell(index, 4).Value = "Người đại diện: " + item.NguoiDaiDien + ", Số điện thoại: " + item.SoDienThoai + ", Email: " + item.Email;
                            worksheet.Cell(index, 5).Value = item.TypeOfProfessionName;
                            worksheet.Cell(index, 6).Value = item.CountryName;
                            worksheet.Cell(index, 7).Value = item.Industrial;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.BusinessNameVi;
                            worksheet.Cell(index, 3).Value = item.DiaChiTruSo;
                            worksheet.Cell(index, 4).Value = "Người đại diện: " + item.NguoiDaiDien + ", Số điện thoại: " + item.SoDienThoai + ", Email: " + item.Email;
                            worksheet.Cell(index, 5).Value = item.TypeOfProfessionName;
                            worksheet.Cell(index, 6).Value = item.CountryName;
                            worksheet.Cell(index, 7).Value = item.Industrial;
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


