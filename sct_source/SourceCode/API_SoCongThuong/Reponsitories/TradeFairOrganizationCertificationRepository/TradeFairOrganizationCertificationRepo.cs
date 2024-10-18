using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class TradeFairOrganizationCertificationRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TradeFairOrganizationCertificationRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TradeFairOrganizationCertificationModel model)
        {
            TradeFairOrganizationCertification data = new TradeFairOrganizationCertification()
            {
                TradeFairOrganizationCertificationId = model.TradeFairOrganizationCertificationId,
                TradeFairName = model.TradeFairName,
                Address = model.Address,
                Scale = model.Scale,
                TextNumber = model.TextNumber,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.TradeFairOrganizationCertifications.AddAsync(data);
            await _context.SaveChangesAsync();

            List<TradeFairOrganizationCertificationAttachFile> list_files = new List<TradeFairOrganizationCertificationAttachFile>();
            foreach (var item in model.ListFiles)
            {
                TradeFairOrganizationCertificationAttachFile file = new TradeFairOrganizationCertificationAttachFile()
                {
                    TradeFairOrganizationCertificationId = data.TradeFairOrganizationCertificationId,
                    LinkFile = item.LinkFile
                };
                list_files.Add(file);
            }
            await _context.TradeFairOrganizationCertificationAttachFiles.AddRangeAsync(list_files);
            await _context.SaveChangesAsync();

            List<TradeFairOrganizationCertificationTime> list_times = new List<TradeFairOrganizationCertificationTime>();
            foreach (var item in model.ListTimes)
            {
                TradeFairOrganizationCertificationTime time = new TradeFairOrganizationCertificationTime()
                {
                    TradeFairOrganizationCertificationId = data.TradeFairOrganizationCertificationId,
                    StartTime = item.StartTime,
                    EndTime = item.StartTime,
                };
                list_times.Add(time);
            }
            await _context.TradeFairOrganizationCertificationTimes.AddRangeAsync(list_times);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TradeFairOrganizationCertificationModel model, IConfiguration config)
        {
            //Check file reomove
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.TradeFairOrganizationCertificationAttachFiles.Where(d => LstFiledel.Contains(d.TradeFairOrganizationCertificationId.ToString())).ToList();
                #region gắn hàm delete file
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                #endregion
                _context.TradeFairOrganizationCertificationAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            //Update data
            var detailinfo = await _context.TradeFairOrganizationCertifications.Where(d => d.TradeFairOrganizationCertificationId == model.TradeFairOrganizationCertificationId).FirstOrDefaultAsync();
            detailinfo.TradeFairName = model.TradeFairName;
            detailinfo.Address = model.Address;
            detailinfo.Scale = model.Scale;
            detailinfo.TextNumber = model.TextNumber;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            //File
            List<TradeFairOrganizationCertificationAttachFile> list_files = new List<TradeFairOrganizationCertificationAttachFile>();
            foreach (var item in model.ListFiles)
            {
                TradeFairOrganizationCertificationAttachFile file = new TradeFairOrganizationCertificationAttachFile()
                {
                    TradeFairOrganizationCertificationId = model.TradeFairOrganizationCertificationId,
                    LinkFile = item.LinkFile
                };
                list_files.Add(file);
            }
            await _context.TradeFairOrganizationCertificationAttachFiles.AddRangeAsync(list_files);
            await _context.SaveChangesAsync();

            //Time
            var removetime = _context.TradeFairOrganizationCertificationTimes.Where(x => x.TradeFairOrganizationCertificationId == model.TradeFairOrganizationCertificationId).ToList();
            _context.TradeFairOrganizationCertificationTimes.RemoveRange(removetime);

            List<TradeFairOrganizationCertificationTime> list_times = new List<TradeFairOrganizationCertificationTime>();
            foreach (var item in model.ListTimes)
            {
                TradeFairOrganizationCertificationTime time = new TradeFairOrganizationCertificationTime()
                {
                    TradeFairOrganizationCertificationId = model.TradeFairOrganizationCertificationId,
                    StartTime = item.StartTime,
                    EndTime = item.StartTime,
                };
                list_times.Add(time);
            }
            await _context.TradeFairOrganizationCertificationTimes.AddRangeAsync(list_times);
            await _context.SaveChangesAsync();
        }


        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.TradeFairOrganizationCertifications.Where(d => d.TradeFairOrganizationCertificationId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            //File
            var del = _context.TradeFairOrganizationCertificationAttachFiles.Where(d => d.TradeFairOrganizationCertificationId == Id).ToList();
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            _context.TradeFairOrganizationCertificationAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();

            //Time
            var removetime = _context.TradeFairOrganizationCertificationTimes.Where(x => x.TradeFairOrganizationCertificationId == Id).ToList();
            _context.TradeFairOrganizationCertificationTimes.RemoveRange(removetime);
        }

        public TradeFairOrganizationCertificationModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.TradeFairOrganizationCertifications.Where(x => x.TradeFairOrganizationCertificationId == Id && !x.IsDel).Select(model => new TradeFairOrganizationCertificationModel()
            {
                TradeFairOrganizationCertificationId = model.TradeFairOrganizationCertificationId,
                TradeFairName = model.TradeFairName,
                Address = model.Address,
                Scale = model.Scale,
                TextNumber = model.TextNumber,
                CreateUserName = _context.Users.Where(x => x.CreateUserId == model.CreateUserId && x.IsDel == false).Select(x => x.FullName).FirstOrDefault() ?? ""
            }).FirstOrDefault();

            if (result == null)
            {
                return new TradeFairOrganizationCertificationModel();
            }

            var list_files = _context.TradeFairOrganizationCertificationAttachFiles.Where(x => x.TradeFairOrganizationCertificationId == Id).Select(model => new TradeFairOrganizationCertificationAttachFileModel
            {
                TradeFairFileId = model.TradeFairFileId,
                TradeFairOrganizationCertificationId = model.TradeFairOrganizationCertificationId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
            });
            result.ListFiles = list_files.ToList();

            var list_times = _context.TradeFairOrganizationCertificationTimes.Where(x => x.TradeFairOrganizationCertificationId == Id).Select(model => new TradeFairOrganizationCertificationTimeModel
            {
                TradeFairTimeId = model.TradeFairTimeId,
                TradeFairOrganizationCertificationId = model.TradeFairOrganizationCertificationId,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
            });
            result.ListTimes = list_times.ToList();

            return result;
        }
    }
}
