using EF_Core.Models;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API_SoCongThuong.Reponsitories.StatisticalImportExportDistrictRepository
{
    public class StatisticalImportExportDistrictRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public StatisticalImportExportDistrictRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public ReturnStatisticalImportExportDistrictData FindData(int type, Guid districtId, DateTime? minDate, DateTime? maxDate)
        {
            var BusinessOfDistrict = _context.Businesses.Where(x => x.DistrictId == districtId && !x.IsDel).Select(x => new { x.BusinessId, x.BusinessNameVi, x.NguoiDaiDien, x.SoDienThoai }).ToList();

            var Data = new List<ResultStatisticalImportExportDistrictData>();
            if (type == 0) //Nhập khẩu
            {
                var Query = _context.ImportGoods.Where(x => !x.IsDel);

                if (minDate != null)
                {
                    Query = Query.Where(x => x.CreateTime.Date >= minDate);
                }

                if (maxDate != null)
                {
                    Query = Query.Where(x => x.CreateTime.Date <= maxDate);
                }

                var ImportData = Query.ToList();

                foreach (var b in BusinessOfDistrict)
                {
                    var Item = new ResultStatisticalImportExportDistrictData();
                    var ItemData = ImportData.Where(x => x.BusinessId == b.BusinessId);

                    Item.BusinessId = b.BusinessId;
                    Item.BusinessName = b.BusinessNameVi;
                    Item.Represent = b.NguoiDaiDien ?? "";
                    Item.PhoneNumber = b.SoDienThoai ?? "";
                    Item.Items = ItemData.Select(x => x.ImportGoodsName).ToList().Distinct().ToList();
                    Item.Total = ItemData.Sum(x => x.Price);

                    Data.Add(Item);
                }

            }
            else if (type == 1) //Xuất khẩu
            {
                var Query = _context.ExportGoods.Where(x => !x.IsDel);

                if (minDate != null)
                {
                    Query = Query.Where(x => x.CreateTime.Date >= minDate);
                }

                if (maxDate != null)
                {
                    Query = Query.Where(x => x.CreateTime.Date <= maxDate);
                }

                var ExportData = Query.ToList();

                foreach (var b in BusinessOfDistrict)
                {
                    var Item = new ResultStatisticalImportExportDistrictData();
                    var ItemData = ExportData.Where(x => x.BusinessId == b.BusinessId);

                    Item.BusinessId = b.BusinessId;
                    Item.BusinessName = b.BusinessNameVi;
                    Item.Represent = b.NguoiDaiDien ?? "";
                    Item.PhoneNumber = b.SoDienThoai ?? "";
                    Item.Items = ItemData.Select(x => x.ExportGoodsName).ToList().Distinct().ToList();
                    Item.Total = ItemData.Sum(x => x.Price);

                    Data.Add(Item);
                }
            }

            var Total = new TotalStatisticalImportExportDistrictData()
            {
                TotalGoods = Data.SelectMany(x => x.Items).ToList().Distinct().Count(),
                TotalValue = Data.Select(x => x.Total).Sum(),
            };

            var Result = new ReturnStatisticalImportExportDistrictData()
            {
                data = Data.Where(x => x.Total != 0).ToList(),
                total = Total,
            };

            return Result;
        }
    }
}
