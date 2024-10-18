using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ApprovedPowerProjectRepository
{
    public class ApprovedPowerProjectRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ApprovedPowerProjectRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(ApprovedPowerProject model)
        {
            await _context.ApprovedPowerProjects.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ApprovedPowerProject model)
        {
            _context.ApprovedPowerProjects.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteApprovedPowerProject(ApprovedPowerProject model)
        {
            var db = await _context.ApprovedPowerProjects.Where(d => d.ApprovedPowerProjectId == model.ApprovedPowerProjectId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.ApprovedPowerProjects.Where(x => x.ApprovedPowerProjectId == id).FirstOrDefaultAsync();
            _context.ApprovedPowerProjects.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ApprovedPowerProject> FindAll()
        {
            var result = _context.ApprovedPowerProjects.Select(d => new ApprovedPowerProject()
            {
                ApprovedPowerProjectId = d.ApprovedPowerProjectId,
                EnergyIndustryId = d.EnergyIndustryId,
                ProjectName = d.ProjectName,
                InvestorName = d.InvestorName,
                PolicyDecision = d.PolicyDecision,
                Wattage = d.Wattage,
                Turbines = d.Turbines,
                Substation = d.Substation,
                PowerOutput = d.PowerOutput,
                Area = d.Area,
                Year = d.Year,
                Status = d.Status,
                Note = d.Note,
                IsDel = d.IsDel,
            });
            return result;
        }

        public IQueryable<ApprovedPowerProject> FindById(Guid Id)
        {
            var result = _context.ApprovedPowerProjects.Where(x => x.ApprovedPowerProjectId == Id).Select(d => new ApprovedPowerProject()
            {
                ApprovedPowerProjectId = d.ApprovedPowerProjectId,
                EnergyIndustryId = d.EnergyIndustryId,
                ProjectName = d.ProjectName,
                InvestorName = d.InvestorName,
                DistrictId = d.DistrictId,
                Address = d.Address,
                PolicyDecision = d.PolicyDecision,
                Wattage = d.Wattage,
                Turbines = d.Turbines,
                Substation = d.Substation,
                PowerOutput = d.PowerOutput,
                Area = d.Area,
                Year = d.Year,
                Status = d.Status,
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }

        public List<ApprovedPowerProjectModel> FindData(QueryRequestBody query)
        {
            List<ApprovedPowerProjectModel> result = new List<ApprovedPowerProjectModel>();

            IQueryable<ApprovedPowerProjectModel> data = (from app in _context.ApprovedPowerProjects
                                                          where !app.IsDel
                                                          join ei in _context.TypeOfEnergies
                                                          on app.EnergyIndustryId equals ei.TypeOfEnergyId into JoinEi
                                                          from eni in JoinEi.DefaultIfEmpty()
                                                          join c in _context.Categories
                                                          on app.Status equals c.CategoryId
                                                          select new ApprovedPowerProjectModel
                                                          {
                                                              ApprovedPowerProjectId = app.ApprovedPowerProjectId,
                                                              EnergyIndustryId = app.EnergyIndustryId,
                                                              EnergyIndustryName = eni.TypeOfEnergyName,
                                                              ProjectName = app.ProjectName,
                                                              InvestorName = app.InvestorName,
                                                              PolicyDecision = app.PolicyDecision,
                                                              Wattage = app.Wattage,
                                                              Turbines = app.Turbines,
                                                              Substation = app.Substation,
                                                              PowerOutput = app.PowerOutput,
                                                              Area = app.Area,
                                                              Year = app.Year,
                                                              Status = app.Status,
                                                              StatusName = c.CategoryName,
                                                              Note = app.Note,
                                                              IsDel = app.IsDel
                                                          }).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                data = data.Where(x =>
                   x.ProjectName.ToLower().Contains(_keywordSearch)
                   || x.InvestorName.ToLower().Contains(_keywordSearch)
                   || x.PolicyDecision.ToLower().Contains(_keywordSearch)
                   || x.Wattage.ToString().Contains(_keywordSearch)
                   || x.Area.ToString().Contains(_keywordSearch)
               );
            }

            if (query.Filter != null && query.Filter.ContainsKey("EnergyIndustryId") && !string.IsNullOrEmpty(query.Filter["EnergyIndustryId"]))
            {
                data = data.Where(x => x.EnergyIndustryId.ToString().Equals(string.Join("", query.Filter["EnergyIndustryId"])));
            }

            if (query.Filter != null && query.Filter.ContainsKey("StatusId") && !string.IsNullOrEmpty(query.Filter["StatusId"]))
            {
                data = data.Where(x => x.Status.ToString().Equals(string.Join("", query.Filter["StatusId"])));
            }

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                data = data.Where(x => x.Year.ToString().Equals(string.Join("", query.Filter["Year"])));
            }

            result = data.ToList();

            return result;
        }
    }
}
