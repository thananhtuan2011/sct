using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class IndustrialPromotionProjectRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public IndustrialPromotionProjectRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(IndustrialPromotionProjectModel model)
        {
            IndustrialPromotionProject data = new IndustrialPromotionProject()
            {
                IndustrialPromotionProjectId = model.IndustrialPromotionProjectId,
                Capital = model.Capital,
                Funding = model.Funding,
                IndustrialPromotionFunding = model.IndustrialPromotionFunding,
                StartDate = model.StartDateTime ?? DateTime.UtcNow,
                EndDate = model.StartDateTime ?? DateTime.UtcNow,
                ProjectName = model.ProjectName,
                ReciprocalEnterpriseFunding = model.ReciprocalEnterpriseFunding,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.IndustrialPromotionProjects.AddAsync(data);
            await _context.SaveChangesAsync();
            List<IndustrialPromotionProjectDetail> details = new List<IndustrialPromotionProjectDetail>();
            foreach (var item in model.Details)
            {
                IndustrialPromotionProjectDetail detail = new IndustrialPromotionProjectDetail()
                {
                    IndustrialPromotionProjectId = data.IndustrialPromotionProjectId,
                    BusinessCode = item.BusinessCode,
                    BusinessId = item.BusinessId,
                    BusinessNameVi = item.BusinessNameVi,
                    DiaChi = item.DiaChi,
                    NganhNghe = item.NganhNghe,
                    NguoiDaiDien = item.NguoiDaiDien
                };
                details.Add(detail);
            }
            await _context.IndustrialPromotionProjectDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }
        public async Task Update(IndustrialPromotionProjectModel model)
        {
            var del = _context.IndustrialPromotionProjectDetails.Where(d => d.IndustrialPromotionProjectId == model.IndustrialPromotionProjectId).ToList();
            _context.IndustrialPromotionProjectDetails.RemoveRange(del);
            await _context.SaveChangesAsync();

            var detailinfo = await _context.IndustrialPromotionProjects.Where(d => d.IndustrialPromotionProjectId == model.IndustrialPromotionProjectId).FirstOrDefaultAsync();
            detailinfo.Capital = model.Capital;
            detailinfo.Funding = model.Funding;
            detailinfo.IndustrialPromotionFunding = model.IndustrialPromotionFunding;
            detailinfo.StartDate = model.StartDateTime ?? DateTime.UtcNow;
            detailinfo.EndDate = model.EndDateTime ?? DateTime.UtcNow;
            detailinfo.ProjectName = model.ProjectName;
            detailinfo.ReciprocalEnterpriseFunding = model.ReciprocalEnterpriseFunding;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            List<IndustrialPromotionProjectDetail> details = new List<IndustrialPromotionProjectDetail>();
            foreach (var item in model.Details)
            {
                IndustrialPromotionProjectDetail detail = new IndustrialPromotionProjectDetail()
                {
                    IndustrialPromotionProjectId = model.IndustrialPromotionProjectId,
                    BusinessCode = item.BusinessCode,
                    BusinessId = item.BusinessId,
                    BusinessNameVi = item.BusinessNameVi,
                    DiaChi = item.DiaChi,
                    NganhNghe = item.NganhNghe,
                    NguoiDaiDien = item.NguoiDaiDien
                };
                details.Add(detail);
            }
            await _context.IndustrialPromotionProjectDetails.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.IndustrialPromotionProjects.Where(d => d.IndustrialPromotionProjectId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.IndustrialPromotionProjectDetails.Where(d => d.IndustrialPromotionProjectId == Id).ToList();
            _context.IndustrialPromotionProjectDetails.RemoveRange(del);
            await _context.SaveChangesAsync();
        }

        public IndustrialPromotionProjectModel FindById(Guid Id)
        {
            var result = _context.IndustrialPromotionProjects.Where(x => x.IndustrialPromotionProjectId == Id && !x.IsDel).Select(model => new IndustrialPromotionProjectModel()
            {
                IndustrialPromotionProjectId = model.IndustrialPromotionProjectId,
                Capital = model.Capital,
                Funding = model.Funding,
                IndustrialPromotionFunding = model.IndustrialPromotionFunding,
                StartDate = model.StartDate.ToString("dd'/'MM'/'yyyy"),
                EndDate = model.EndDate.ToString("dd'/'MM'/'yyyy"),
                ProjectName = model.ProjectName,
                ReciprocalEnterpriseFunding = model.ReciprocalEnterpriseFunding,
            }).FirstOrDefault();

            if (result == null)
            {
                return new IndustrialPromotionProjectModel();
            }

            var details = _context.IndustrialPromotionProjectDetails.Where(x => x.IndustrialPromotionProjectId == Id).Select(x => new IndustrialPromotionProjectDetailModel
            {
                BusinessCode = x.BusinessCode,
                BusinessId = x.BusinessId,
                BusinessNameVi = x.BusinessNameVi,
                DiaChi = x.DiaChi,
                NganhNghe = x.NganhNghe,
                NguoiDaiDien = x.NguoiDaiDien,
                IndustrialPromotionProjectDetailId = x.IndustrialPromotionProjectDetailId,
                IndustrialPromotionProjectId = x.IndustrialPromotionProjectId
            });
            result.Details = details.ToList();

            return result;
        }
    }
}
