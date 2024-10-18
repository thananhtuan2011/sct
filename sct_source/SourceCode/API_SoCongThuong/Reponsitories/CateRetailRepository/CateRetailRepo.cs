using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Bibliography;

namespace API_SoCongThuong.Reponsitories
{
    public class CateRetailRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateRetailRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateRetailModel model)
        {
            CateRetail data = new CateRetail()
            {
                CateRetailCode = model.CateRetailCode,
                CheckUserId = model.CheckUserId,
                ConfirmTime = model.ConfirmTime,
                ConfirmUserId = model.ConfirmUserId,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                ReportMonth = model.ReportMonth
            };
            await _context.CateRetails.AddAsync(data);
            await _context.SaveChangesAsync();
            if (model.Details != null && model.Details.Count > 0)
            {
                List<CateRetailDetail> LstDetail = new List<CateRetailDetail>();
                foreach (var item in model.Details)
                {
                    CateRetailDetail detail = new CateRetailDetail()
                    {
                        CateRetailId = data.CateRetailId,
                        Criteria = item.Criteria,
                        CumulativeToReportingMonth = item.CumulativeToReportingMonth,
                        EstimateReportingMonth = item.EstimateReportingMonth,
                        PerformLastmonth = item.PerformLastmonth,
                        PerformReporting = item.PerformReporting,
                        Type = item.Type
                    };
                    LstDetail.Add(detail);
                }
                await _context.CateRetailDetails.AddRangeAsync(LstDetail);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Update(CateRetailModel model)
        {
            var detailinfo = await _context.CateRetails.Where(d => d.CateRetailId == model.CateRetailId).FirstOrDefaultAsync();
            detailinfo.CateRetailCode = model.CateRetailCode;
            detailinfo.CheckUserId = model.CheckUserId;
            detailinfo.ConfirmTime = model.ConfirmTime;
            detailinfo.ConfirmUserId = model.ConfirmUserId;
            detailinfo.CreateUserId = model.CreateUserId;
            detailinfo.CreateTime = model.CreateTime;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            detailinfo.ReportMonth = model.ReportMonth;
            if (model.Details != null && model.Details.Count > 0)
            {
                List<CateRetailDetail> LstDetail_Insert = new List<CateRetailDetail>();
                List<CateRetailDetail> LstDetail_Del = new List<CateRetailDetail>();
                foreach (var item in model.Details)
                {
                    if (!item.IsDel && item.CateRetailDetailId.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        CateRetailDetail detail = new CateRetailDetail()
                        {
                            CateRetailId = model.CateRetailId,
                            Criteria = item.Criteria,
                            CumulativeToReportingMonth = item.CumulativeToReportingMonth,
                            EstimateReportingMonth = item.EstimateReportingMonth,
                            PerformLastmonth = item.PerformLastmonth,
                            PerformReporting = item.PerformReporting,
                            Type = item.Type
                        };
                        LstDetail_Insert.Add(detail);
                    }
                    else if (item.IsDel)
                    {
                        CateRetailDetail detail = new CateRetailDetail()
                        {
                            CateRetailId = model.CateRetailId,
                            CateRetailDetailId = (Guid)item.CateRetailDetailId,
                            Criteria = item.Criteria,
                            CumulativeToReportingMonth = item.CumulativeToReportingMonth,
                            EstimateReportingMonth = item.EstimateReportingMonth,
                            PerformLastmonth = item.PerformLastmonth,
                            PerformReporting = item.PerformReporting,
                            Type = item.Type
                        };
                        LstDetail_Del.Add(detail);
                    }

                }
                await _context.CateRetailDetails.AddRangeAsync(LstDetail_Insert);
                _context.CateRetailDetails.RemoveRange(LstDetail_Del);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid CateRetailId)
        {
            var detailinfo = await _context.CateRetails.Where(d => d.CateRetailId == CateRetailId).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            var del_detail = _context.CateRetailDetails.Where(d => d.CateRetailId == CateRetailId)
                .Select((dt) => new CateRetailDetail
                {
                    Criteria = dt.Criteria,
                    CateRetailDetailId = dt.CateRetailDetailId,
                    CateRetailId = dt.CateRetailDetailId,
                    CumulativeToReportingMonth = dt.CumulativeToReportingMonth,
                    EstimateReportingMonth = dt.EstimateReportingMonth,
                    PerformLastmonth = dt.PerformLastmonth,
                    PerformReporting = dt.PerformReporting,
                    Type = dt.Type
                }).ToList();
            _context.CateRetailDetails.RemoveRange(del_detail);
            await _context.SaveChangesAsync();
        }
        public async Task Deletes(List<Guid> CateRetailIds)
        {
            List<CateRetail> items = new List<CateRetail>();
            foreach (var idremove in CateRetailIds)
            {
                CateRetail item = new CateRetail();
                var detailinfo = await _context.CateRetails.Where(d => d.CateRetailId == idremove).FirstOrDefaultAsync();
                item.CateRetailId = idremove;
                item.IsDel = true;
                items.Add(item);
                var del_detail = _context.CateRetailDetails.Where(d => d.CateRetailId == idremove)
                .Select((dt) => new CateRetailDetail
                {
                    Criteria = dt.Criteria,
                    CateRetailDetailId = dt.CateRetailDetailId,
                    CateRetailId = dt.CateRetailDetailId,
                    CumulativeToReportingMonth = dt.CumulativeToReportingMonth,
                    EstimateReportingMonth = dt.EstimateReportingMonth,
                    PerformLastmonth = dt.PerformLastmonth,
                    PerformReporting = dt.PerformReporting,
                    Type = dt.Type
                }).ToList();
                _context.CateRetailDetails.RemoveRange(del_detail);
            }
            _context.CateRetails.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
        public CateRetail FindById(Guid CateRetailId)
        {
            var result = _context.CateRetails.Where(x => x.CateRetailId == CateRetailId && !x.IsDel).Select(d => new CateRetail()
            {
                CateRetailId = d.CateRetailId,
                CateRetailCode = d.CateRetailCode,
                CheckUserId = d.CheckUserId,
                ConfirmUserId = d.ConfirmUserId,
                ConfirmTime = d.ConfirmTime,
                CreateTime = d.CreateTime,
                CreateUserId = d.CreateUserId,
                IsAction = d.IsAction,
                ReportMonth = d.ReportMonth
            }).FirstOrDefault();

            return result;
        }
        public List<CateRetailDetailModel> FindDetailById(Guid CateRetailId, Guid OldCateRetailId = default)
        {
            var CurrentData = _context.CateRetailDetails.Where(x => x.CateRetailId == CateRetailId).Join(
                _context.Categories, d => d.Criteria, e => e.CategoryId,
                (d, e) => new CateRetailDetailModel()
                {
                    Criteria = d.Criteria,
                    CateRetailDetailId = d.CateRetailDetailId,
                    CumulativeToReportingMonth = d.CumulativeToReportingMonth,
                    EstimateReportingMonth = d.EstimateReportingMonth,
                    PerformLastmonth = d.PerformLastmonth,
                    PerformReporting = d.PerformReporting,
                    Type = 1,
                    CriteriaName = e.CategoryName,
                    IsDel = false,
                }).ToList();

            if (OldCateRetailId != Guid.Empty)
            {
                var LastYearData = _context.CateRetailDetails.Where(x => x.CateRetailId == OldCateRetailId).Join(
                _context.Categories, d => d.Criteria, e => e.CategoryId,
                (d, e) => new CateRetailDetailModel()
                {
                    Criteria = d.Criteria,
                    CateRetailDetailId = d.CateRetailDetailId,
                    CumulativeToReportingMonth = d.CumulativeToReportingMonth,
                    EstimateReportingMonth = d.EstimateReportingMonth,
                    PerformLastmonth = d.PerformLastmonth,
                    PerformReporting = d.PerformReporting,
                    Type = 2,
                    CriteriaName = e.CategoryName,
                    IsDel = false,
                }).ToList();

                var Result = CurrentData.Concat(LastYearData).ToList();

                return Result;
            }
            else
            {
                return CurrentData;
            }
        }

        #region get danh sách chỉ tiêu
        public List<CateCriterion> FindDetailById()
        {
            var result = _context.CateCriteria.Where(x => x.IsDel == false).Select(d => new CateCriterion()
            {
                CateCriteriaId = d.CateCriteriaId,
                CateCriteriaName = d.CateCriteriaName,
            }).ToList();

            return result;
        }
        #endregion
        #region get danh sách User
        public IQueryable<User> FindUser()
        {
            var result = _context.Users.Where(x => x.IsDel == false).Select(d => new User()
            {
                UserId = d.UserId,
                UserName = d.UserName,
                FullName = d.FullName,
                IsDel = d.IsDel,
            });

            return result;
        }
        #endregion

        #region get danh sách Category
        public IQueryable<Category> FindCriteria()
        {
            var result = _context.Categories.Where(x => x.CategoryTypeCode == "RETAIL").Select(d => new Category()
            {
                CategoryId = d.CategoryId,
                CategoryName = d.CategoryName,
            });

            return result;
        }
        #endregion
    }
}
