using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Classes;

namespace API_SoCongThuong.Reponsitories.TradePromotionProjectManagementRepository
{
    public class TradePromotionProjectManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public TradePromotionProjectManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(TradePromotionProjectManagementModel model)
        {
            TradePromotionProjectManagement data = new TradePromotionProjectManagement()
            {
                TradePromotionProjectManagementName = model.TradePromotionProjectManagementName,
                ImplementingAgencies = model.ImplementingAgencies,
                Cost = model.Cost,
                CurrencyUnit = model.CurrencyUnit,
                TimeStart = model.TimeStart,
                TimeEnd = model.TimeEnd,
                NumberOfApprovalDocuments = model.NumberOfApprovalDocuments,
                ImplementationResults = model.ImplementationResults,
                Status = model.Status ,
                Reason = model.Reason != "null" ? model.Reason : "",
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };
            await _context.TradePromotionProjectManagements.AddAsync(data);
            await _context.SaveChangesAsync();
            List<TradePromotionProjectManagementBussiness> businessList = new List<TradePromotionProjectManagementBussiness>();
            if (model.BusinessDetails != null)
            {
                foreach (var business in model.BusinessDetails)
                {
                    TradePromotionProjectManagementBussiness businessItem = new TradePromotionProjectManagementBussiness()
                    {
                        TradePromotionProjectManagementId = data.TradePromotionProjectManagementId,
                        BusinessId = business.BusinessId,
                        BusinessCode = business.BusinessCode,
                        BusinessNameVi = business.BusinessNameVi,
                        NganhNghe = business.NganhNghe,
                        DiaChi = business.DiaChi,
                        NguoiDaiDien = business.NguoiDaiDien
                    };
                    businessList.Add(businessItem);
                }

                await _context.TradePromotionProjectManagementBussinesses.AddRangeAsync(businessList);
                await _context.SaveChangesAsync();
            }
            List<TradePromotionProjectManagementAttachFile> details = new List<TradePromotionProjectManagementAttachFile>();
            foreach (var item in model.Details)
            {
                TradePromotionProjectManagementAttachFile detail = new TradePromotionProjectManagementAttachFile()
                {
                    TradePromotionProjectManagementId = data.TradePromotionProjectManagementId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.TradePromotionProjectManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TradePromotionProjectManagementModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.TradePromotionProjectManagementAttachFiles.Where(d => LstFiledel.Contains(d.TradePromotionProjectManagementAttachFileId.ToString())).ToList();
                #region gắn hàm delete file
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                #endregion
                _context.TradePromotionProjectManagementAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            var detailInfo = await _context.TradePromotionProjectManagements.Where(d => d.TradePromotionProjectManagementId == model.TradePromotionProjectManagementId).FirstOrDefaultAsync();

            detailInfo.TradePromotionProjectManagementName = model.TradePromotionProjectManagementName;
            detailInfo.ImplementingAgencies = model.ImplementingAgencies;
            detailInfo.Cost = model.Cost;
            detailInfo.CurrencyUnit = model.CurrencyUnit;
            detailInfo.TimeStart = model.TimeStart;
            detailInfo.TimeEnd = model.TimeEnd;
            detailInfo.NumberOfApprovalDocuments = model.NumberOfApprovalDocuments;
            detailInfo.ImplementationResults = model.ImplementationResults;
            detailInfo.Reason = model.Reason != "null" ? model.Reason : "";
            detailInfo.UpdateUserId = model.UpdateUserId;
            detailInfo.UpdateTime = model.UpdateTime;

            List<TradePromotionProjectManagementAttachFile> details = new List<TradePromotionProjectManagementAttachFile>();

            foreach (var item in model.Details)
            {
                TradePromotionProjectManagementAttachFile detail = new TradePromotionProjectManagementAttachFile()
                {
                    TradePromotionProjectManagementId = model.TradePromotionProjectManagementId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.TradePromotionProjectManagementAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();

            var listBusiness =  _context.TradePromotionProjectManagementBussinesses.Where(d => d.TradePromotionProjectManagementId == model.TradePromotionProjectManagementId).ToList();
            _context.TradePromotionProjectManagementBussinesses.RemoveRange(listBusiness);
            await _context.SaveChangesAsync();

            if(model.BusinessDetails != null)
            {
                List<TradePromotionProjectManagementBussiness> businessList = new List<TradePromotionProjectManagementBussiness>();

                foreach (var business in model.BusinessDetails)
                {
                    TradePromotionProjectManagementBussiness businessItem = new TradePromotionProjectManagementBussiness()
                    {
                        TradePromotionProjectManagementId = model.TradePromotionProjectManagementId,
                        BusinessId = business.BusinessId,
                        BusinessCode = business.BusinessCode,
                        BusinessNameVi = business.BusinessNameVi,
                        NganhNghe = business.NganhNghe,
                        DiaChi = business.DiaChi,
                        NguoiDaiDien = business.NguoiDaiDien
                    };
                    businessList.Add(businessItem);
                }

                await _context.TradePromotionProjectManagementBussinesses.AddRangeAsync(businessList);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTradePromotionProjectManagement(TradePromotionProjectManagement model)
        {
            var db = await _context.TradePromotionProjectManagements.Where(d => d.TradePromotionProjectManagementId == model.TradePromotionProjectManagementId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.TradePromotionProjectManagements.Where(d => d.TradePromotionProjectManagementId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.TradePromotionProjectManagementAttachFiles.Where(d => d.TradePromotionProjectManagementId == Id).ToList();
            #region gắn hàm delete file
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            #endregion
            _context.TradePromotionProjectManagementAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();

            var listBusiness = _context.TradePromotionProjectManagementBussinesses.Where(d => d.TradePromotionProjectManagementId == Id).ToList();
            _context.TradePromotionProjectManagementBussinesses.RemoveRange(listBusiness);
            await _context.SaveChangesAsync();
        }

        public TradePromotionProjectManagementModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.TradePromotionProjectManagements.Where(x => x.TradePromotionProjectManagementId == Id).Select(d => new TradePromotionProjectManagementModel()
            {
                TradePromotionProjectManagementId = d.TradePromotionProjectManagementId,
                TradePromotionProjectManagementName = d.TradePromotionProjectManagementName,
                ImplementingAgencies = d.ImplementingAgencies,
                Cost = d.Cost,
                CurrencyUnit = d.CurrencyUnit,
                TimeStart = d.TimeStart,
                TimeEnd = d.TimeEnd,
                NumberOfApprovalDocuments = d.NumberOfApprovalDocuments,
                ImplementationResults = d.ImplementationResults,
                Status = d.Status,
                Reason = d.Reason,
                IsDel = d.IsDel,
            }).FirstOrDefault();

            if (result == null)
            {
                return new TradePromotionProjectManagementModel();
            }

            var details = _context.TradePromotionProjectManagementAttachFiles.Where(x => x.TradePromotionProjectManagementId == Id).Select(model => new TradePromotionProjectManagementAttachFileModel
            {
                TradePromotionProjectManagementAttachFileId = model.TradePromotionProjectManagementAttachFileId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
            });

            var businessDetail = _context.TradePromotionProjectManagementBussinesses.Where(x => x.TradePromotionProjectManagementId == Id).Select(model => new TradePromotionProjectManagementDetailModel
            {
                TradePromotionProjectManagementDetailId = model.Id,
                BusinessId = model.BusinessId,
                BusinessCode = model.BusinessCode,
                BusinessNameVi = model.BusinessNameVi,
                NganhNghe = model.NganhNghe,
                DiaChi = model.DiaChi,
                NguoiDaiDien = model.NguoiDaiDien
            }); ;
            result.Details = details.ToList();
            result.BusinessDetails = businessDetail.ToList();

            return result;
        }

    }
}
