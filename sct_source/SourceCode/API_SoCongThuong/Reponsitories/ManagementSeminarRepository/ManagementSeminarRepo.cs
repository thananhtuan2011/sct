using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class ManagementSeminarRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ManagementSeminarRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(ManagementSeminarModel model)
        {
            ManagementSeminar data = new ManagementSeminar()
            {
                BusinessId = model.BusinessId,
                ProfileCode = model.ProfileCode,
                Title = model.Title,
                DistrictId = model.DistrictId,
                Address = model.Address,
                Contact = model.Contact,
                PhoneNumber = model.PhoneNumber,
                NumberParticipant = model.NumberParticipant,
                Note = model.Note,
                CreateTime = model.CreateTime,
                CreateUserId = model.CreateUserId
            };
            await _context.ManagementSeminars.AddAsync(data);
            await _context.SaveChangesAsync();

            if(model.listTime.Count > 0)
            {
                List<TimeManagementSeminar> listTimeData = new List<TimeManagementSeminar>();
                foreach(var item in model.listTime)
                {
                    TimeManagementSeminar timeData = new TimeManagementSeminar();
                    timeData.ManagementSeminarId = data.ManagementSeminarId;
                    timeData.StartTime = item.StartTime;
                    timeData.EndTime = item.EndTime;
                    timeData.CreateUserId = model.CreateUserId;
                    timeData.CreateTime = model.CreateTime;
                    listTimeData.Add(timeData);
                }
                await _context.TimeManagementSeminars.AddRangeAsync(listTimeData);
                await _context.SaveChangesAsync();
            }
        }
        public async Task Update(ManagementSeminarModel model)
        {
            var detailinfo = await _context.ManagementSeminars.Where(d => d.ManagementSeminarId == model.ManagementSeminarId).FirstOrDefaultAsync();
            if(detailinfo != null)
            {
                detailinfo.BusinessId = model.BusinessId;
                detailinfo.ProfileCode = model.ProfileCode;
                detailinfo.Title = model.Title;
                detailinfo.DistrictId = model.DistrictId;
                detailinfo.Address = model.Address;
                detailinfo.Contact = model.Contact;
                detailinfo.PhoneNumber = model.PhoneNumber;
                detailinfo.NumberParticipant = model.NumberParticipant;
                detailinfo.Note = model.Note;
                detailinfo.UpdateTime = model.UpdateTime;
                detailinfo.UpdateUserId = model.UpdateUserId;
                await _context.SaveChangesAsync();
            }

            var listDel = _context.TimeManagementSeminars.Where(x => x.ManagementSeminarId == model.ManagementSeminarId).ToList();
            if (listDel.Count > 0)
            {
                _context.TimeManagementSeminars.RemoveRange(listDel);
                await _context.SaveChangesAsync();
            }

            if (model.listTime.Count > 0)
            {
                List<TimeManagementSeminar> listTimeData = new List<TimeManagementSeminar>();
                foreach (var item in model.listTime)
                {
                    TimeManagementSeminar timeData = new TimeManagementSeminar();
                    timeData.ManagementSeminarId = model.ManagementSeminarId;
                    timeData.StartTime = item.StartTime;
                    timeData.EndTime = item.EndTime;
                    timeData.CreateUserId = model.CreateUserId;
                    timeData.CreateTime = model.CreateTime;
                    listTimeData.Add(timeData);
                }
                await _context.TimeManagementSeminars.AddRangeAsync(listTimeData);
                await _context.SaveChangesAsync();
            }

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.ManagementSeminars.Where(d => d.ManagementSeminarId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            var listDel = _context.TimeManagementSeminars.Where(x => x.ManagementSeminarId == Id).ToList();
            if (listDel.Count > 0)
            {
                _context.TimeManagementSeminars.RemoveRange(listDel);
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
        }

        public ManagementSeminarModel FindById(Guid Id)
        {
            var result = _context.ManagementSeminars.Where(x => x.ManagementSeminarId == Id && !x.IsDel).Select(model => new ManagementSeminarModel()
            {
                BusinessId = model.BusinessId,
                ProfileCode = model.ProfileCode,
                Title = model.Title,
                DistrictId = model.DistrictId,
                Address = model.Address,
                Contact = model.Contact,
                PhoneNumber = model.PhoneNumber,
                NumberParticipant = model.NumberParticipant,
                Note = model.Note,
                ManagementSeminarId = model.ManagementSeminarId
            }).FirstOrDefault();

            if (result != null)
            {
                List<TimeManagementSeminarModel> listTime = _context.TimeManagementSeminars.Where(x => x.ManagementSeminarId == Id && !x.IsDel).Select(model => new TimeManagementSeminarModel()
                {
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                }).ToList();
                result.listTime = listTime;
            }
          
            return result;
        }
    }
}

