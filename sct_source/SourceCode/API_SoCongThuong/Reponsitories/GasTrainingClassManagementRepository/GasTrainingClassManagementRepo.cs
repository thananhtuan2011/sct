using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Classes;

namespace API_SoCongThuong.Reponsitories.GasTrainingClassManagementRepository
{
    public class GasTrainingClassManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public GasTrainingClassManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(GasTrainingClassManagementModel model)
        {
            GasTrainingClassManagement data = new GasTrainingClassManagement()
            {
                Topic = model.Topic,
                Location = model.Location,
                Participant = model.Participant,
                TimeStart = model.TimeStart,
                NumberOfAttendees = model.NumberOfAttendees,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };
            await _context.GasTrainingClassManagements.AddAsync(data);
            await _context.SaveChangesAsync();

            List<GasTrainingClassManagementAttachFile> details = new List<GasTrainingClassManagementAttachFile>();
            foreach (var item in model.Details)
            {
                GasTrainingClassManagementAttachFile detail = new GasTrainingClassManagementAttachFile()
                {
                    GasTrainingClassManagementId = data.GasTrainingClassManagementId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.GasTrainingClassManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Update(GasTrainingClassManagementModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.GasTrainingClassManagementAttachFiles.Where(d => LstFiledel.Contains(d.GasTrainingClassManagementAttachFileId.ToString())).ToList();
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                _context.GasTrainingClassManagementAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            var detailInfo = await _context.GasTrainingClassManagements.Where(d => d.GasTrainingClassManagementId == model.GasTrainingClassManagementId).FirstOrDefaultAsync();

            detailInfo.Topic = model.Topic;
            detailInfo.Location = model.Location;
            detailInfo.Participant = model.Participant;
            detailInfo.NumberOfAttendees = model.NumberOfAttendees;
            detailInfo.TimeStart = model.TimeStart;
            detailInfo.UpdateUserId = model.UpdateUserId;
            detailInfo.UpdateTime = model.UpdateTime;

            List<GasTrainingClassManagementAttachFile> details = new List<GasTrainingClassManagementAttachFile>();

            foreach (var item in model.Details)
            {
                GasTrainingClassManagementAttachFile detail = new GasTrainingClassManagementAttachFile()
                {
                    GasTrainingClassManagementId = model.GasTrainingClassManagementId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }

            await _context.GasTrainingClassManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.GasTrainingClassManagements.Where(d => d.GasTrainingClassManagementId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.GasTrainingClassManagementAttachFiles.Where(d => d.GasTrainingClassManagementId == Id).ToList();
            #region gắn hàm delete file
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            #endregion
            _context.GasTrainingClassManagementAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();

        }

        public GasTrainingClassManagementModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.GasTrainingClassManagements.Where(x => x.GasTrainingClassManagementId == Id).Select(d => new GasTrainingClassManagementModel()
            {
                GasTrainingClassManagementId = d.GasTrainingClassManagementId,
                Topic = d.Topic,
                Location = d.Location,
                Participant = d.Participant,
                NumberOfAttendees = d.NumberOfAttendees,
                TimeStart = d.TimeStart,
                IsDel = d.IsDel,
            }).FirstOrDefault();

            if (result == null)
            {
                return new GasTrainingClassManagementModel();
            }

            var details = _context.GasTrainingClassManagementAttachFiles.Where(x => x.GasTrainingClassManagementId == Id).Select(model => new GasTrainingClassManagementAttachFileModel
            {
                GasTrainingClassManagementAttachFileId = model.GasTrainingClassManagementAttachFileId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
            });

            result.Details = details.ToList();

            return result;
        }
    }
}
