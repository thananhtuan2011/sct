using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using Minio.DataModel;
using System.Net;
using System.Runtime.CompilerServices;

namespace API_SoCongThuong.Reponsitories
{
    public class RuralDevelopmentPlanRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public RuralDevelopmentPlanRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }
        public async Task Insert(RuralDevelopmentPlanModel model)
        {
            RuralDevelopmentPlan data = new RuralDevelopmentPlan()
            {
                RuralDevelopmentPlanId = model.RuralDevelopmentPlanId,
                SuperMarketShoppingMallName = model.SuperMarketShoppingMallName,
                Address = model.Address,
                TotalInvestment = model.TotalInvestment,
                Budget = model.Budget,
                OutOfBudget = model.OutOfBudget,
                Type = model.Type,
                StageId = model.StageId,
                CreateUserId = model.CreateUserId,
                CreateTime = model.CreateTime,
            };
            await _context.RuralDevelopmentPlans.AddAsync(data);
            await _context.SaveChangesAsync();

            List<RuralDevelopmentPlanStage> stage_data = new List<RuralDevelopmentPlanStage>();
            foreach (var item in model.Stages)
            {
                RuralDevelopmentPlanStage stage = new RuralDevelopmentPlanStage()
                {
                    PlanStageId = Guid.Empty,
                    RuralDevelopmentPlanId = data.RuralDevelopmentPlanId,
                    StageId = (Guid)model.StageId,
                    Year = item.Year,
                    Budget = item.Budget,
                };
                stage_data.Add(stage);
            }
            await _context.RuralDevelopmentPlanStages.AddRangeAsync(stage_data);
            await _context.SaveChangesAsync();
        }

        public async Task Update(RuralDevelopmentPlanModel model)
        {
            //Update data
            var detailinfo = await _context.RuralDevelopmentPlans.Where(d => d.RuralDevelopmentPlanId == model.RuralDevelopmentPlanId).FirstOrDefaultAsync();
            detailinfo.SuperMarketShoppingMallName = model.SuperMarketShoppingMallName;
            detailinfo.Address = model.Address;
            detailinfo.TotalInvestment = model.TotalInvestment;
            detailinfo.Budget = model.Budget;
            detailinfo.OutOfBudget = model.OutOfBudget;
            detailinfo.Type = model.Type;
            detailinfo.StageId = model.StageId;
            detailinfo.UpdateUserId = model.UpdateUserId;
            detailinfo.UpdateTime = model.UpdateTime;

            //Stages
            var removetime = _context.RuralDevelopmentPlanStages.Where(x => x.RuralDevelopmentPlanId == model.RuralDevelopmentPlanId).ToList();
            _context.RuralDevelopmentPlanStages.RemoveRange(removetime);

            List<RuralDevelopmentPlanStage> stage_data = new List<RuralDevelopmentPlanStage>();
            foreach (var item in model.Stages)
            {
                RuralDevelopmentPlanStage stage = new RuralDevelopmentPlanStage()
                {
                    PlanStageId = Guid.Empty,
                    RuralDevelopmentPlanId = model.RuralDevelopmentPlanId,
                    StageId = (Guid)model.StageId,
                    Year = item.Year,
                    Budget = item.Budget,
                };
                stage_data.Add(stage);
            }
            await _context.RuralDevelopmentPlanStages.AddRangeAsync(stage_data);
            await _context.SaveChangesAsync();
        }


        public async Task Delete(Guid Id)
        {
            var detailinfo = await _context.RuralDevelopmentPlans.Where(d => d.RuralDevelopmentPlanId == Id).FirstOrDefaultAsync();
            detailinfo.IsDel = true;
            await _context.SaveChangesAsync();

            //Stages
            var removetime = _context.RuralDevelopmentPlanStages.Where(x => x.RuralDevelopmentPlanId == Id).ToList();
            _context.RuralDevelopmentPlanStages.RemoveRange(removetime);
        }

        public RuralDevelopmentPlanModel FindById(Guid Id)
        {
            var result = _context.RuralDevelopmentPlans.Where(x => x.RuralDevelopmentPlanId == Id && !x.IsDel).Select(model => new RuralDevelopmentPlanModel()
            {
                RuralDevelopmentPlanId = model.RuralDevelopmentPlanId,
                SuperMarketShoppingMallName = model.SuperMarketShoppingMallName,
                Address = model.Address,
                TotalInvestment = model.TotalInvestment,
                Budget = model.Budget,
                OutOfBudget = model.OutOfBudget,
                Type = model.Type,
                StageId = model.StageId,
            }).FirstOrDefault();

            if (result == null)
            {
                return new RuralDevelopmentPlanModel();
            }

            var stage_data = _context.RuralDevelopmentPlanStages.Where(x => x.RuralDevelopmentPlanId == Id).Select(model => new PlanStageModel()
            {
                PlanStageId = model.PlanStageId,
                RuralDevelopmentPlanId = model.RuralDevelopmentPlanId,
                StageId = model.StageId,
                Year = model.Year,
                Budget = model.Budget,
            });
            result.Stages = stage_data.ToList();
            return result;
        }

        public List<RuralDevelopmentPlanModel> FindData(QueryRequestBody query)
        {
            List<RuralDevelopmentPlanModel> result = new List<RuralDevelopmentPlanModel>();

            IQueryable<RuralDevelopmentPlanModel> _data = _context.RuralDevelopmentPlans.Where(x => !x.IsDel).Select(x => new RuralDevelopmentPlanModel
            {
                RuralDevelopmentPlanId = x.RuralDevelopmentPlanId,
                SuperMarketShoppingMallName = x.SuperMarketShoppingMallName,
                Address = x.Address,
                TotalInvestment = x.TotalInvestment,
                Budget = x.Budget,
                OutOfBudget = x.OutOfBudget,
                Type = x.Type,
                StageId = x.StageId,
                Stages = _context.RuralDevelopmentPlanStages.Where(s => s.RuralDevelopmentPlanId == x.RuralDevelopmentPlanId).Select(_ => new PlanStageModel
                {
                    RuralDevelopmentPlanId = _.RuralDevelopmentPlanId,
                    PlanStageId = _.PlanStageId,
                    Budget = _.Budget,
                    StageId = _.StageId,
                    Year = _.Year,
                }).ToList()
            }).ToList().AsQueryable();

            //Search
            string _keywordSearch = "";
            if (query.SearchValue != null && query.SearchValue != "")
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x => x.Address.ToLower().Contains(_keywordSearch) 
                || x.SuperMarketShoppingMallName.ToLower().Contains(_keywordSearch));
            }

            //Filter
            if (query.Filter != null && query.Filter.ContainsKey("Stage") && !string.IsNullOrEmpty(query.Filter["Stage"]))
            {
                _data = _data.Where(x => x.StageId.ToString() == query.Filter["Stage"]);
            }

            result = _data.ToList();

            return result;
        }
    }
}
