using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.MultiLevelSalesManagementRepository
{
    public class MultiLevelSalesManagementRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public MultiLevelSalesManagementRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(MultiLevelSalesManagement model)
        {
            MultiLevelSalesManagement data = new MultiLevelSalesManagement()
            {
                MultiLevelSalesManagementId = model.MultiLevelSalesManagementId,
                BusinessId = model.BusinessId,
                StartDate = model.StartDate,
                YearReport = model.YearReport,
                MultiLevelSellingPlace = model.MultiLevelSellingPlace,
                ContactPersonName = model.ContactPersonName,
                ContactPersonPhoneNumber = model.ContactPersonPhoneNumber,
                ContactPersonAddress = model.ContactPersonAddress,
                Participants = model.Participants,
                NewParticipants = model.NewParticipants,
                Terminations = model.Terminations,
                BasicTrainings = model.BasicTrainings,
                Turnover = model.Turnover,
                Commission = model.Commission,
                PromotionalValue = model.PromotionalValue,
                TaxDeduction = model.TaxDeduction,
                BuyBackGoods = model.BuyBackGoods,
            };
            await _context.MultiLevelSalesManagements.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public async Task Update(MultiLevelSalesManagement model)
        {
            var detailinfo = await _context.MultiLevelSalesManagements.Where(d => d.MultiLevelSalesManagementId == model.MultiLevelSalesManagementId).FirstOrDefaultAsync();
            detailinfo.BusinessId = model.BusinessId;
            detailinfo.StartDate = model.StartDate;
            detailinfo.YearReport = model.YearReport;
            detailinfo.MultiLevelSellingPlace = model.MultiLevelSellingPlace;
            detailinfo.ContactPersonName = model.ContactPersonName;
            detailinfo.ContactPersonPhoneNumber = model.ContactPersonPhoneNumber;
            detailinfo.ContactPersonAddress = model.ContactPersonAddress;
            detailinfo.Participants = model.Participants;
            detailinfo.NewParticipants = model.NewParticipants;
            detailinfo.Terminations = model.Terminations;
            detailinfo.BasicTrainings = model.BasicTrainings;
            detailinfo.Turnover = model.Turnover;
            detailinfo.Commission = model.Commission;
            detailinfo.PromotionalValue = model.PromotionalValue;
            detailinfo.TaxDeduction = model.TaxDeduction;
            detailinfo.BuyBackGoods = model.BuyBackGoods;

            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var db = await _context.MultiLevelSalesManagements.Where(d => d.MultiLevelSalesManagementId == Id).FirstOrDefaultAsync();
            db.IsDel = true;
            await _context.SaveChangesAsync();
        }

        //public async Task Delete(Guid id)
        //{
        //    var itemRemove = await _context.MultiLevelSalesManagement.Where(x => x.MultiLevelSalesManagementId == id).FirstOrDefaultAsync();
        //    _context.MultiLevelSalesManagement.Remove(itemRemove);
        //    await _context.SaveChangesAsync();
        //}

        //public IQueryable<MultiLevelSalesManagement> FindAll()
        //{
        //    var result = _context.MultiLevelSalesManagements.Select(d => new MultiLevelSalesManagement()
        //    {
        //        MultiLevelSalesManagementId = d.MultiLevelSalesManagementId,
        //        MultiLevelSalesManagementCode = d.MultiLevelSalesManagementCode,
        //        MultiLevelSalesManagementName = d.MultiLevelSalesManagementName,
        //        IsDel = d.IsDel,
        //    });

        //    return result;
        //}

        public IQueryable<MultiLevelSalesManagementModel> FindById(Guid Id)
        {
            var result = _context.MultiLevelSalesManagements.Where(x => x.MultiLevelSalesManagementId == Id).Select(d => new MultiLevelSalesManagementModel()
            {
                MultiLevelSalesManagementId = d.MultiLevelSalesManagementId,
                BusinessId = d.BusinessId,
                BusinessName = _context.Businesses.Where(x => x.BusinessId == d.BusinessId).FirstOrDefault().BusinessNameVi ?? "",
                StartDate = d.StartDate,
                YearReport = d.YearReport,
                MultiLevelSellingPlace = d.MultiLevelSellingPlace,
                ContactPersonName = d.ContactPersonName,
                ContactPersonPhoneNumber = d.ContactPersonPhoneNumber,
                ContactPersonAddress = d.ContactPersonAddress,
                Participants = d.Participants,
                NewParticipants = d.NewParticipants,
                Terminations = d.Terminations,
                BasicTrainings = d.BasicTrainings,
                Turnover = d.Turnover,
                Commission = d.Commission,
                PromotionalValue = d.PromotionalValue,
                TaxDeduction = d.TaxDeduction,
                BuyBackGoods = d.BuyBackGoods,
                IsDel = d.IsDel,
            });

            return result;
        }
    }
}
