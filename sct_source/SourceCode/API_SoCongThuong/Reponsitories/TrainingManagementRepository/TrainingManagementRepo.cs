using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class TrainingManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TrainingManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TrainingManagementModel model)
        {
            TrainingManagement data = new TrainingManagement()
            {
                TrainingManagementId = model.TrainingManagementId,
                Content = model.Content,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Time = model.Time ?? "",
                DistrictId = model.DistrictId,
                Address = model.Address,
                Participating = model.Participating ?? "",
                NumParticipating = model.NumParticipating,
                ImplementationCost = model.ImplementationCost,
                Annunciator = model.Annunciator ?? "",
                Note = model.Note ?? "",
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.TrainingManagements.AddAsync(data);
            await _context.SaveChangesAsync();
            List<TrainingManagementAttachFile> details = new List<TrainingManagementAttachFile>();
            foreach (var item in model.Details)
            {
                TrainingManagementAttachFile detail = new TrainingManagementAttachFile()
                {
                    TrainingManagementId = data.TrainingManagementId,
                    LinkFile = item.LinkFile
                };
                details.Add(detail);
            }
            await _context.TrainingManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Update(TrainingManagementModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.TrainingManagementAttachFiles.Where(d => LstFiledel.Contains(d.TrainingManagementAttachFileId.ToString())).ToList();
                #region gắn hàm delete file
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                #endregion
                _context.TrainingManagementAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            var info = await _context.TrainingManagements.Where(d => d.TrainingManagementId == model.TrainingManagementId).FirstOrDefaultAsync();
            if (info != null)
            {
                info.Content = model.Content;
                info.StartDate = model.StartDate;
                info.EndDate = model.EndDate;
                info.Time = model.Time ?? "";
                info.DistrictId = model.DistrictId;
                info.Address = model.Address;
                info.Participating = model.Participating ?? "";
                info.NumParticipating = model.NumParticipating;
                info.ImplementationCost = model.ImplementationCost;
                info.Annunciator = model.Annunciator ?? "";
                info.Note = model.Note ?? "";
                info.UpdateUserId = model.UpdateUserId;
                info.UpdateTime = model.UpdateTime;

                List<TrainingManagementAttachFile> details = new List<TrainingManagementAttachFile>();
                foreach (var item in model.Details)
                {
                    TrainingManagementAttachFile detail = new TrainingManagementAttachFile()
                    {
                        TrainingManagementId = model.TrainingManagementId,
                        LinkFile = item.LinkFile
                    };
                    details.Add(detail);
                }
                await _context.TrainingManagementAttachFiles.AddRangeAsync(details);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.TrainingManagements.Where(d => d.TrainingManagementId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.TrainingManagementAttachFiles.Where(d => d.TrainingManagementId == Id).ToList();
            #region gắn hàm delete file
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            #endregion
            _context.TrainingManagementAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();
        }

        public TrainingManagementModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.TrainingManagements.Where(x => x.TrainingManagementId == Id && !x.IsDel).Select(model => new TrainingManagementModel()
            {
                TrainingManagementId = model.TrainingManagementId,
                Content = model.Content,
                StartDate = model.StartDate,
                StartDateDisplay = model.StartDate.ToString("dd'/'MM'/'yyyy"),
                EndDate = model.EndDate,
                EndDateDisplay = model.EndDate.HasValue ? model.EndDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                Time = model.Time ?? "",
                DistrictId = model.DistrictId,
                Address = model.Address,
                Participating = model.Participating ?? "",
                NumParticipating = model.NumParticipating,
                ImplementationCost = model.ImplementationCost,
                Annunciator = model.Annunciator ?? "",
                Note = model.Note ?? "",
            }).FirstOrDefault();

            if (result == null)
            {
                return new TrainingManagementModel();
            }

            var details = _context.TrainingManagementAttachFiles.Where(x => x.TrainingManagementId == Id).Select(model => new TrainingManagementAttachFileModel
            {
                TrainingManagementAttachFileId = model.TrainingManagementAttachFileId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
            });

            result.Details = details.ToList();

            return result;
        }

        public List<TrainingManagementModel> FindData(QueryRequestBody query)
        {
            IQueryable<TrainingManagementModel> _data = _context.TrainingManagements.Where(x => !x.IsDel)
                .Select(info => new TrainingManagementModel
                {
                    TrainingManagementId = info.TrainingManagementId,
                    Content = info.Content,
                    StartDate = info.StartDate,
                    StartDateDisplay = info.StartDate.ToString("dd'/'MM'/'yyyy"),
                    DistrictId = info.DistrictId,
                    Address = info.Address,
                    Participating = info.Participating,
                    NumParticipating = info.NumParticipating,
                    ImplementationCost = info.ImplementationCost,
                    Annunciator = info.Annunciator,
                });

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                    x.Content.ToLower().Contains(_keywordSearch)
                    || x.Address.ToLower().Contains(_keywordSearch)
                    || x.Participating.ToLower().Contains(_keywordSearch)
                    || x.NumParticipating.ToString().Contains(_keywordSearch)
                    || x.ImplementationCost.ToString().Contains(_keywordSearch)
                );
            }

            if (query.Filter != null && query.Filter.ContainsKey("DistrictId") && !string.IsNullOrEmpty(query.Filter["DistrictId"]))
            {
                _data = _data.Where(x => x.DistrictId.ToString() == query.Filter["DistrictId"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("MinTime") && !string.IsNullOrEmpty(query.Filter["MinTime"]))
            {
                _data = _data.Where(x =>
                    x.StartDate >= DateTime.ParseExact(query.Filter["MinTime"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("MaxTime") && !string.IsNullOrEmpty(query.Filter["MaxTime"]))
            {
                _data = _data.Where(x =>
                    x.StartDate <= DateTime.ParseExact(query.Filter["MaxTime"], "dd/MM/yyyy", null));
            }

            return _data.ToList();
        }
    }
}
