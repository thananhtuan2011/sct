using EF_Core.Models;
using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore;

namespace API_SoCongThuong.Reponsitories.ReportAdministrativeProceduresRepository
{
    public class ReportAdministrativeProceduresRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public ReportAdministrativeProceduresRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public async Task Insert(ReportAdministrativeProceduresModel model)
        {
            ReportAdministrativeProcedure data = new ReportAdministrativeProcedure()
            {
                Period = model.Period,
                Year = model.Year,
                AdministrativeProceduresField = model.AdministrativeProceduresField,
                OnlineInPeriod = model.OnlineInPeriod,
                OfflineInPeriod = model.OfflineInPeriod,
                FromPreviousPeriod = model.FromPreviousPeriod,
                OnTimeProcessed = model.OnTimeProcessed,
                OutOfDateProcessed = model.OutOfDateProcessed,
                BeforeDeadlineProcessed = model.BeforeDeadlineProcessed,
                OnTimeProcessing = model.OnTimeProcessing,
                OutOfDateProcessing = model.OutOfDateProcessing,
                CreateTime = (DateTime)model.CreateTime!,
                CreateUserId = (Guid)model.CreateUserId!,
            };
            await _context.ReportAdministrativeProcedures.AddAsync(data);
            await _context.SaveChangesAsync();
        }

        public IQueryable<ReportAdministrativeProceduresModel> FindById(Guid Id)
        {
            var result = _context.ReportAdministrativeProcedures.Where(x => x.ReportId == Id && !x.IsDel).Select(f => new ReportAdministrativeProceduresModel()
            {
                ReportId = f.ReportId,
                Period = f.Period,
                Year = f.Year,
                AdministrativeProceduresField = f.AdministrativeProceduresField,
                //Nhận
                OnlineInPeriod = f.OnlineInPeriod,
                OfflineInPeriod = f.OfflineInPeriod,
                FromPreviousPeriod = f.FromPreviousPeriod,
                //Đã xữ lý
                OnTimeProcessed = f.OnTimeProcessed,
                OutOfDateProcessed = f.OutOfDateProcessed,
                BeforeDeadlineProcessed = f.BeforeDeadlineProcessed,
                //Đang xữ lý
                OnTimeProcessing = f.OnTimeProcessing,
                OutOfDateProcessing = f.OutOfDateProcessing
            });

            return result;
        }

