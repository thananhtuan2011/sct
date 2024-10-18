using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories
{
    public class BusinessMultiLevelRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public BusinessMultiLevelRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(BusinessMultiLevelModel model)
        {
             BusinessMultiLevel data = new  BusinessMultiLevel()
             {
                 BusinessMultiLevelId = model. BusinessMultiLevelId,
                 BusinessId = model.BusinessId,
                 DistrictId = model.DistrictId,
                 Address = model.Address,
                 StartDate = model.StartDate,
                 Status = model.Status,
                 NumCert = model.NumCert,
                 CertDate = model.CertDate,
                 CertExp = model.CertExp,
                 Contact = model.Contact,
                 PhoneNumber = model.PhoneNumber,
                 AddressContact = model.AddressContact,
                 Goods = model.Goods,
                 LocalConfirm = model.LocalConfirm,
                 Note = model.Note,
                 CreateTime = model.CreateTime,
                 CreateUserId = model.CreateUserId
             };
            await _context. BusinessMultiLevels.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task Update( BusinessMultiLevelModel model)
        {
            var detailinfo = await _context. BusinessMultiLevels.Where(d => d. BusinessMultiLevelId == model. BusinessMultiLevelId).FirstOrDefaultAsync();
            if(detailinfo != null)
            {
                detailinfo.BusinessId = model.BusinessId;
                detailinfo.DistrictId = model.DistrictId;
                detailinfo.Address = model.Address;
                detailinfo.StartDate = model.StartDate;
                detailinfo.Status = model.Status;
                detailinfo.NumCert = model.NumCert;
                detailinfo.CertDate = model.CertDate;
                detailinfo.CertExp = model.CertExp;
                detailinfo.Contact = model.Contact;
                detailinfo.PhoneNumber = model.PhoneNumber;
                detailinfo.AddressContact = model.AddressContact;
                detailinfo.Goods = model.Goods;
                detailinfo.LocalConfirm = model.LocalConfirm;
                detailinfo.Note = model.Note;
                detailinfo.UpdateTime = model.UpdateTime;
                detailinfo.UpdateUserId = model.UpdateUserId;
            }

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var db = await _context. BusinessMultiLevels.Where(d => d. BusinessMultiLevelId == Id).FirstOrDefaultAsync();
            db.IsDel = true;
            await _context.SaveChangesAsync();
        }

        public IQueryable< BusinessMultiLevelModel> FindById(Guid Id)
        {
            var result = _context. BusinessMultiLevels.Where(x => x. BusinessMultiLevelId == Id).Select(d => new  BusinessMultiLevelModel()
            {
                BusinessMultiLevelId = d.BusinessMultiLevelId,
                BusinessId = d.BusinessId,
                DistrictId = d.DistrictId,
                Address = d.Address,
                StartDate = d.StartDate,
                Status = d.Status,
                NumCert = d.NumCert,
                CertDate = d.CertDate,
                CertExp = d.CertExp,
                Contact = d.Contact,
                PhoneNumber = d.PhoneNumber,
                AddressContact = d.AddressContact,
                Goods = d.Goods,
                LocalConfirm = d.LocalConfirm,
                Note = d.Note,
                IsDel = d.IsDel,
            });

            return result;
        }
    }
}

