using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Classes;

namespace API_SoCongThuong.Reponsitories.ManageConfirmPromotionRepository
{
    public class ManageConfirmPromotionRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ManageConfirmPromotionRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ManageConfirmPromotionModel model)
        {
            ManageConfirmPromotion data = new ManageConfirmPromotion()
            {
                ManageConfirmPromotionName = model.ManageConfirmPromotionName,
                GoodsServices = model.GoodsServices,
                GoodsServicesPay = model.GoodsServicesPay,
                TimeStart = model.TimeStart,
                TimeEnd = model.TimeEnd,
                NumberOfDocuments = model.NumberOfDocuments,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };

            await _context.ManageConfirmPromotions.AddAsync(data);
            await _context.SaveChangesAsync();
            
            List<ManageConfirmPromotionAttachFile> details = new List<ManageConfirmPromotionAttachFile>();
            foreach (var item in model.Details)
            {
                ManageConfirmPromotionAttachFile detail = new ManageConfirmPromotionAttachFile()
                {
                    ManageConfirmPromotionId = data.ManageConfirmPromotionId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.ManageConfirmPromotionAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ManageConfirmPromotionModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.ManageConfirmPromotionAttachFiles.Where(d => LstFiledel.Contains(d.ManageConfirmPromotionAttachFileId.ToString())).ToList();
                #region gắn hàm delete file
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                #endregion
                _context.ManageConfirmPromotionAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            var detailInfo = await _context.ManageConfirmPromotions.Where(d => d.ManageConfirmPromotionId == model.ManageConfirmPromotionId).FirstOrDefaultAsync();

            detailInfo.ManageConfirmPromotionName = model.ManageConfirmPromotionName;
            detailInfo.GoodsServices = model.GoodsServices;
            detailInfo.GoodsServicesPay = model.GoodsServicesPay;
            detailInfo.TimeStart = model.TimeStart;
            detailInfo.TimeEnd = model.TimeEnd;
            detailInfo.NumberOfDocuments = model.NumberOfDocuments;
            detailInfo.UpdateUserId = model.UpdateUserId;
            detailInfo.UpdateTime = model.UpdateTime;

            List<ManageConfirmPromotionAttachFile> details = new List<ManageConfirmPromotionAttachFile>();

            foreach (var item in model.Details)
            {
                ManageConfirmPromotionAttachFile detail = new ManageConfirmPromotionAttachFile()
                {
                    ManageConfirmPromotionId = model.ManageConfirmPromotionId,
                    LinkFile = item.LinkFile,
                };
                details.Add(detail);
            }
            await _context.ManageConfirmPromotionAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteManageConfirmPromotion(ManageConfirmPromotion model)
        {
            var db = await _context.ManageConfirmPromotions.Where(d => d.ManageConfirmPromotionId == model.ManageConfirmPromotionId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.ManageConfirmPromotions.Where(d => d.ManageConfirmPromotionId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.ManageConfirmPromotionAttachFiles.Where(d => d.ManageConfirmPromotionId == Id).ToList();
            #region gắn hàm delete file
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            #endregion
            _context.ManageConfirmPromotionAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();


        }

        public ManageConfirmPromotionModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.ManageConfirmPromotions.Where(x => x.ManageConfirmPromotionId == Id).Select(d => new ManageConfirmPromotionModel()
            {
                ManageConfirmPromotionId = d.ManageConfirmPromotionId,
                ManageConfirmPromotionName = d.ManageConfirmPromotionName,
                GoodsServices = d.GoodsServices,
                GoodsServicesPay = d.GoodsServicesPay,
                TimeStart = d.TimeStart,
                TimeEnd = d.TimeEnd,
                NumberOfDocuments = d.NumberOfDocuments,
                IsDel = d.IsDel,
            }).FirstOrDefault();

            if (result == null)
            {
                return new ManageConfirmPromotionModel();
            }

            var details = _context.ManageConfirmPromotionAttachFiles.Where(x => x.ManageConfirmPromotionId == Id).Select(model => new ManageConfirmPromotionAttachFileModel
            {
                ManageConfirmPromotionAttachFileId = model.ManageConfirmPromotionAttachFileId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
            });

            result.Details = details.ToList();
 
            return result;
        }

    }
}
