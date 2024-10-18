using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ImportGoodsRepository
{
    public class ImportGoodsRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ImportGoodsRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ImportGood model)
        {
            await _context.ImportGoods.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ImportGood model)
        {
            _context.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteImportGoods(ImportGood model)
        {
            var db = await _context.ImportGoods.Where(d => d.ImportGoodsId == model.ImportGoodsId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.ImportGoods.Where(x => x.ImportGoodsId == id).FirstOrDefaultAsync();
            _context.ImportGoods.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ImportGood> FindAll()
        {
            var result = _context.ImportGoods.Select(d => new ImportGood()
            {
                ImportGoodsName = d.ImportGoodsName,
                ItemGroupId = d.ItemGroupId,
                TypeOfEconomicId = d.TypeOfEconomicId,
                BusinessId = d.BusinessId,
                CountryId = d.CountryId,
                Amount = d.Amount,
                AmountUnit = d.AmountUnit,
                Price = d.Price,
                ImportTime = d.ImportTime,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<ImportGood> FindById(Guid Id)
        {
            var result = _context.ImportGoods.Where(x => x.ImportGoodsId == Id).Select(d => new ImportGood()
            {
                ImportGoodsName = d.ImportGoodsName,
                ItemGroupId = d.ItemGroupId,
                TypeOfEconomicId = d.TypeOfEconomicId,
                BusinessId = d.BusinessId,
                CountryId = d.CountryId,
                Amount = d.Amount,
                AmountUnit = d.AmountUnit,
                Price = d.Price,
                ImportTime = d.ImportTime,
                IsDel = d.IsDel,
            });

            return result;
        }

        public List<ImportGoodsView> FindData(QueryRequestBody query)
        {
            List<ImportGoodsView> result = new List<ImportGoodsView>();

            var _data = from I in _context.ImportGoods.Where(x => !x.IsDel)
                        join C_ITEM in _context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                            on I.ItemGroupId equals C_ITEM.CategoryId
                        join C_ECO in _context.Categories.Select(x => new { x.CategoryId, x.CategoryName })
                            on I.TypeOfEconomicId equals C_ECO.CategoryId
                        join C in _context.Countries
                            on I.CountryId equals C.CountryId
                        join U in _context.Units.Select(x => new { x.UnitId, x.UnitName })
                            on I.AmountUnit equals U.UnitId
                        //join B in _repoBusiness._context.Businesses.Where(x => !x.IsDel)
                        //    on I.BusinessId equals B.BusinessId
                        select (new ImportGoodsView
                        {
                            ImportGoodsId = I.ImportGoodsId,
                            ImportGoodsName = I.ImportGoodsName,
                            ItemGroupName = C_ITEM.CategoryName,
                            TypeOfEconomicName = C_ECO.CategoryName,
                            CountryName = C.CountryName,
                            Amount = U.UnitName == "Tấn" ? I.Amount.ToString() : (I.Amount / 1000).ToString(),
                            Price = I.Price,
                            ImportTime = I.ImportTime.ToString("dd'/'MM'/'yyyy"),
                            ImportTimeDate = I.ImportTime,
                            //BusinessName = B.BusinessNameVi,
                        });

            //Search
            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.ToList().AsQueryable().Where(x =>
                   x.ImportGoodsName.ToLower().Contains(_keywordSearch)
                   || x.ItemGroupName.ToLower().Contains(_keywordSearch)
                   || x.TypeOfEconomicName.ToLower().Contains(_keywordSearch)
                   || x.CountryName.ToLower().Contains(_keywordSearch)
                   || x.Amount.ToLower().Contains(_keywordSearch)
                   || x.Price.ToString().ToLower().Contains(_keywordSearch)
               );
            }

            //Filter
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            if (query.Filter != null && query.Filter.ContainsKey("Month") && !string.IsNullOrEmpty(query.Filter["Month"]))
            {
                _data = _data.Where(x => x.ImportTimeDate.HasValue && x.ImportTimeDate.Value.Month.ToString() == query.Filter["Month"]);
            }
            else
            {
                _data = _data.Where(x => x.ImportTimeDate.HasValue && x.ImportTimeDate.Value.Month == currentMonth);
            }

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                _data = _data.Where(x => x.ImportTimeDate.HasValue && x.ImportTimeDate.Value.Year.ToString() == query.Filter["Year"]);
            }
            else
            {
                _data = _data.Where(x => x.ImportTimeDate.HasValue && x.ImportTimeDate.Value.Year == currentYear);
            }

            result = _data.ToList();

            return result;
        }
    }
}
