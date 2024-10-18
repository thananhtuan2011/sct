using EF_Core.Models;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace API_SoCongThuong.Reponsitories.StatisticalImportExportProvincialRepository
{
    public class StatisticalImportExportProvincialRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public StatisticalImportExportProvincialRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public ReturnStatisticalImportExportProvincialData FindData(DateTime? minDate, DateTime? maxDate)
        {
            var DistrictData = _context.Districts.Where(x => !x.IsDel).Select(x => new 
            {
                DistrictId = x.DistrictId,
                DistrictName = x.DistrictName,
                NumOfBusiness = _context.Businesses.Where(b => b.DistrictId == x.DistrictId).Count(),
            }).OrderBy(x => x.DistrictName).ToList();

            var QueryImport = _context.ImportGoods.Where(x => !x.IsDel)
                                                 .Join(_context.Businesses,
                                                       ib => ib.BusinessId,
                                                       b => b.BusinessId,
                                                       (ib, b) => new { 
                                                           BusinessId = ib.BusinessId, 
                                                           CreateTime = ib.CreateTime, 
                                                           Value = ib.Price, 
                                                           DistrictId = b.DistrictId });
            var QueryExport = _context.ExportGoods.Where(x => !x.IsDel)
                                                 .Join(_context.Businesses,
                                                       eb => eb.BusinessId,
                                                       b => b.BusinessId,
                                                       (eb, b) => new {
                                                           BusinessId = eb.BusinessId,
                                                           CreateTime = eb.CreateTime,
                                                           Value = eb.Price,
                                                           DistrictId = b.DistrictId
                                                       });
            if (minDate != null)
            {
                QueryImport = QueryImport.Where(x => x.CreateTime.Date >= minDate);
                QueryExport = QueryExport.Where(x => x.CreateTime.Date >= minDate);
            };
            if (maxDate != null)
            {
                QueryImport = QueryImport.Where(x => x.CreateTime.Date <= maxDate);
                QueryExport = QueryExport.Where(x => x.CreateTime.Date <= maxDate);
            };

            var ImportLocal = QueryImport.ToList();
            var ExportLocal = QueryExport.ToList();

            var Data = new List<ResultStatisticalImportExportProvincialData>();
            foreach (var district in DistrictData)
            {
                var Item = new ResultStatisticalImportExportProvincialData();
                var ImportData = ImportLocal.Where(x => x.DistrictId == district.DistrictId);
                var ExportData = ExportLocal.Where(x => x.DistrictId == district.DistrictId);

                Item.DistrictId = district.DistrictId;
                Item.DistrictName = district.DistrictName;
                Item.NumOfBusiness = ImportData.Select(x => x.BusinessId).Concat(ExportData.Select(x => x.BusinessId)).ToList().Distinct().Count();
                Item.TotalImport = ImportData.Sum(x => x.Value);
                Item.TotalExport = ExportData.Sum(x => x.Value);

                Data.Add(Item);
            };

            var Total = new TotalStatisticalImportExportProvincialData()
            {
                TotalNumOfBusiness = Data.Sum(x => x.NumOfBusiness),
                TotalValueImport = Data.Sum(x => x.TotalImport),
                TotalValueExport = Data.Sum(x => x.TotalExport)
            };

            var Result = new ReturnStatisticalImportExportProvincialData()
            {
                data = Data,
                total = Total,
            };

            return Result;
        }
    }
}