        public async Task Update(ReportAdministrativeProceduresModel model)
        {
            var detailInfo = await _context.ReportAdministrativeProcedures.Where(d => d.ReportId == model.ReportId).FirstOrDefaultAsync();
            if (detailInfo != null)
            {
                detailInfo.Period = model.Period;
                detailInfo.Year = model.Year;
                detailInfo.AdministrativeProceduresField = model.AdministrativeProceduresField;
                detailInfo.OnlineInPeriod = model.OnlineInPeriod;
                detailInfo.OfflineInPeriod = model.OfflineInPeriod;
                detailInfo.FromPreviousPeriod = model.FromPreviousPeriod;
                detailInfo.OnTimeProcessed = model.OnTimeProcessed;
                detailInfo.OutOfDateProcessed = model.OutOfDateProcessed;
                detailInfo.BeforeDeadlineProcessed = model.BeforeDeadlineProcessed;
                detailInfo.OnTimeProcessing = model.OnTimeProcessing;
                detailInfo.OutOfDateProcessing = model.OutOfDateProcessing;
                detailInfo.UpdateTime = (DateTime)model.UpdateTime!;
                detailInfo.UpdateUserId = (Guid)model.UpdateUserId!;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid Id)
        {
            var db = await _context.ReportAdministrativeProcedures.Where(d => d.ReportId == Id).FirstOrDefaultAsync();
            db.IsDel = true;
            await _context.SaveChangesAsync();
        }

        public List<ResultReportAdministrativeProceduresModel> FindData(int year, int periods)
        {
            #region Code cũ
            //Set Time & Periods:
            //DateTime MinDate = new DateTime();
            //DateTime MaxDate = new DateTime();
            //DateTime MinDatePrevious = new DateTime();
            //DateTime MaxDatePrevious = new DateTime();
            //DateTime Now = DateTime.Now.Date;

            //if (periods == 1)
            //{
            //    MinDate = new DateTime(year - 1, 12, 15).Date;
            //    MaxDate = new DateTime(year, 3, 14).Date;
            //    MinDatePrevious = new DateTime(year - 1, 9, 15).Date;
            //    MaxDatePrevious = new DateTime(year - 1, 12, 14).Date;
            //}
            //else if (periods == 2)
            //{
            //    MinDate = new DateTime(year, 3, 15).Date;
            //    MaxDate = new DateTime(year, 6, 14).Date;
            //    MinDatePrevious = new DateTime(year - 1, 12, 15).Date;
            //    MaxDatePrevious = new DateTime(year, 3, 14).Date;
            //}
            //else if (periods == 3)
            //{
            //    MinDate = new DateTime(year, 6, 15).Date;
            //    MaxDate = new DateTime(year, 9, 14).Date;
            //    MinDatePrevious = new DateTime(year, 3, 15).Date;
            //    MaxDatePrevious = new DateTime(year, 6, 14).Date;
            //}
            //else if (periods == 4)
            //{
            //    MinDate = new DateTime(year, 9, 15).Date;
            //    MaxDate = new DateTime(year, 12, 14).Date;
            //    MinDatePrevious = new DateTime(year, 6, 15).Date;
            //    MaxDatePrevious = new DateTime(year, 9, 14).Date;
            //}
            //else
            //{
            //    MinDate = new DateTime(year - 1, 12, 15).Date;
            //    MaxDate = new DateTime(year, 12, 14).Date;
            //    MinDatePrevious = new DateTime(year - 2, 12, 15).Date;
            //    MaxDatePrevious = new DateTime(year - 1, 12, 14).Date;
            //}
            #endregion

            #region Code cũ
            //var dateRanges = new Dictionary<int, Tuple<DateTime, DateTime, DateTime, DateTime>>()
            //{
            //    {1, Tuple.Create(new DateTime(year - 1, 12, 15), new DateTime(year, 3, 14), new DateTime(year - 1, 9, 15), new DateTime(year - 1, 12, 14))},
            //    {2, Tuple.Create(new DateTime(year, 3, 15), new DateTime(year, 6, 14), new DateTime(year - 1, 12, 15), new DateTime(year, 3, 14))},
            //    {3, Tuple.Create(new DateTime(year, 6, 15), new DateTime(year, 9, 14), new DateTime(year, 3, 15), new DateTime(year, 6, 14))},
            //    {4, Tuple.Create(new DateTime(year, 9, 15), new DateTime(year, 12, 14), new DateTime(year, 6, 15), new DateTime(year, 9, 14))},
            //    {5, Tuple.Create(new DateTime(year - 1, 12, 15), new DateTime(year, 12, 14), new DateTime(year - 2, 12, 15), new DateTime(year - 1, 12, 14))}
            //};

            //var dateRange = dateRanges[periods];
            //var MinDate = dateRange.Item1.Date;
            //var MaxDate = dateRange.Item2.Date;
            //var MinDatePrevious = dateRange.Item3.Date;
            //var MaxDatePrevious = dateRange.Item4.Date;
            //var Now = DateTime.Now.Date;
            #endregion

            //Get Data From DB:
            var FieldData = _context.Categories.Where(x => x.CategoryTypeCode == "ADMINISTRATIVE_PROCEDURE_FIELD").OrderBy(o => o.Piority).Select(x => new FieldAdministrativeProceduresModel
            {
                FieldId = x.CategoryId,
                FieldName = x.CategoryName
            }).ToList();

            var JoinTable = from r in _context.ReportAdministrativeProcedures
                                               where !r.IsDel && r.Year == year
                                               join c in _context.Categories
                                               on r.Period equals c.CategoryId
                                               select new { r, c };

            var ReportData = JoinTable;

            if (periods != 5)
            {
                ReportData = from j in JoinTable
                             where j.c.Piority == periods
                             select j;
            }

            //var AdministrativeProceduresData = _context.AdministrativeProcedures.Where(x => !x.IsDel && x.DayReception.Date >= MinDatePrevious && x.DayReception.Date <= MaxDate).ToList();

                //Data Processing:
            var Result = new List<ResultReportAdministrativeProceduresModel>();

            foreach (FieldAdministrativeProceduresModel Field in FieldData)
            {
                var Item = new ResultReportAdministrativeProceduresModel();
                #region Code cũ
                ////Ngoài kỳ
                //var DataItemPrevious = AdministrativeProceduresData.Where(x => x.AdministrativeProceduresField == Field.FieldId && x.DayReception.Date <= MaxDatePrevious);
                //if (DataItemPrevious != null)
                //{
                //    //Từ kỳ trước:
                //    Item.FromPreviousPeriod = DataItemPrevious.Where(x => x.Status == 2 &&
                //                                                  x.SettlementTerm.Date >= MinDate)
                //                                              .Sum(x => x.AmountOfRecords) +
                //                              DataItemPrevious.Where(x => x.Status == 3 &&
                //                                                  x.SettlementTerm.Date >= MinDatePrevious &&
                //                                                  x.SettlementTerm.Date <= MaxDatePrevious)
                //                                              .Sum(x => x.AmountOfRecords) +
                //                              DataItemPrevious.Where(x => x.Status == 3 &&
                //                                                  x.SettlementTerm.Date >= MinDate)
                //                                              .Sum(x => x.AmountOfRecords) +
                //                              DataItemPrevious.Where(x => x.Status == 1)
                //                                              .Sum(x => x.AmountOfRecords);
                //}

                ////Trong kỳ
                //var DataItem = AdministrativeProceduresData.Where(x => x.AdministrativeProceduresField == Field.FieldId && x.DayReception.Date >= MinDate);
                //if (DataItem != null)
                //{
                //    //Thông tin lĩnh vực giải quyết:
                //    Item.AdministrativeProceduresField = Field.FieldId;
                //    Item.FieldName = Field.FieldName;

                //    //Đã giải quyết - trong kỳ:
                //    var DataItemProcessed = DataItem.Where(x => x.Status == 3 && x.FinishDay?.Date <= MaxDate);
                //    Item.BeforeDeadlineProcessed = DataItemProcessed.Where(x => x.FinishDay?.Date < x.SettlementTerm.Date).Sum(x => x.AmountOfRecords);
                //    Item.OnTimeProcessed = DataItemProcessed.Where(x => x.FinishDay?.Date == x.SettlementTerm.Date).Sum(x => x.AmountOfRecords);
                //    Item.OutOfDateProcessed = DataItemProcessed.Where(x => x.FinishDay?.Date > x.SettlementTerm.Date).Sum(x => x.AmountOfRecords);
                //    Item.TotalProcessed = Item.BeforeDeadlineProcessed + Item.OnTimeProcessed + Item.OutOfDateProcessed;

                //    //Đang giải quyết - trong kỳ:
                //    var DataItemProcessing = DataItem.Where(x => x.Status == 2 && x.SettlementTerm.Date <= MaxDate);
                //    Item.OnTimeProcessing = DataItemProcessing.Where(x => x.SettlementTerm.Date <= Now).Sum(x => x.AmountOfRecords);
                //    Item.OutOfDateProcessing = DataItemProcessing.Where(x => x.SettlementTerm.Date > Now).Sum(x => x.AmountOfRecords);
                //    Item.TotalProcessing = Item.OnTimeProcessing + Item.OutOfDateProcessing;

                //    //Đã tiếp nhận - trong kỳ:
                //    Item.OnlineInPeriod = DataItem.Where(x => x.DayReception.Date <= MaxDate && x.ReceptionForm == 2).Sum(x => x.AmountOfRecords);
                //    Item.OfflineInPeriod = DataItem.Where(x => x.DayReception.Date <= MaxDate && x.ReceptionForm == 1).Sum(x => x.AmountOfRecords);
                //    Item.TotalReceive = Item.OnlineInPeriod + Item.OfflineInPeriod + Item.FromPreviousPeriod;
                //}
                #endregion

                var Data = ReportData.Where(x => x.r.AdministrativeProceduresField == Field.FieldId).ToList();

                //Thông tin lĩnh vực giải quyết:
                Item.AdministrativeProceduresField = Field.FieldId;
                Item.FieldName = Field.FieldName;

                //Ngoài kỳ
                //Từ kỳ trước:
                Item.FromPreviousPeriod = Data.Sum(x => x.r.FromPreviousPeriod);

                //Trong kỳ
                //Đã giải quyết - trong kỳ:
                Item.BeforeDeadlineProcessed = Data.Sum(x => x.r.BeforeDeadlineProcessed);
                Item.OnTimeProcessed = Data.Sum(x => x.r.OnTimeProcessed);
                Item.OutOfDateProcessed = Data.Sum(x => x.r.OutOfDateProcessed);
                Item.TotalProcessed = Item.BeforeDeadlineProcessed + Item.OnTimeProcessed + Item.OutOfDateProcessed;

                //Đang giải quyết - trong kỳ:
                Item.OnTimeProcessing = Data.Sum(x => x.r.OnTimeProcessing);
                Item.OutOfDateProcessing = Data.Sum(x => x.r.OutOfDateProcessing);
                Item.TotalProcessing = Item.OnTimeProcessing + Item.OutOfDateProcessing;

                //Đã tiếp nhận - trong kỳ:
                Item.OnlineInPeriod = Data.Sum(x => x.r.OnlineInPeriod);
                Item.OfflineInPeriod = Data.Sum(x => x.r.OfflineInPeriod);
                Item.TotalReceive = Item.OnlineInPeriod + Item.OfflineInPeriod + Item.FromPreviousPeriod;

                Result.Add(Item);
            }

            return Result;
        }
    }
}
