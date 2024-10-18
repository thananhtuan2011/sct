namespace API_SoCongThuong.Models.TestModel
{
    public class TestModel
    {
        public long id { get; set; }
        public string name { get; set; } = "";
    }

    public class removeListItems
    {
        public List<long> ids { get; set; }
    }
}
