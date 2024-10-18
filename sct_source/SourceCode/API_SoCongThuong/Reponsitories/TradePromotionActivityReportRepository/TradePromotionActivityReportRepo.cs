using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class TradePromotionActivityReportRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TradePromotionActivityReportRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TradePromotionActivityReportModel model)
        {
            TradePromotionActivityReport data = new TradePromotionActivityReport()
            {
                TradePromotionActivityReportId = model.TradePromotionActivityReportId,
                ScaleId = model.ScaleId,
                PlanName = model.PlanName,
                PlanToJoin = model.PlanToJoin,
                StartDate = model.StartDate ?? DateTime.Now,
                EndDate = model.EndDate,
                Time = model.Time,
                DistrictId = model.DistrictId,
                Address = model.Address,
                ImplementationCost = model.ImplementationCost,
                FundingSupport = model.FundingSupport,
                Scale = model.Scale ?? "",
                NumParticipating = model.NumParticipating,
                Note = model.Note ?? "",
                CreateTime = model.CreateTime ?? DateTime.Now,
                CreateUserId = model.CreateUserId ?? Guid.Empty,
            };

            await _context.TradePromotionActivityReports.AddAsync(data);
            await _context.SaveChangesAsync();

            //Add Business
            if (model.ParticipatingBusinesses!.Any())
            {
                List<TradePromotionActivityReportParticipatingBusiness> addBusinesses = new List<TradePromotionActivityReportParticipatingBusiness>();
                foreach (var business in model.ParticipatingBusinesses!)
                {
                    TradePromotionActivityReportParticipatingBusiness item = new TradePromotionActivityReportParticipatingBusiness()
                    {
                        TradePromotionActivityReportId = data.TradePromotionActivityReportId,
                        BusinessId = business.BusinessId ?? Guid.Empty,
                        BusinessName = business.BusinessName,
                        Address = business.Address,
                    };
                    addBusinesses.Add(item);
                }
                await _context.TradePromotionActivityReportParticipatingBusinesses.AddRangeAsync(addBusinesses);
                await _context.SaveChangesAsync();
            }

            //Add File
            if (model.Files!.Any())
            {
                List<TradePromotionActivityReportAttachFile> addFiles = new List<TradePromotionActivityReportAttachFile>();
                foreach (var file in model.Files!)
                {
                    TradePromotionActivityReportAttachFile item = new TradePromotionActivityReportAttachFile()
                    {
                        TradePromotionActivityReportId = data.TradePromotionActivityReportId,
                        LinkFile = file.LinkFile,
                    };
                    addFiles.Add(item);
                }
                await _context.TradePromotionActivityReportAttachFiles.AddRangeAsync(addFiles);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(TradePromotionActivityReportModel model, IConfiguration config)
        {
            var info = await _context.TradePromotionActivityReports.Where(d => d.TradePromotionActivityReportId == model.TradePromotionActivityReportId).FirstOrDefaultAsync();
            if (info != null)
            {
                //Delete File
                if (!string.IsNullOrEmpty(model.LIdDel))
                {
                    List<string> LIds = model.LIdDel.Split(",").ToList();
                    var LFileDel = await _context.TradePromotionActivityReportAttachFiles.Where(x => LIds.Contains(x.TradePromotionActivityReportAttachFileId.ToString())).ToListAsync();
                    if (LFileDel.Any())
                    {
                        foreach (var fdel in LFileDel)
                        {
                            string linkdel = fdel.LinkFile;
                            var result = Ulities.RemoveFileMinio(linkdel, config);
                        }

                        _context.TradePromotionActivityReportAttachFiles.RemoveRange(LFileDel);
                        await _context.SaveChangesAsync();
                    }
                }

                //Delete old business
                List<TradePromotionActivityReportParticipatingBusiness> OldParticipatingBusiness = await _context.TradePromotionActivityReportParticipatingBusinesses
                    .Where(x => x.TradePromotionActivityReportId == model.TradePromotionActivityReportId).ToListAsync();
                if (OldParticipatingBusiness.Any())
                {
                    _context.TradePromotionActivityReportParticipatingBusinesses.RemoveRange(OldParticipatingBusiness);
                    await _context.SaveChangesAsync();
                }

                //Update Item
                info.ScaleId = model.ScaleId;
                info.PlanName = model.PlanName;
                info.PlanToJoin = model.PlanToJoin;
                info.StartDate = model.StartDate ?? DateTime.Now;
                info.EndDate = model.EndDate;
                info.Time = model.Time;
                info.DistrictId = model.DistrictId;
                info.Address = model.Address;
                info.ImplementationCost = model.ImplementationCost;
                info.FundingSupport = model.FundingSupport;
                info.Scale = model.Scale ?? "";
                info.NumParticipating = model.NumParticipating;
                info.Note = model.Note ?? "";
                info.UpdateTime = model.UpdateTime;
                info.UpdateUserId = model.UpdateUserId;

                _context.TradePromotionActivityReports.Update(info);
                await _context.SaveChangesAsync();

                //Add Business
                if (model.ParticipatingBusinesses!.Any())
                {
                    List<TradePromotionActivityReportParticipatingBusiness> addBusinesses = new List<TradePromotionActivityReportParticipatingBusiness>();
                    foreach (var business in model.ParticipatingBusinesses!)
                    {
                        TradePromotionActivityReportParticipatingBusiness item = new TradePromotionActivityReportParticipatingBusiness()
                        {
                            TradePromotionActivityReportId = model.TradePromotionActivityReportId,
                            BusinessId = business.BusinessId ?? Guid.Empty,
                            BusinessName = business.BusinessName,
                            Address = business.Address,
                        };
                        addBusinesses.Add(item);
                    }

                    await _context.TradePromotionActivityReportParticipatingBusinesses.AddRangeAsync(addBusinesses);
                    await _context.SaveChangesAsync();
                }

                //Add File
                if (model.Files!.Any())
                {
                    List<TradePromotionActivityReportAttachFile> addFiles = new List<TradePromotionActivityReportAttachFile>();
                    foreach (var file in model.Files!)
                    {
                        TradePromotionActivityReportAttachFile item = new TradePromotionActivityReportAttachFile()
                        {
                            TradePromotionActivityReportId = model.TradePromotionActivityReportId,
                            LinkFile = file.LinkFile,
                        };
                        addFiles.Add(item);
                    }

                    await _context.TradePromotionActivityReportAttachFiles.AddRangeAsync(addFiles);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var info = await _context.TradePromotionActivityReports.Where(d => d.TradePromotionActivityReportId == Id).FirstOrDefaultAsync();
            if (info != null)
            {
                info.IsDel = true;
                _context.TradePromotionActivityReports.Update(info);
                await _context.SaveChangesAsync();

                //Delete File
                var LFileDel = await _context.TradePromotionActivityReportAttachFiles.Where(x => x.TradePromotionActivityReportId == Id).ToListAsync();
                if (LFileDel.Any())
                {
                    foreach (var fdel in LFileDel)
                    {
                        string linkdel = fdel.LinkFile;
                        var result = Ulities.RemoveFileMinio(linkdel, config);
                    }

                    _context.TradePromotionActivityReportAttachFiles.RemoveRange(LFileDel);
                    await _context.SaveChangesAsync();
                }

                //Delete business
                List<TradePromotionActivityReportParticipatingBusiness> ParticipatingBusiness = await _context.TradePromotionActivityReportParticipatingBusinesses
                    .Where(x => x.TradePromotionActivityReportId == Id).ToListAsync();
                if (ParticipatingBusiness.Any())
                {
                    _context.TradePromotionActivityReportParticipatingBusinesses.RemoveRange(ParticipatingBusiness);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public TradePromotionActivityReportModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.TradePromotionActivityReports.Where(x => x.TradePromotionActivityReportId == Id && !x.IsDel).Select(item => new TradePromotionActivityReportModel()
            {
                TradePromotionActivityReportId = item.TradePromotionActivityReportId,
                ScaleId = item.ScaleId,
                PlanName = item.PlanName,
                PlanToJoin = item.PlanToJoin,
                StartDateDisplay = item.StartDate.ToString("dd'/'MM'/'yyyy"),
                EndDateDisplay = item.EndDate.HasValue ? item.EndDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                Time = item.Time,
                DistrictId = item.DistrictId,
                Address = item.Address,
                ImplementationCost = item.ImplementationCost,
                FundingSupport = item.FundingSupport,
                Scale = item.Scale ?? "",
                NumParticipating = item.NumParticipating,
                Note = item.Note ?? "",
            }).FirstOrDefault();

            if (result != null)
            {
                //ParticipatingBusiness
                List<TradePromotionActivityReportBusinessModel> LBusiness = _context.TradePromotionActivityReportParticipatingBusinesses
                    .Where(x => x.TradePromotionActivityReportId == Id)
                    .Select(x => new TradePromotionActivityReportBusinessModel
                    {
                        TradePromotionActivityReportDetailId = x.TradePromotionActivityReportId,
                        TradePromotionActivityReportId = x.TradePromotionActivityReportId,
                        BusinessId = x.BusinessId,
                        BusinessName = x.BusinessName,
                        Address = x.Address,
                    })
                    .ToList();
                result.ParticipatingBusinesses = LBusiness;


                List<TradePromotionActivityReportAttachFileModel> LFiles = _context.TradePromotionActivityReportAttachFiles
                    .Where(x => x.TradePromotionActivityReportId == Id).Select(item => new TradePromotionActivityReportAttachFileModel
                    {
                        TradePromotionActivityReportAttachFileId = item.TradePromotionActivityReportAttachFileId,
                        TradePromotionActivityReportId = item.TradePromotionActivityReportId,
                        LinkFile = string.IsNullOrEmpty(item.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + item.LinkFile,
                    }).ToList();
                result.Files = LFiles;
            }

            return result;
        }

        public List<TradePromotionActivityReportModel> FindData(QueryRequestBody query)
        {
            IQueryable<TradePromotionActivityReportModel> _data = (from t in _context.TradePromotionActivityReports
                                                                   where !t.IsDel
                                                                   select new TradePromotionActivityReportModel
                                                                   {
                                                                       TradePromotionActivityReportId = t.TradePromotionActivityReportId,
                                                                       ScaleId = t.ScaleId,
                                                                       PlanName = t.PlanName,
                                                                       StartDateDisplay = "Ngày " + t.StartDate.ToString("dd'/'MM'/'yyyy")
                                                                        + (t.EndDate.HasValue ? "đến " + t.EndDate.Value.ToString("dd'/'MM'/'yyyy") : "")
                                                                        + ", tại " + t.Address.Trim(),
                                                                       Time = t.Time,
                                                                       PlanToJoin = t.PlanToJoin,
                                                                       ImplementationCost = t.ImplementationCost,
                                                                       FundingSupport = t.FundingSupport,
                                                                       Scale = t.Scale,
                                                                       NumParticipating = t.NumParticipating,
                                                                       StartDate = t.StartDate,
                                                                       EndDate = t.EndDate,
                                                                       Participating = string.Join(", ", _context.TradePromotionActivityReportParticipatingBusinesses
                                                                        .Where(x => x.TradePromotionActivityReportId == t.TradePromotionActivityReportId)
                                                                        .Select(t => t.BusinessName.Trim() + " (" + t.Address + ")").ToList()),
                                                                   }).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.PlanName.ToLower().Contains(_keywordSearch)
                || x.StartDateDisplay.ToLower().Contains(_keywordSearch)
                || x.ImplementationCost.ToString().Contains(_keywordSearch)
                || x.FundingSupport.ToString().Contains(_keywordSearch)
                || x.Scale.ToLower().Contains(_keywordSearch)
                || x.NumParticipating.ToString().Contains(_keywordSearch)
                );
            }

            int _countRows = _data.Count();
            if (_countRows == 0)
            {
                return new List<TradePromotionActivityReportModel>();
            }

            if (query.Filter != null && query.Filter.ContainsKey("ScaleId") && !string.IsNullOrEmpty(query.Filter["ScaleId"]))
            {
                _data = _data.Where(x =>
                            x.ScaleId.ToString().Equals(query.Filter["ScaleId"]));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinTime") && !string.IsNullOrEmpty(query.Filter["MinTime"]))
            {
                _data = _data.Where(x =>
                            x.StartDate >=
                            DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null).Date);
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxTime")
                && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
            {
                _data = _data.Where(x =>
                           x.EndDate <=
                            DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null).Date);
            }

            return _data.ToList();
        }
    }
}
