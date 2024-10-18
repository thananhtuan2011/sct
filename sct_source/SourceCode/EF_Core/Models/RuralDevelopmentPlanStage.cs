using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class RuralDevelopmentPlanStage
    {
        /// <summary>
        /// Giai đoạn
        /// </summary>
        public Guid PlanStageId { get; set; }
        public Guid RuralDevelopmentPlanId { get; set; }
        /// <summary>
        /// Tên giai đoạn
        /// </summary>
        public Guid StageId { get; set; }
        /// <summary>
        /// Năm bắt đầu
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Năm kết thúc
        /// </summary>
        public long? Budget { get; set; }
    }
}
