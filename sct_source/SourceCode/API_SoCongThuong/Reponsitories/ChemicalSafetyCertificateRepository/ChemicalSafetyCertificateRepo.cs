using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

namespace API_SoCongThuong.Reponsitories.ChemicalSafetyCertificateRepository
{
    public class ChemicalSafetyCertificateRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ChemicalSafetyCertificateRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(ChemicalSafetyCertificateModel model)
        {
            ChemicalSafetyCertificate SaveData = new ChemicalSafetyCertificate();
            SaveData.BusinessId = model.BusinessId;
            SaveData.Address = model.Address;
            SaveData.PhoneNumber = model.PhoneNumber;
            SaveData.Fax = model.Fax;
            SaveData.BusinessAddress = model.BusinessAddress;
            SaveData.BusinessCode = model.BusinessCode;
            SaveData.Provider = model.Provider;
            SaveData.BusinessCertificateDate = model.BusinessCertificateDate != null ? DateTime.ParseExact(model.BusinessCertificateDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : null;
            SaveData.LicenseDate = model.LicenseDate != null ? DateTime.ParseExact(model.LicenseDate, "dd'/'MM'/'yyyy", null) : null;
            SaveData.Num = model.Num;
            SaveData.ValidTill = model.ValidTill != null ? DateTime.ParseExact(model.ValidTill, "dd'/'MM'/'yyyy", null) : null;
            if (model.ListChemical.Count > 0 && model.Num != null && model.ValidTill != null)
            {
                SaveData.Status = 1;
            }
            else
            {
                SaveData.Status = 0;
            }
            SaveData.CreateUserId = model.CreateUserId ?? Guid.Empty;
            SaveData.CreateTime = DateTime.Now;

            await _context.ChemicalSafetyCertificates.AddAsync(SaveData);
            await _context.SaveChangesAsync();


            if (model.ListChemical.Count > 0)
            {
                List<ChemicalSafetyCertificateChemicalInfo> Range = new List<ChemicalSafetyCertificateChemicalInfo>();
                foreach (var chemical in model.ListChemical)
                {
                    ChemicalSafetyCertificateChemicalInfo item = new ChemicalSafetyCertificateChemicalInfo
                    {
                        ChemicalSafetyCertificateId = SaveData.ChemicalSafetyCertificateId,
                        TradeName = chemical.TradeName,
                        NameOfChemical = chemical.NameOfChemical,
                        Cascode = chemical.Cascode,
                        ChemicalFormula = chemical.ChemicalFormula,
                        Content = chemical.Content,
                        Mass = chemical.Mass
                    };
                    Range.Add(item);
                }
                _context.ChemicalSafetyCertificateChemicalInfos.AddRange(Range);
                _context.SaveChanges();
            }

            if (model.Details.Count > 0)
            {
                List<ChemicalSafetyCertificateAttachFile> details = new List<ChemicalSafetyCertificateAttachFile>();
                foreach (var item in model.Details)
                {
                    ChemicalSafetyCertificateAttachFile detail = new ChemicalSafetyCertificateAttachFile()
                    {
                        ChemicalSafetyCertificateId = SaveData.ChemicalSafetyCertificateId,
                        LinkFile = item.LinkFile,
                    };
                    details.Add(detail);
                }
                await _context.ChemicalSafetyCertificateAttachFiles.AddRangeAsync(details);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Update(ChemicalSafetyCertificateModel model, IConfiguration config)
        {
            ChemicalSafetyCertificate? SaveData = _context.ChemicalSafetyCertificates.Where(x => x.ChemicalSafetyCertificateId == model.ChemicalSafetyCertificateId && !x.IsDel).FirstOrDefault();
            if (SaveData != null)
            {
                SaveData.BusinessId = model.BusinessId;
                SaveData.Address = model.Address;
                SaveData.PhoneNumber = model.PhoneNumber;
                SaveData.Fax = model.Fax;
                SaveData.BusinessAddress = model.BusinessAddress;
                SaveData.BusinessCode = model.BusinessCode;
                SaveData.Provider = model.Provider;
                SaveData.BusinessCertificateDate = model.BusinessCertificateDate != null ? DateTime.ParseExact(model.BusinessCertificateDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : null;
                SaveData.LicenseDate = model.LicenseDate != null ? DateTime.ParseExact(model.LicenseDate, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : null;
                SaveData.Num = model.Num;
                SaveData.ValidTill = model.ValidTill != null ? DateTime.ParseExact(model.ValidTill, "dd'/'MM'/'yyyy", CultureInfo.InvariantCulture) : null;
                if (model.ListChemical.Count > 0 && model.Num != null && model.ValidTill != null)
                {
                    SaveData.Status = 1;
                }
                else
                {
                    SaveData.Status = 0;
                }

                SaveData.UpdateUserId = model.UpdateUserId;
                SaveData.UpdateTime = DateTime.Now;

                _context.ChemicalSafetyCertificates.Update(SaveData);
                await _context.SaveChangesAsync();


                List<ChemicalSafetyCertificateChemicalInfo> DelRange = _context.ChemicalSafetyCertificateChemicalInfos.Where(x => x.ChemicalSafetyCertificateId == model.ChemicalSafetyCertificateId).ToList();
                if (DelRange.Count > 0)
                {
                    _context.ChemicalSafetyCertificateChemicalInfos.RemoveRange(DelRange);
                    await _context.SaveChangesAsync();
                }

                if (model.ListChemical.Count > 0)
                {
                    List<ChemicalSafetyCertificateChemicalInfo> Range = new List<ChemicalSafetyCertificateChemicalInfo>();
                    foreach (var chemical in model.ListChemical)
                    {
                        ChemicalSafetyCertificateChemicalInfo item = new ChemicalSafetyCertificateChemicalInfo
                        {
                            ChemicalSafetyCertificateId = SaveData.ChemicalSafetyCertificateId,
                            TradeName = chemical.TradeName,
                            NameOfChemical = chemical.NameOfChemical,
                            Cascode = chemical.Cascode,
                            ChemicalFormula = chemical.ChemicalFormula,
                            Content = chemical.Content,
                            Mass = chemical.Mass
                        };
                        Range.Add(item);
                    }
                    _context.ChemicalSafetyCertificateChemicalInfos.AddRange(Range);
                    _context.SaveChanges();
                }

                if (!string.IsNullOrEmpty(model.IdFiles))
                {
                    var LstFiledel = model.IdFiles.Split(",").ToList();
                    var del = _context.ChemicalSafetyCertificateAttachFiles.Where(d => LstFiledel.Contains(d.ChemicalSafetyCertificateAttachFileId.ToString())).ToList();
                    foreach (var fdel in del)
                    {
                        string linkdel = fdel.LinkFile;
                        var result = Ulities.RemoveFileMinio(linkdel, config);
                    }
                    _context.ChemicalSafetyCertificateAttachFiles.RemoveRange(del);
                    await _context.SaveChangesAsync();
                }

                if (model.Details.Count > 0)
                {
                    List<ChemicalSafetyCertificateAttachFile> details = new List<ChemicalSafetyCertificateAttachFile>();
                    foreach (var item in model.Details)
                    {
                        ChemicalSafetyCertificateAttachFile detail = new ChemicalSafetyCertificateAttachFile()
                        {
                            ChemicalSafetyCertificateId = SaveData.ChemicalSafetyCertificateId,
                            LinkFile = item.LinkFile,
                        };
                        details.Add(detail);
                    }
                    await _context.ChemicalSafetyCertificateAttachFiles.AddRangeAsync(details);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task Delete(ChemicalSafetyCertificate model)
        {
            _context.ChemicalSafetyCertificates.Update(model);
            await _context.SaveChangesAsync();
        }

        public ChemicalSafetyCertificateModel FindById(Guid Id, IConfiguration config)
        {
            var result = _context.ChemicalSafetyCertificates
                .Where(x => x.ChemicalSafetyCertificateId == Id && !x.IsDel)
                .Select(f => new ChemicalSafetyCertificateModel()
                {
                    ChemicalSafetyCertificateId = f.ChemicalSafetyCertificateId,
                    BusinessId = f.BusinessId,
                    Address = f.Address,
                    PhoneNumber = f.PhoneNumber,
                    Fax = f.Fax ?? "",
                    BusinessAddress = f.BusinessAddress,
                    BusinessCode = f.BusinessCode,
                    Provider = f.Provider,
                    BusinessCertificateDate = f.BusinessCertificateDate.HasValue ? f.BusinessCertificateDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                    LicenseDate = f.LicenseDate.HasValue ? f.LicenseDate.Value.ToString("dd'/'MM'/'yyyy") : "",
                    Num = f.Num ?? "",
                    ValidTill = f.ValidTill.HasValue ? f.ValidTill.Value.ToString("dd'/'MM'/'yyyy") : "",
                    ListChemical = _context.ChemicalSafetyCertificateChemicalInfos
                        .Where(x => x.ChemicalSafetyCertificateId == Id)
                        .Select(x => new ChemicalInfoModel
                        {
                            TradeName = x.TradeName,
                            NameOfChemical = x.NameOfChemical,
                            Cascode = x.Cascode,
                            ChemicalFormula = x.ChemicalFormula,
                            Content = x.Content,
                            Mass = x.Mass,
                        }).ToList(),
                }).FirstOrDefault();

            if (result != null)
            {
                var details = _context.ChemicalSafetyCertificateAttachFiles.Where(x => x.ChemicalSafetyCertificateId == Id).Select(model => new ChemicalSafetyCertificateAttachFileModel
                {
                    ChemicalSafetyCertificateAttachFileId = model.ChemicalSafetyCertificateAttachFileId,
                    LinkFile = string.IsNullOrEmpty(model.LinkFile) ? "" : config.GetValue<string>("MinioConfig:Protocol") + config.GetValue<string>("MinioConfig:MinioServer") + model.LinkFile,
                });
                result.Details = details.ToList();
            }

            return result;
        }

        public async Task UpdateStatus(ChemicalSafetyCertificate model)
        {
            _context.ChemicalSafetyCertificates.Update(model);
            await _context.SaveChangesAsync();
        }
    }
}
