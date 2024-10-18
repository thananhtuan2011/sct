using System;
using System.Collections.Generic;

namespace EF_Core.Models
{
    public partial class CateCriteriaNumberSevenDetail
    {
        public Guid CateCriteriaNumberSevenDetailId { get; set; }
        public Guid CateCriteriaNumberSevenId { get; set; }
        public Guid DistrictId { get; set; }
        /// <summary>
        /// số lượng xã trong huyện
        /// </summary>
        public int NumberOfWard { get; set; }
        /// <summary>
        /// số xã đạt tiêu chuẩn tiêu chí số 7
        /// </summary>
        public int NumberOfQualifyingWard { get; set; }
        public int NumberOfWardWithMarket { get; set; }
        /// <summary>
        /// số lượng xã đạt tiêu chuẩn hạ tầng thương mại 
        /// </summary>
        public int NumberOfWardCommercialInfrastructure { get; set; }
        /// <summary>
        /// số xã đạt nông thôn mới
        /// </summary>
        public int NumberOfWardNewCountryside { get; set; }
        /// <summary>
        /// số lượng xã đạt tiêu chuẩn hạ tầng thương mại 
        /// </summary>
        public int NumberOfWardCommercialInfrastructurePlan { get; set; }
        /// <summary>
        /// số xã đạt nông thôn mới
        /// </summary>
        public int NumberOfWardNewCountrysidePlan { get; set; }
        /// <summary>
        /// số lượng xã đạt tiêu chuẩn hạ tầng thương mại 
        /// </summary>
        public int NumberOfWardCommercialInfrastructureEstimate { get; set; }
        /// <summary>
        /// số xã đạt nông thôn mới
        /// </summary>
        public int NumberOfWardNewCountrysideEstimate { get; set; }
    }
}
