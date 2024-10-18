using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace API_SoCongThuong.Models
{
    public class ImportResult
    {
        public string? TableName { get; set; }
        public string? FullTableName { get; set; }
        public string? Column { get; set; }
        public string? ColumnName { get; set; }
        public bool? IsNull { get; set; }
        public string? Ref { get; set; }
        public string? RefId { get; set; }
        public string? RefTable { get; set; }
        public bool? Exist { get; set; }
        public string DataType { get; set; }
        public string Url { get; set; }
    }

    public class ImportModel
    {
        public int idCol { get; set; }
        public string nameCol { get; set; }
        public string idRowMap { get; set; }
        public string nameMap { get; set; }
        public string type { get; set; }
        public bool required { get; set; }
        [JsonPropertyName("ref")]
        [JsonProperty("ref")]
        public string reftable { get; set; }
        public bool isNull { get; set; }
        public string refCol { get; set; }
        public string refId { get; set; }
        public bool exist { get; set; }
    }
    public class ImportFormData
    {
        public string? data { get; set; }
    }
}
