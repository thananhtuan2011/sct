using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ElectricityInspectorCardRepository
{
    public class ElectricityInspectorCardRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ElectricityInspectorCardRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ElectricityInspectorCard model)
        {
            await _context.ElectricityInspectorCards.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ElectricityInspectorCard model)
        {
            _context.ElectricityInspectorCards.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteElectricityInspectorCard(ElectricityInspectorCard model)
        {
            var db = await _context.ElectricityInspectorCards.Where(d => d.ElectricityInspectorCardId == model.ElectricityInspectorCardId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.ElectricityInspectorCards.Where(x => x.ElectricityInspectorCardId == id).FirstOrDefaultAsync();
            _context.ElectricityInspectorCards.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ElectricityInspectorCardModel> FindById(Guid Id)
        {
            var result = _context.ElectricityInspectorCards.Where(x => x.ElectricityInspectorCardId == Id).Select(d => new ElectricityInspectorCardModel()
            {
                ElectricityInspectorCardId = d.ElectricityInspectorCardId,
                InspectorName = d.InspectorName,
                Birthday = d.Birthday.ToString("dd'/'MM'/'yyyy"),
                LicenseDate = d.LicenseDate.ToString("dd'/'MM'/'yyyy"),
                Degree = d.Degree,
                Unit = d.Unit,
                Seniority = d.Seniority,
                CardColor = d.CardColor,
                IsDel = d.IsDel,
            });

            return result;
        }

        public List<ElectricityInspectorCardModel> FindData(QueryRequestBody query)
        {
            List<ElectricityInspectorCardModel> result = new List<ElectricityInspectorCardModel>();

            IQueryable<ElectricityInspectorCardModel> data = (from e in _context.ElectricityInspectorCards
                                                              where !e.IsDel
                                                              join c in _context.Categories.Where(x => !x.IsDel && x.CategoryTypeCode == "CARD_COLOR")
                                                              on e.CardColor equals c.CategoryId
                                                              select new ElectricityInspectorCardModel
                                                              {
                                                                  ElectricityInspectorCardId = e.ElectricityInspectorCardId,
                                                                  InspectorName = e.InspectorName,
                                                                  Birthday = e.Birthday.ToString("dd'/'MM'/'yyyy"),
                                                                  BirthdayDate = e.Birthday,
                                                                  LicenseDate = e.LicenseDate.ToString("dd'/'MM'/'yyyy"),
                                                                  LicenseDateDate = e.LicenseDate,
                                                                  Degree = e.Degree,
                                                                  Unit = e.Unit,
                                                                  Seniority = e.Seniority,
                                                                  CardColor = e.CardColor,
                                                                  CardColorName = c.CategoryName,
                                                              }).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                data = data.Where(x =>
                   x.InspectorName.ToLower().Contains(_keywordSearch)
                   || x.Degree.ToLower().Contains(_keywordSearch)
                   || x.Unit.ToLower().Contains(_keywordSearch)
                   || x.Seniority.ToLower().Contains(_keywordSearch)
                   || x.CardColorName.ToLower().Contains(_keywordSearch)
               );
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinDate") && !string.IsNullOrEmpty(query.Filter["MinDate"]))
            {
                data = data.Where(x => DateTime.ParseExact(x.LicenseDate, "dd/MM/yyyy", null) >= DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxDate") && !string.IsNullOrEmpty(query.Filter["MaxDate"]))
            {
                data = data.Where(x => DateTime.ParseExact(x.LicenseDate, "dd/MM/yyyy", null) <= DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null));
            }

            result = data.ToList();

            return result;
        }
    }
}
