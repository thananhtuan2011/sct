using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ManagementElectricityActivitiesMonthReportRepo
{
    public class ManagementElectricityActivitiesMonthReportRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ManagementElectricityActivitiesMonthReportRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ManagementElectricityActivitiesMonthReportModel model)
        {
            MeaMonthReport SaveData = new MeaMonthReport();
            SaveData.UpdateDate = DateTime.ParseExact(model.UpdateDate, "dd'/'MM'/'yyyy", null);
            SaveData.Month = model.Month;
            SaveData.Year = model.Year;
            SaveData.CreateUserId = (Guid)model.CreateUserId!;
            SaveData.CreateTime = (DateTime)model.CreateTime!;
            await _context.MeaMonthReports.AddAsync(SaveData);
            await _context.SaveChangesAsync();

            if (model.AllFile!.Any())
            {
                List<MeaMonthReportAttachFile> Details = new List<MeaMonthReportAttachFile>();
                foreach (var item in model.AllFile!)
                {
                    MeaMonthReportAttachFile detail = new MeaMonthReportAttachFile()
                    {
                        MonthReportId = SaveData.ReportMonthId,
                        LinkFile = item.LinkFile!,
                    };
                    Details.Add(detail);
                }
                await _context.MeaMonthReportAttachFiles.AddRangeAsync(Details);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Update(MeaMonthReport SaveData, ManagementElectricityActivitiesMonthReportModel model, IConfiguration config)
        {
            SaveData.UpdateTime = DateTime.ParseExact(model.UpdateDate, "dd'/'MM'/'yyyy", null);
            SaveData.Month = model.Month;
            SaveData.Year = model.Year;
            SaveData.CreateUserId = (Guid)model.CreateUserId!;
            SaveData.CreateTime = (DateTime)model.CreateTime!;

            _context.MeaMonthReports.Update(SaveData);

            if (model.DelFileIds.Length > 0)
            {
                var DelIds = model.DelFileIds.Split(',');
                if (DelIds.Length > 0)
                {
                    var DelItems = _context.MeaMonthReportAttachFiles.Where(x => DelIds.Contains(x.FileId.ToString())).ToList();
                    if (DelItems.Any())
                    {
                        var LDelLink = DelItems.Select(x => x.LinkFile).ToList();
                        foreach (var l in LDelLink)
                        {
                            var result = Ulities.RemoveFileMinio(l, config);
                        }
                        _context.MeaMonthReportAttachFiles.RemoveRange(DelItems);
                    }
                }
            }

            if (model.AllFile!.Any())
            {
                List<MeaMonthReportAttachFile> Details = new List<MeaMonthReportAttachFile>();
                foreach (var item in model.AllFile!)
                {
                    MeaMonthReportAttachFile detail = new MeaMonthReportAttachFile()
                    {
                        MonthReportId = SaveData.ReportMonthId,
                        LinkFile = item.LinkFile!,
                    };
                    Details.Add(detail);
                }
                await _context.MeaMonthReportAttachFiles.AddRangeAsync(Details);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(MeaMonthReport model, IConfiguration config)
        {
            _context.MeaMonthReports.Update(model);
            var DelItems = _context.MeaMonthReportAttachFiles.Where(x => x.MonthReportId == model.ReportMonthId).ToList();
            if (DelItems.Any())
            {
                var LDelLink = DelItems.Select(x => x.LinkFile).ToList();
                foreach (var l in LDelLink)
                {
                    var result = Ulities.RemoveFileMinio(l, config);
                }
                _context.MeaMonthReportAttachFiles.RemoveRange(DelItems);
            }
            await _context.SaveChangesAsync();
        }

        public IQueryable<ManagementElectricityActivitiesMonthReportModel> FindById(Guid Id, IConfiguration config)
        {
            var result = _context.MeaMonthReports.Where(x => x.ReportMonthId == Id && !x.IsDel).Select(f => new ManagementElectricityActivitiesMonthReportModel()
            {
                MonthReportId = f.ReportMonthId,
                UpdateDate = f.UpdateDate.ToString("dd'/'MM'/'yyyy"),
                Month = f.Month,
                Year = f.Year,
                AllFile = _context.MeaMonthReportAttachFiles
                            .Where(x => x.MonthReportId.Equals(Id))
                            .Select(x => new MonthReportAttchFileModel()
                            {
                                FileId = x.FileId,
                                MonthReportId = x.MonthReportId,
                                LinkFile = string.IsNullOrEmpty(x.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + x.LinkFile,
                                CreateTime = x.CreateTime.ToString("dd'/'MM'/'yyyy")
                            }).ToList(),
            });

            return result;
        }
    }
}
