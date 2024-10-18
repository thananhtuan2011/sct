using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class TradePromotionOtherRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TradePromotionOtherRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TradePromotionOtherModel model)
        {
            TradePromotionOther data = new TradePromotionOther()
            {
                TradePromotionOtherId = model.TradePromotionOtherId,
                TypeOfActivity = model.TypeOfActivity,
                Content = model.Content,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Time = model.Time ?? "",
                DistrictId = model.DistrictId,
                Address = model.Address,
                ImplementationCost = model.ImplementationCost,
                Participating = model.Participating ?? "",
                Coordination = model.Coordination ?? "",
                Result = model.Result ?? "",
                Note = model.Note ?? "",
            };
            await _context.TradePromotionOthers.AddAsync(data);
            await _context.SaveChangesAsync();

            if (model.Details.Any())
            {
                List<TradePromotionOtherAttachFile> details = new List<TradePromotionOtherAttachFile>();
                foreach (var item in model.Details)
                {
                    TradePromotionOtherAttachFile detail = new TradePromotionOtherAttachFile()
                    {
                        TradePromotionOtherId = data.TradePromotionOtherId,
                        LinkFile = item.LinkFile
                    };
                    details.Add(detail);
                }
                await _context.TradePromotionOtherAttachFiles.AddRangeAsync(details);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(TradePromotionOtherModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.LIdDel))
            {
                List<string> LstFiledel = model.LIdDel.Split(",").ToList();
                var del = _context.TradePromotionOtherAttachFiles.Where(d => LstFiledel.Contains(d.TradePromotionOtherAttachFileId.ToString())).ToList();
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                _context.TradePromotionOtherAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            var info = await _context.TradePromotionOthers.Where(d => d.TradePromotionOtherId == model.TradePromotionOtherId).FirstOrDefaultAsync();
            if (info != null)
            {
                info.TypeOfActivity = model.TypeOfActivity;
                info.Content = model.Content;
                info.StartDate = model.StartDate;
                info.EndDate = model.EndDate;
                info.Time = model.Time ?? "";
                info.DistrictId = model.DistrictId;
                info.Address = model.Address;
                info.ImplementationCost = model.ImplementationCost;
                info.Participating = model.Participating ?? "";
                info.Coordination = model.Coordination ?? "";
                info.Result = model.Result ?? "";
                info.Note = model.Note ?? "";

                _context.TradePromotionOthers.Update(info);
                await _context.SaveChangesAsync();
            }

            if (model.Details.Any())
            {
                List<TradePromotionOtherAttachFile> details = new List<TradePromotionOtherAttachFile>();
                foreach (var item in model.Details)
                {
                    TradePromotionOtherAttachFile detail = new TradePromotionOtherAttachFile()
                    {
                        TradePromotionOtherId = model.TradePromotionOtherId,
                        LinkFile = item.LinkFile
                    };
                    details.Add(detail);
                }

                await _context.TradePromotionOtherAttachFiles.AddRangeAsync(details);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var info = await _context.TradePromotionOthers.Where(d => d.TradePromotionOtherId == Id).FirstOrDefaultAsync();
            if (info != null)
            {
                info.IsDel = true;
                _context.TradePromotionOthers.Update(info);
                await _context.SaveChangesAsync();
            }

            //Delete File
            var LFileDel = await _context.TradePromotionOtherAttachFiles.Where(x => x.TradePromotionOtherId == Id).ToListAsync();
            if (LFileDel.Any())
            {
                foreach (var fdel in LFileDel)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }

                _context.TradePromotionOtherAttachFiles.RemoveRange(LFileDel);
                await _context.SaveChangesAsync();
            }
        }

        public TradePromotionOtherModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.TradePromotionOthers.Where(x => x.TradePromotionOtherId == Id && !x.IsDel).Select(model => new TradePromotionOtherModel()
            {
                TradePromotionOtherId = model.TradePromotionOtherId,
                TypeOfActivity = model.TypeOfActivity,
                Content = model.Content,
                StartDate = model.StartDate,
                StartDateDisplay = model.StartDate.ToString("dd'/'MM'/'yyyy"),
                EndDate = model.EndDate,
                EndDateDisplay = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                Time = model.Time ?? "",
                DistrictId = model.DistrictId,
                Address = model.Address,
                ImplementationCost = model.ImplementationCost,
                Participating = model.Participating ?? "",
                Coordination = model.Coordination ?? "",
                Result = model.Result ?? "",
                Note = model.Note ?? "",
            }).FirstOrDefault();

            if (result == null)
            {
                return new TradePromotionOtherModel();
            }
            else
            {
                List<TradePromotionOtherDetailModel> LFiles = _context.TradePromotionOtherAttachFiles
                    .Where(x => x.TradePromotionOtherId == Id).Select(item => new TradePromotionOtherDetailModel
                    {
                        TradePromotionOtherAttachFileId = item.TradePromotionOtherAttachFileId,
                        TradePromotionOtherId = item.TradePromotionOtherId,
                        LinkFile = string.IsNullOrEmpty(item.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + item.LinkFile,
                    }).ToList();
                result.Details = LFiles;
            }

            return result;
        }

        public List<TradePromotionOtherModel> FindData(QueryRequestBody query)
        {
            IQueryable<TradePromotionOtherModel> _data = _context.TradePromotionOthers.Where(x => !x.IsDel)
                .Select(info => new TradePromotionOtherModel
                {
                    TradePromotionOtherId = info.TradePromotionOtherId,
                    TypeOfActivity = info.TypeOfActivity,
                    Content = info.Content,
                    Address = info.Address,
                    StartDate = info.StartDate,
                    StartDateDisplay = info.StartDate.ToString("dd'/'MM'/'yyyy"),
                    EndDate = info.EndDate,
                    EndDateDisplay = info.EndDate.HasValue ? info.EndDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                    Time = info.Time ?? "",
                    ImplementationCost = info.ImplementationCost,
                    Participating = info.Participating ?? "",
                    Coordination = info.Coordination ?? "",
                    Result = info.Result ?? "",
                    Note = info.Note ?? "",
                }).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(
                    x => x.Address.ToLower().Contains(_keywordSearch)
                    || x.Content.ToLower().Contains(_keywordSearch)
                    || x.ImplementationCost.ToString().Contains(_keywordSearch)
                );
            }

            if (query.Filter != null && query.Filter.ContainsKey("Type") && !string.IsNullOrEmpty(query.Filter["Type"]))
            {
                _data = _data.Where(x => x.TypeOfActivity.ToString() == query.Filter["Type"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinTime") && !string.IsNullOrEmpty(query.Filter["MinTime"]))
            {
                _data = _data.Where(x =>
                            x.StartDate >=
                            DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxTime") && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
            {
                _data = _data.Where(x =>
                           x.StartDate <=
                            DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
            }

            return _data.ToList();
        }
    }
}
