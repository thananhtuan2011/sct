using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.MultiLevelSalesParticipantsRepository
{
    public class MultiLevelSalesParticipantsRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public MultiLevelSalesParticipantsRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(MultiLevelSalesParticipant model)
        {
            await _context.MultiLevelSalesParticipants.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(MultiLevelSalesParticipant model)
        {
            _context.MultiLevelSalesParticipants.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(MultiLevelSalesParticipant model)
        {
            var db = await _context.MultiLevelSalesParticipants.Where(d => d.MultiLevelSalesParticipantsId == model.MultiLevelSalesParticipantsId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public IQueryable<MultiLevelSalesParticipant> FindById(Guid Id)
        {
            var result = _context.MultiLevelSalesParticipants.Where(x => x.MultiLevelSalesParticipantsId == Id).Select(d => new MultiLevelSalesParticipant()
            {
                MultiLevelSalesParticipantsId = d.MultiLevelSalesParticipantsId,
                MultiLevelSalesParticipantsCode = d.MultiLevelSalesParticipantsCode,
                ParticipantsName = d.ParticipantsName,
                Birthday = d.Birthday,
                PhoneNumber = d.PhoneNumber,
                IdentityCardNumber = d.IdentityCardNumber,
                DateOfIssuance = d.DateOfIssuance,
                PlaceOfIssue = d.PlaceOfIssue,
                Gender = d.Gender,
                JoinDate = d.JoinDate,
                Province = d.Province,
                Address = d.Address,
                IsDel = d.IsDel,
            });

            return result;
        }

        public bool findByMultiLevelSalesParticipantsCode(string MultiLevelSalesParticipantsCode, Guid? MultiLevelSalesParticipantsId)
        {
            if (MultiLevelSalesParticipantsId != null)
            {
                var Code = _context.MultiLevelSalesParticipants.Where(x => x.MultiLevelSalesParticipantsId == MultiLevelSalesParticipantsId && x.MultiLevelSalesParticipantsCode == MultiLevelSalesParticipantsCode && !x.IsDel).FirstOrDefault();
                if (Code != null)
                {
                    return false;
                }
            }
            var isMultiLevelSalesParticipantsCode = _context.MultiLevelSalesParticipants.Where(x => x.MultiLevelSalesParticipantsCode == MultiLevelSalesParticipantsCode && !x.IsDel).FirstOrDefault();
            if (isMultiLevelSalesParticipantsCode == null)
            {
                return false;
            }
            return true;
        }
    }
}
