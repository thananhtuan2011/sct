using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Bibliography;

namespace API_SoCongThuong.Reponsitories
{
    public class ConsumerServiceRevenueRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ConsumerServiceRevenueRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ConsumerServiceRevenueModel model)
        {
            ConsumerServiceRevenue data = new ConsumerServiceRevenue()
            {
                ConsumerServiceRevenueCode = model.ConsumerServiceRevenueCode,
                CheckUserId = model.CheckUserId,
                ConfirmTime = model.ConfirmTime,
                ConfirmUserId = model.ConfirmUserId,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
                ReportMonth = model.ReportMonth
            };
            await _context.ConsumerServiceRevenues.AddAsync(data);
            await _context.SaveChangesAsync();
            if (model.Details != null && model.Details.Count > 0)
            {
                List<ConsumerServiceRevenueDetail> LstDetail = new List<ConsumerServiceRevenueDetail>();
                foreach (var item in model.Details)
                {
                    ConsumerServiceRevenueDetail detail = new ConsumerServiceRevenueDetail()
                    {
                        ConsumerServiceRevenueId = data.ConsumerServiceRevenueId,
                        Criteria = item.Criteria,
                        CumulativeToReportingMonth = item.CumulativeToReportingMonth,
                        EstimateReportingMonth = item.EstimateReportingMonth,
                        PerformLastmonth = item.PerformLastmonth,
                        PerformReporting = item.PerformReporting,
                        Type = item.Type
                    };
                    LstDetail.Add(detail);
                }
                await _context.ConsumerServiceRevenueDetails.AddRangeAsync(LstDetail);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Update(ConsumerServiceRevenueModel model)
        {
            var detailinfo = await _context.ConsumerServiceRevenues.Where(d => d.ConsumerServiceRevenueId == model.ConsumerServiceRevenueId).FirstOrDefaultAsync();
            detailinfo.ConsumerServiceRevenueCode = model.ConsumerServiceRevenueCode;
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
                List<ConsumerServiceRevenueDetail> LstDetail_Insert = new List<ConsumerServiceRevenueDetail>();
                List<ConsumerServiceRevenueDetail> LstDetail_Del = new List<ConsumerServiceRevenueDetail>();
                foreach (var item in model.Details)
                {
                    if (!item.IsDel && item.ConsumerServiceRevenueDetailId.ToString() == "00000000-0000-0000-0000-000000000000")
                    {
                        ConsumerServiceRevenueDetail detail = new ConsumerServiceRevenueDetail()
                        {
                            ConsumerServiceRevenueId = model.ConsumerServiceRevenueId,
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
                        ConsumerServiceRevenueDetail detail = new ConsumerServiceRevenueDetail()
                        {
                            ConsumerServiceRevenueId = model.ConsumerServiceRevenueId,
                            ConsumerServiceRevenueDetailId = (Guid)item.ConsumerServiceRevenueDetailId,
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
                await _context.ConsumerServiceRevenueDetails.AddRangeAsync(LstDetail_Insert);
                _context.ConsumerServiceRevenueDetails.RemoveRange(LstDetail_Del);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid ConsumerServiceRevenueId)
        {
            var detailinfo = await _context.ConsumerServiceRevenues.Where(d => d.ConsumerServiceRevenueId == ConsumerServiceRevenueId).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            var del_detail = _context.ConsumerServiceRevenueDetails.Where(d => d.ConsumerServiceRevenueId == ConsumerServiceRevenueId)
                .Select((dt) => new ConsumerServiceRevenueDetail
                {
                    Criteria = dt.Criteria,
                    ConsumerServiceRevenueDetailId = dt.ConsumerServiceRevenueDetailId,
                    ConsumerServiceRevenueId = dt.ConsumerServiceRevenueDetailId,
                    CumulativeToReportingMonth = dt.CumulativeToReportingMonth,
                    EstimateReportingMonth = dt.EstimateReportingMonth,
                    PerformLastmonth = dt.PerformLastmonth,
                    PerformReporting = dt.PerformReporting,
                    Type = dt.Type
                }).ToList();
            _context.ConsumerServiceRevenueDetails.RemoveRange(del_detail);
            await _context.SaveChangesAsync();
        }
        public async Task Deletes(List<Guid> ConsumerServiceRevenueIds)
        {
            List<ConsumerServiceRevenue> items = new List<ConsumerServiceRevenue>();
            foreach (var idremove in ConsumerServiceRevenueIds)
            {
                ConsumerServiceRevenue item = new ConsumerServiceRevenue();
                var detailinfo = await _context.ConsumerServiceRevenues.Where(d => d.ConsumerServiceRevenueId == idremove).FirstOrDefaultAsync();
                item.ConsumerServiceRevenueId = idremove;
                item.IsDel = true;
                items.Add(item);
                var del_detail = _context.ConsumerServiceRevenueDetails.Where(d => d.ConsumerServiceRevenueId == idremove)
                .Select((dt) => new ConsumerServiceRevenueDetail
                {
                    Criteria = dt.Criteria,
                    ConsumerServiceRevenueDetailId = dt.ConsumerServiceRevenueDetailId,
                    ConsumerServiceRevenueId = dt.ConsumerServiceRevenueDetailId,
                    CumulativeToReportingMonth = dt.CumulativeToReportingMonth,
                    EstimateReportingMonth = dt.EstimateReportingMonth,
                    PerformLastmonth = dt.PerformLastmonth,
                    PerformReporting = dt.PerformReporting,
                    Type = dt.Type
                }).ToList();
                _context.ConsumerServiceRevenueDetails.RemoveRange(del_detail);
            }
            _context.ConsumerServiceRevenues.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
        public ConsumerServiceRevenue FindById(Guid ConsumerServiceRevenueId)
        {
            var result = _context.ConsumerServiceRevenues.Where(x => x.ConsumerServiceRevenueId == ConsumerServiceRevenueId && !x.IsDel).Select(d => new ConsumerServiceRevenue()
            {
                ConsumerServiceRevenueId = d.ConsumerServiceRevenueId,
                ConsumerServiceRevenueCode = d.ConsumerServiceRevenueCode,
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
        public List<ConsumerServiceRevenueDetailModel> FindDetailById(Guid ConsumerServiceRevenueId, Guid OldConsumerServiceRevenueId = default)
        {
            var CurrentData = _context.ConsumerServiceRevenueDetails.Where(x => x.ConsumerServiceRevenueId == ConsumerServiceRevenueId).Join(
                _context.Categories, d => d.Criteria, e => e.CategoryId,
                (d, e) => new ConsumerServiceRevenueDetailModel()
                {
                    Criteria = d.Criteria,
                    ConsumerServiceRevenueDetailId = d.ConsumerServiceRevenueDetailId,
                    CumulativeToReportingMonth = d.CumulativeToReportingMonth,
                    EstimateReportingMonth = d.EstimateReportingMonth,
                    PerformLastmonth = d.PerformLastmonth,
                    PerformReporting = d.PerformReporting,
                    Type = 1,
                    CriteriaName = e.CategoryName,
                    IsDel = false,
                }).ToList();

            if (OldConsumerServiceRevenueId != Guid.Empty)
            {
                var LastYearData = _context.ConsumerServiceRevenueDetails.Where(x => x.ConsumerServiceRevenueId == OldConsumerServiceRevenueId).Join(
                _context.Categories, d => d.Criteria, e => e.CategoryId,
                (d, e) => new ConsumerServiceRevenueDetailModel()
                {
                    Criteria = d.Criteria,
                    ConsumerServiceRevenueDetailId = d.ConsumerServiceRevenueDetailId,
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
