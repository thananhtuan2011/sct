using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class CateManageAncolLocalBussinesRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CateManageAncolLocalBussinesRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(CateManageAncolLocalBussinesModel model)
        {
            CateManageAncolLocalBussine data = new CateManageAncolLocalBussine()
            {
                CateManageAncolLocalBussinessId = model.CateManageAncolLocalBussinessId,
                BusinessId = model.BusinessId,
                DateChange = model.DateChange,
                DateRelease = model.DateRelease,
                Investment = model.Investment,
                NumberOfWorker = model.NumberOfWorker,
                TypeOfProfessionId = model.TypeOfProfessionId,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.CateManageAncolLocalBussines.AddAsync(data);
            await _context.SaveChangesAsync();
            List<CateManageAncolLocalBussinesDetail> lstworkers = new List<CateManageAncolLocalBussinesDetail>();
            foreach (var item in model.LstWorkers)
            {
                CateManageAncolLocalBussinesDetail workers = new CateManageAncolLocalBussinesDetail()
                {
                    CateManageAncolLocalBussinessId = data.CateManageAncolLocalBussinessId,
                    Fullname = item.Fullname,
                    Type = item.Type
                };
                lstworkers.Add(workers);
            }
            List<CateManageAncolLocalBussinesTypeOfProfession> lstprofession = new List<CateManageAncolLocalBussinesTypeOfProfession>();
            foreach (var item in model.LstProfession)
            {
                CateManageAncolLocalBussinesTypeOfProfession profession = new CateManageAncolLocalBussinesTypeOfProfession()
                {
                    CateManageAncolLocalBussinessId = data.CateManageAncolLocalBussinessId,
                    TypeOfProfessionId = item.TypeOfProfessionId
                };
                lstprofession.Add(profession);
            }
            await _context.CateManageAncolLocalBussinesDetails.AddRangeAsync(lstworkers);
            await _context.CateManageAncolLocalBussinesTypeOfProfessions.AddRangeAsync(lstprofession);
            await _context.SaveChangesAsync();
        }
        public async Task Update(CateManageAncolLocalBussinesModel model)
        {
            var detailinfo = await _context.CateManageAncolLocalBussines.Where(d => d.CateManageAncolLocalBussinessId == model.CateManageAncolLocalBussinessId).FirstOrDefaultAsync();
            detailinfo.BusinessId = model.BusinessId;
            detailinfo.DateChange = model.DateChange;
            detailinfo.DateRelease = model.DateRelease;
            detailinfo.Investment = model.Investment;
            detailinfo.NumberOfWorker = model.NumberOfWorker;
            detailinfo.TypeOfProfessionId = model.TypeOfProfessionId;
            detailinfo.IsActive = model.IsActive;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            List<CateManageAncolLocalBussinesDetail> lstworkers = new List<CateManageAncolLocalBussinesDetail>();
            List<CateManageAncolLocalBussinesDetail> lstworkers_del = new List<CateManageAncolLocalBussinesDetail>();
            foreach (var item in model.LstWorkers)
            {
                if (!item.IsDel && item.CateManageAncolLocalBussinesDetailId == null)
                {
                    CateManageAncolLocalBussinesDetail workers = new CateManageAncolLocalBussinesDetail()
                    {
                        CateManageAncolLocalBussinessId = model.CateManageAncolLocalBussinessId,
                        Fullname = item.Fullname,
                        Type = item.Type
                    };
                    lstworkers.Add(workers);
                }
                else if (item.IsDel && item.CateManageAncolLocalBussinesDetailId != null)
                {
                    //CateManageAncolLocalBussinesDetail workers = new CateManageAncolLocalBussinesDetail()
                    //{
                    //    CateManageAncolLocalBussinessId = model.CateManageAncolLocalBussinessId,
                    //    Fullname = item.Fullname,
                    //    Type = item.Type
                    //};
                    var workers = _context.CateManageAncolLocalBussinesDetails
                                  .Where(x => x.CateManageAncolLocalBussinesDetailId == item.CateManageAncolLocalBussinesDetailId
                                              && x.Fullname == item.Fullname
                                              && x.Type == item.Type).FirstOrDefault();
                    lstworkers_del.Add(workers);
                }
            }
            await _context.CateManageAncolLocalBussinesDetails.AddRangeAsync(lstworkers);
            _context.CateManageAncolLocalBussinesDetails.RemoveRange(lstworkers_del);

            List<CateManageAncolLocalBussinesTypeOfProfession> lstprofession = new List<CateManageAncolLocalBussinesTypeOfProfession>();
            List<CateManageAncolLocalBussinesTypeOfProfession> lstprofession_del = new List<CateManageAncolLocalBussinesTypeOfProfession>();

            foreach (var item in model.LstProfession)
            {
                if (!item.IsDel && item.CateManageAncolLocalBussinesTypeProfessionId == null)
                {
                    CateManageAncolLocalBussinesTypeOfProfession profession = new CateManageAncolLocalBussinesTypeOfProfession()
                    {
                        CateManageAncolLocalBussinesTypeProfessionId = Guid.Empty,
                        CateManageAncolLocalBussinessId = model.CateManageAncolLocalBussinessId,
                        TypeOfProfessionId = item.TypeOfProfessionId
                    };
                    lstprofession.Add(profession);
                }
                else if (item.IsDel)
                {
                    //CateManageAncolLocalBussinesTypeOfProfession profession = new CateManageAncolLocalBussinesTypeOfProfession()
                    //{
                    //    CateManageAncolLocalBussinesTypeProfessionId = (Guid)item.CateManageAncolLocalBussinesTypeProfessionId,
                    //    CateManageAncolLocalBussinessId = model.CateManageAncolLocalBussinessId,
                    //    TypeOfProfessionId = item.TypeOfProfessionId,

                    //};

                    var profession_remove = _context.CateManageAncolLocalBussinesTypeOfProfessions
                                            .Where(x => x.CateManageAncolLocalBussinesTypeProfessionId == item.CateManageAncolLocalBussinesTypeProfessionId).FirstOrDefault();

                    lstprofession_del.Add(profession_remove);
                }
            }
            await _context.CateManageAncolLocalBussinesTypeOfProfessions.AddRangeAsync(lstprofession);
            _context.CateManageAncolLocalBussinesTypeOfProfessions.RemoveRange(lstprofession_del);

            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.CateManageAncolLocalBussines.Where(d => d.CateManageAncolLocalBussinessId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;

            var lstworkers = _context.CateManageAncolLocalBussinesDetails.Where(d => d.CateManageAncolLocalBussinessId == Id).ToList();
            var lstprofession = _context.CateManageAncolLocalBussinesTypeOfProfessions.Where(d => d.CateManageAncolLocalBussinessId == Id).ToList();

            _context.CateManageAncolLocalBussinesDetails.RemoveRange(lstworkers);
            _context.CateManageAncolLocalBussinesTypeOfProfessions.RemoveRange(lstprofession);

            await _context.SaveChangesAsync();
        }
        public async Task Deletes(List<Guid> Ids)
        {
            List<CateManageAncolLocalBussine> items = new List<CateManageAncolLocalBussine>();
            foreach (var idremove in Ids)
            {
                CateManageAncolLocalBussine item = new CateManageAncolLocalBussine();
                var detailinfo = await _context.CateManageAncolLocalBussines.Where(d => d.CateManageAncolLocalBussinessId == idremove).FirstOrDefaultAsync();
                item.CateManageAncolLocalBussinessId = idremove;
                item.IsDel = true;
                items.Add(item);

                var lstworkers = _context.CateManageAncolLocalBussinesDetails.Where(d => d.CateManageAncolLocalBussinessId == idremove).ToList();
                var lstprofession = _context.CateManageAncolLocalBussinesTypeOfProfessions.Where(d => d.CateManageAncolLocalBussinessId == idremove).ToList();
                _context.CateManageAncolLocalBussinesDetails.RemoveRange(lstworkers);
                _context.CateManageAncolLocalBussinesTypeOfProfessions.RemoveRange(lstprofession);
            }
            _context.CateManageAncolLocalBussines.UpdateRange(items);
            await _context.SaveChangesAsync();
        }
        public CateManageAncolLocalBussinesModel FindById(Guid Id)
        {
            var result = _context.CateManageAncolLocalBussines.Where(x => x.CateManageAncolLocalBussinessId == Id && !x.IsDel).Select(d => new CateManageAncolLocalBussinesModel()
            {
                BusinessId = d.BusinessId,
                CateManageAncolLocalBussinessId = d.CateManageAncolLocalBussinessId,
                DateChange = d.DateChange,
                DateRelease = d.DateRelease,
                Investment = d.Investment,
                NumberOfWorker = d.NumberOfWorker,
                TypeOfProfessionId = d.TypeOfProfessionId,
                IsAction = d.IsAction,
                IsActive = d.IsActive,
            }).FirstOrDefault();

            if (result == null)
            {
                return new CateManageAncolLocalBussinesModel();
            }

            var LstWorkers = _context.CateManageAncolLocalBussinesDetails.Where(x => x.CateManageAncolLocalBussinessId == Id).ToList();
            var LstProfession = _context.CateManageAncolLocalBussinesTypeOfProfessions.Where(x => x.CateManageAncolLocalBussinessId == Id).ToList();

            List<CateManageAncolLocalBussinesDetailModel> workers = new List<CateManageAncolLocalBussinesDetailModel>();
            foreach (var item in LstWorkers)
            {
                CateManageAncolLocalBussinesDetailModel worker = new CateManageAncolLocalBussinesDetailModel()
                {
                    CateManageAncolLocalBussinesDetailId = item.CateManageAncolLocalBussinesDetailId,
                    Fullname = item.Fullname,
                    Type = item.Type,
                    IsDel = false
                };
                workers.Add(worker);
            }
            List<CateManageAncolLocalBussinesTypeOfProfessionModel> profession = new List<CateManageAncolLocalBussinesTypeOfProfessionModel>();
            foreach (var item in LstProfession)
            {
                CateManageAncolLocalBussinesTypeOfProfessionModel profe = new CateManageAncolLocalBussinesTypeOfProfessionModel()
                {
                    CateManageAncolLocalBussinesTypeProfessionId = item.CateManageAncolLocalBussinesTypeProfessionId,
                    TypeOfProfessionId = item.TypeOfProfessionId,
                    IsDel = false
                };
                profession.Add(profe);
            }
            result.LstWorkers = workers;
            result.LstProfession = profession;

            return result;
        }

        public List<Business> GetListBusiness()
        {
            var bus = _context.Businesses.Where(x => !x.IsDel).ToList();
            return bus;
        }

        public List<TypeOfProfession> GetListTypeOfProfession()
        {
            var profess = _context.TypeOfProfessions.Where(x => !x.IsDel).ToList();
            return profess;
        }

        public List<ExportCateManageAncolLocalBussinesModel> FindData(QueryRequestBody query)
        {
            List<ExportCateManageAncolLocalBussinesModel> result = new List<ExportCateManageAncolLocalBussinesModel>();

            var _data = (from cc in _context.CateManageAncolLocalBussines
                         where !cc.IsDel
                         join pro in _context.TypeOfProfessions on cc.TypeOfProfessionId equals pro.TypeOfProfessionId into joinPro
                         from profess in joinPro.DefaultIfEmpty()
                         join bus in _context.Businesses on cc.BusinessId equals bus.BusinessId into joinBus
                         from busi in joinBus.DefaultIfEmpty()
                         join tob in _context.TypeOfBusinesses on busi.LoaiHinhDoanhNghiep equals tob.TypeOfBusinessId into joinTob
                         from tyob in joinTob.DefaultIfEmpty()
                         join dis in _context.Districts on busi.DistrictId equals dis.DistrictId into joinDis
                         from dist in joinDis.DefaultIfEmpty()
                         join com in _context.Communes on busi.CommuneId equals com.CommuneId into joinCom
                         from comm in joinCom.DefaultIfEmpty()
                         select new ExportCateManageAncolLocalBussinesModel
                         {
                             CateManageAncolLocalBussinessId = cc.CateManageAncolLocalBussinessId,
                             BusinessCode = busi.BusinessCode,
                             BusinessName = busi.BusinessNameVi,
                             Address = busi.DiaChiTruSo ?? "",
                             District = dist.DistrictName,
                             Commune = comm.CommuneName,
                             Investment = cc.Investment,
                             ActionName = cc.IsActive.GetValueOrDefault() ? "Đang hoạt động" : "Tạm ngưng",
                             PhoneNumber = busi.SoDienThoai,
                             Email = busi.Email,
                             RepresentName = busi.NguoiDaiDien ?? "",
                             RepresentBirthDay = busi.NgaySinh.HasValue ? busi.NgaySinh.Value.ToString("dd/MM/yyyy") : "",
                             CitizenId = busi.Cccd ?? "",
                             IssuerCitizenId = busi.NoiCap ?? "",
                             CitizenIdDate = busi.NgayCap.HasValue ? busi.NgayCap.Value.ToString("dd/MM/yyyy") : "",
                             Owner = busi.GiamDoc,
                             TypeOfProfessionName = profess.TypeOfProfessionName ?? "",
                             DateReleaseDisplay = cc.DateRelease.ToString("dd/MM/yyyy"),
                             DateRelease = cc.DateRelease,
                             DateChangeDisplay = cc.DateChange.HasValue ? cc.DateChange.Value.ToString("dd/MM/yyyy") : "",
                             TypeOfBusinessName = tyob.TypeOfBusinessName,
                             NumberOfWorker = cc.NumberOfWorker,
                         }).ToList()
                        .GroupJoin(_context.CateManageAncolLocalBussinesDetails,
                         cm => cm.CateManageAncolLocalBussinessId,
                         lw => lw.CateManageAncolLocalBussinessId,
                         (cm, JoinLw) => new ExportCateManageAncolLocalBussinesModel
                         {
                             CateManageAncolLocalBussinessId = cm.CateManageAncolLocalBussinessId,
                             BusinessCode = cm.BusinessCode,
                             BusinessName = cm.BusinessName,
                             Address = cm.Address ?? "",
                             District = cm.District,
                             Commune = cm.Commune,
                             Investment = cm.Investment,
                             ActionName = cm.ActionName,
                             PhoneNumber = cm.PhoneNumber,
                             Email = cm.Email,
                             RepresentName = cm.RepresentName,
                             RepresentBirthDay = cm.RepresentBirthDay,
                             CitizenId = cm.CitizenId,
                             IssuerCitizenId = cm.IssuerCitizenId,
                             CitizenIdDate = cm.CitizenIdDate,
                             Owner = cm.Owner,
                             TypeOfProfessionName = cm.TypeOfProfessionName,
                             DateReleaseDisplay = cm.DateReleaseDisplay,
                             DateRelease = cm.DateRelease,
                             DateChangeDisplay = cm.DateChangeDisplay,
                             TypeOfBusinessName = cm.TypeOfBusinessName,
                             NumberOfWorker = cm.NumberOfWorker,
                             LstCapitalContributing = string.Join(", ", JoinLw.Where(x => x.Type == 1).Select(x => x.Fullname).ToList()),
                             LstShareholder = string.Join(", ", JoinLw.Where(x => x.Type == 2).Select(x => x.Fullname).ToList())
                         }).ToList()
                         .GroupJoin(
                            _context.CateManageAncolLocalBussinesTypeOfProfessions.Join(_context.TypeOfProfessions, cma => cma.TypeOfProfessionId, top => top.TypeOfProfessionId, (cma, top) => new
                            {
                                cma.CateManageAncolLocalBussinessId,
                                top.TypeOfProfessionName
                            }), cma => cma.CateManageAncolLocalBussinessId, cmap => cmap.CateManageAncolLocalBussinessId, 
                            (cma, JoinToP) => new ExportCateManageAncolLocalBussinesModel
                            {
                                CateManageAncolLocalBussinessId = cma.CateManageAncolLocalBussinessId,
                                BusinessCode = cma.BusinessCode,
                                BusinessName = cma.BusinessName,
                                Address = cma.Address ?? "",
                                District = cma.District,
                                Commune = cma.Commune,
                                Investment = cma.Investment,
                                ActionName = cma.ActionName,
                                PhoneNumber = cma.PhoneNumber,
                                Email = cma.Email,
                                RepresentName = cma.RepresentName,
                                RepresentBirthDay = cma.RepresentBirthDay,
                                CitizenId = cma.CitizenId,
                                IssuerCitizenId = cma.IssuerCitizenId,
                                CitizenIdDate = cma.CitizenIdDate,
                                Owner = cma.Owner,
                                TypeOfProfessionName = cma.TypeOfProfessionName,
                                LstProfession = string.Join(", ",JoinToP.Select(x => x.TypeOfProfessionName).ToList()),
                                DateReleaseDisplay = cma.DateReleaseDisplay,
                                DateRelease = cma.DateRelease,
                                DateChangeDisplay = cma.DateChangeDisplay,
                                TypeOfBusinessName = cma.TypeOfBusinessName,
                                NumberOfWorker = cma.NumberOfWorker,
                                LstCapitalContributing = cma.LstCapitalContributing,
                                LstShareholder = cma.LstCapitalContributing,
                            }
                            ).ToList().AsQueryable();

            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                x.BusinessName.ToLower().Contains(_keywordSearch)
                || x.BusinessCode.ToLower().Contains(_keywordSearch)
                || x.RepresentName.ToLower().Contains(_keywordSearch)
                || x.Investment.ToString().Contains(_keywordSearch)
                || x.TypeOfProfessionName.ToLower().Contains(_keywordSearch)
                || x.NumberOfWorker.ToString().Contains(_keywordSearch)
                || x.ActionName.ToLower().Contains(_keywordSearch)
                );
            }

            //Filter
            if (query.Filter != null && query.Filter.ContainsKey("YearReport"))
            {
                _data = _data.Where(x => x.DateRelease.Year.ToString() == query.Filter["YearReport"]);
            }
            else
            {
                _data = _data.Where(x => x.DateRelease.Year == DateTime.Now.Year);
            }

            result = _data.ToList();

            return result;
        }
    }
}
