using API_SoCongThuong.Models;
using ClosedXML.Excel;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.Collections.Generic;

namespace API_SoCongThuong.Reponsitories.AdminFormalitiesRepository
{
    public class AdminFormalitiesRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public AdminFormalitiesRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(AdministrativeFormality model)
        {
            await _context.AdministrativeFormalities.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(AdministrativeFormality model)
        {
            _context.AdministrativeFormalities.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAdminFormalities(Guid Id)
        {
            var db = await _context.AdministrativeFormalities.Where(d => d.AdminFormalitiesId == Id).FirstOrDefaultAsync();
            if (db != null)
            {
                db.IsDel = true;
                _context.AdministrativeFormalities.Update(db);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Delete(Guid id)
        {
            var itemRemove = await _context.AdministrativeFormalities.Where(x => x.AdminFormalitiesId == id).FirstOrDefaultAsync();
            _context.AdministrativeFormalities.Remove(itemRemove);
            await _context.SaveChangesAsync();
        }

        public IQueryable<AdministrativeFormality> FindAll()
        {
            var result = _context.AdministrativeFormalities.Select(d => new AdministrativeFormality()
            {
                AdminFormalitiesId = d.AdminFormalitiesId,
                AdminFormalitiesCode = d.AdminFormalitiesCode,
                AdminFormalitiesName = d.AdminFormalitiesName,
                FieldId = d.FieldId,
                Dvclevel = d.Dvclevel,
                DocUrl = d.DocUrl,
                IsDel = d.IsDel,
            });

            return result;
        }

        public IQueryable<AdministrativeFormality> FindById(Guid Id)
        {
            var result = _context.AdministrativeFormalities.Where(x => x.AdminFormalitiesId == Id).Select(d => new AdministrativeFormality()
            {
                AdminFormalitiesId = d.AdminFormalitiesId,
                AdminFormalitiesCode = d.AdminFormalitiesCode,
                AdminFormalitiesName = d.AdminFormalitiesName,
                FieldId = d.FieldId,
                Dvclevel = d.Dvclevel,
                DocUrl = d.DocUrl,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByAdminFormalityCode(string adminFormalityCode, Guid? adminFormalityId)
        {
            if (adminFormalityId != null)
            {
                var AdminFormalityCode = _context.AdministrativeFormalities.Where(x => x.AdminFormalitiesId == adminFormalityId && x.AdminFormalitiesCode == adminFormalityCode && !x.IsDel).FirstOrDefault();
                if (AdminFormalityCode != null)
                {
                    return false;
                }
            }
            var isAdminFormalityCode = _context.AdministrativeFormalities.Where(x => x.AdminFormalitiesCode == adminFormalityCode && !x.IsDel).FirstOrDefault();
            if (isAdminFormalityCode == null)
            {
                return false;
            }
            return true;
        }

        public async Task ConvertHtmlToPdf()
        {
            var htmlFilePath = @"Upload/Templates/GCN_DDK_ATHC.html";
            var pdfFilePath = @"Upload/Templates/test.pdf";

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
            });

            using var page = await browser.NewPageAsync();
            var html = File.ReadAllText(htmlFilePath);
            await page.SetContentAsync(html);
            await page.PdfAsync(pdfFilePath, new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                PreferCSSPageSize = true,
            });
        }

        //public async Task ImportData(IFormFile File)
        //{
        //    List<AdministrativeFormality> AddList = new List<AdministrativeFormality>();
        //    using (var workbook = new XLWorkbook(File.OpenReadStream()))
        //    {
        //        IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
        //        for (var i = 2; i < 126; i++)
        //        {
        //            AdministrativeFormality Item = new AdministrativeFormality();
        //            Item.AdminFormalitiesCode = worksheet.Cell(i, 1).Value.ToString();
        //            Item.AdminFormalitiesName = worksheet.Cell(i, 5).Value.ToString();
        //            Item.FieldId = new Guid(worksheet.Cell(i, 3).Value.ToString());
        //            Item.Dvclevel = worksheet.Cell(i, 2).Value.GetNumber() == 1 ? 1 : 2;
        //            Item.DocUrl = worksheet.Cell(i, 6).Value.ToString();
        //            Item.CreateUserId = new Guid("bca51f82-8300-424e-a72f-3a0810fd8555");
        //            AddList.Add(Item);
        //        }
        //    }
        //    await _context.AdministrativeFormalities.AddRangeAsync(AddList);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task ImportData(IFormFile File)
        //{
        //    var allCommune = _context.Communes.Where(x => !x.IsDel).ToList();
        //    List<Target1708> AddDataStage1 = new List<Target1708>();
        //    List<Target1708> AddDataStage2 = new List<Target1708>();
        //    foreach (var commune in allCommune)
        //    {
        //        Target1708 stage1 = new Target1708()
        //        {
        //            StageId = Guid.Parse("D0BF3F28-FFB6-4F09-9B9F-23FE7C063EC9"),
        //            DistrictId = commune.DistrictId,
        //            CommuneId = commune.CommuneId,
        //            NewRuralCriteria = false,
        //            NewRuralCriteriaRaised = false,
        //            Note = "",
        //            CreateUserId = Guid.Parse("bca51f82-8300-424e-a72f-3a0810fd8555"),
        //            CreateTime = DateTime.Now,
        //        };
        //        AddDataStage1.Add(stage1);

        //        Target1708 stage2 = new Target1708()
        //        {
        //            StageId = Guid.Parse("EBBAC895-2D88-425F-846B-3A796A7A2713"),
        //            DistrictId = commune.DistrictId,
        //            CommuneId = commune.CommuneId,
        //            NewRuralCriteria = false,
        //            NewRuralCriteriaRaised = false,
        //            Note = "",
        //            CreateUserId = Guid.Parse("bca51f82-8300-424e-a72f-3a0810fd8555"),
        //            CreateTime = DateTime.Now,
        //        };
        //        AddDataStage2.Add(stage2);
        //    }
        //    var a = 1;
        //    await _context.Target1708s.AddRangeAsync(AddDataStage1);
        //    await _context.Target1708s.AddRangeAsync(AddDataStage2);
        //    await _context.SaveChangesAsync();

        //    //List<CommuneModel> ListCommuneNew = new List<CommuneModel>();
        //    //using (var workbook = new XLWorkbook(File.OpenReadStream()))
        //    //{
        //    //    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
        //    //    for (var i = 2; i < 159; i++)
        //    //    {
        //    //        CommuneModel Item = new CommuneModel();
        //    //        Item.CommuneCode = worksheet.Cell(i, 4).Value.GetText();
        //    //        Item.CommuneName = worksheet.Cell(i, 3).Value.GetText();
        //    //        Item.DistrictId = Guid.Parse(worksheet.Cell(i, 2).Value.GetText());
        //    //        Item.DistrictName = worksheet.Cell(i, 1).Value.GetText();
        //    //        ListCommuneNew.Add(Item);
        //    //    }

        //    //    List<Commune> NewData = new List<Commune>();
        //    //    List<Commune> AllCommune = await _context.Communes.Where(x => !x.IsDel).ToListAsync();
        //    //    var GroupCommune = AllCommune.GroupBy(x => x.DistrictId);

        //    //    foreach (var District in GroupCommune)
        //    //    {
        //    //        var CommuneOfDistrict = ListCommuneNew.Where(x => x.DistrictId == District.Key).ToList();
        //    //        int index = 0;
        //    //        foreach (var Commune in District)
        //    //        {
        //    //            Commune.CommuneCode = CommuneOfDistrict[index].CommuneCode;
        //    //            Commune.CommuneName = CommuneOfDistrict[index].CommuneName;
        //    //            NewData.Add(Commune);
        //    //            index++;
        //    //        }
        //    //    }
        //    //    _context.Communes.UpdateRange(NewData);
        //    //    await _context.SaveChangesAsync();
        //    //}
        //}

        //public class NganhNgheModel
        //{
        //    public string Code { get; set; } = null!;
        //    public string Name { get; set; } = null!;
        //    public string? ParentCode { get; set; }
        //}

        //Import ngành nghề
        //public async Task ImportData(IFormFile File)
        //{
        //    using (var workbook = new XLWorkbook(File.OpenReadStream()))
        //    {
        //        IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
        //        List<NganhNgheModel> Level1 = new List<NganhNgheModel>();
        //        List<NganhNgheModel> Level2 = new List<NganhNgheModel>();
        //        List<NganhNgheModel> Level3 = new List<NganhNgheModel>();
        //        List<NganhNgheModel> Level4 = new List<NganhNgheModel>();
        //        List<NganhNgheModel> Level5 = new List<NganhNgheModel>();
        //        string Parent2 = "";
        //        string Parent3 = "";
        //        string Parent4 = "";
        //        string Parent5 = "";

        //        for (var i = 2; i < 935; i++)
        //        {
        //            var row = worksheet.Row(i);
        //            string c1 = row.Cell(1).Value.ToString();
        //            string c2 = row.Cell(2).Value.ToString();
        //            string c3 = row.Cell(3).Value.ToString();
        //            string c4 = row.Cell(4).Value.ToString();
        //            string c5 = row.Cell(5).Value.ToString();

        //            if (!string.IsNullOrEmpty(c1))
        //            {
        //                NganhNgheModel ItemLV1 = new NganhNgheModel()
        //                {
        //                    Code = c1,
        //                    Name = row.Cell(6).Value.ToString(),
        //                    ParentCode = null,
        //                };
        //                Level1.Add(ItemLV1);

        //                Parent2 = c1;
        //                Parent3 = "";
        //                Parent4 = "";
        //                Parent5 = "";
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(c2) && !string.IsNullOrEmpty(c3) && !string.IsNullOrEmpty(c4) && !string.IsNullOrEmpty(c5))
        //                {
        //                    NganhNgheModel ItemLV2 = new NganhNgheModel()
        //                    {
        //                        Code = c2,
        //                        Name = row.Cell(6).Value.ToString(),
        //                        ParentCode = Parent2,
        //                    };
        //                    Level2.Add(ItemLV2);
        //                    Parent3 = c2;

        //                    NganhNgheModel ItemLV3 = new NganhNgheModel()
        //                    {
        //                        Code = c3,
        //                        Name = row.Cell(6).Value.ToString(),
        //                        ParentCode = Parent3,
        //                    };
        //                    Level3.Add(ItemLV3);
        //                    Parent4 = c3;

        //                    NganhNgheModel ItemLV4 = new NganhNgheModel()
        //                    {
        //                        Code = c4,
        //                        Name = row.Cell(6).Value.ToString(),
        //                        ParentCode = Parent4,
        //                    };
        //                    Level4.Add(ItemLV4);
        //                    Parent5 = c4;

        //                    NganhNgheModel ItemLV5 = new NganhNgheModel()
        //                    {
        //                        Code = c5,
        //                        Name = row.Cell(6).Value.ToString(),
        //                        ParentCode = Parent5,
        //                    };
        //                    Level5.Add(ItemLV5);
        //                }

        //                if (!string.IsNullOrEmpty(c2))
        //                {
        //                    if (string.IsNullOrEmpty(c3))
        //                    {
        //                        NganhNgheModel ItemLV2 = new NganhNgheModel()
        //                        {
        //                            Code = c2,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent2,
        //                        };
        //                        Level2.Add(ItemLV2);
        //                        Parent3 = c2;
        //                    }

        //                    if (string.IsNullOrEmpty(c4))
        //                    {
        //                        NganhNgheModel ItemLV2 = new NganhNgheModel()
        //                        {
        //                            Code = c2,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent2,
        //                        };
        //                        Level2.Add(ItemLV2);
        //                        Parent3 = c2;

        //                        NganhNgheModel ItemLV3 = new NganhNgheModel()
        //                        {
        //                            Code = c3,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent3,
        //                        };
        //                        Level3.Add(ItemLV3);
        //                        Parent4 = c3;
        //                    }

        //                    if (string.IsNullOrEmpty(c5))
        //                    {
        //                        NganhNgheModel ItemLV2 = new NganhNgheModel()
        //                        {
        //                            Code = c2,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent2,
        //                        };
        //                        Level2.Add(ItemLV2);
        //                        Parent3 = c2;

        //                        NganhNgheModel ItemLV3 = new NganhNgheModel()
        //                        {
        //                            Code = c3,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent3,
        //                        };
        //                        Level3.Add(ItemLV3);
        //                        Parent4 = c3;

        //                        NganhNgheModel ItemLV4 = new NganhNgheModel()
        //                        {
        //                            Code = c4,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent4,
        //                        };
        //                        Level4.Add(ItemLV4);
        //                        Parent5 = c4;
        //                    }
        //                }

        //                if (!string.IsNullOrEmpty(c3))
        //                {
        //                    if (string.IsNullOrEmpty(c4))
        //                    {
        //                        NganhNgheModel ItemLV3 = new NganhNgheModel()
        //                        {
        //                            Code = c3,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent3,
        //                        };
        //                        Level3.Add(ItemLV3);
        //                        Parent4 = c3;
        //                    }

        //                    if (string.IsNullOrEmpty(c5))
        //                    {
        //                        NganhNgheModel ItemLV3 = new NganhNgheModel()
        //                        {
        //                            Code = c3,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent3,
        //                        };
        //                        Level3.Add(ItemLV3);
        //                        Parent4 = c3;

        //                        NganhNgheModel ItemLV4 = new NganhNgheModel()
        //                        {
        //                            Code = c4,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent4,
        //                        };
        //                        Level4.Add(ItemLV4);
        //                        Parent5 = c4;
        //                    }
        //                    else
        //                    {
        //                        NganhNgheModel ItemLV3 = new NganhNgheModel()
        //                        {
        //                            Code = c3,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent3,
        //                        };
        //                        Level3.Add(ItemLV3);
        //                        Parent4 = c3;

        //                        NganhNgheModel ItemLV4 = new NganhNgheModel()
        //                        {
        //                            Code = c4,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent4,
        //                        };
        //                        Level4.Add(ItemLV4);
        //                        Parent5 = c4;

        //                        NganhNgheModel ItemLV5 = new NganhNgheModel()
        //                        {
        //                            Code = c5,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent5,
        //                        };
        //                        Level5.Add(ItemLV5);
        //                    }
        //                }

        //                if (!string.IsNullOrEmpty(c4))
        //                {
        //                    if (string.IsNullOrEmpty(c5))
        //                    {
        //                        NganhNgheModel ItemLV4 = new NganhNgheModel()
        //                        {
        //                            Code = c4,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent4,
        //                        };
        //                        Level4.Add(ItemLV4);
        //                        Parent5 = c4;
        //                    }
        //                    else
        //                    {
        //                        NganhNgheModel ItemLV4 = new NganhNgheModel()
        //                        {
        //                            Code = c4,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent4,
        //                        };
        //                        Level4.Add(ItemLV4);
        //                        Parent5 = c4;

        //                        NganhNgheModel ItemLV5 = new NganhNgheModel()
        //                        {
        //                            Code = c5,
        //                            Name = row.Cell(6).Value.ToString(),
        //                            ParentCode = Parent5,
        //                        };
        //                        Level5.Add(ItemLV5);
        //                    }
        //                }

        //                if (!string.IsNullOrEmpty(c5))
        //                {
        //                    NganhNgheModel ItemLV5 = new NganhNgheModel()
        //                    {
        //                        Code = c5,
        //                        Name = row.Cell(6).Value.ToString(),
        //                        ParentCode = Parent5,
        //                    };
        //                    Level5.Add(ItemLV5);
        //                }
        //            }
        //        }

        //        Level2 = Level2.DistinctBy(x => x.Code).ToList();
        //        Level3 = Level3.DistinctBy(x => x.Code).Where(x => !string.IsNullOrEmpty(x.Code)).ToList();
        //        Level4 = Level4.DistinctBy(x => x.Code).Where(x => !string.IsNullOrEmpty(x.Code)).ToList();
        //        Level5 = Level5.DistinctBy(x => x.Code).ToList();



        //        var addData = new List<TypeOfProfession>();
        //        //foreach (var i in Level1)
        //        //{
        //        //    Industry item = new Industry()
        //        //    {
        //        //        IndustryCode = i.Code,
        //        //        IndustryName = i.Name,
        //        //        ParentIndustryId = Guid.Empty,
        //        //        CreateUserId = Guid.Parse("bca51f82-8300-424e-a72f-3a0810fd8555"),
        //        //        CreateTime = DateTime.Now,
        //        //    };
        //        //    addData.Add(item);
        //        //}

        //        //var Lv2Data = (from L2 in Level2
        //        //               join L1 in _context.Industries
        //        //                 on L2.ParentCode equals L1.IndustryCode
        //        //               select new
        //        //               {
        //        //                   Code = L2.Code,
        //        //                   Name = L2.Name,
        //        //                   Parent = L1.IndustryId
        //        //               }).ToList();



        //        //var Lv3Data = (from L3 in Level3
        //        //               join L1 in _context.Industries
        //        //                 on L3.ParentCode equals L1.IndustryCode
        //        //               select new
        //        //               {
        //        //                   Code = L3.Code,
        //        //                   Name = L3.Name,
        //        //                   Parent = L1.IndustryId
        //        //               }).ToList();

        //        var lstData = _context.Industries.ToList();
        //        var AllData = _context.Industries.Select(x => new IndustryModel
        //        {
        //            IndustryId = x.IndustryId,
        //            IndustryCode = x.IndustryCode,
        //            IndustryName = x.IndustryName,
        //            ParentIndustryId = x.ParentIndustryId ?? Guid.Empty,
        //            IndustryLevel = 0
        //        }).ToList();

        //        for (int i = 0; i < AllData.Count(); i++)
        //        {
        //            AllData[i].IndustryLevel = countLevel(AllData[i].IndustryId, lstData, 0);
        //        }

        //        var LV2Data = AllData.Where(x => x.IndustryLevel == 1).ToList();

        //        //var Lv4Data = (from L5 in Level5
        //        //               join L4 in LV4Data
        //        //                 on L5.ParentCode equals L4.IndustryCode
        //        //               select new
        //        //               {
        //        //                   Code = L5.Code,
        //        //                   Name = L5.Name,
        //        //                   Parent = L4.IndustryId
        //        //               }).ToList();

        //        foreach (var i in LV2Data)
        //        {
        //            TypeOfProfession item = new TypeOfProfession()
        //            {
        //                TypeOfProfessionCode = i.IndustryCode.Trim(),
        //                TypeOfProfessionName = i.IndustryName.Trim(),
        //                CreateUserId = Guid.Parse("bca51f82-8300-424e-a72f-3a0810fd8555"),
        //                CreateTime = DateTime.Now,
        //            };
        //            addData.Add(item);
        //        }
        //        await _context.TypeOfProfessions.AddRangeAsync(addData);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        //private int countLevel(Guid id, List<Industry> lstData, int curLevel)
        //{
        //    if (lstData != null && lstData.Count() > 0)
        //    {
        //        var data = lstData.Where(x => x.IndustryId == id).FirstOrDefault();
        //        if (data == null)
        //        {
        //            return 0;
        //        }
        //        else
        //        {
        //            if (data.ParentIndustryId != null && data.ParentIndustryId.HasValue) // && data.ParentIndustryId != Guid.Empty
        //            {
        //                int nextLevel = curLevel + 1;
        //                return countLevel(data.ParentIndustryId.Value, lstData, nextLevel);
        //            }
        //            else
        //            {
        //                return curLevel;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
    }
}
