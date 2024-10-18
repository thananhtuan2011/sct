using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ManagementElectricityActivitiesRepository
{
    public class ManagementElectricityActivitiesRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ManagementElectricityActivitiesRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ManagementElectricityActivity model)
        {
            await _context.ManagementElectricityActivities.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ManagementElectricityActivity model)
        {
            _context.ManagementElectricityActivities.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(ManagementElectricityActivity model)
        {
            _context.ManagementElectricityActivities.Update(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ManagementElectricityActivity> FindById(Guid Id)
        {
            var result = _context.ManagementElectricityActivities.Where(x => x.ManagementElectricityActivitiesId == Id && !x.IsDel).Select(f => new ManagementElectricityActivity()
            {
                ManagementElectricityActivitiesId = f.ManagementElectricityActivitiesId,
                ProjectName = f.ProjectName,
                DistrictId = f.DistrictId,
                Wattage = f.Wattage,
                MaxWattage = f.MaxWattage,
                Type = f.Type,
                DateOfAcceptance = f.DateOfAcceptance,
                ConnectorAgreement = f.ConnectorAgreement,
                PowerPurchaseAgreement = f.PowerPurchaseAgreement,
                AnotherContent = f.AnotherContent
            });

            return result;
        }

        public List<ManagementElectricityActivitiesModel> FindData(QueryRequestBody query)
        {
            List<ManagementElectricityActivitiesModel> result = new List<ManagementElectricityActivitiesModel>();

            IQueryable<ManagementElectricityActivitiesModel> data = (from m in _context.ManagementElectricityActivities
                                                                      where !m.IsDel
                                                                      join d in _context.Districts
                                                                      on m.DistrictId equals d.DistrictId
                                                                      select new ManagementElectricityActivitiesModel
                                                                      {
                                                                          ManagementElectricityActivitiesId = m.ManagementElectricityActivitiesId,
                                                                          ProjectName = m.ProjectName,
                                                                          Type = m.Type,
                                                                          TypeName = m.Type == 1 ? "Trong khu công nghiệp" : m.Type == 2 ? "Kết hợp trại nông nghiệp" : "Khác",
                                                                          MaxWattage = m.MaxWattage,
                                                                          Wattage = m.Wattage,
                                                                          DateOfAcceptance = m.DateOfAcceptance,
                                                                          ConnectorAgreement = m.ConnectorAgreement,
                                                                          PowerPurchaseAgreement = m.PowerPurchaseAgreement,
                                                                          AnotherContent = m.AnotherContent,
                                                                          DistrictId = m.DistrictId,
                                                                          DistrictName = d.DistrictName,
                                                                      }).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                data = data.Where(x => x.ProjectName.ToLower().Contains(_keywordSearch)
                        || x.TypeName.ToLower().Contains(_keywordSearch)
                        || x.MaxWattage.ToString().Contains(_keywordSearch)
                        || x.DistrictName.ToLower().Contains(_keywordSearch));
            }

            if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
            {
                data = data.Where(x => x.Type.ToString() == query.Filter["Type"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("District") && !string.IsNullOrEmpty(query.Filter["District"]))
            {
                data = data.Where(x => x.DistrictId.ToString() == query.Filter["District"]);
            }

            result = data.ToList();

            return result;
        }
    }
}
