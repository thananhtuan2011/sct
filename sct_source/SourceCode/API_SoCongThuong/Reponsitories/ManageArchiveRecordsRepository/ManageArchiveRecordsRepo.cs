using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class ManageArchiveRecordsRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ManageArchiveRecordsRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(ManageArchiveRecord model)
        {
            await _context.ManageArchiveRecords.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ManageArchiveRecordsModel> FindById(Guid Id)
        {
            var result = _context.ManageArchiveRecords.Where(x => x.ManageArchiveRecordsId == Id && !x.IsDel).Select(f => new ManageArchiveRecordsModel()
            {
                ManageArchiveRecordsId = f.ManageArchiveRecordsId,
                RecordsFinancePlanId = f.RecordsFinancePlanId,
                CodeFile = f.CodeFile,
                Title = f.Title,
                ReceptionTime = f.ReceptionTime,
                StorageTime = f.StorageTime,
                Location = f.Location,
                StoreDocumentsAt = f.StoreDocumentsAt,
                StoreFilesAt = f.StoreFilesAt,
                Creator = f.Creator,
                Note = f.Note,
            });

            return result;
        }

        public async Task Update(ManageArchiveRecord model)
        {
            _context.ManageArchiveRecords.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ManageArchiveRecord model)
        {
            var db = await _context.ManageArchiveRecords.Where(d => d.ManageArchiveRecordsId == model.ManageArchiveRecordsId).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsDel = model.IsDel;
            }
            await _context.SaveChangesAsync();
        }

        public List<ManageArchiveRecordsModel> FindData(QueryRequestBody query)
        {
            List<ManageArchiveRecordsModel> result = new List<ManageArchiveRecordsModel>();

            Dictionary<int, string> GroupName = new Dictionary<int, string>()
            {
                {1, "An toàn thực phẩm"},
                {2, "Bảo vệ môi trường"},
                {3, "An toàn hóa chất"},
                {4, "Công tác phòng chống cháy nổ"},
                {5, "Lĩnh vực kinh doanh khí"}
            };

            IQueryable<ManageArchiveRecordsModel> _data = _context.ManageArchiveRecords.Where(x => !x.IsDel).Select(f => new ManageArchiveRecordsModel
            {
                ManageArchiveRecordsId = f.ManageArchiveRecordsId,
                RecordsFinancePlanId = f.RecordsFinancePlanId,
                RecordsFinancePlan = GroupName[f.RecordsFinancePlanId],
                CodeFile = f.CodeFile,
                Title = f.Title,
                ReceptionTime = f.ReceptionTime,
                StorageTime = f.StorageTime,
                Location = f.Location,
                StoreDocumentsAt = f.StoreDocumentsAt,
                StoreFilesAt = f.StoreFilesAt,
                Creator = f.Creator,
                Note = f.Note,
            }).ToList().AsQueryable();

            //Search
            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   x.CodeFile.ToLower().Contains(_keywordSearch)
                   || x.RecordsFinancePlan.ToLower().Contains(_keywordSearch)
                   || x.Title.ToLower().Contains(_keywordSearch)
               );
            }

            //Filter
            int currentYear = DateTime.Now.Year;
            if (query.Filter != null && query.Filter.ContainsKey("RecordsFinancePlan") && !string.IsNullOrEmpty(query.Filter["RecordsFinancePlan"]))
            {
                _data = _data.Where(x => x.RecordsFinancePlanId.ToString() == query.Filter["RecordsFinancePlan"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                _data = _data.Where(x => x.ReceptionTime.Year.ToString() == query.Filter["Year"]);
            }
            else
            {
                _data = _data.Where(x => x.ReceptionTime.Year == currentYear);
            }

            result = _data.ToList();

            return result;
        }
    }
}
