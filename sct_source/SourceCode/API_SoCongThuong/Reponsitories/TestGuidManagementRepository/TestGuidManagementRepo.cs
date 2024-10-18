using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Classes;

namespace API_SoCongThuong.Reponsitories.TestGuidManagementRepository
{
    public class TestGuidManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TestGuidManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TestGuidManagementModel model)
        {
            TestGuidManagement data = new TestGuidManagement()
            {
                InspectionAgency = model.InspectionAgency,
                Time = model.Time,
                CoordinationAgency = model.CoordinationAgency,
                Result = model.Result,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };
            await _context.TestGuidManagements.AddAsync(data);
            await _context.SaveChangesAsync();

            List<TestGuidManagementAttachFile> details = new List<TestGuidManagementAttachFile>();
            foreach (var item in model.Details)
            {
                TestGuidManagementAttachFile detail = new TestGuidManagementAttachFile()
                {
                    TestGuidManagementId = data.TestGuidManagementId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.TestGuidManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TestGuidManagementModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.TestGuidManagementAttachFiles.Where(d => LstFiledel.Contains(d.TestGuidManagementAttachFileId.ToString())).ToList();
                #region gắn hàm delete file
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                #endregion
                _context.TestGuidManagementAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            var detailInfo = await _context.TestGuidManagements.Where(d => d.TestGuidManagementId == model.TestGuidManagementId).FirstOrDefaultAsync();

            detailInfo.InspectionAgency = model.InspectionAgency;
            detailInfo.Time = model.Time;
            detailInfo.CoordinationAgency = model.CoordinationAgency;
            detailInfo.Result = model.Result;
            detailInfo.UpdateUserId = model.UpdateUserId;
            detailInfo.UpdateTime = model.UpdateTime;

            List<TestGuidManagementAttachFile> details = new List<TestGuidManagementAttachFile>();

            foreach (var item in model.Details)
            {
                TestGuidManagementAttachFile detail = new TestGuidManagementAttachFile()
                {
                    TestGuidManagementId = model.TestGuidManagementId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.TestGuidManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.TestGuidManagements.Where(d => d.TestGuidManagementId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.TestGuidManagementAttachFiles.Where(d => d.TestGuidManagementId == Id).ToList();
            #region gắn hàm delete file
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            #endregion
            _context.TestGuidManagementAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();

        }

        public TestGuidManagementModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.TestGuidManagements.Where(x => x.TestGuidManagementId == Id).Select(d => new TestGuidManagementModel()
            {
                InspectionAgency = d.InspectionAgency,
                Time = d.Time,
                CoordinationAgency = d.CoordinationAgency,
                Result = d.Result,
                IsDel = d.IsDel,
            }).FirstOrDefault();

            if (result == null)
            {
                return new TestGuidManagementModel();
            }

            var details = _context.TestGuidManagementAttachFiles.Where(x => x.TestGuidManagementId == Id).Select(model => new TestGuidManagementAttachFileModel
            {
                TestGuidManagementAttachFileId = model.TestGuidManagementAttachFileId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
            });

            result.Details = details.ToList();

            return result;
        }

    }
}
