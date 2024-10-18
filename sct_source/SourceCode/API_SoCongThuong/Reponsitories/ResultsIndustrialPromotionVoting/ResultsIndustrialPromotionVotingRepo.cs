using API_SoCongThuong.Controllers;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class ResultsIndustrialPromotionVotingRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ResultsIndustrialPromotionVotingRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ResultsIndustrialPromotionVotingModel model)
        {
            ResultsIndustrialPromotionVotingRp data = new ResultsIndustrialPromotionVotingRp()
            {
                ResultsIndustrialPromotionVotingId = model.ResultsIndustrialPromotionVotingId,
                Locallity = model.Locallity,
                NumberCertified = model.NumberCertified,
                NumbersRegister = model.NumbersRegister,
                Targets = model.Targets,
                Unit = model.Unit,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.ResultsIndustrialPromotionVotingRps.AddAsync(data);
            await _context.SaveChangesAsync();
        }
        public async Task Update(ResultsIndustrialPromotionVotingModel model)
        {
            var detailinfo = await _context.ResultsIndustrialPromotionVotingRps.Where(d => d.ResultsIndustrialPromotionVotingId == model.ResultsIndustrialPromotionVotingId).FirstOrDefaultAsync();
            detailinfo.ResultsIndustrialPromotionVotingId = model.ResultsIndustrialPromotionVotingId;
            detailinfo.Locallity = model.Locallity;
            detailinfo.NumbersRegister = model.NumbersRegister;
            detailinfo.NumberCertified = model.NumberCertified;
            detailinfo.Targets = model.Targets;
            detailinfo.Unit = model.Unit;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.ResultsIndustrialPromotionVotingRps.Where(d => d.ResultsIndustrialPromotionVotingId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();

        }
        public ResultsIndustrialPromotionVotingModel FindById(Guid Id)
        {
            var result = _context.ResultsIndustrialPromotionVotingRps.Where(x => x.ResultsIndustrialPromotionVotingId == Id && !x.IsDel).Select(d => new ResultsIndustrialPromotionVotingModel()
            {
                ResultsIndustrialPromotionVotingId = d.ResultsIndustrialPromotionVotingId,
                Locallity = d.Locallity,
                NumbersRegister = d.NumbersRegister,
                NumberCertified = d.NumberCertified,
                Targets = d.Targets,
                Unit = d.Unit,
            }).FirstOrDefault();
            return result;
        }
    }
}
