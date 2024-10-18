using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.FinancialPlanTargetRepository
{
    public class FinancialPlanTargetRepo
    {
        public SoHoa_SoCongThuongContext _context;

        public FinancialPlanTargetRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(FinancialPlanTarget model)
        {
            await _context.FinancialPlanTargets.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(FinancialPlanTarget model)
        {
            _context.FinancialPlanTargets.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(FinancialPlanTarget model)
        {
            _context.FinancialPlanTargets.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<FinancialPlanTargetModel> FindById(Guid Id)
        {
            var result = _context.FinancialPlanTargets.Where(x => x.FinancialPlanTargetsId == Id && !x.IsDel).Select(x => new FinancialPlanTargetModel()
            {
                FinancialPlanTargetsId = x.FinancialPlanTargetsId,
                Name = x.Name,
                Type = x.Type,
                Unit = x.Unit,
                Date = x.Year.ToString() + "-" + x.Month.ToString("D2"),
                Plan = x.Planning,
                EstimatedMonth = x.EstimatedMonth,
                CumulativeToMonth = x.CumulativeToMonth,
            });

            return result;
        }

        public (List<FinancialPlanTargetModel>? Data, int Month, int Year) FindData(QueryRequestBody query)
        {
            int MaxMonth;
            int MaxYear;

            if (query.Filter != null && query.Filter.ContainsKey("Date") && !string.IsNullOrEmpty(query.Filter["Date"]))
            {
                var date = query.Filter["Date"].Split("-");
                MaxMonth = int.Parse(date[1]);
                MaxYear = int.Parse(date[0]);
            }
            else
            {
                var MaxByMonthYear = _context.FinancialPlanTargets.Where(x => !x.IsDel).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).FirstOrDefault();
                if (MaxByMonthYear != null)
                {
                    MaxMonth = MaxByMonthYear.Month;
                    MaxYear = MaxByMonthYear.Year;
                }
                else
                {
                    return (null, 0, 0);
                }
            }

            int LastMonth = MaxMonth - 1 == 0 ? 12 : MaxMonth - 1;
            int LastYear = MaxMonth - 1 == 0 ? MaxYear - 1 : MaxYear;

            var currentData = _context.FinancialPlanTargets.Where(f => !f.IsDel && f.Month == MaxMonth && f.Year == MaxYear).ToList();
            var lastMonthData = _context.FinancialPlanTargets.Where(f => !f.IsDel && f.Month == LastMonth && f.Year == LastYear).ToList();
            var lastYearData = _context.FinancialPlanTargets.Where(f => !f.IsDel && f.Month == MaxMonth && f.Year == MaxYear - 1).ToList();
            var _data = (from cr in currentData
                         join lm in lastMonthData on cr.Name equals lm.Name into JoinLm
                         from lmd in JoinLm.DefaultIfEmpty()
                         join ly in lastYearData on cr.Name equals ly.Name into JoinMy
                         from lyd in JoinMy.DefaultIfEmpty()
                         select new FinancialPlanTargetModel
                         {
                             FinancialPlanTargetsId = cr.FinancialPlanTargetsId,
                             Type = cr.Type,
                             Name = cr.Name,
                             Year = cr.Year,
                             Unit = cr.Unit,
                             Plan = cr.Planning,
                             Date = MaxYear.ToString() + "-" + MaxMonth.ToString("D2"),
                             ValueSameMonthLastYear = lyd?.EstimatedMonth ?? 0,
                             ValueLastMonth = lmd?.EstimatedMonth ?? 0,
                             EstimatedMonth = cr.EstimatedMonth,
                             CumulativeToMonth = cr.CumulativeToMonth,
                             CumulativeToMonthLastYear = lyd?.CumulativeToMonth ?? 0,
                             CompareLastMonth = lmd?.EstimatedMonth != null && lmd.EstimatedMonth != 0 ? Math.Round(cr.EstimatedMonth / lmd.EstimatedMonth * 100, 2) : 0,
                             ComparedSameMonth = lyd?.EstimatedMonth != null && lyd.EstimatedMonth != 0 ? Math.Round(cr.EstimatedMonth / lyd.EstimatedMonth * 100, 2) : 0,
                             AccumulatedComparedYearPlan = cr.Planning != 0 ? Math.Round(cr.EstimatedMonth / cr.Planning * 100, 2) : 0,
                             AccumulatedComparedPeriod = lyd?.CumulativeToMonth != null && lyd.CumulativeToMonth != 0 ? Math.Round(cr.CumulativeToMonth / lyd.CumulativeToMonth * 100, 2) : 0
                         }).ToList().AsQueryable();

            ////Search Data
            //string _keywordSearch = "";
            //if (query.SearchValue != null && query.SearchValue != "")
            //{
            //    _keywordSearch = query.SearchValue.Trim().ToLower();
            //    _data = _data.Where(x =>
            //        x.Name.ToLower().Contains(_keywordSearch)
            //        || x.Year.ToString().Contains(_keywordSearch)
            //        || x.Unit.ToLower().Contains(_keywordSearch)
            //        || x.CompareLastMonth.ToString().Contains(_keywordSearch)
            //        || x.ComparedSameMonth.ToString().Contains(_keywordSearch)
            //        || x.AccumulatedComparedYearPlan.ToString().Contains(_keywordSearch)
            //        || x.AccumulatedComparedPeriod.ToString().Contains(_keywordSearch)
            //    );
            //}

            ////Filter Data
            //if (query.Filter != null && query.Filter.ContainsKey("Target") && !string.IsNullOrEmpty(query.Filter["Target"]))
            //{
            //    var Target = string.Join("", query.Filter["Target"]);
            //    if (int.Parse(Target) == 4)
            //    {
            //        _data = _data.Where(x => x.Type > 7 && x.Type < 11);
            //    }
            //    else if (int.Parse(Target) == 3)
            //    {
            //        _data = _data.Where(x => x.Type > 2 && x.Type < 8);
            //    }
            //    else if (int.Parse(Target) == 2)
            //    {
            //        _data = _data.Where(x => x.Type == 2);
            //    }
            //    else if (int.Parse(Target) == 1)
            //    {
            //        _data = _data.Where(x => x.Type == 1);
            //    }
            //}

            //if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            //{
            //    _data = _data.Where(x => x.CreateTime >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            //}

            //if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            //{
            //    _data = _data.Where(x => x.CreateTime <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
            //}

            if (_data.Any())
            {
                return (_data.ToList(), MaxMonth, MaxYear);
            }
            else
            {
                return (null, 0, 0);
            }
        }
    }
}
