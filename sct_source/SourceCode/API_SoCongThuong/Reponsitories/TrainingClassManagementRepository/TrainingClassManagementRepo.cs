using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Classes;

namespace API_SoCongThuong.Reponsitories.TrainingClassManagementRepository
{
    public class TrainingClassManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TrainingClassManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TrainingClassManagementModel model)
        {
            TrainingClassManagement data = new TrainingClassManagement()
            {
                Topic = model.Topic,
                Location = model.Location,
                Participant = model.Participant,
                TimeStart = model.TimeStart,
                NumberOfAttendees = model.NumberOfAttendees,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };
            await _context.TrainingClassManagements.AddAsync(data);
            await _context.SaveChangesAsync();
            
            List<TrainingClassManagementAttachFile> details = new List<TrainingClassManagementAttachFile>();
            foreach (var item in model.Details)
            {
                TrainingClassManagementAttachFile detail = new TrainingClassManagementAttachFile()
                {
                    TrainingClassManagementId = data.TrainingClassManagementId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.TrainingClassManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TrainingClassManagementModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.TrainingClassManagementAttachFiles.Where(d => LstFiledel.Contains(d.TrainingClassManagementAttachFileId.ToString())).ToList();
                #region gắn hàm delete file
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                #endregion
                _context.TrainingClassManagementAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            var detailInfo = await _context.TrainingClassManagements.Where(d => d.TrainingClassManagementId == model.TrainingClassManagementId).FirstOrDefaultAsync();

            detailInfo.Topic = model.Topic;
            detailInfo.Location = model.Location;
            detailInfo.Participant = model.Participant;
            detailInfo.NumberOfAttendees = model.NumberOfAttendees;
            detailInfo.TimeStart = model.TimeStart;
            detailInfo.UpdateUserId = model.UpdateUserId;
            detailInfo.UpdateTime = model.UpdateTime;

            List<TrainingClassManagementAttachFile> details = new List<TrainingClassManagementAttachFile>();

            foreach (var item in model.Details)
            {
                TrainingClassManagementAttachFile detail = new TrainingClassManagementAttachFile()
                {
                    TrainingClassManagementId = model.TrainingClassManagementId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.TrainingClassManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.TrainingClassManagements.Where(d => d.TrainingClassManagementId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.TrainingClassManagementAttachFiles.Where(d => d.TrainingClassManagementId == Id).ToList();
            #region gắn hàm delete file
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            #endregion
            _context.TrainingClassManagementAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();

        }

        public TrainingClassManagementModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.TrainingClassManagements.Where(x => x.TrainingClassManagementId == Id).Select(d => new TrainingClassManagementModel()
            {
                TrainingClassManagementId = d.TrainingClassManagementId,
                Topic = d.Topic,
                Location = d.Location,
                Participant = d.Participant,
                NumberOfAttendees = d.NumberOfAttendees,
                TimeStart = d.TimeStart,
                IsDel = d.IsDel,
            }).FirstOrDefault();

            if (result == null)
            {
                return new TrainingClassManagementModel();
            }

            var details = _context.TrainingClassManagementAttachFiles.Where(x => x.TrainingClassManagementId == Id).Select(model => new TrainingClassManagementAttachFileModel
            {
                TrainingClassManagementAttachFileId = model.TrainingClassManagementAttachFileId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
            });

            result.Details = details.ToList();

            return result;
        }

    }
}
