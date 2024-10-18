using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using RestSharp.Extensions;

namespace API_SoCongThuong.Reponsitories.BuildAndUpgradeRepository
{
    public class BuildAndUpgradeRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public BuildAndUpgradeRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(BuildAndUpgradeMarket model)
        {
            await _context.BuildAndUpgradeMarkets.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(BuildAndUpgradeMarket model)
        {
            _context.BuildAndUpgradeMarkets.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteBuildAndUpgrade(BuildAndUpgradeMarket model)
        {
            var db = await _context.BuildAndUpgradeMarkets.Where(d => d.BuildAndUpgradeId == model.BuildAndUpgradeId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.BuildAndUpgradeMarkets.Where(x => x.BuildAndUpgradeId == id).FirstOrDefaultAsync();
            _context.BuildAndUpgradeMarkets.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<BuildAndUpgradeMarket> FindAll()
        {
            var result = _context.BuildAndUpgradeMarkets.Select(d => new BuildAndUpgradeMarket()
            {
                BuildAndUpgradeId = d.BuildAndUpgradeId,
                BuildAndUpgradeName = d.BuildAndUpgradeName,
                Address = d.Address,
                TotalInvestment = d.TotalInvestment,
                TotalInvestmentUnit = d.TotalInvestmentUnit != null ? d.TotalInvestmentUnit : "null",
                RealizedCapital = d.RealizedCapital,
                RealizedCapitalUnit = d.RealizedCapitalUnit != null ? d.RealizedCapitalUnit : "null",
                BudgetCapital = d.BudgetCapital,
                BudgetCapitalUnit = d.BudgetCapitalUnit != null ? d.BudgetCapitalUnit : "null",
                LandUseCapital = d.LandUseCapital,
                LandUseCapitalUnit = d.LandUseCapitalUnit != null ? d.LandUseCapitalUnit : "null",
                Loans = d.Loans,
                LoansUnit = d.LoansUnit != null ? d.LoansUnit : "null",
                AnotherCapital = d.AnotherCapital,
                AnotherCapitalUnit = d.AnotherCapitalUnit != null ? d.AnotherCapitalUnit : "null",
                IsBuild = d.IsBuild,
                IsUpgrade = d.IsUpgrade,
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<BuildAndUpgradeMarket> FindById(Guid Id)
        {
            var result = _context.BuildAndUpgradeMarkets.Where(x => x.BuildAndUpgradeId == Id && !x.IsDel).Select(d => new BuildAndUpgradeMarket()
            {
                BuildAndUpgradeId = d.BuildAndUpgradeId,
                BuildAndUpgradeName = d.BuildAndUpgradeName,
                CommercialId = d.CommercialId,
                Address = d.Address,
                TotalInvestment = d.TotalInvestment,
                TotalInvestmentUnit = d.TotalInvestmentUnit != null ? d.TotalInvestmentUnit : "null",
                RealizedCapital = d.RealizedCapital,
                RealizedCapitalUnit = d.RealizedCapitalUnit != null ? d.RealizedCapitalUnit : "null",
                BudgetCapital = d.BudgetCapital,
                BudgetCapitalUnit = d.BudgetCapitalUnit != null ? d.BudgetCapitalUnit : "null",
                LandUseCapital = d.LandUseCapital,
                LandUseCapitalUnit = d.LandUseCapitalUnit != null ? d.LandUseCapitalUnit : "null",
                Loans = d.Loans,
                LoansUnit = d.LoansUnit != null ? d.LoansUnit : "null",
                AnotherCapital = d.AnotherCapital,
                AnotherCapitalUnit = d.AnotherCapitalUnit != null ? d.AnotherCapitalUnit : "null",
                IsBuild = d.IsBuild,
                IsUpgrade = d.IsUpgrade,
                Note = d.Note,
                IsDel = d.IsDel,
                Year = d.Year
            });

            return result;
        }

        public List<BuildAndUpgradeModel> FindData(QueryRequestBody query)
        {
            List<BuildAndUpgradeModel> result = new List<BuildAndUpgradeModel>();
            string _keywordSearch = "";
            IQueryable<BuildAndUpgradeModel> _data = from b in _context.BuildAndUpgradeMarkets
                                                     where !b.IsDel
                                                     join cm in _context.CommercialManagements on b.CommercialId equals cm.CommercialId into cmGroup
                                                     from cmm in cmGroup.DefaultIfEmpty()
                                                     join d in _context.Districts on cmm.DistrictId equals d.DistrictId into dGroup
                                                     from di in dGroup.DefaultIfEmpty()
                                                     join commune in _context.Communes on cmm.CommuneId equals commune.CommuneId into communeGroup
                                                     from res2 in communeGroup.DefaultIfEmpty()
                                                     select new BuildAndUpgradeModel
                                                     {
                                                         BuildAndUpgradeId = b.BuildAndUpgradeId,
                                                         BuildAndUpgradeName = b.BuildAndUpgradeName,
                                                         Name = cmm.Name,
                                                         DistrictId = di.DistrictId,
                                                         DistrictsName = di.DistrictName,
                                                         CommuneName = res2.CommuneName,
                                                         Address = b.Address,
                                                         TotalInvestment = b.TotalInvestment,
                                                         TotalInvestmentUnit = b.TotalInvestmentUnit,
                                                         RealizedCapital = b.RealizedCapital,
                                                         RealizedCapitalUnit = b.RealizedCapitalUnit,
                                                         BudgetCapital = b.BudgetCapital,
                                                         BudgetCapitalUnit = b.BudgetCapitalUnit,
                                                         LandUseCapital = b.LandUseCapital,
                                                         LandUseCapitalUnit = b.LandUseCapitalUnit,
                                                         Loans = b.Loans,
                                                         LoansUnit = b.LoansUnit,
                                                         AnotherCapital = b.AnotherCapital,
                                                         AnotherCapitalUnit = b.AnotherCapitalUnit,
                                                         IsBuild = b.IsBuild,
                                                         IsUpgrade = b.IsUpgrade,
                                                         Note = b.Note,
                                                         IsDel = b.IsDel,
                                                         Year = b.Year,
                                                     };

            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.Name.ToLower().Contains(_keywordSearch)
                   || x.TotalInvestment.ToString().Contains(_keywordSearch) || x.RealizedCapital.ToString().Contains(_keywordSearch)
                   || x.BudgetCapital.ToString().Contains(_keywordSearch) || x.LandUseCapital.ToString().Contains(_keywordSearch)
                   || x.Loans.ToString().Contains(_keywordSearch) || x.AnotherCapital.ToString().Contains(_keywordSearch)
               );
            }

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                _data = _data.Where(x => x.Year.ToString() == query.Filter["Year"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
            {
                _data = _data.Where(x => x.DistrictId == Guid.Parse(query.Filter["District"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
            {
                if (query.Filter["Type"] == "isBuild")
                {
                    _data = _data.Where(x => x.IsBuild == true);
                }
                else
                {
                    _data = _data.Where(x => x.IsBuild == false);
                }
            }

            return result = _data.OrderBy(x => x.Year).ToList();
        }

        public String GetStringValue (decimal? value, string? unit)
        {
            String result = "";
            if (value.HasValue && unit.HasValue())
            {
                if (unit == "TY")
                {
                    result = (value / 1_000_000_000).ToString() + "tỷ";
                } 
                else if (unit == "TRIEU")
                {
                    result = (value / 1_000_000).ToString() + "triệu";
                }
            }
            return result;
        }

        public string GetStringValueUnitBillion(decimal? value)
        {
            if (value != null)
            {
                String result = "";
                result = (value / 1_000_000_000).ToString();
                return result;
            }
            return "";
        }
    }
}
