using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.StatisticalRepository;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using EF_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalController : ControllerBase
    {
        private StatisticalRepo _repo;
        public StatisticalController(SoHoa_SoCongThuongContext context)
        {
            _repo = new StatisticalRepo(context);
        }

        [Route("StatisticalByProvince")]
        [HttpGet]
        public object StatisticalByProvince()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                //UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                //if (loginData == null)
                //{
                //    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                //}
                var district = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName,
                }).OrderBy(d => d.DistrictName).ToList();
                var typeOfMarketCode = _repo._context.Categories.Where(x => x.CategoryTypeCode == "MARKET").ToList();
                var _data = from p in _repo._context.CommercialManagements
                            where !p.IsDel
                            group p by new { p.DistrictId, p.Type } into g
                            select new { g.Key.Type, g.Key.DistrictId, Count = g.Count() };

                var commercialData = _data.ToList();

                var _temp = from p in _repo._context.CommercialManagements
                            where (!p.IsDel && (p.TypeOfMarket != null && p.TypeOfMarket > 0) && p.Type.ToString() == "45EABBD0-756A-4898-800A-0E0EF829646C")
                            group p by new { p.DistrictId, p.Form } into g
                            select new { g.Key.Form, g.Key.DistrictId, Count = g.Count() };
                var _dataTypeOfMarket = _temp.ToList();

                var resultData = new List<StatisticalByProvinceDetailModel>();
                for (int i = 0; i < district.Count(); i++)
                {
                    var item = new StatisticalByProvinceDetailModel();
                    for (int j = 0; j < commercialData.Count(); j++)
                    {
                        if (commercialData[j].DistrictId == district[i].DistrictId)
                        {
                            for (int k = 0; k < typeOfMarketCode.Count(); k++)
                            {
                                if (commercialData[j].Type == typeOfMarketCode[k].CategoryId)
                                {
                                    if (typeOfMarketCode[k].CategoryCode == "M1")
                                    {
                                        for (int m = 0; m < _dataTypeOfMarket.Count(); m++)
                                        {
                                            if (_dataTypeOfMarket[m].DistrictId == district[i].DistrictId)
                                            {
                                                if (_dataTypeOfMarket[m].Form == 1)
                                                {
                                                    item.SLChoNgoaiQuyHoach = _dataTypeOfMarket[m].Count;
                                                    item.Total += _dataTypeOfMarket[m].Count;
                                                }
                                                else if (_dataTypeOfMarket[m].Form == 2)
                                                {
                                                    item.SLChoDem = _dataTypeOfMarket[m].Count;
                                                    item.Total += _dataTypeOfMarket[m].Count;
                                                }
                                                else if (_dataTypeOfMarket[m].Form == 3)
                                                {
                                                    item.SLChoNoi = _dataTypeOfMarket[m].Count;
                                                    item.Total += _dataTypeOfMarket[m].Count;
                                                }
                                                else
                                                {
                                                    item.SLChoTrongQuyHoach = _dataTypeOfMarket[m].Count;
                                                    item.Total += _dataTypeOfMarket[m].Count;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        switch (typeOfMarketCode[k].CategoryCode)
                                        {
                                            case "M2":
                                                item.SLSieuThi = commercialData[j].Count;
                                                item.Total += commercialData[j].Count;
                                                break;
                                            case "M3":
                                                item.SLTrungTamThuongMai = commercialData[j].Count;
                                                item.Total += commercialData[j].Count;
                                                break;
                                            case "M4":
                                                item.SLCuaHangTienLoi = commercialData[j].Count;
                                                item.Total += commercialData[j].Count;
                                                break;
                                            case "M5":
                                                item.SLCuaHangChuyenDoanh = commercialData[j].Count;
                                                item.Total += commercialData[j].Count;
                                                break;
                                            case "M6":
                                                item.SLCuaHangTapHoa = commercialData[j].Count;
                                                item.Total += commercialData[j].Count;
                                                break;
                                            case "M7":
                                                item.SLTrungTamLogictis = commercialData[j].Count;
                                                item.Total += commercialData[j].Count;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    item.DistrictId = district[i].DistrictId;
                    item.DistrictName = district[i].DistrictName;
                    resultData.Add(item);
                }
                return resultData;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpGet("ExportByProvince")]
        public IActionResult ExportByProvince()
        {
            //Query data
            var data = StatisticalByProvince();

            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Thongkesoluongchosieuthitrungtamthuongmaitinh.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 5;
                    int row = 1;

                    worksheet.Cell(1, 1).Value = "THỐNG KÊ SỐ LƯỢNG CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI - TỈNH BẾN TRE";

                    //Thêm dữ liệu vào file:
                    foreach (var item in (dynamic)data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.DistrictName;
                            worksheet.Cell(index, 3).Value = item.Total;
                            worksheet.Cell(index, 4).Value = item.SLTrungTamThuongMai;
                            worksheet.Cell(index, 5).Value = item.SLSieuThi;
                            worksheet.Cell(index, 6).Value = item.SLChoTrongQuyHoach;
                            worksheet.Cell(index, 7).Value = item.SLChoNgoaiQuyHoach;
                            worksheet.Cell(index, 8).Value = item.SLChoDem;
                            worksheet.Cell(index, 9).Value = item.SLChoNoi;
                            worksheet.Cell(index, 10).Value = item.SLCuaHangTienLoi;
                            worksheet.Cell(index, 11).Value = item.SLCuaHangTapHoa;
                            worksheet.Cell(index, 12).Value = item.SLCuaHangChuyenDoanh;
                            worksheet.Cell(index, 13).Value = item.SLTrungTamLogictis;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.DistrictName;
                            worksheet.Cell(index, 3).Value = item.Total;
                            worksheet.Cell(index, 4).Value = item.SLTrungTamThuongMai;
                            worksheet.Cell(index, 5).Value = item.SLSieuThi;
                            worksheet.Cell(index, 6).Value = item.SLChoTrongQuyHoach;
                            worksheet.Cell(index, 7).Value = item.SLChoNgoaiQuyHoach;
                            worksheet.Cell(index, 8).Value = item.SLChoDem;
                            worksheet.Cell(index, 9).Value = item.SLChoNoi;
                            worksheet.Cell(index, 10).Value = item.SLCuaHangTienLoi;
                            worksheet.Cell(index, 11).Value = item.SLCuaHangTapHoa;
                            worksheet.Cell(index, 12).Value = item.SLCuaHangChuyenDoanh;
                            worksheet.Cell(index, 13).Value = item.SLTrungTamLogictis;
                            index++;
                            row++;
                        }
                    }
                    var delrow = worksheet.Row(index);
                    delrow.Delete();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("StatisticalByCommune")]
        [HttpPost]
        public object StatisticalByCommune([FromBody] QueryStatisticalBody query)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                var _data = from p in _repo._context.CommercialManagements
                            where !p.IsDel
                            join ps in _repo._context.Categories on p.Type equals ps.CategoryId
                            select new
                            {
                                CommercialId = p.CommercialId,
                                Type = ps.CategoryCode,
                                Name = p.Name,
                                DistrictId = p.DistrictId,
                                CommuneId = p.CommuneId,
                                TypeOfMarket = p.TypeOfMarket,
                                Form = p.Form,
                                TypePickup = ps.CategoryCode == "M1" ? (p.TypeOfMarket == 1 ? "M11" : p.Form == 1 ? "M121" : p.Form == 2 ? "M122" : "M123") : ps.CategoryCode,

                            };
                if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                {
                    var districtIdFilter = query.Filter["DistrictId"];
                    _data = _data.Where(x => x.DistrictId.ToString() == districtIdFilter);
                }

                if (query.Filter != null && query.Filter.ContainsKey("CommuneId"))
                {
                    var CommuneIdFilter = query.Filter["CommuneId"];
                    _data = _data.Where(x => x.CommuneId.ToString() == CommuneIdFilter);
                }

                // TypeOfCategoryId -- TypePickup
                // M11 Chợ trong quy hoạch
                // M121 Chợ ngoài quy hoạch
                // M122 Chợ đêm
                // M123 Chợ nổi
                // M2 Siêu thị
                // M3 Trung tâm thương mại
                // M4 Cửa hàng tiện lợi
                // M5 Cửa hàng chuyên doanh
                // M6 Cửa hàng tạp hoá, thực phẩm truyền thống
                // M7 Trung tâm Logistics
                if (query.Filter != null && query.Filter.ContainsKey("TypeOfCategoryId"))
                {
                    var TypeOfCategoryFilter = query.Filter["TypeOfCategoryId"];
                    _data = _data.Where(x => x.TypePickup == TypeOfCategoryFilter);
                }
                var total = new TotalStatisticalByProvinceDetailModel();
                var resultData = new List<StatisticalByCommuneDetailModel>();
                var Result = new StatisticalByCommuneModel();
                if (_data.Count() == 0)
                {
                    return Result;
                }
                var _dataTotal = _data.GroupBy(x => x.DistrictId).Select(g => new { Key = g.Key, KhuVuc = g.Select(x => x.CommuneId).Distinct().Count(), LoaiHinh = g.Select(x => x.TypePickup).Distinct().Count() });
                var dataTotal = _dataTotal.ToList();
                var data = _data.ToList();

                var _totalLoaiHinh = _data.Select(x => x.TypePickup).Distinct().Count();


                var _dataDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(x => new
                {
                    DistrictId = x.DistrictId,
                    Name = x.DistrictName
                }).ToList();

                for (int i = 0; i < _dataDistrict.Count(); i++)
                {
                    for (int j = 0; j < dataTotal.Count(); j++)
                    {
                        if (_dataDistrict[i].DistrictId == dataTotal[j].Key)
                        {
                            total.KhuVuc += dataTotal[j].KhuVuc;
                            total.LoaiHinh = dataTotal.Count() == 1 ? dataTotal[j].LoaiHinh : (int)_totalLoaiHinh;
                        }
                    }
                }

                var _dataCommune = _repo._context.Communes.Where(x => !x.IsDel).Select(x => new
                {
                    CommuneId = x.CommuneId,
                    Name = x.CommuneName
                }).ToList();
                var _dataMarket = _repo._context.MarketManagements
                    .Where(s => !s.IsDel)
                    .GroupBy(s => s.MarketId)
                    .Select(g => new
                    {
                        MarketId = g.Key,
                        SoSap = g.Select(s => s.NganhHangKinhDoanh).Distinct().Count(),
                        SoNganhHangKinhDoanh = _repo._context.MarketManagementDetails.Where(x => x.MarketId == g.Key).Count()
                    });
                var dataMarket = _dataMarket.ToList();
                for (int i = 0; i < data.Count(); i++)
                {
                    var item = new StatisticalByCommuneDetailModel();
                    item.MaHuyen = data[i].DistrictId;
                    item.MaXa = data[i].CommuneId;
                    item.MaCho = data[i].CommercialId;
                    item.TenCho = data[i].Name;
                    item.LoaiHinh = data[i].TypePickup == "M11" ? "Chợ trong quy hoạch" : data[i].TypePickup == "M121" ? "Chợ ngoài quy hoạch" : data[i].TypePickup == "M122" ? "Chợ đêm" : data[i].TypePickup == "M123" ? "Chợ nổi" : data[i].TypePickup == "M2" ? "Siêu thị" : data[i].TypePickup == "M3" ? "Trung tâm thương mại" : data[i].TypePickup == "M4" ? "Cửa hàng tiện lợi" : data[i].TypePickup == "M5" ? "Cửa hàng chuyên doanh" : data[i].TypePickup == "M6" ? "Cửa hàng tạp hoá" : "Trung tâm Logistics";

                    // lấy tên Huyện
                    for (int j = 0; j < _dataDistrict.Count(); j++)
                    {
                        if (data[i].DistrictId == _dataDistrict[j].DistrictId)
                        {
                            item.TenHuyen = _dataDistrict[j].Name;
                            break;
                        }
                    }
                    // Lấy tên Xã
                    for (int j = 0; j < _dataCommune.Count(); j++)
                    {
                        if (data[i].CommuneId == _dataCommune[j].CommuneId)
                        {
                            item.TenXa = _dataCommune[j].Name;
                            break;
                        }
                    }

                    for (int j = 0; j < dataMarket.Count(); j++)
                    {
                        if (data[i].CommercialId == dataMarket[j].MarketId)
                        {

                            item.SoSap = dataMarket[j].SoSap;
                            item.SoNganhHangKinhDoanh = dataMarket[j].SoNganhHangKinhDoanh != null ? (int)dataMarket[j].SoNganhHangKinhDoanh : 0;
                            total.SoSap += item.SoSap;
                            total.SoNganhHangKinhDoanh = item.SoNganhHangKinhDoanh;
                        }
                    }
                    resultData.Add(item);
                }
                Result.Details = resultData;
                Result.Total = total;
                return Result;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpPost("ExportByDistrict")]
        public IActionResult ExportByDistrict([FromBody] QueryStatisticalBody query)
        {
            //Query data
            try
            {
                StatisticalByCommuneModel data = (StatisticalByCommuneModel)StatisticalByCommune(query);
                if (data == null)
                {
                    return BadRequest();
                }

                using (var workbook = new XLWorkbook(@"Upload/Templates/Thongkesoluongchosieuthitrungtamthuongmaihuyen.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 4;
                    int row = 1;

                    worksheet.Cell(3, 3).Value = data.Total.KhuVuc;
                    worksheet.Cell(3, 4).Value = data.Total.LoaiHinh;
                    worksheet.Cell(3, 5).Value = data.Total.SoSap;
                    worksheet.Cell(3, 6).Value = data.Total.SoNganhHangKinhDoanh;

                    //if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                    //{
                    //    var DistrictName = _repo._context.Districts.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"] && !x.IsDel).FirstOrDefault().DistrictName.ToUpper();
                    //    worksheet.Cell(1, 1).Value = "THỐNG KÊ SỐ LƯỢNG CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI - HUYỆN " + DistrictName;
                    //} else
                    //{
                    //    worksheet.Cell(1, 1).Value = "THỐNG KÊ SỐ LƯỢNG CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI - HUYỆN";
                    //}

                    if (query.Filter != null && query.Filter.TryGetValue("DistrictId", out var districtId))
                    {
                        var district = _repo._context.Districts.FirstOrDefault(x => x.DistrictId.ToString() == districtId && !x.IsDel);
                        worksheet.Cell(1, 1).Value = "THỐNG KÊ SỐ LƯỢNG CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI - HUYỆN " + (district?.DistrictName?.ToUpper() ?? "");
                    }
                    else
                    {
                        worksheet.Cell(1, 1).Value = "THỐNG KÊ SỐ LƯỢNG CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI - HUYỆN";
                    }

                    //Thêm dữ liệu vào file:
                    foreach (var item in data.Details)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenCho;
                            worksheet.Cell(index, 3).Value = item.TenXa;
                            worksheet.Cell(index, 4).Value = item.LoaiHinh;
                            worksheet.Cell(index, 5).Value = item.SoSap;
                            worksheet.Cell(index, 6).Value = item.SoNganhHangKinhDoanh;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenCho;
                            worksheet.Cell(index, 3).Value = item.TenXa;
                            worksheet.Cell(index, 4).Value = item.LoaiHinh;
                            worksheet.Cell(index, 5).Value = item.SoSap;
                            worksheet.Cell(index, 6).Value = item.SoNganhHangKinhDoanh;
                            index++;
                            row++;
                        }
                    }
                    var delrow = worksheet.Row(index);
                    delrow.Delete();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [Route("StatisticalHasBeenBuildUpgradedByProvince")]
        [HttpGet]
        public object StatisticalHasBeenBuildUpgradedByProvince()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                //UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                //if (loginData == null)
                //{
                //    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                //}
                var _dataDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName,
                }).OrderBy(d => d.DistrictName).ToList();

                var _dataBuildAndUpgrade = _repo._context.BuildAndUpgradeMarkets.Where(s => (!s.IsDel && (s.IsBuild || s.IsUpgrade))).ToList();
                var _countDataBuildAndUpgrade = _dataBuildAndUpgrade.GroupBy(x => x.DistrictId).Select(g => new
                {
                    DistrictId = g.Key,
                    SoLuongChoST = g.Select(x => x.CommercialId).Distinct().Count()
                }).ToList();

                var result = new List<StatisticalHasBeenBuildUpgradedDetailModel>();
                var total = new TotalStatisticalHasBeenBuildUpgradedDetailModel();
                for (int i = 0; i < _dataDistrict.Count(); i++)
                {
                    var item = new StatisticalHasBeenBuildUpgradedDetailModel();
                    for (int j = 0; j < _dataBuildAndUpgrade.Count(); j++)
                    {
                        if (_dataDistrict[i].DistrictId == _dataBuildAndUpgrade[j].DistrictId)
                        {
                            if (_dataBuildAndUpgrade[j].TotalInvestmentUnit == "TY")
                            {
                                item.TongVonDautu += (_dataBuildAndUpgrade[j].TotalInvestment != null ? (decimal)_dataBuildAndUpgrade[j].TotalInvestment : 0);
                            }
                            else
                            {
                                item.TongVonDautu += (_dataBuildAndUpgrade[j].TotalInvestment != null ? (decimal)_dataBuildAndUpgrade[j].TotalInvestment / 1000 : 0);
                            }

                            if (_dataBuildAndUpgrade[j].RealizedCapitalUnit == "TY")
                            {
                                item.VonDaThucHien += (_dataBuildAndUpgrade[j].RealizedCapital != null ? (decimal)_dataBuildAndUpgrade[j].RealizedCapital : 0);
                            }
                            else
                            {
                                item.VonDaThucHien += (_dataBuildAndUpgrade[j].RealizedCapital != null ? (decimal)_dataBuildAndUpgrade[j].RealizedCapital / 1000 : 0);
                            }

                            if (_dataBuildAndUpgrade[j].BudgetCapitalUnit == "TY")
                            {
                                item.VonNganSach += (_dataBuildAndUpgrade[j].BudgetCapital != null ? (decimal)_dataBuildAndUpgrade[j].BudgetCapital : 0);
                            }
                            else
                            {
                                item.VonNganSach += (_dataBuildAndUpgrade[j].BudgetCapital != null ? (decimal)_dataBuildAndUpgrade[j].BudgetCapital / 1000 : 0);
                            }

                            if (_dataBuildAndUpgrade[j].LandUseCapitalUnit == "TY")
                            {
                                item.VonCQSDDat += (_dataBuildAndUpgrade[j].LandUseCapital != null ? (decimal)_dataBuildAndUpgrade[j].LandUseCapital : 0);
                            }
                            else
                            {
                                item.VonCQSDDat += (_dataBuildAndUpgrade[j].LandUseCapital != null ? (decimal)_dataBuildAndUpgrade[j].LandUseCapital / 1000 : 0);
                            }

                            if (_dataBuildAndUpgrade[j].LoansUnit == "TY")
                            {
                                item.VonVay += (_dataBuildAndUpgrade[j].Loans != null ? (decimal)_dataBuildAndUpgrade[j].Loans : 0);
                            }
                            else
                            {
                                item.VonVay += (_dataBuildAndUpgrade[j].Loans != null ? (decimal)_dataBuildAndUpgrade[j].Loans / 1000 : 0);
                            }

                            if (_dataBuildAndUpgrade[j].AnotherCapitalUnit == "TY")
                            {
                                item.VonKhac += (_dataBuildAndUpgrade[j].AnotherCapital != null ? (decimal)_dataBuildAndUpgrade[j].AnotherCapital : 0);
                            }
                            else
                            {
                                item.VonKhac += (_dataBuildAndUpgrade[j].AnotherCapital != null ? (decimal)_dataBuildAndUpgrade[j].AnotherCapital / 1000 : 0);
                            }

                        }
                    }
                    for (int j = 0; j < _countDataBuildAndUpgrade.Count(); j++)
                    {
                        if (_dataDistrict[i].DistrictId == _countDataBuildAndUpgrade[j].DistrictId)
                        {
                            item.TongCho = (int)_countDataBuildAndUpgrade[j].SoLuongChoST;
                            break;
                        }
                    }
                    item.MaHuyen = _dataDistrict[i].DistrictId;
                    item.TenHuyen = _dataDistrict[i].DistrictName;
                    result.Add(item);
                    total.TongCho += item.TongCho;
                    total.TongVonDautu += item.TongVonDautu;
                    total.VonDaThucHien += item.VonDaThucHien;
                    total.VonNganSach += item.VonNganSach;
                    total.VonCQSDDat += item.VonCQSDDat;
                    total.VonVay += item.VonVay;
                    total.VonKhac += item.VonKhac;
                }
                var dataRessult = new StatisticalHasBeenBuildUpgradedModel();
                dataRessult.Details = result.Where(x => x.TongCho != 0).ToList();
                dataRessult.Total = total;
                return dataRessult;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [Route("StatisticalHasNotBuildUpgradedByProvince")]
        [HttpGet]
        public object StatisticalHasNotBuildUpgradedByProvince()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var _dataDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName,
                }).OrderBy(d => d.DistrictName).ToList();

                var data = from p in _repo._context.CommercialManagements
                           where !p.IsDel && !(from o in _repo._context.BuildAndUpgradeMarkets
                                               where !o.IsDel
                                               select o.CommercialId).Contains(p.CommercialId)
                           select p;
                var _countMarket = data.GroupBy(x => x.DistrictId).Select(g => new
                {
                    DistrictId = g.Key,
                    count = g.Select(x => x.CommercialId).Count()
                }).ToList();
                var result = new List<StatisticalHasNotBuildUpgradedDetailModel>();
                int total = 0;
                for (int i = 0; i < _dataDistrict.Count(); i++)
                {
                    var item = new StatisticalHasNotBuildUpgradedDetailModel();
                    for (int j = 0; j < _countMarket.Count(); j++)
                    {
                        if (_dataDistrict[i].DistrictId == _countMarket[j].DistrictId)
                        {
                            item.TongCho = _countMarket[j].count;
                            total += item.TongCho;
                            break;
                        }
                    }
                    item.DistrictId = _dataDistrict[i].DistrictId;
                    item.DistrictName = _dataDistrict[i].DistrictName;

                    result.Add(item);
                }
                var final = new { Details = result.Where(x => x.TongCho != 0).ToList(), total };

                return final;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [Route("StatisticalHasNotBuildUpgraded/{id}")]
        [HttpGet]

        public object StatisticalHasNotBuildUpgradedById(Guid id)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                var _dataDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName,
                }).ToList();

                var _data = from p in _repo._context.CommercialManagements
                            where !p.IsDel && p.DistrictId == id && !(from o in _repo._context.BuildAndUpgradeMarkets
                                                                      where !o.IsDel
                                                                      select o.CommercialId).Contains(p.CommercialId)
                            join ps in _repo._context.Communes on p.CommuneId equals ps.CommuneId
                            join m in _repo._context.Categories on p.Type equals m.CategoryId
                            select new
                            {
                                CommercialId = p.CommercialId,
                                TenCho = p.Name,
                                KhuVuc = ps.CommuneName,
                                LoaiHinh = m.CategoryCode == "M1" ? p.TypeOfMarket != 2 ? "Chợ trong quy hoạch" : p.Form == 3 ? "Chợ nổi" : p.Form == 2 ? "Chợ đêm" : "Chợ ngoài quy hoạch" : m.CategoryName
                            };
                var data = _data.ToList();
                var _dataMarketManagement = _repo._context.MarketManagements
                    .Where(x => !x.IsDel && x.DistrictId == id)
                    .GroupBy(s => s.MarketId)
                    .Select(g => new
                    {
                        MarketId = g.Key,
                        SLNganhHang = _repo._context.MarketManagementDetails.Where(x => x.MarketId == g.Key).Count(),
                        SoSap = g.Select(x => x.BoothNumber).Sum()
                    }).ToList();
                var result = new List<StatisticalHasNotBuildUpgradedByIdDetailModel>();

                //for (int i = 0; i < data.Count(); i++)
                //{
                //    var item = new StatisticalHasNotBuildUpgradedByIdDetailModel();
                //    for (int j = 0; j < _dataMarketManagement.Count(); j++)
                //    {
                //        if (data[i].CommercialId == _dataMarketManagement[j].MarketId)
                //        {
                //            item.SoSap = _dataMarketManagement[j].SoSap != null ? (int)_dataMarketManagement[j].SoSap : 0;
                //            item.SoNganhHangKinhDoanh = _dataMarketManagement[j].SLNganhHang != null ? (int)_dataMarketManagement[j].SLNganhHang : 0;
                //            break;
                //        }
                //    }
                //    item.CommercialId = data[i].CommercialId;
                //    item.TenCho = data[i].TenCho;
                //    item.KhuVuc = data[i].KhuVuc;
                //    item.LoaiHinh = data[i].LoaiHinh;
                //    result.Add(item);
                //}

                for (int i = 0; i < data.Count; i++)
                {
                    var item = new StatisticalHasNotBuildUpgradedByIdDetailModel();
                    var marketManagement = _dataMarketManagement.FirstOrDefault(m => m.MarketId == data[i].CommercialId);
                    if (marketManagement != null)
                    {
                        item.SoSap = marketManagement.SoSap ?? 0;
                        item.SoNganhHangKinhDoanh = marketManagement.SLNganhHang;
                    }
                    item.CommercialId = data[i].CommercialId;
                    item.TenCho = data[i].TenCho;
                    item.KhuVuc = data[i].KhuVuc;
                    item.LoaiHinh = data[i].LoaiHinh;
                    result.Add(item);
                }

                return result;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpGet("ExportStatisticalHasBeenBuildUpgraded")]
        public IActionResult ExportStatisticalHasBeenBuildUpgraded()
        {
            //Query data
            StatisticalHasBeenBuildUpgradedModel data = (StatisticalHasBeenBuildUpgradedModel)StatisticalHasBeenBuildUpgradedByProvince();

            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Thongkexaydungnangcapchosieuthitrungtamthuongmaitinh.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 4;
                    int row = 1;

                    worksheet.Cell(1, 1).Value = "THỐNG KÊ XÂY DỰNG, NÂNG CẤP CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI - TỈNH BẾN TRE";

                    //Thêm dữ liệu vào file:
                    foreach (var item in (dynamic)data.Details)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenHuyen;
                            worksheet.Cell(index, 3).Value = item.TongCho;
                            worksheet.Cell(index, 4).Value = item.TongVonDautu;
                            worksheet.Cell(index, 5).Value = item.VonDaThucHien;
                            worksheet.Cell(index, 6).Value = item.VonNganSach;
                            worksheet.Cell(index, 7).Value = item.VonCQSDDat;
                            worksheet.Cell(index, 8).Value = item.VonVay;
                            worksheet.Cell(index, 9).Value = item.VonKhac;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenHuyen;
                            worksheet.Cell(index, 3).Value = item.TongCho;
                            worksheet.Cell(index, 4).Value = item.TongVonDautu;
                            worksheet.Cell(index, 5).Value = item.VonDaThucHien;
                            worksheet.Cell(index, 6).Value = item.VonNganSach;
                            worksheet.Cell(index, 7).Value = item.VonCQSDDat;
                            worksheet.Cell(index, 8).Value = item.VonVay;
                            worksheet.Cell(index, 9).Value = item.VonKhac;
                            index++;
                            row++;
                        }
                    }
                    var delrow = worksheet.Row(index);
                    delrow.Delete();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ExportStatisticalHasNotBuildUpgraded/{id}")]
        public IActionResult ExportStatisticalHasNotBuildUpgraded(Guid id)
        {
            //Query data
            var data = StatisticalHasNotBuildUpgradedById(id);

            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                var districtName = _repo._context.Districts.Where(x => !x.IsDel && x.DistrictId == id).Select(x => x.DistrictName).FirstOrDefault();
                if (districtName != null)
                {
                    districtName = districtName.ToUpper();
                }
                using (var workbook = new XLWorkbook(@"Upload/Templates/Thongkechosieuthitrungtamthuongmaichuanangcaopxaydung.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 4;
                    int row = 1;
                    worksheet.Cell(1, 1).Value = $"THỐNG KÊ CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI CHƯA XÂY DỰNG, NÂNG CẤP KHU VỰC {districtName}";

                    //Thêm dữ liệu vào file:
                    foreach (var item in (dynamic)data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenCho;
                            worksheet.Cell(index, 3).Value = item.KhuVuc;
                            worksheet.Cell(index, 4).Value = item.LoaiHinh;
                            worksheet.Cell(index, 5).Value = item.SoSap;
                            worksheet.Cell(index, 6).Value = item.SoNganhHangKinhDoanh;

                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenCho;
                            worksheet.Cell(index, 3).Value = item.KhuVuc;
                            worksheet.Cell(index, 4).Value = item.LoaiHinh;
                            worksheet.Cell(index, 5).Value = item.SoSap;
                            worksheet.Cell(index, 6).Value = item.SoNganhHangKinhDoanh;
                            index++;
                            row++;
                        }
                    }
                    var delrow = worksheet.Row(index);
                    delrow.Delete();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("StatisticalMarket")]
        [HttpPost]
        public object StatisticalMarket([FromBody] QueryStatisticalBody query)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                var _data = from p in _repo._context.MarketManagements
                            where !p.IsDel
                            join d in _repo._context.Districts on p.DistrictId equals d.DistrictId
                            join m in _repo._context.CommercialManagements on p.MarketId equals m.CommercialId
                            select new
                            {
                                DistrictId = p.DistrictId,
                                DistrictName = d.DistrictName,
                                CommuneId = p.CommuneId,
                                MarketId = p.MarketId,
                                MarketName = m.Name,
                                NganhHangKinhDoanh = p.NganhHangKinhDoanh,
                                GiaTrongNhaLong = p.GiaTrongNhaLong,
                                GiaNgoaiNhaLong = p.GiaNgoaiNhaLong,
                                DeXuatGiaMoi = p.DeXuatGiaMoi
                            };

                if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                {
                    var districtIdFilter = query.Filter["DistrictId"];
                    _data = _data.Where(x => x.DistrictId.ToString() == districtIdFilter);
                }

                if (query.Filter != null && query.Filter.ContainsKey("CommuneId"))
                {
                    var CommuneIdFilter = query.Filter["CommuneId"];
                    _data = _data.Where(x => x.CommuneId.ToString() == CommuneIdFilter);
                }

                if (query.Filter != null && query.Filter.ContainsKey("MarketId"))
                {
                    var MarketIdFilter = query.Filter["MarketId"];
                    _data = _data.Where(x => x.MarketId.ToString() == MarketIdFilter);
                }
                if (query.Filter != null && query.Filter.ContainsKey("BusinessLineId"))
                {
                    var BusinessLineIdFilter = query.Filter["BusinessLineId"];
                    _data = _data.Where(x => x.NganhHangKinhDoanh.ToString() == BusinessLineIdFilter);
                }
                var result = new List<StatisticalMarketModel>();
                var data = _data.ToList();
                if (data.Count() == 0)
                {
                    return result;
                }
                for (int i = 0; i < data.Count(); i++)
                {
                    var item = new StatisticalMarketModel();
                    item.TenCho = data[i].MarketName;
                    item.Huyen = data[i].DistrictName;
                    item.GiaTrongNhaLong = data[i].GiaTrongNhaLong != null ? (decimal)data[i].GiaTrongNhaLong : 0;
                    item.GiaNgoaiNhaLong = data[i].GiaNgoaiNhaLong != null ? (decimal)data[i].GiaNgoaiNhaLong : 0;
                    item.DeXuatGiaMoi = data[i].DeXuatGiaMoi != null ? (decimal)data[i].DeXuatGiaMoi : 0;
                    result.Add(item);
                }
                return result;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpPost("ExportStatisticalMarket")]
        public IActionResult ExportStatisticalMarket([FromBody] QueryStatisticalBody query)
        {
            //Query data
            var data = StatisticalMarket(query);

            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Thongkethongtinchosieuthitrungtamthuongmaitinh.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 5;
                    int row = 1;

                    worksheet.Cell(1, 1).Value = "THỐNG KÊ THÔNG TIN CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI - TỈNH BẾN TRE";

                    //Thêm dữ liệu vào file:
                    foreach (var item in (dynamic)data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenCho;
                            worksheet.Cell(index, 3).Value = item.Huyen;
                            worksheet.Cell(index, 4).Value = item.GiaTrongNhaLong;
                            worksheet.Cell(index, 5).Value = item.GiaNgoaiNhaLong;
                            worksheet.Cell(index, 6).Value = item.DeXuatGiaMoi;
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenCho;
                            worksheet.Cell(index, 3).Value = item.Huyen;
                            worksheet.Cell(index, 4).Value = item.GiaTrongNhaLong;
                            worksheet.Cell(index, 5).Value = item.GiaNgoaiNhaLong;
                            worksheet.Cell(index, 6).Value = item.DeXuatGiaMoi;
                            index++;
                            row++;
                        }
                    }
                    var delrow = worksheet.Row(index);
                    delrow.Delete();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Thống kê cửa hàng kinh doanh
        [Route("StatisticalBusinessStore")]
        [HttpPost]
        public object StatisticalBusinessStore([FromBody] QueryStatisticalBody query)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                var _dataCigarette = from cb in _repo._context.CigaretteBusinesses
                                     where !cb.IsDel
                                     join b in _repo._context.Businesses on cb.CigaretteBusinessName equals b.BusinessId
                                     where !b.IsDel
                                     select new
                                     {
                                         CigaretteBusinessId = cb.CigaretteBusinessId,
                                         CigaretteBusinessName = cb.CigaretteBusinessName,
                                         Huyen = _repo._context.Districts.Where(x => !x.IsDel && x.DistrictId == b.DistrictId).FirstOrDefault(),
                                         NguoiDaiDien = b.NguoiDaiDien,
                                         TenDoanhNghiep = b.BusinessNameVi,
                                         SoDienThoai = b.SoDienThoai,
                                         DistrictId = b.DistrictId,
                                     };

                var _dataPetro = from p in _repo._context.PetroleumBusinesses
                                 where !p.IsDel
                                 join b in _repo._context.Businesses on p.PetroleumBusinessName equals b.BusinessId
                                 where !b.IsDel
                                 select new
                                 {
                                     PetroleumBusinessId = p.PetroleumBusinessId,
                                     PetroleumBusinessName = p.PetroleumBusinessName,
                                     NguoiDaiDien = b.NguoiDaiDien,
                                     TenDoanhNghiep = b.BusinessNameVi,
                                     SoDienThoai = b.SoDienThoai,
                                     DistrictId = b.DistrictId,
                                     Huyen = _repo._context.Districts.Where(x => !x.IsDel && x.DistrictId == b.DistrictId).FirstOrDefault(),
                                 };
                var _dataAlcol = from p in _repo._context.AlcoholBusinesses
                                 where !p.IsDel
                                 join b in _repo._context.Businesses on p.AlcoholBusinessName equals b.BusinessId
                                 where !b.IsDel
                                 select new
                                 {
                                     AlcoholBusinessId = p.AlcoholBusinessId,
                                     AlcoholBusinessName = p.AlcoholBusinessName,
                                     NguoiDaiDien = b.NguoiDaiDien,
                                     TenDoanhNghiep = b.BusinessNameVi,
                                     SoDienThoai = b.SoDienThoai,
                                     DistrictId = b.DistrictId,
                                     Huyen = _repo._context.Districts.Where(x => !x.IsDel && x.DistrictId == b.DistrictId).FirstOrDefault(),
                                 };
                if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                {
                    var districtidfilter = query.Filter["DistrictId"];
                    _dataCigarette = _dataCigarette.Where(x => x.DistrictId.ToString() == districtidfilter);
                    _dataPetro = _dataPetro.Where(x => x.DistrictId.ToString() == districtidfilter);
                    _dataAlcol = _dataAlcol.Where(x => x.DistrictId.ToString() == districtidfilter);

                }

                if (query.Filter != null && query.Filter.ContainsKey("BusinessId"))
                {
                    var BusinessId = query.Filter["BusinessId"];
                    _dataCigarette = _dataCigarette.Where(x => x.CigaretteBusinessName.ToString() == BusinessId);
                    _dataPetro = _dataPetro.Where(x => x.PetroleumBusinessName.ToString() == BusinessId);
                    _dataAlcol = _dataAlcol.Where(x => x.AlcoholBusinessName.ToString() == BusinessId);
                }

                if (query.Filter != null && query.Filter.ContainsKey("NguoiDaiDien"))
                {
                    var nguoiDaiDien = query.Filter["NguoiDaiDien"];
                    _dataCigarette = _dataCigarette.Where(x => x.NguoiDaiDien.Contains(nguoiDaiDien));
                    _dataPetro = _dataPetro.Where(x => x.NguoiDaiDien.Contains(nguoiDaiDien));
                    _dataAlcol = _dataAlcol.Where(x => x.NguoiDaiDien.Contains(nguoiDaiDien));
                }

                var dataCigarette = _dataCigarette.ToList();
                var dataPetro = _dataPetro.ToList();
                var dataAlcol = _dataAlcol.ToList();

                List<BusinessDetailModel> listData = new List<BusinessDetailModel>();
                for (int i = 0; i < dataCigarette.Count(); i++)
                {
                    BusinessDetailModel item = new BusinessDetailModel();
                    item.BusinessId = dataCigarette[i].CigaretteBusinessName;
                    item.CigaretteBusinessId = dataCigarette[i].CigaretteBusinessId;
                    item.Huyen = dataCigarette[i].Huyen.DistrictName;
                    item.NguoiDaiDien = dataCigarette[i].NguoiDaiDien;
                    item.TenDoanhNghiep = dataCigarette[i].TenDoanhNghiep;
                    item.SoDienThoai = dataCigarette[i].SoDienThoai;
                    listData.Add(item);
                }
                int count = listData.Count();
                for (int i = 0; i < dataPetro.Count(); i++)
                {
                    bool IsAdd = false;
                    for (int j = 0; j < count; j++)
                    {
                        if (dataPetro[i].PetroleumBusinessName == listData[j].BusinessId)
                        {
                            listData[j].PetroleumBusinessId = dataPetro[i].PetroleumBusinessId;
                            IsAdd = true;
                            break;
                        }
                    }
                    if (!IsAdd)
                    {
                        BusinessDetailModel item = new BusinessDetailModel();
                        item.BusinessId = dataPetro[i].PetroleumBusinessName;
                        item.PetroleumBusinessId = dataPetro[i].PetroleumBusinessId;
                        item.Huyen = dataPetro[i].Huyen.DistrictName;
                        item.NguoiDaiDien = dataPetro[i].NguoiDaiDien;
                        item.TenDoanhNghiep = dataPetro[i].TenDoanhNghiep;
                        item.SoDienThoai = dataPetro[i].SoDienThoai;
                        listData.Add(item);
                    }
                }

                count = listData.Count();
                for (int i = 0; i < dataAlcol.Count(); i++)
                {
                    bool IsAdd = false;
                    for (int j = 0; j < count; j++)
                    {
                        if (dataAlcol[i].AlcoholBusinessName == listData[j].BusinessId)
                        {
                            listData[j].AlcoholBusinessId = dataAlcol[i].AlcoholBusinessId;
                            IsAdd = true;
                            break;
                        }
                    }
                    if (!IsAdd)
                    {
                        BusinessDetailModel item = new BusinessDetailModel();
                        item.BusinessId = dataAlcol[i].AlcoholBusinessName;
                        item.AlcoholBusinessId = dataAlcol[i].AlcoholBusinessId;
                        item.Huyen = dataAlcol[i].Huyen.DistrictName;
                        item.NguoiDaiDien = dataAlcol[i].NguoiDaiDien;
                        item.TenDoanhNghiep = dataAlcol[i].TenDoanhNghiep;
                        item.SoDienThoai = dataAlcol[i].SoDienThoai;
                        listData.Add(item);
                    }
                }

                var cigaretteStore = (from cbs in _repo._context.CigaretteBusinessStores
                                      where !cbs.IsDel
                                      join d in _repo._context.Districts on cbs.Huyen equals d.DistrictId
                                      where !d.IsDel
                                      select new
                                      {
                                          CigaretteBusinessId = cbs.CigaretteBusinessId,
                                          TenDoanhNghiep = cbs.TenDoanhNghiep,
                                          NguoiDaiDien = cbs.NguoiDaiDien,
                                          SoDienThoai = cbs.SoDienThoai,
                                          Huyen = d.DistrictName
                                      }).ToList();

                var petroStore = (from p in _repo._context.PetroleumBusinessStores
                                  where !p.IsDel
                                  join d in _repo._context.Districts on p.Huyen equals d.DistrictId
                                  where !d.IsDel
                                  select new
                                  {
                                      PetroleumBusinessId = p.PetroleumBusinessId,
                                      TenDoanhNghiep = p.TenCuaHang,
                                      NguoiDaiDien = p.NguoiDaiDien,
                                      SoDienThoai = p.SoDienThoai,
                                      Huyen = d.DistrictName,
                                      PetroleumDetailId = p.PetroleumDetailId
                                  }).ToList();

                var alcolStore = (from p in _repo._context.AlcoholBussinessDetails
                                  where !p.IsDel
                                  join d in _repo._context.Districts on p.Huyen equals d.DistrictId
                                  where !d.IsDel
                                  select new
                                  {
                                      AlcoholBusinessId = p.AlcoholBusinessId,
                                      TenDoanhNghiep = p.TenDoanhNghiep,
                                      NguoiDaiDien = p.NguoiDaiDien,
                                      SoDienThoai = p.SoDienThoai,
                                      Huyen = d.DistrictName
                                  }).ToList();

                List<StatisticalBusinessStoreModel> __resultData = new List<StatisticalBusinessStoreModel>();
                for (int i = 0; i < listData.Count(); i++)
                {

                    StatisticalBusinessStoreModel __data = new StatisticalBusinessStoreModel();
                    List<BusinessStoreDetailModel> listCigaretteItem = new List<BusinessStoreDetailModel>();
                    List<BusinessStoreDetailModel> listPetroItem = new List<BusinessStoreDetailModel>();
                    List<BusinessStoreDetailModel> listAlcolItem = new List<BusinessStoreDetailModel>();


                    for (int j = 0; j < cigaretteStore.Count(); j++)
                    {
                        BusinessStoreDetailModel item = new BusinessStoreDetailModel();
                        if (listData[i].CigaretteBusinessId == cigaretteStore[j].CigaretteBusinessId)
                        {
                            item.Huyen = cigaretteStore[j].Huyen;
                            item.NguoiDaiDien = cigaretteStore[j].NguoiDaiDien;
                            item.TenDoanhNghiep = cigaretteStore[j].TenDoanhNghiep;
                            item.SoDienThoai = cigaretteStore[j].SoDienThoai;
                            listCigaretteItem.Add(item);
                        }
                    }

                    for (int j = 0; j < petroStore.Count(); j++)
                    {
                        BusinessStoreDetailModel item = new BusinessStoreDetailModel();
                        if (listData[i].PetroleumBusinessId == petroStore[j].PetroleumBusinessId)
                        {
                            item.Huyen = petroStore[j].Huyen;
                            item.NguoiDaiDien = petroStore[j].NguoiDaiDien;
                            item.TenDoanhNghiep = petroStore[j].TenDoanhNghiep;
                            item.SoDienThoai = petroStore[j].SoDienThoai;
                            item.BusinessId = petroStore[j].PetroleumDetailId;
                            listPetroItem.Add(item);
                        }
                    }

                    for (int j = 0; j < alcolStore.Count(); j++)
                    {
                        BusinessStoreDetailModel item = new BusinessStoreDetailModel();
                        if (listData[i].AlcoholBusinessId == alcolStore[j].AlcoholBusinessId)
                        {
                            item.Huyen = alcolStore[j].Huyen;
                            item.NguoiDaiDien = alcolStore[j].NguoiDaiDien;
                            item.TenDoanhNghiep = alcolStore[j].TenDoanhNghiep;
                            item.SoDienThoai = alcolStore[j].SoDienThoai;
                            listAlcolItem.Add(item);
                        }
                    }
                    __data.QuanLyThuocLa = listCigaretteItem;
                    __data.QuanLyXangDau = listPetroItem;
                    __data.QuanLyRuou = listAlcolItem;
                    __data.NguoiDaiDien = listData[i].NguoiDaiDien;
                    __data.TenDoanhNghiep = listData[i].TenDoanhNghiep;
                    __data.SoDienThoai = listData[i].SoDienThoai;
                    __data.Huyen = listData[i].Huyen;
                    __resultData.Add(__data);
                }

                return __resultData;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        // Thống kê tình hình kinh doanh rượu thủ công
        [Route("StatisticalCraftAlcoholBusiness")]
        [HttpPost]
        public object StatisticalCraftAlcoholBusiness([FromBody] QueryStatisticalBody query)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                var dataCateReportSoldAncol = from cral in _repo._context.CateReportSoldAncols
                                              where !cral.IsDel
                                              group cral by new { cral.BusinessId, cral.Year } into g
                                              select new
                                              {
                                                  MaDoanhNghiep = g.Key.BusinessId,
                                                  Nam = g.Key.Year,
                                                  SoLuongMua = g.Select(s => s.QuantityBoughtOfYear).Sum(),
                                                  GiaTriMua = g.Select(s => s.TotalPriceBoughtOfYear).Sum(),
                                                  SoLuongBan = g.Select(s => s.QuantitySoldOfYear).Sum(),
                                                  GiaTriBan = g.Select(s => s.TotalPriceSoldOfYear).Sum(),
                                              } into groupData
                                              join b in _repo._context.Businesses on groupData.MaDoanhNghiep equals b.BusinessId
                                              select new
                                              {
                                                  MaDoanhNghiep = groupData.MaDoanhNghiep,
                                                  Nam = groupData.Nam,
                                                  SoLuongMua = groupData.SoLuongMua,
                                                  GiaTriMua = groupData.GiaTriMua,
                                                  SoLuongBan = groupData.SoLuongBan,
                                                  GiaTriBan = groupData.GiaTriBan,
                                                  TenDoanhNghiep = b.BusinessNameVi,
                                                  DiaChi = b.DiaChiTruSo,
                                                  DistrictId = b.DistrictId
                                              };
                //  dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.Nam == "2025");

                if (query.Filter != null && query.Filter.ContainsKey("Year"))
                {
                    var year = query.Filter["Year"];
                    dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.Nam.ToString() == year);
                }

                var countBusiness = from item in dataCateReportSoldAncol
                                    group item by item.DistrictId into n
                                    select new
                                    {
                                        DistrictId = n.Key,
                                        count = n.Count()
                                    };
                var dataCountBusiness = (from d in _repo._context.Districts
                                         where !d.IsDel
                                         join item in countBusiness on d.DistrictId equals item.DistrictId into groupData
                                         from item in groupData.DefaultIfEmpty()
                                         select new
                                         {
                                             DistrictId = d.DistrictId,
                                             DistrictName = d.DistrictName,
                                             Count = item.count == null ? 0 : item.count
                                         }).OrderBy(x => x.DistrictName);
                var _dataCountBusiness = dataCountBusiness.ToList();
                if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                {
                    var districtidfilter = query.Filter["DistrictId"];
                    dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.DistrictId.ToString() == districtidfilter);

                }
                else
                {
                    dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.DistrictId == _dataCountBusiness[0].DistrictId);
                }

                if (query.Filter != null && query.Filter.ContainsKey("BusinessId"))
                {
                    var BusinessId = query.Filter["BusinessId"];
                    dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.MaDoanhNghiep.ToString() == BusinessId);

                }

                return new { dataCateReportSoldAncol, dataCountBusiness };
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        // Thống kê tình hình kinh doanh rượu công nghiệp
        [Route("StatisticalIndustrialAlcoholBusiness")]
        [HttpPost]
        public object StatisticalIndustrialAlcoholBusiness([FromBody] QueryStatisticalBody query)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                var dataCateReportSoldAncol = from cral in _repo._context.CateReportTurnOverIndustAncols
                                              where !cral.IsDel
                                              group cral by new { cral.BusinessId, cral.Year } into g
                                              select new
                                              {
                                                  MaDoanhNghiep = g.Key.BusinessId,
                                                  Nam = g.Key.Year,
                                                  SoLuongMua = g.Select(s => s.QuantityBoughtOfYear).Sum(),
                                                  GiaTriMua = g.Select(s => s.TotalPriceBoughtOfYear).Sum(),
                                                  SoLuongBan = g.Select(s => s.QuantitySoldOfYear).Sum(),
                                                  GiaTriBan = g.Select(s => s.TotalPriceSoldOfYear).Sum(),
                                              } into groupData
                                              join b in _repo._context.Businesses on groupData.MaDoanhNghiep equals b.BusinessId
                                              select new
                                              {
                                                  MaDoanhNghiep = groupData.MaDoanhNghiep,
                                                  Nam = groupData.Nam,
                                                  SoLuongMua = groupData.SoLuongMua,
                                                  GiaTriMua = groupData.GiaTriMua,
                                                  SoLuongBan = groupData.SoLuongBan,
                                                  GiaTriBan = groupData.GiaTriBan,
                                                  TenDoanhNghiep = b.BusinessNameVi,
                                                  DiaChi = b.DiaChiTruSo,
                                                  DistrictId = b.DistrictId
                                              };

                if (query.Filter != null && query.Filter.ContainsKey("Year"))
                {
                    var year = query.Filter["Year"];
                    dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.Nam.ToString() == year);
                }

                var countBusiness = from item in dataCateReportSoldAncol
                                    group item by item.DistrictId into n
                                    select new
                                    {
                                        DistrictId = n.Key,
                                        count = n.Count()
                                    };
                var dataCountBusiness = (from d in _repo._context.Districts
                                         where !d.IsDel
                                         join item in countBusiness on d.DistrictId equals item.DistrictId into groupData
                                         from item in groupData.DefaultIfEmpty()
                                         select new
                                         {
                                             DistrictId = d.DistrictId,
                                             DistrictName = d.DistrictName,
                                             Count = item.count == null ? 0 : item.count
                                         }).OrderBy(x => x.DistrictName);

                var _dataCountBusiness = dataCountBusiness.ToList();


                if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                {
                    var districtidfilter = query.Filter["DistrictId"];
                    dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.DistrictId.ToString() == districtidfilter).OrderBy(x => x.TenDoanhNghiep);

                }
                else
                {
                    dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.DistrictId == _dataCountBusiness[0].DistrictId);
                }

                if (query.Filter != null && query.Filter.ContainsKey("BusinessId"))
                {
                    var BusinessId = query.Filter["BusinessId"];
                    dataCateReportSoldAncol = dataCateReportSoldAncol.Where(x => x.MaDoanhNghiep.ToString() == BusinessId).OrderBy(x => x.TenDoanhNghiep);

                }
                //bool _orderBy_ASC = true;
                //Func<object, object> _orderByExpression = x => x.ThoiGianToChuc;

                //if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                //{
                //    dataCateReportSoldAncol = dataCateReportSoldAncol
                //        .OrderBy(_orderByExpression)

                //}
                //else //Sắp xếp dữ liệu theo desc
                //{
                //    dataCateReportSoldAncol = dataCateReportSoldAncol
                //        .OrderByDescending(_orderByExpression)

                //}

                return new { dataCateReportSoldAncol, dataCountBusiness };
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [Route("StatisticalFairParticipant")]
        [HttpPost]
        public IActionResult StatisticalFairParticipant([FromBody] QueryStatisticalBody query)
        {
            BaseModels<StatisticalFairParticipantModel> model = new BaseModels<StatisticalFairParticipantModel>();
            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                Func<StatisticalFairParticipantModel, object> _orderByExpression = x => x.ThoiGianToChuc; //Khởi tạo mặc định sắp xếp dữ liệu
                IQueryable<StatisticalFairParticipantModel> _data = _repo._context.ParticipateSupportFairs.Where(x => !x.IsDel).GroupJoin(
                    _repo._context.Users,
                    cc => cc.CreateUserId,
                    u => u.UserId,
                     (cc, u) => new { cc, u }).SelectMany(result => result.u.DefaultIfEmpty(), (info, us) => new { info, us }).GroupJoin(
                    _repo._context.Countries,
                    query => query.info.cc.Country,
                    coun => coun.CountryId,
                    (query, coun) => new { query, coun }).SelectMany(rs => rs.coun.DefaultIfEmpty(), (info1, coun) => new StatisticalFairParticipantModel
                    {
                        ParticipateSupportFairId = info1.query.info.cc.ParticipateSupportFairId,
                        TenChuongTrinh = info1.query.info.cc.ParticipateSupportFairName,
                        DistrictId = info1.query.info.cc.DistrictId,
                        QuyMo = info1.query.info.cc.Scale,
                        Huyen = _repo._context.Districts.Where(x => !x.IsDel && x.DistrictId == info1.query.info.cc.DistrictId).Select(x => x.DistrictName).FirstOrDefault(),
                        DuKienKetThuc = string.Format("{0:dd/MM/yyyy}", info1.query.info.cc.EndTime),
                        DuKienToChuc = info1.query.info.cc.StartTime.ToString("dd/MM/yyyy"),
                        ThoiGianToChuc = info1.query.info.cc.StartTime,
                        ThoiGianKetThuc = info1.query.info.cc.EndTime,
                        SoLuongDoanhNghiep = _repo._context.ParticipateSupportFairDetails.Where(x => x.ParticipateSupportFairId == info1.query.info.cc.ParticipateSupportFairId).Count()
                    });

                if (query.Filter != null && query.Filter.ContainsKey("DistrictId"))
                {
                    var districtidfilter = query.Filter["DistrictId"];
                    _data = _data.Where(x => x.DistrictId.ToString() == districtidfilter);

                }

                if (query.Filter != null && query.Filter.ContainsKey("TimeStart"))
                {
                    var timeStart = query.Filter["TimeStart"];
                    _data = _data.Where(x => x.ThoiGianToChuc >= DateTime.ParseExact(timeStart, "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("TimeEnd"))
                {
                    var timeEnd = query.Filter["TimeEnd"];
                    _data = _data.Where(x => x.ThoiGianToChuc <= DateTime.ParseExact(timeEnd, "dd/MM/yyyy", null));
                }

                //bool _orderBy_ASC = true;
                //if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                //{
                //    model.items = _data
                //        .OrderBy(_orderByExpression)
                //        .ToList();
                //}
                //else //Sắp xếp dữ liệu theo desc
                //{
                //    model.items = _data
                //        .OrderByDescending(_orderByExpression)
                //        .ToList();
                //}

                model.items = _data.OrderBy(_orderByExpression).ToList();
                int _countRows = _data.Count(); //Đếm số dòng của table đã select được

                //  model.items = _data.ToList();

                model.status = 1;
                model.total = _countRows;
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [HttpPost("ExportStatisticalFairParticipant")]
        public IActionResult ExportStatisticalFairParticipant([FromBody] QueryStatisticalBody query)
        {
            //Query data
            var _data = StatisticalFairParticipant(query);
            var value = (dynamic)_data;
            var data = value.Value;

            if (data.total == 0)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Thongkedanhsachthamgiahoicho.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 3;
                    int row = 1;

                    worksheet.Cell(1, 1).Value = "THỐNG KÊ DANH SÁCH THAM GIA HỘI CHỢ";

                    //Thêm dữ liệu vào file:
                    int tongQuyMo = 0;
                    int tongDoanhNghiep = 0;
                    foreach (var item in (dynamic)data.items)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenChuongTrinh;
                            worksheet.Cell(index, 3).Value = item.Huyen;
                            worksheet.Cell(index, 4).Value = item.DuKienToChuc;
                            worksheet.Cell(index, 5).Value = item.DuKienKetThuc;
                            worksheet.Cell(index, 6).Value = item.QuyMo;
                            worksheet.Cell(index, 7).Value = item.SoLuongDoanhNghiep;
                            index++;
                            row++;
                            tongQuyMo += Int32.Parse(item.QuyMo);
                            tongDoanhNghiep += item.SoLuongDoanhNghiep;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.TenChuongTrinh;
                            worksheet.Cell(index, 3).Value = item.Huyen;
                            worksheet.Cell(index, 4).Value = item.DuKienToChuc;
                            worksheet.Cell(index, 5).Value = item.DuKienKetThuc;
                            worksheet.Cell(index, 6).Value = item.QuyMo;
                            worksheet.Cell(index, 7).Value = item.SoLuongDoanhNghiep;
                            index++;
                            row++;
                            tongQuyMo += Int32.Parse(item.QuyMo);
                            tongDoanhNghiep += item.SoLuongDoanhNghiep;
                        }
                    }
                    worksheet.Cell(++index, 2).Value = data.total;
                    worksheet.Cell(index, 6).Value = tongQuyMo;
                    worksheet.Cell(index, 7).Value = tongDoanhNghiep;
                    var delrow = worksheet.Row(--index);
                    delrow.Delete();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Thống kê tình hình sản xuất Rượu công nghiệp
        [Route("StatisticalIndustAlcol")]
        [HttpGet]
        public object StatisticalIndustAlcol()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var _dataDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName,
                }).OrderBy(x => x.DistrictName).ToList();

                var data = (from p in _repo._context.CateReportProduceIndustlAncols
                            join b in _repo._context.Businesses on p.BusinessId equals b.BusinessId
                            join d in _repo._context.Districts on b.DistrictId equals d.DistrictId
                            where !p.IsDel && !b.IsDel && !d.IsDel
                            select new CateReportProduceIndustlAncolModel
                            {
                                BusinessId = p.BusinessId,
                                DistrictId = d.DistrictId,
                            }).ToList();

                var _countBusiness = data.GroupBy(x => x.DistrictId).Select(g => new
                {
                    DistrictId = g.Key,
                    count = g.Select(x => x.BusinessId).Count()
                }).ToList();

                var result = new List<StatisticalIndusAlcolDetailModel>();
                int total = 0;
                for (int i = 0; i < _dataDistrict.Count(); i++)
                {
                    var item = new StatisticalIndusAlcolDetailModel();
                    for (int j = 0; j < _countBusiness.Count(); j++)
                    {
                        if (_dataDistrict[i].DistrictId == _countBusiness[j].DistrictId)
                        {
                            item.TongDoanhNghiep = _countBusiness[j].count;
                            total += item.TongDoanhNghiep;
                            break;
                        }
                    }
                    item.DistrictId = _dataDistrict[i].DistrictId;
                    item.DistrictName = _dataDistrict[i].DistrictName;

                    result.Add(item);
                }
                var final = new { Details = result, total };

                return final;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }


        [Route("StatisticalIndusAlcolById/{id}")]
        [HttpGet]

        public object StatisticalIndusAlcolById(Guid id)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var _dataDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName,
                }).ToList();

                var _data = from cb in _repo._context.CateReportProduceIndustlAncols
                            join b in _repo._context.Businesses on cb.BusinessId equals b.BusinessId
                            join d in _repo._context.Districts on b.DistrictId equals d.DistrictId
                            where !cb.IsDel && !b.IsDel && d.DistrictId == id
                            select new
                            {
                                BusinessId = cb.BusinessId,
                                TenDoanhNghiep = b.BusinessNameVi,
                                SoDienThoai = b.SoDienThoai,
                                NguoiDaiDien = b.NguoiDaiDien,
                                ChungLoai = cb.TypeofWine,
                                DistrictId = b.DistrictId,
                                SanLuongSanXuat = cb.QuantityProduction,
                                SanLuongTieuThu = cb.QuantityConsume,
                                CongXuatThietKe = cb.DesignCapacity,
                                VonDauTu = cb.Investment
                            };

                var data = _data.ToList();
                return data;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }

        [Route("StatisticalProductAlcol")]
        [HttpGet]
        public object StatisticalProductAlcol()
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var _dataDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName,
                }).OrderBy(x => x.DistrictName).ToList();

                var data = (from p in _repo._context.CateReportProduceCrafttAncolForEconomics
                            join b in _repo._context.Businesses on p.BusinessId equals b.BusinessId
                            join d in _repo._context.Districts on b.DistrictId equals d.DistrictId
                            where !p.IsDel && !b.IsDel && !d.IsDel
                            select new CateReportProduceCrafttAncolForEconomicModel
                            {
                                CateReportProduceCrafttAncolForEconomicId = p.CateReportProduceCrafttAncolForEconomicId,

                                AlcoholBusinessName = b.BusinessNameVi,
                                PhoneNumber = b.SoDienThoai,
                                Address = b.DiaChiTruSo,
                                LicenseCode = b.GiayPhepSanXuat ?? "",
                                LicenseDateDisplay = b.NgayCapPhep.HasValue ? b.NgayCapPhep.Value.ToString("dd'/'MM'/'yyyy") : "",
                                Representative = b.NguoiDaiDien,
                                DistrictName = d.DistrictName,
                                DistrictId = d.DistrictId,

                                BusinessId = p.BusinessId,
                                Quantity = p.Quantity,
                                QuantityConsume = p.QuantityConsume,
                                TypeofWine = p.TypeofWine,
                                YearReport = p.YearReport,
                            }).ToList();

                var _countBusiness = data.GroupBy(x => x.DistrictId).Select(g => new
                {
                    DistrictId = g.Key,
                    count = g.Where(x => x.BusinessId != Guid.Empty).Select(x => x.BusinessId).Count()
                }).ToList();

                var result = new List<StatisticalProductAlcolDetailModel>();
                int total = 0;
                for (int i = 0; i < _dataDistrict.Count(); i++)
                {
                    var item = new StatisticalProductAlcolDetailModel();
                    for (int j = 0; j < _countBusiness.Count(); j++)
                    {
                        if (_dataDistrict[i].DistrictId == _countBusiness[j].DistrictId)
                        {
                            item.TongDoanhNghiep = _countBusiness[j].count;
                            total += item.TongDoanhNghiep;
                            break;
                        }
                    }
                    item.DistrictId = _dataDistrict[i].DistrictId;
                    item.DistrictName = _dataDistrict[i].DistrictName;

                    result.Add(item);
                }
                var final = new { Details = result, total };

                return final;
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }


        [Route("StatisticalProductAlcolById/{id}")]
        [HttpGet]

        public object StatisticalProductAlcolById(Guid id)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                var _dataDistrict = _repo._context.Districts.Where(x => !x.IsDel).Select(d => new District()
                {
                    DistrictId = d.DistrictId,
                    DistrictName = d.DistrictName,
                }).ToList();

                var _dataProductAlcol = from cb in _repo._context.CateReportProduceCrafttAncolForEconomics
                                        join b in _repo._context.Businesses on cb.BusinessId equals b.BusinessId
                                        join d in _repo._context.Districts on b.DistrictId equals d.DistrictId
                                        where !cb.IsDel && !b.IsDel && !d.IsDel && d.DistrictId == id
                                        select new
                                        {
                                            BusinessId = cb.BusinessId,
                                            TenDoanhNghiep = b.BusinessNameVi,
                                            SoDienThoai = b.SoDienThoai,
                                            NguoiDaiDien = b.NguoiDaiDien,
                                            ChungLoai = cb.TypeofWine,
                                            SanLuongSanXuat = cb.Quantity,
                                            SanLuongTieuThu = cb.QuantityConsume,
                                        };

                var data = _dataProductAlcol.ToList();

                return data;


            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }
    }
}
