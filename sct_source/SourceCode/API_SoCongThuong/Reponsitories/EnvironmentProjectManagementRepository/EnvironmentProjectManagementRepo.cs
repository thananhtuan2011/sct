using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class EnvironmentProjectManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public EnvironmentProjectManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(EnvironmentProjectManagementModel model)
        {
            EnvironmentProjectManagement data = new EnvironmentProjectManagement()
            {
                EnvironmentProjectManagementId = model.EnvironmentProjectManagementId,
                ProjectName = model.ProjectName,
                ImplementationContent = model.ImplementationContent,
                ApprovedFunding = model.ApprovedFunding,
                ImplementationCost = model.ImplementationCost,
                CoordinationUnit = model.CoordinationUnit,
                YearOfImplementationFrom = model.YearOfImplementationFrom,
                YearOfImplementationTo = model.YearOfImplementationTo,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.EnvironmentProjectManagements.AddAsync(data);
            await _context.SaveChangesAsync();
            List<EnvironmentProjectManagementAttachFile> details = new List<EnvironmentProjectManagementAttachFile>();
            foreach (var item in model.FileUpload)
            {
                EnvironmentProjectManagementAttachFile detail = new EnvironmentProjectManagementAttachFile()
                {
                    EnvironmentProjectManagementId = data.EnvironmentProjectManagementId,
                    LinkFile = item.LinkFile
                };
                details.Add(detail);
            }
            await _context.EnvironmentProjectManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Update(EnvironmentProjectManagementModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.EnvironmentProjectManagementAttachFiles.Where(d => LstFiledel.Contains(d.EnvironmentProjectManagementAttachFileId.ToString())).ToList();

                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                _context.EnvironmentProjectManagementAttachFiles.RemoveRange(del);

                await _context.SaveChangesAsync();
            }

            var detailinfo = await _context.EnvironmentProjectManagements.Where(d => d.EnvironmentProjectManagementId == model.EnvironmentProjectManagementId).FirstOrDefaultAsync();
            detailinfo.ProjectName = model.ProjectName;
            detailinfo.ImplementationContent = model.ImplementationContent;
            detailinfo.ApprovedFunding = model.ApprovedFunding;
            detailinfo.ImplementationCost = model.ImplementationCost;
            detailinfo.CoordinationUnit = model.CoordinationUnit;
            detailinfo.YearOfImplementationFrom = model.YearOfImplementationFrom;
            detailinfo.YearOfImplementationTo = model.YearOfImplementationTo;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            List<EnvironmentProjectManagementAttachFile> details = new List<EnvironmentProjectManagementAttachFile>();
            foreach (var item in model.FileUpload)
            {
                EnvironmentProjectManagementAttachFile detail = new EnvironmentProjectManagementAttachFile()
                {
                    EnvironmentProjectManagementId = model.EnvironmentProjectManagementId,
                    LinkFile = item.LinkFile
                };
                details.Add(detail);
            }
            await _context.EnvironmentProjectManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.EnvironmentProjectManagements.Where(d => d.EnvironmentProjectManagementId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.EnvironmentProjectManagementAttachFiles.Where(d => d.EnvironmentProjectManagementId == Id).ToList();
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            _context.EnvironmentProjectManagementAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();
        }

        public EnvironmentProjectManagementModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.EnvironmentProjectManagements.Where(x => x.EnvironmentProjectManagementId == Id && !x.IsDel).Select(model => new EnvironmentProjectManagementModel()
            {
                EnvironmentProjectManagementId = model.EnvironmentProjectManagementId,
                ProjectName = model.ProjectName,
                ImplementationContent = model.ImplementationContent,
                ApprovedFunding = model.ApprovedFunding,
                ImplementationCost = model.ImplementationCost,
                CoordinationUnit = model.CoordinationUnit,
                YearOfImplementationFrom = model.YearOfImplementationFrom,
                YearOfImplementationTo = model.YearOfImplementationTo,
            }).FirstOrDefault();

            if (result == null)
            {
                return new EnvironmentProjectManagementModel();
            }

            var details = _context.EnvironmentProjectManagementAttachFiles.Where(x => x.EnvironmentProjectManagementId == Id).Select(model => new EnvironmentProjectManagementAttachFileModel
            {
                EnvironmentProjectManagementAttachFileId = model.EnvironmentProjectManagementAttachFileId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
            });
            result.FileUpload = details.ToList();

            return result;
        }
    }
}
