using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace API_SoCongThuong.Reponsitories.FoodSafetyCertificateRepository
{
    public class FoodSafetyCertificateRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public FoodSafetyCertificateRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(FoodSafetyCertificateModel model)
        {
            //Thêm GCN
            FoodSafetyCertificate SaveData = new FoodSafetyCertificate();
            SaveData.ProfileCode = model.ProfileCode;
            SaveData.ProfileName = model.ProfileName;
            SaveData.BusinessId = model.BusinessId;
            SaveData.ManagerName = model.ManagerName;
            SaveData.DistrictId = model.DistrictId;
            SaveData.Address = model.Address;
            SaveData.PhoneNumber = model.PhoneNumber;
            SaveData.Num = model.Num;
            SaveData.ValidTill = model.ValidTill != null ? DateTime.ParseExact(model.ValidTill, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : null;
            SaveData.LicenseDate = model.LicenseDate != null ? DateTime.ParseExact(model.LicenseDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : null;
            if (model.ProductionData.Any() && model.Num != null && model.ValidTill != null && model.LicenseDate != null)
            {
                SaveData.Status = 1;
            }
            else
            {
                SaveData.Status = 0;
            }
            SaveData.Note = model.Note;
            SaveData.CreateUserId = model.CreateUserId ?? Guid.Empty;
            SaveData.CreateTime = DateTime.Now;
            await _context.FoodSafetyCertificates.AddAsync(SaveData);
            await _context.SaveChangesAsync();

            //Thêm loại hình sản xuất / kinh doanh
            if (model.ProductionData.Any())
            {
                List<FoodSafetyCertificateItem> Items = new List<FoodSafetyCertificateItem>();
                foreach (var i in model.ProductionData)
                {
                    FoodSafetyCertificateItem item = new FoodSafetyCertificateItem()
                    {
                        FoodSafetyCertificateId = SaveData.FoodSafetyCertificateId,
                        Type = i.Type,
                        ProductName = i.ProductName,
                    };
                    Items.Add(item);
                }
                if (Items.Any())
                {
                    await _context.FoodSafetyCertificateItems.AddRangeAsync(Items);
                    await _context.SaveChangesAsync();
                }
            }

            //Thêm File
            if (model.Details.Any())
            {
                List<FoodSafetyCertificateAttachFile> details = new List<FoodSafetyCertificateAttachFile>();
                foreach (var item in model.Details)
                {
                    FoodSafetyCertificateAttachFile detail = new FoodSafetyCertificateAttachFile()
                    {
                        FoodSafetyCertificateId = SaveData.FoodSafetyCertificateId,
                        LinkFile = item.LinkFile,
                    };
                    details.Add(detail);
                }
                if (details.Any())
                {
                    await _context.FoodSafetyCertificateAttachFiles.AddRangeAsync(details);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task Update(FoodSafetyCertificateModel model, IConfiguration config)
        {
            //Xóa loại hình sản xuất / kinh doanh
            var OldItems = _context.FoodSafetyCertificateItems.Where(x => x.FoodSafetyCertificateId == model.FoodSafetyCertificateId).ToList();
            if (OldItems.Any())
            {
                _context.FoodSafetyCertificateItems.RemoveRange(OldItems);
                await _context.SaveChangesAsync();
            }

            //Xóa File
            if (!string.IsNullOrEmpty(model.IdFiles))
            {
                var LstFiledel = model.IdFiles.Split(",").ToList();
                var del = _context.FoodSafetyCertificateAttachFiles.Where(d => LstFiledel.Contains(d.FoodSafetyCertificateAttachFileId.ToString())).ToList();
                foreach (var fdel in del)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                _context.FoodSafetyCertificateAttachFiles.RemoveRange(del);
                await _context.SaveChangesAsync();
            }

            //Chỉnh sửa GCN
            FoodSafetyCertificate? SaveData = _context.FoodSafetyCertificates.Where(x => x.FoodSafetyCertificateId == model.FoodSafetyCertificateId && !x.IsDel).FirstOrDefault();
            if (SaveData != null)
            {
                SaveData.ProfileCode = model.ProfileCode;
                SaveData.ProfileName = model.ProfileName;
                SaveData.BusinessId = model.BusinessId;
                SaveData.ManagerName = model.ManagerName;
                SaveData.DistrictId = model.DistrictId;
                SaveData.Address = model.Address;
                SaveData.PhoneNumber = model.PhoneNumber;
                SaveData.Num = model.Num;
                SaveData.ValidTill = model.ValidTill != null ? DateTime.ParseExact(model.ValidTill, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : null;
                SaveData.LicenseDate = model.LicenseDate != null ? DateTime.ParseExact(model.LicenseDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : null;
                if (model.ProductionData.Any() && model.Num != null && model.ValidTill != null && model.LicenseDate != null)
                {
                    SaveData.Status = 1;
                }
                else
                {
                    SaveData.Status = 0;
                }
                SaveData.Note = model.Note;
                SaveData.UpdateUserId = model.UpdateUserId ?? null;
                SaveData.UpdateTime = DateTime.Now;

                _context.FoodSafetyCertificates.Update(SaveData);
                await _context.SaveChangesAsync();

                //Thêm loại hình sản xuất / kinh doanh
                if (model.ProductionData.Any())
                {
                    List<FoodSafetyCertificateItem> Items = new List<FoodSafetyCertificateItem>();
                    foreach (var i in model.ProductionData)
                    {
                        FoodSafetyCertificateItem item = new FoodSafetyCertificateItem()
                        {
                            FoodSafetyCertificateId = SaveData.FoodSafetyCertificateId,
                            Type = i.Type,
                            ProductName = i.ProductName,
                        };
                        Items.Add(item);
                    }

                    if (Items.Any())
                    {
                        await _context.FoodSafetyCertificateItems.AddRangeAsync(Items);
                        await _context.SaveChangesAsync();
                    }
                }


                //Thêm File
                if (model.Details.Any())
                {
                    List<FoodSafetyCertificateAttachFile> details = new List<FoodSafetyCertificateAttachFile>();
                    foreach (var item in model.Details)
                    {
                        FoodSafetyCertificateAttachFile detail = new FoodSafetyCertificateAttachFile()
                        {
                            FoodSafetyCertificateId = SaveData.FoodSafetyCertificateId,
                            LinkFile = item.LinkFile,
                        };
                        details.Add(detail);
                    }

                    if (details.Any())
                    {
                        await _context.FoodSafetyCertificateAttachFiles.AddRangeAsync(details);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task Delete(FoodSafetyCertificate model, IConfiguration config)
        {
            _context.FoodSafetyCertificates.Update(model);
            await _context.SaveChangesAsync();

            var DelFiles = _context.FoodSafetyCertificateAttachFiles.Where(d => d.FoodSafetyCertificateId == model.FoodSafetyCertificateId).ToList();
            if (DelFiles.Any())
            {
                foreach (var fdel in DelFiles)
                {
                    string linkdel = fdel.LinkFile;
                    var result = Ulities.RemoveFileMinio(linkdel, config);
                }
                _context.FoodSafetyCertificateAttachFiles.RemoveRange(DelFiles);
                await _context.SaveChangesAsync();
            }

            var DelItems = _context.FoodSafetyCertificateItems.Where(d => d.FoodSafetyCertificateId == model.FoodSafetyCertificateId).ToList();
            if (DelItems.Any())
            {
                _context.FoodSafetyCertificateItems.RemoveRange(DelItems);
                await _context.SaveChangesAsync();
            }
        }

        public FoodSafetyCertificateModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.FoodSafetyCertificates
                .Where(x => x.FoodSafetyCertificateId == Id && !x.IsDel)
                .Select(f => new FoodSafetyCertificateModel()
                {
                    FoodSafetyCertificateId = f.FoodSafetyCertificateId,
                    ProfileCode = f.ProfileCode,
                    ProfileName = f.ProfileName,
                    BusinessId = f.BusinessId,
                    ManagerName = f.ManagerName,
                    DistrictId = f.DistrictId,
                    Address = f.Address,
                    PhoneNumber = f.PhoneNumber,
                    Num = f.Num ?? "",
                    LicenseDate = f.LicenseDate.HasValue ? f.LicenseDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                    ValidTill = f.ValidTill.HasValue ? f.ValidTill.Value.ToString("dd'/'MM'/'yyyy") : "",
                    Note = f.Note ?? "",
                }).FirstOrDefault();

            if (result != null)
            {
                var LFiles = _context.FoodSafetyCertificateAttachFiles.Where(x => x.FoodSafetyCertificateId == Id).Select(model => new FoodSafetyCertificateAttachFileModel
                {
                    FoodSafetyCertificateAttachFileId = model.FoodSafetyCertificateAttachFileId,
                    LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
                });
                result.Details = LFiles.ToList();

                var LItems = _context.FoodSafetyCertificateItems.Where(x => x.FoodSafetyCertificateId == Id).Select(model => new FoodSafetyCertificateItemModel
                {
                    Type = model.Type,
                    ProductName = model.ProductName,
                });
                result.ProductionData = LItems.ToList();
            }

            return result;
        }

        public async Task UpdateStatus(FoodSafetyCertificate model)
        {
            _context.FoodSafetyCertificates.Update(model);
            await _context.SaveChangesAsync();
        }
    }
}
