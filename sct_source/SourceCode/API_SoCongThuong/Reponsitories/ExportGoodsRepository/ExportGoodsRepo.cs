using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ExportGoodsRepository
{
    public class ExportGoodsRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ExportGoodsRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ExportGood model)
        {
            await _context.ExportGoods.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ExportGood model)
        {
            var db = await _context.ExportGoods.Where(d => d.ExportGoodsId == model.ExportGoodsId).FirstOrDefaultAsync();
            db.ExportGoodsName = model.ExportGoodsName;
            db.ItemGroupId = model.ItemGroupId;
            db.TypeOfEconomicId = model.TypeOfEconomicId;
            db.BusinessId = model.BusinessId;
            db.CountryId = model.CountryId;
            db.Amount = model.Amount;
            db.AmountUnit = model.AmountUnit;
            db.Price = model.Price;
            db.ExportTime = model.ExportTime;
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteExportGoods(ExportGood model)
        {
            var db = await _context.ExportGoods.Where(d => d.ExportGoodsId == model.ExportGoodsId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.ExportGoods.Where(x => x.ExportGoodsId == id).FirstOrDefaultAsync();
            _context.ExportGoods.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ExportGood> FindAll()
        {
            var result = _context.ExportGoods.Select(d => new ExportGood()
            {
                ExportGoodsName = d.ExportGoodsName,
                ItemGroupId = d.ItemGroupId,
                TypeOfEconomicId = d.TypeOfEconomicId,
                BusinessId = d.BusinessId,
                CountryId = d.CountryId,
                Amount = d.Amount,
                AmountUnit = d.AmountUnit,
                Price = d.Price,
                ExportTime = d.ExportTime,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<ExportGood> FindById(Guid Id)
        {
            var result = _context.ExportGoods.Where(x => x.ExportGoodsId == Id).Select(d => new ExportGood()
            {
                ExportGoodsName = d.ExportGoodsName,
                ItemGroupId = d.ItemGroupId,
                TypeOfEconomicId = d.TypeOfEconomicId,
                BusinessId = d.BusinessId,
                CountryId = d.CountryId,
                Amount = d.Amount,
                AmountUnit = d.AmountUnit,
                Price = d.Price,
                ExportTime = d.ExportTime,
                IsDel = d.IsDel,
            });

            return result;
        }

        public List<ExportGoodsView> FindData(QueryRequestBody query)
        {
            List<ExportGoodsView> result = new List<ExportGoodsView>();
            
            var _data = from I in _context.ExportGoods.Where(x => !x.IsDel)
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
                        select (new ExportGoodsView
                        {
                            ExportGoodsId = I.ExportGoodsId,
                            ExportGoodsName = I.ExportGoodsName,
                            ItemGroupName = C_ITEM.CategoryName,
                            TypeOfEconomicName = C_ECO.CategoryName,
                            CountryName = C.CountryName,
                            Amount = U.UnitName == "Tấn" ? I.Amount.ToString() : (I.Amount / 1000).ToString(),
                            Price = I.Price,
                            ExportTime = I.ExportTime.ToString("dd'/'MM'/'yyyy"),
                            ExportTimeDate = I.ExportTime,
                            //BusinessName = B.BusinessNameVi,
                        });

            //Search
            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.ToList().AsQueryable().Where(x =>
                   x.ExportGoodsName.ToLower().Contains(_keywordSearch) 
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
                _data = _data.Where(x => x.ExportTimeDate.HasValue && x.ExportTimeDate.Value.Month.ToString() == query.Filter["Month"]);
            } 
            else
            {
                _data = _data.Where(x => x.ExportTimeDate.HasValue && x.ExportTimeDate.Value.Month == currentMonth);
            }

            if (query.Filter != null && query.Filter.ContainsKey("Year") && !string.IsNullOrEmpty(query.Filter["Year"]))
            {
                _data = _data.Where(x => x.ExportTimeDate.HasValue && x.ExportTimeDate.Value.Year.ToString() == query.Filter["Year"]);
            }
            else
            {
                _data = _data.Where(x => x.ExportTimeDate.HasValue && x.ExportTimeDate.Value.Year == currentYear);
            }

            result = _data.ToList();

            return result;
        }
    }
}
