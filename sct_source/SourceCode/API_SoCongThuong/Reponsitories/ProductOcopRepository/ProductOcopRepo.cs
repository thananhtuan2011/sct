using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class ProductOcopRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ProductOcopRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(ProductOcopModel model)
        {
            ProductOcop data = new ProductOcop()
            {
                ProductOcopid = model.ProductOcopid,
                Address = model.Address,
                Ingredients = model.Ingredients,
                Ratings = model.Ratings,
                ProductOwner = model.ProductOwner,
                ProductName = model.ProductName,
                PhoneNumber = model.PhoneNumber ?? "",
                Preserve = model.Preserve,
                Expiry = model.Expiry,
                DistrictId = model.DistrictId,
                ApprovalDecision = model.ApprovalDecision ?? "",
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.ProductOcops.AddAsync(data);
            await _context.SaveChangesAsync();
            List<ProductOcopAttachFile> details = new List<ProductOcopAttachFile>();
            foreach (var item in model.Details)
            {
                ProductOcopAttachFile detail = new ProductOcopAttachFile()
                {
                    ProductOcopid = data.ProductOcopid,
                    LinkFile = item.LinkFile,
                    Type = item.Type
                };
                details.Add(detail);
            }
            await _context.ProductOcopAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Update(ProductOcopModel model, IConfiguration config)
        {
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.ProductOcopAttachFiles.Where(d => LstFiledel.Contains(d.ProductOcopattachFileId.ToString())).ToList();
                #region gắn hàm delete file
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                #endregion
                _context.ProductOcopAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            var detailinfo = await _context.ProductOcops.Where(d => d.ProductOcopid == model.ProductOcopid).FirstOrDefaultAsync();
            detailinfo.Address = model.Address;
            detailinfo.Ingredients = model.Ingredients;
            detailinfo.Ratings = model.Ratings;
            detailinfo.ProductOwner = model.ProductOwner;
            detailinfo.ProductName = model.ProductName;
            detailinfo.PhoneNumber = model.PhoneNumber ?? "";
            detailinfo.ApprovalDecision = model.ApprovalDecision ?? "";
            detailinfo.Preserve = model.Preserve;
            detailinfo.Expiry = model.Expiry;
            detailinfo.DistrictId = model.DistrictId;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            List<ProductOcopAttachFile> details = new List<ProductOcopAttachFile>();
            foreach (var item in model.Details)
            {
                ProductOcopAttachFile detail = new ProductOcopAttachFile()
                {
                    ProductOcopid = model.ProductOcopid,
                    LinkFile = item.LinkFile,
                    Type = item.Type
                };
                details.Add(detail);
            }
            await _context.ProductOcopAttachFiles.AddRangeAsync(details);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid Id, IConfiguration config)
        {
            var detailinfo = await _context.ProductOcops.Where(d => d.ProductOcopid == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var del = _context.ProductOcopAttachFiles.Where(d => d.ProductOcopid == Id).ToList();
            #region gắn hàm delete file
            foreach (var fdel in del)
            {
                string linkdel = fdel.LinkFile;
                var result = Ulities.RemoveFileMinio(linkdel, config);
            }
            #endregion
            _context.ProductOcopAttachFiles.RemoveRange(del);
            await _context.SaveChangesAsync();
        }

        public ProductOcopModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.ProductOcops.Where(x => x.ProductOcopid == Id && !x.IsDel).Select(model => new ProductOcopModel()
            {
                ProductOcopid = model.ProductOcopid,
                Address = model.Address,
                Ingredients = model.Ingredients,
                Ratings = model.Ratings,
                ProductOwner = model.ProductOwner,
                ProductName = model.ProductName,
                PhoneNumber = model.PhoneNumber ?? "",
                ApprovalDecision = model.ApprovalDecision ?? "",
                Preserve = model.Preserve,
                Expiry = model.Expiry,
                DistrictId = model.DistrictId,
            }).FirstOrDefault();

            if (result == null)
            {
                return new ProductOcopModel();
            }

            var details = _context.ProductOcopAttachFiles.Where(x => x.ProductOcopid == Id).Select(model => new ProductOcopAttachFileModel
            {
                ProductOcopattachFileId = model.ProductOcopattachFileId,
                LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
                Type = model.Type
            });
            result.Details = details.ToList();

            return result;
        }
    }
}
