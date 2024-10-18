using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.SampleContractRepository
{
    public class SampleContractRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public SampleContractRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(SampleContract model)
        {
            await _context.SampleContracts.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task Update(SampleContract model)
        {
            _context.SampleContracts.Update(model);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(SampleContract model)
        {
            var db = await _context.SampleContracts.Where(d => d.SampleContractId == model.SampleContractId).FirstOrDefaultAsync();
            db.IsDel = model.IsDel;
            await _context.SaveChangesAsync();
        }

        public IQueryable<SampleContractModel> FindById(Guid Id)
        {
            var result = _context.SampleContracts.Where(x => x.SampleContractId == Id).Select(d => new SampleContractModel()
            {
                SampleContractId = d.SampleContractId,
                SampleContractField = d.SampleContractField,
                SampleContractFieldName = _context.Categories.Where(x => x.CategoryId == d.SampleContractField).FirstOrDefault().CategoryName ?? "",
                RegistrationTime = d.RegistrationTime,
                ProfileNumber = d.ProfileNumber,
                RegistrantName = d.RegistrantName,
                PhoneNumber = d.PhoneNumber,
                BusinessName = d.BusinessName,
                TaxCode = d.TaxCode,
                BusinessPhoneNumber = d.BusinessPhoneNumber,
                Address = d.Address,
                IsDel = d.IsDel
            }) ;

            return result;
        }

        public bool findByProfileNumber(string ProfileNumber, Guid? SampleContractId)
        {
            if (SampleContractId != null)
            {
                var Code = _context.SampleContracts.Where(x => x.SampleContractId == SampleContractId && x.ProfileNumber == ProfileNumber && !x.IsDel).FirstOrDefault();
                if (Code != null)
                {
                    return false;
                }
            }
            var isProfileNumber = _context.SampleContracts.Where(x => x.ProfileNumber == ProfileNumber && !x.IsDel).FirstOrDefault();
            if (isProfileNumber == null)
            {
                return false;
            }
            return true;
        }
    }
}
