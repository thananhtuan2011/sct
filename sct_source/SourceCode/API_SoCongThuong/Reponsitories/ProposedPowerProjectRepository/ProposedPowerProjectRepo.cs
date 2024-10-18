using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ProposedPowerProjectRepository
{
    public class ProposedPowerProjectRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ProposedPowerProjectRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ProposedPowerProject model)
        {
            await _context.ProposedPowerProjects.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ProposedPowerProject model)
        {
            _context.ProposedPowerProjects.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProposedPowerProject(ProposedPowerProject model)
        {
            var db = await _context.ProposedPowerProjects.Where(d => d.ProposedPowerProjectId == model.ProposedPowerProjectId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.ProposedPowerProjects.Where(x => x.ProposedPowerProjectId == id).FirstOrDefaultAsync();
            _context.ProposedPowerProjects.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ProposedPowerProject> FindAll()
        {
            var result = _context.ProposedPowerProjects.Select(d => new ProposedPowerProject()
            {
                ProposedPowerProjectId = d.ProposedPowerProjectId,
                ProjectName = d.ProjectName,
                InvestorName = d.InvestorName,
                PolicyDecision = d.PolicyDecision,
                Wattage = d.Wattage,
                Address = d.Address,
                Note = d.Note,
                IsDel = d.IsDel,
            });
            return result;
        }

        public IQueryable<ProposedPowerProjectModel> FindById(Guid Id)
        {
            var result = _context.ProposedPowerProjects.Where(x => x.ProposedPowerProjectId == Id).Select(d => new ProposedPowerProjectModel()
            {
                ProposedPowerProjectId = d.ProposedPowerProjectId,
                EnergyIndustryId = d.EnergyIndustryId,
                ProjectName = d.ProjectName,
                StatusId = d.StatusId,
                InvestorName = d.InvestorName,
                PolicyDecision = d.PolicyDecision,
                Wattage = d.Wattage,
                Address = d.Address,
                Note = d.Note,
                //ProposedDate = d.ProposedDate.ToString("dd'/'MM'/'yyyy"),
                IsDel = d.IsDel,
            });

            return result;
        }

        public List<ProposedPowerProjectModel> FindData(QueryRequestBody query)
        {
            List<ProposedPowerProjectModel> result = new List<ProposedPowerProjectModel>();

            IQueryable<ProposedPowerProjectModel> data = from app in _context.ProposedPowerProjects
                                                         where !app.IsDel
                                                         join ei in _context.TypeOfEnergies
                                                         on app.EnergyIndustryId equals ei.TypeOfEnergyId into JoinEi
                                                         from eni in JoinEi.DefaultIfEmpty()
                                                         join c in _context.Categories
                                                         on app.StatusId equals c.CategoryId into JoinCa
                                                         from ca in JoinCa.DefaultIfEmpty()
                                                         select new ProposedPowerProjectModel
                                                         {
                                                             ProposedPowerProjectId = app.ProposedPowerProjectId,
                                                             EnergyIndustryId = eni.TypeOfEnergyId,
                                                             EnergyIndustryName = eni.TypeOfEnergyName,
                                                             ProjectName = app.ProjectName,
                                                             StatusId = app.StatusId,
                                                             StatusName = ca.CategoryName,
                                                             InvestorName = app.InvestorName,
                                                             Address = app.Address,
                                                             Wattage = app.Wattage,
                                                             PolicyDecision = app.PolicyDecision,
                                                             Note = app.Note,
                                                         };

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                data = data.Where(x =>
                   x.ProjectName.ToLower().Contains(_keywordSearch)
                   || x.InvestorName.ToLower().Contains(_keywordSearch)
                   || x.PolicyDecision.ToLower().Contains(_keywordSearch)
                   || x.Wattage.ToString().Contains(_keywordSearch)
                   || x.Address.ToLower().Contains(_keywordSearch)
               );
            }

            if (query.Filter != null && query.Filter.ContainsKey("EnergyIndustryId") && !string.IsNullOrEmpty(query.Filter["EnergyIndustryId"]))
            {
                data = data.Where(x => x.EnergyIndustryId.ToString().Equals(string.Join("", query.Filter["EnergyIndustryId"])));
            }

            if (query.Filter != null && query.Filter.ContainsKey("StatusId") && !string.IsNullOrEmpty(query.Filter["StatusId"]))
            {
                data = data.Where(x => x.StatusId.ToString().Equals(string.Join("", query.Filter["StatusId"])));
            }

            result = data.ToList();

            return result;
        }
    }
}
