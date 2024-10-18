using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace API_SoCongThuong.Models
{
    public class QueryStatisticalImportExportProvincialBody
    {
        [JsonPropertyName("filter")]//for derelize of NEWTON.JSON
        [JsonProperty("filter")]//for serilize of NEWTON.JSON
        public Dictionary<string, string> Filter { get; set; }
    }

    public class ResultStatisticalImportExportProvincialData
    {
        //Id huyện
        public Guid DistrictId { get; set; }
        //Tên huyện
        public string DistrictName { get; set; } = null!;
        //Số lượng doanh nghiệp của huyện
        public int NumOfBusiness { get; set; }
        //Tổng nhập khẩu
        public decimal TotalImport { get; set; }
        //Tổng xuất khẩu
        public decimal TotalExport { get; set; }
    }

    public class TotalStatisticalImportExportProvincialData
    {
        //Tổng số huyện
        public int TotalNumOfBusiness { get; set; }
        //Tổng nhập khẩu
        public decimal TotalValueImport { get; set; }
        //Tổng xuất khẩu
        public decimal TotalValueExport { get; set; }
    }

    public class ReturnStatisticalImportExportProvincialData
    {
        public List<ResultStatisticalImportExportProvincialData> data { get; set; }
        public TotalStatisticalImportExportProvincialData total { get; set; }
    }
}
