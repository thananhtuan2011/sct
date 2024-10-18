using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace API_SoCongThuong.Models
{
    public class QueryStatisticalImportExportDistrictBody
    {
        [JsonPropertyName("filter")]//for derelize of NEWTON.JSON
        [JsonProperty("filter")]//for serilize of NEWTON.JSON
        public Dictionary<string, string> Filter { get; set; }
    }

    public class ResultStatisticalImportExportDistrictData
    {
        //Id doanh nghiệp
        public Guid BusinessId { get; set; }
        //Tên doanh nghiệp
        public string BusinessName { get; set; } = null!;
        //Đại diện
        public string Represent { get; set; } = null!;
        //Số điện thoại
        public string PhoneNumber { get; set; } = null!;
        //Mặt hàng
        public List<string> Items { get; set; }
        //Tổng xuất - nhập khẩu
        public decimal Total { get; set; }
    }

    public class TotalStatisticalImportExportDistrictData
    {
        public int TotalGoods { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class ReturnStatisticalImportExportDistrictData
    {
        public List<ResultStatisticalImportExportDistrictData> data { get; set; }
        public TotalStatisticalImportExportDistrictData total { get; set; }
    }
}
