namespace API_SoCongThuong.Models
{
    public class Target7Model
    {
        public Guid Target7Id { get; set; }
        public int Year { get; set; }
        public Guid StageId { get; set; }
        public string? StageName { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public Guid DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public Guid CommuneId { get; set; }
        public string? CommuneName { get; set; }
        public int MarketInPlaning { get; set; }
        public bool PlanCommercial { get; set; }
        public bool PlanMarket { get; set; }
        public bool EstimateCommercial { get; set; }
        public bool EstimateMarket { get; set; }
        public bool NewRuralCriteriaRaised { get; set; }
        public string? Note { get; set; }
    }

    public class Target7ProviceModel 
    {
        public Guid DistrictId { get; set; }
        public string DistrictName { get; set; } = "";
        public int NumberCommune { get; set; } = 0;
        //Thực hiện 6 tháng/năm cùng kỳ năm trước 
        //Số xã đạt tiêu chí cơ sở hạ tầng thương mại nông thôn
        public int NumPreviousCommercial { get; set; } = 0;
        //Số xã có chợ đạt chuẩn nông thôn mới
        public int NumPreviousMarket { get; set; } = 0;
        /// <summary>
        /// Kế hoạch 6 tháng/năm
        /// </summary>
        public int NumPlanCommercial { get; set; } = 0;
        public int NumPlanMarket { get; set; } = 0;
        /// <summary>
        /// Ước tính thực hiện 6 tháng/năm	
        /// </summary>
        public int NumEstimateCommercial { get; set; } = 0;
        public int NumEstimateMarket { get; set; } = 0;
        // Số xã đạt chuẩn tiêu chí số 7 (Lũy kế) đến kỳ báo cáo
        public int NumCommunePassTarget { get; set; } = 0;
        //Số xã có chợ trong quy hoạch
        public int NumCommuneHaveMarketPlanning { get; set; } = 0;

    }
}
