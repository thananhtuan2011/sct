using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using API_SoCongThuong.Classes;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using DocumentFormat.OpenXml.InkML;

namespace API_SoCongThuong.Reponsitories.RegulationConformityAMRepository
{
    public class RegulationConformityAMRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public RegulationConformityAMRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(RegulationConformityAMModel model)
        {
            RegulationConformityAm data = new RegulationConformityAm()
            {
                DayReception = DateTime.ParseExact(model.DayReception, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date,
                EstablishmentId = model.EstablishmentId,
                DistrictId = model.DistrictId,
                Address = model.Address,
                Phone = model.Phone,
                Num = model.Num,
                ProductName = model.ProductName,
                Content = model.Content,
                DateOfPublication = DateTime.ParseExact(model.DateOfPublication, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date,
                Note = model.Note,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime
            };
            await _context.RegulationConformityAms.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task Update(RegulationConformityAMModel model)
        {
            var detailInfo = await _context.RegulationConformityAms.Where(d => d.RegulationConformityAmid == model.RegulationConformityAMId).FirstOrDefaultAsync();
            if (detailInfo != null)
            {
                if (detailInfo.DateOfPublication != DateTime.ParseExact(model.DateOfPublication, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date)
                {
                    RegulationConformityAmLog Log = new RegulationConformityAmLog()
                    {
                        ItemId = detailInfo.RegulationConformityAmid,
                        UserId = model.UpdateUserId ?? Guid.Empty,
                        LogTime = DateTime.Now,
                        Property = "Ngày công bố trên Web của sở",
                        OldValue = detailInfo.DateOfPublication.ToString("dd/MM/yyyy"),
                        NewValue = model.DateOfPublication,
                    };
                    await _context.RegulationConformityAmLogs.AddAsync(Log);
                    await _context.SaveChangesAsync();
                }

                if (detailInfo.Content != model.Content)
                {
                    RegulationConformityAmLog Log = new RegulationConformityAmLog()
                    {
                        ItemId = detailInfo.RegulationConformityAmid,
                        UserId = model.UpdateUserId ?? Guid.Empty,
                        LogTime = DateTime.Now,
                        Property = "Nội dung",
                        OldValue = detailInfo.Content,
                        NewValue = model.Content,
                    };
                    await _context.RegulationConformityAmLogs.AddAsync(Log);
                    await _context.SaveChangesAsync();
                }

                detailInfo.DayReception = DateTime.ParseExact(model.DayReception, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
                detailInfo.EstablishmentId = model.EstablishmentId;
                detailInfo.DistrictId = model.DistrictId;
                detailInfo.Address = model.Address;
                detailInfo.Phone = model.Phone;
                detailInfo.Num = model.Num;
                detailInfo.ProductName = model.ProductName;
                detailInfo.Content = model.Content;
                detailInfo.DateOfPublication = DateTime.ParseExact(model.DateOfPublication, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
                detailInfo.Note = model.Note;
                detailInfo.UpdateUserId = model.UpdateUserId;
                detailInfo.UpdateTime = model.UpdateTime;

                _context.RegulationConformityAms.Update(detailInfo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid Id)
        {
            //Update status công bố
            var detailinfo = await _context.RegulationConformityAms.Where(d => d.RegulationConformityAmid == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            _context.RegulationConformityAms.Update(detailinfo);
            await _context.SaveChangesAsync();

            //Xóa Logs
            var Logs = _context.RegulationConformityAmLogs.Where(x => x.UserId == Id).ToList();
            _context.RegulationConformityAmLogs.RemoveRange(Logs);
            await _context.SaveChangesAsync();
        }

        public RegulationConformityAMModel FindById(Guid Id)
        {
            var result = _context.RegulationConformityAms.Where(x => x.RegulationConformityAmid == Id).Select(d => new RegulationConformityAMModel()
            {
                RegulationConformityAMId = d.RegulationConformityAmid,
                DayReception = d.DayReception.ToString("dd'/'MM'/'yyyy"),
                EstablishmentId = d.EstablishmentId,
                DistrictId = d.DistrictId,
                Address = d.Address,
                Phone = d.Phone,
                Num = d.Num,
                ProductName = d.ProductName,
                Content = d.Content,
                DateOfPublication = d.DateOfPublication.ToString("dd'/'MM'/'yyyy"),
                Note = d.Note,
                IsDel = d.IsDel,
            }).FirstOrDefault();

            if (result == null)
            {
                return new RegulationConformityAMModel();
            }

            return result;
        }

        public List<RegulationConformityAmLogModel> FindLogsById(Guid Id)
        {
            var result = (from l in _context.RegulationConformityAmLogs
                          where l.ItemId == Id
                          join u in _context.Users
                          on l.UserId equals u.UserId
                          select new RegulationConformityAmLogModel
                          {
                              LogId = l.LogId,
                              ItemId = l.ItemId,
                              UserId = l.UserId,
                              UserName = u.FullName,
                              LogTime = l.LogTime,
                              LogTimeDisplay = l.LogTime.AddHours(7).ToString("dd'/'MM'/'yyyy HH:mm"),
                              Property = l.Property,
                              OldValue = l.OldValue,
                              NewValue = l.NewValue,
                          }).OrderBy(x => x.LogTime).ToList();

            if (!result.Any())
            {
                return new List<RegulationConformityAmLogModel>();
            }

            return result;
        }
    }
}
