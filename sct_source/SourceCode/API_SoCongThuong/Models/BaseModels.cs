using API_SoCongThuong.Classes;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace API_SoCongThuong.Models
{
    public class BaseModels<T>
    {
        public int status { get; set; } = 0;
        public List<T>? items { get; set; }
        public T? data { get; set; }
        public int total { get; set; } = 0;
        public ErrorModel? error { get; set; }
    }

    public class QueryRequestBody
    {
        [JsonPropertyName("filter")]//for derelize of NEWTON.JSON
        [JsonProperty("filter")]//for serilize of NEWTON.JSON
        public Dictionary<string, string> Filter { get; set; }

        [JsonPropertyName("paginator")]
        [JsonProperty("paginator")]
        public Panigator Panigator { get; set; }

        [JsonPropertyName("searchTerm")]
        [JsonProperty("searchTerm")]
        public string SearchValue { get; set; }

        [JsonPropertyName("sorting")]
        [JsonProperty("sorting")]
        public SortParams Sort { get; set; }
    }

    public class Panigator
    {
        [JsonPropertyName("page")]
        [JsonProperty("page")]
        public int PageIndex { get; set; }

        [JsonPropertyName("pageSize")]
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonPropertyName("more")]
        [JsonProperty("more")]
        public bool More { get; set; } = false;
    }

    public class SortParams
    {
        [JsonPropertyName("column")]
        [JsonProperty("column")]
        public string ColumnName { get; set; }

        [JsonPropertyName("direction")]
        [JsonProperty("direction")]
        public string Direction { get; set; }
    }
    public class ErrorModel
    {
        [JsonPropertyName("code")]
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonPropertyName("msg")]
        [JsonProperty("msg")]
        public string Msg { get; set; }

        //public ErrorModel(int pCode)
        //{
        //    this.Code = pCode;
        //    this.Msg = ErrMsg_Const.GetMsg(this.Code);
        //}

        //public ErrorModel(int pCode, string pExtra_Msg = "", string pMsg = "")
        //{
        //    this.Code = pCode;
        //    this.Msg = ErrMsg_Const.GetMsg(this.Code) + pExtra_Msg;
        //}
        //public ErrorModel(int pCode, string Msg = "")
        //{
        //    this.Code = pCode;
        //    this.Msg = ErrMsg_Const.GetMsg(this.Code) + " | " + Msg;
        //}

        public ErrorModel()
        {

        }
    }
}
