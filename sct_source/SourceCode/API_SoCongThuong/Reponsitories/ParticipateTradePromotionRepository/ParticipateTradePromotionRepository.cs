using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class ParticipateTradePromotionRepository
    {
        public SoHoa_SoCongThuongContext _context;
        public ParticipateTradePromotionRepository(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public ParticipateTradePromotionModel FindByCode(string code, IConfiguration config)
        {
            Guid Id = _context.Businesses.Where(x => !x.IsDel && x.BusinessCode == code).Select(x => x.BusinessId).FirstOrDefault();

            var result = new ParticipateTradePromotionModel();
            if (Id == Guid.Empty)
            {
                result = _context.ParticipateSupportFairDetails.Where(x => x.BusinessCode == code).Select(model => new ParticipateTradePromotionModel()
                {
                    BusinessId = model.BusinessId,
                    BusinessNameVi = model.BusinessNameVi,
                    DiaChiTruSo = model.DiaChi,
                    NguoiDaiDien = model.NguoiDaiDien,
                }).FirstOrDefault();

                if (result == null)
                {
                    result = _context.IndustrialPromotionProjectDetails.Where(x => x.BusinessCode == code).Select(model => new ParticipateTradePromotionModel()
                    {
                        BusinessId = model.BusinessId,
                        BusinessNameVi = model.BusinessNameVi,
                        DiaChiTruSo = model.DiaChi,
                        NguoiDaiDien = model.NguoiDaiDien,
                    }).FirstOrDefault();
                }
            }
            else
            {
                result = _context.Businesses.Where(x => x.BusinessCode == code && !x.IsDel).Select(model => new ParticipateTradePromotionModel()
                {
                    BusinessId = model.BusinessId,
                    BusinessNameVi = model.BusinessNameVi,
                    DiaChiTruSo = model.DiaChiTruSo,
                    MaSoThue = model.MaSoThue,
                    NguoiDaiDien = model.NguoiDaiDien,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    NgayCapPhep = model.NgayCapPhep,
                }).FirstOrDefault();
            }

            if (result == null)
            {
                return new ParticipateTradePromotionModel();
            }

            var details = _context.ParticipateSupportFairDetails.Where(x => x.BusinessCode == code).Join(
                    _context.ParticipateSupportFairs.Where(x => !x.IsDel),
                    cc => cc.ParticipateSupportFairId, cd => cd.ParticipateSupportFairId,
                     (cc, cd) =>
                        new ParticipateTradePromotionDetailModel
                        {
                            ParticipateSupportFairName = cd.ParticipateSupportFairName,
                            Address = cd.Address,
                            Country = cd.Country,
                            Scale = cd.Scale,
                            StartTime = cd.StartTime,
                            EndTime = (DateTime)cd.EndTime,
                            PlanJoin = cd.PlanJoin,

                        });

            var detail2s = _context.BusinessIndustries.Where(x => x.BusinessId == Id).Join(
                   _context.Industries.Where(x => !x.IsDel),
                   cc => cc.IndustryId, cd => cd.IndustryId,
                    (cc, cd) =>
                       new ParticipateTradePromotionDetail2Model
                       {
                           IndustryName = cd.IndustryName,
                           IndustryCode = cd.IndustryCode,
                       });

            var businessLine = _context.ParticipateSupportFairDetails.Where(x => x.BusinessId == Guid.Empty && x.BusinessCode == code).Select(x => new ParticipateTradePromotionDetail2Model
            {
                IndustryName = x.NganhNghe
            });

            var businessLine2 = _context.IndustrialPromotionProjectDetails.Where(x => x.BusinessId == Guid.Empty && x.BusinessCode == code).Select(x => new ParticipateTradePromotionDetail2Model
            {
                IndustryName = x.NganhNghe
            });

            var DetailsIndustryPromotion = _context.IndustrialPromotionProjectDetails.Where(x => x.BusinessCode == code).Join(
                  _context.IndustrialPromotionProjects.Where(x => !x.IsDel),
                  cc => cc.IndustrialPromotionProjectId, cd => cd.IndustrialPromotionProjectId,
                   (cc, cd) =>
                      new ParticipateTradePromotionDetailModel
                      {
                          ParticipateSupportFairName = cd.ProjectName,
                          StartDate = cd.StartDate.ToString("dd'/'MM'/'yyyy"),
                          EndDate = cd.EndDate.ToString("dd'/'MM'/'yyyy"),
                          CapitalName = cd.Capital == 1 ? "Trung ương" : "Địa phương",
                          Funding = cd.Funding
                      });

            var businessLineTotal = Enumerable.Concat(
                    detail2s.AsEnumerable(),
                    businessLine.AsEnumerable()
                    );

            businessLineTotal = Enumerable.Concat(
                    businessLineTotal.AsEnumerable(),
                    businessLine2.AsEnumerable()
                    );

            result.Detail1s = details.ToList();
            result.Detail2s = businessLineTotal.DistinctBy(i => i.IndustryCode).DistinctBy(i => i.IndustryName).ToList();
            result.Detail3s = DetailsIndustryPromotion.ToList();

            return result;
        }

    }

}
