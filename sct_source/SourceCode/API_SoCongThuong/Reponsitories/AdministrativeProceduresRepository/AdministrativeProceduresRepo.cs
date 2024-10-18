using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace API_SoCongThuong.Reponsitories.AdministrativeProceduresRepository
{
    public class AdministrativeProceduresRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public AdministrativeProceduresRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        /** Trả dữ liệu cho API Root Find*/
        public List<AdministrativeProceduresModel> GetData(QueryRequestBody query)
        {
            /** Query Data */
            IQueryable<AdministrativeProceduresModel> data =
                _context.AdministrativeProcedures
                .Where(x => !x.IsDel)
                .GroupJoin(
                    _context.Categories,
                    ap => ap.AdministrativeProceduresField,
                    c => c.CategoryId,
                    (ap, c) => new { ap, c }
                    )
                .SelectMany(
                    r => r.c.DefaultIfEmpty(),
                    (adp, ct) => new AdministrativeProceduresModel
                    {
                        AdministrativeProceduresId = adp.ap.AdministrativeProceduresId,
                        AdministrativeProceduresField = adp.ap.AdministrativeProceduresField,
                        AdministrativeProceduresFieldName = ct.CategoryName ?? "",
                        AdministrativeProceduresCode = adp.ap.AdministrativeProceduresCode,
                        Status = adp.ap.Status,
                        StatusName = adp.ap.Status == 1 ? "Chưa xử lý" : (adp.ap.Status == 2 ? "Đang xử lý" : "Đã xử lý"),
                        ReceptionForm = adp.ap.ReceptionForm,
                        ReceptionFormName = adp.ap.ReceptionForm == 1 ? "Trực tiếp" : "Trực tuyến",
                        AdministrativeProceduresName = adp.ap.AdministrativeProceduresName,
                        AmountOfRecords = adp.ap.AmountOfRecords,
                        IsDel = adp.ap.IsDel
                    });

            /** Search & Filter */
            var filteredData = GetFilter(query, GetSearch(query, data));

            /** Sort Order data */
            var sortOrder = GetSortOrder(query, filteredData);

            return sortOrder.Where(x => !x.IsDel).ToList();
        }

        /** Search */
        private IQueryable<AdministrativeProceduresModel> GetSearch (QueryRequestBody query, IQueryable<AdministrativeProceduresModel> data)
        {
            if (!string.IsNullOrEmpty(query.SearchValue))
            {
                var keyword = query.SearchValue.Trim().ToLower();
                data = data.Where(x =>
                    x.AdministrativeProceduresFieldName.ToLower().Contains(keyword) ||
                    x.AdministrativeProceduresCode.ToLower().Contains(keyword) ||
                    x.StatusName.ToLower().Contains(keyword) ||
                    x.ReceptionFormName.ToLower().Contains(keyword) ||
                    x.AdministrativeProceduresName.ToLower().Contains(keyword) ||
                    x.AmountOfRecords.ToString().Contains(keyword)
                );
            }

            return data;
        }

        /** Filter */
        private IQueryable<AdministrativeProceduresModel> GetFilter(QueryRequestBody query, IQueryable<AdministrativeProceduresModel> data)
        {
            if (query.Filter != null)
            {
                if (query.Filter.ContainsKey("AdministrativeProceduresField") && !string.IsNullOrEmpty(query.Filter["AdministrativeProceduresField"].ToString()))
                {
                    data = data.Where(x => x.AdministrativeProceduresField.ToString() == query.Filter["AdministrativeProceduresField"]);
                }
                if (query.Filter.ContainsKey("Status") && !string.IsNullOrEmpty(query.Filter["Status"].ToString()))
                {
                    data = data.Where(x => x.Status == int.Parse(query.Filter["Status"]));
                }
                if (query.Filter.ContainsKey("ReceptionForm") && !string.IsNullOrEmpty(query.Filter["ReceptionForm"].ToString()))
                {
                    data = data.Where(x => x.ReceptionForm == int.Parse(query.Filter["ReceptionForm"]));
                }
            }

            return data;
        }

        /** Sắp xếp dữ liệu */
        private IOrderedEnumerable<AdministrativeProceduresModel> GetSortOrder (QueryRequestBody query, IQueryable<AdministrativeProceduresModel> data)
        {
            //Khởi tạo các trường để sắp xếp
            Dictionary<string, Func<AdministrativeProceduresModel, object>> keySelectors = new Dictionary<string, Func<AdministrativeProceduresModel, object>>
            {
                { "AdministrativeProceduresFieldName", x => x.AdministrativeProceduresFieldName },
                { "AdministrativeProceduresCode", x => x.AdministrativeProceduresCode },
                { "AdministrativeProceduresName", x => x.AdministrativeProceduresName },
                { "AmountOfRecords", x => x.AmountOfRecords },
                { "StatusName", x => x.StatusName },
                { "ReceptionFormName", x => x.ReceptionFormName },
            };

            //Trường sắp xếp mặc định
            Func<AdministrativeProceduresModel, object> orderByExpression = x => x.AdministrativeProceduresCode;

            //Mặc định là sắp xếp tăng dần
            bool orderBy_ASC = true;

            if (query.Sort != null && !string.IsNullOrEmpty(query.Sort.ColumnName) && keySelectors.ContainsKey(query.Sort.ColumnName))
            {
                //Sắp xếp asc hoặc desc
                orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);
                
                //Trường cần sắp xếp
                orderByExpression = keySelectors[query.Sort.ColumnName]; 
            }

            IOrderedEnumerable<AdministrativeProceduresModel> sortOrder;
            if (orderBy_ASC)
            {
                sortOrder = data.OrderBy(orderByExpression);
            }
            else
            {
                sortOrder = data.OrderByDescending(orderByExpression);
            }

            return sortOrder;
        }

        /** Phân trang và Build model trả về client */
        public BaseModels<AdministrativeProceduresModel> BuildModel(QueryRequestBody query, List<AdministrativeProceduresModel> data)
        {
            var model = new BaseModels<AdministrativeProceduresModel>
            {
                status = 1,
                total = data.Count
            };

            if (query.Panigator.More)
            {
                model.items = data;
                return model;
            }
            else
            {
                int startIndex = (query.Panigator.PageIndex - 1) * query.Panigator.PageSize;
                int endIndex = startIndex + query.Panigator.PageSize;

                model.items = data.Skip(startIndex).Take(query.Panigator.PageSize).ToList();
                return model;
            }
        }

        /** Build Error Model nếu lỗi */
        public BaseModels<AdminFormalitiesViewModel> BuildErrorModel(Exception ex)
        {
            return new BaseModels<AdminFormalitiesViewModel>
            {
                status = 0,
                error = new ErrorModel
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex?.Message
                }
            };
        }

        public async Task Insert(AdministrativeProceduresModel model)
        {
            AdministrativeProcedure data = new AdministrativeProcedure()
            {
                AdministrativeProceduresId = model.AdministrativeProceduresId,
                AdministrativeProceduresField = model.AdministrativeProceduresField,
                AdministrativeProceduresCode = model.AdministrativeProceduresCode,
                Status = model.Status,
                ReceptionForm = model.ReceptionForm,
                AdministrativeProceduresName = model.AdministrativeProceduresName,
                AmountOfRecords = model.AmountOfRecords,
                DayReception = DateTime.ParseExact(model.DayReception, "dd/MM/yyyy", null).Date,
                SettlementTerm = DateTime.ParseExact(model.SettlementTerm, "dd/MM/yyyy", null).Date,
                FinishDay = model.FinishDay != null ? DateTime.ParseExact(model.FinishDay, "dd/MM/yyyy", null).Date : null,
                CreateTime = model.CreateTime,
                CreateUserId = model.CreateUserId,

            };
            await _context.AdministrativeProcedures.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task Update(AdministrativeProceduresModel model)
        {
            var detailinfo = await _context.AdministrativeProcedures.Where(d => d.AdministrativeProceduresId == model.AdministrativeProceduresId).FirstOrDefaultAsync();
            detailinfo.AdministrativeProceduresField = model.AdministrativeProceduresField;
            detailinfo.AdministrativeProceduresCode = model.AdministrativeProceduresCode;
            detailinfo.Status = model.Status;
            detailinfo.ReceptionForm = model.ReceptionForm;
            detailinfo.AdministrativeProceduresName = model.AdministrativeProceduresName;
            detailinfo.AmountOfRecords = model.AmountOfRecords;
            detailinfo.DayReception = DateTime.ParseExact(model.DayReception, "dd/MM/yyyy", null).Date;
            detailinfo.SettlementTerm = DateTime.ParseExact(model.SettlementTerm, "dd/MM/yyyy", null).Date;
            detailinfo.FinishDay = model.FinishDay != null ? DateTime.ParseExact(model.FinishDay, "dd/MM/yyyy", null).Date : null;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var db = await _context.AdministrativeProcedures.Where(d => d.AdministrativeProceduresId == Id).FirstOrDefaultAsync();
            db.IsDel = true;
            await _context.SaveChangesAsync();
        }

        //public async Task Delete(Guid id)
        //{
        //    var itemRemove = await _context.AdministrativeProcedures.Where(x => x.AdministrativeProceduresId == id).FirstOrDefaultAsync();
        //    _context.AdministrativeProcedures.Remove(itemRemove);
        //    await _context.SaveChangesAsync();
        //}

        public IQueryable<AdministrativeProcedure> FindAll()
        {
            var result = _context.AdministrativeProcedures.Select(d => new AdministrativeProcedure()
            {
                AdministrativeProceduresId = d.AdministrativeProceduresId,
                AdministrativeProceduresCode = d.AdministrativeProceduresCode,
                AdministrativeProceduresName = d.AdministrativeProceduresName,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<AdministrativeProceduresModel> FindById(Guid Id)
        {
            var result = _context.AdministrativeProcedures.Where(x => x.AdministrativeProceduresId == Id).Select(d => new AdministrativeProceduresModel()
            {
                AdministrativeProceduresId = d.AdministrativeProceduresId,
                AdministrativeProceduresField = d.AdministrativeProceduresField,
                AdministrativeProceduresFieldName = _context.Categories.Where(res => res.CategoryId == d.AdministrativeProceduresField).FirstOrDefault().CategoryName ?? "",
                AdministrativeProceduresCode = d.AdministrativeProceduresCode,
                Status = d.Status,
                StatusName = d.Status == 1 ? "Chưa xử lý" : (d.Status == 2 ? "Đang xử lý" : "Đã xử lý"),
                ReceptionForm = d.ReceptionForm,
                ReceptionFormName = d.ReceptionForm == 1 ? "Trực tiếp" : "Trực tuyến",
                AdministrativeProceduresName = d.AdministrativeProceduresName,
                AmountOfRecords = d.AmountOfRecords,
                DayReception = d.DayReception.ToString("dd'/'MM'/'yyyy"),
                SettlementTerm = d.SettlementTerm.ToString("dd'/'MM'/'yyyy"),
                FinishDay = d.FinishDay.HasValue ? d.FinishDay.Value.ToString("dd'/'MM'/'yyyy") : null,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByAdministrativeProceduresCode(string Code, Guid? Id)
        {
            if (Id != null)
            {
                var AdministrativeProceduresCode = _context.AdministrativeProcedures.Where(x => x.AdministrativeProceduresId == Id && x.AdministrativeProceduresCode == Code && !x.IsDel).FirstOrDefault();
                if (AdministrativeProceduresCode != null)
                {
                    return false;
                }
            }
            var isAdministrativeProceduresCode = _context.AdministrativeProcedures.Where(x => x.AdministrativeProceduresCode == Code && !x.IsDel).FirstOrDefault();
            if (isAdministrativeProceduresCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
