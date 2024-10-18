using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.RooftopSolarProjectManagementRepository
{
    public class RooftopSolarProjectManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public RooftopSolarProjectManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(RooftopSolarProjectManagement model)
        {
            await _context.RooftopSolarProjectManagements.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(RooftopSolarProjectManagement model)
        {
            _context.RooftopSolarProjectManagements.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteRooftopSolarProjectManagement(RooftopSolarProjectManagement model)
        {
            var db = await _context.RooftopSolarProjectManagements.Where(d => d.RooftopSolarProjectManagementId == model.RooftopSolarProjectManagementId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.RooftopSolarProjectManagements.Where(x => x.RooftopSolarProjectManagementId == id).FirstOrDefaultAsync();
            _context.RooftopSolarProjectManagements.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<RooftopSolarProjectManagement> FindAll()
        {
            var result = _context.RooftopSolarProjectManagements.Select(d => new RooftopSolarProjectManagement()
            {
                RooftopSolarProjectManagementId = d.RooftopSolarProjectManagementId,
                ProjectName = d.ProjectName,
                InvestorName = d.InvestorName,
                Address = d.Address,
                Area = d.Area,
                SurveyPolicy = d.SurveyPolicy,
                Wattage = d.Wattage,
                Progress = d.Progress,
                IsDel = d.IsDel,
            });
            return result;
        }

        public IQueryable<RooftopSolarProjectManagement> FindById(Guid Id)
        {
            var result = _context.RooftopSolarProjectManagements.Where(x => x.RooftopSolarProjectManagementId == Id).Select(d => new RooftopSolarProjectManagement()
            {
                RooftopSolarProjectManagementId = d.RooftopSolarProjectManagementId,
                ProjectName = d.ProjectName,
                InvestorName = d.InvestorName,
                Address = d.Address,
                Area = d.Area,
                SurveyPolicy = d.SurveyPolicy,
                Wattage = d.Wattage,
                Progress = d.Progress,
                IsDel = d.IsDel,
                District = d.District,
                OperationDay = d.OperationDay,
        });

            return result;
        }

        public List<RooftopSolarProjectManagementModel> FindData(QueryRequestBody query)
        {
            List<RooftopSolarProjectManagementModel> result = new List<RooftopSolarProjectManagementModel>();

            IQueryable<RooftopSolarProjectManagementModel> data = (from r in _context.RooftopSolarProjectManagements where !r.IsDel
                                                                    join d in _context.Districts on r.District equals d.DistrictId
                                                                    select new RooftopSolarProjectManagementModel
                                                                    {
                                                                        RooftopSolarProjectManagementId = r.RooftopSolarProjectManagementId,
                                                                        ProjectName = r.ProjectName,
                                                                        InvestorName = r.InvestorName,
                                                                        Address = r.Address,
                                                                        Area = r.Area,
                                                                        SurveyPolicy = r.SurveyPolicy,
                                                                        Wattage = r.Wattage,
                                                                        Progress = r.Progress,
                                                                        IsDel = r.IsDel,
                                                                        OperationDay = r.OperationDay,
                                                                        District = r.District,
                                                                        DistrictName = d.DistrictName,
                                                                    }).ToList().AsQueryable();
            

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                data = data.Where(x =>
                   x.ProjectName.ToLower().Contains(_keywordSearch)
                   || x.InvestorName.ToLower().Contains(_keywordSearch)
                   || x.Address.ToLower().Contains(_keywordSearch)
                   || x.SurveyPolicy.ToLower().Contains(_keywordSearch)
                   //|| x.Area.ToString().Contains(_keywordSearch)
                   || x.Wattage.ToString().Contains(_keywordSearch)
                   || x.Progress.ToLower().Contains(_keywordSearch)
               );
            }
            if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
            {
                data = data.Where(x => x.District == Guid.Parse(query.Filter["District"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("Wattage") && !string.IsNullOrEmpty(query.Filter["Wattage"]))
            {
                string wattage = query.Filter["Wattage"];
                switch (wattage)
                {
                    case "INSTALL_CAPACITY_01":
                        data = data.Where(x => (1000 * x.Wattage) < 10);
                        break;
                    case "INSTALL_CAPACITY_02":
                        data = data.Where(x => (1000 * x.Wattage) >= 10 && (1000 * x.Wattage) < 100);
                        break;
                    case "INSTALL_CAPACITY_03":
                        data = data.Where(x => (1000 * x.Wattage) >= 100 && (1000 * x.Wattage) < 1000);
                        break;
                    case "INSTALL_CAPACITY_04":
                        data = data.Where(x => x.Wattage > 1);
                        break;
                };
            }

            result = data.ToList();

            return result;
        }
    }
}
