using EF_Core.Models;

namespace API_SoCongThuong.Models
{
    public partial class IndustrialManagementTargetModel
    {
        public Guid IndustrialManagementTargetId { get; set; }
        public Guid ParentTargetId { get; set; }
        public Guid? GroupTargetId { get; set; }
        public bool? IsAction { get; set; }
        public bool IsDel { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public Guid? UpdateUserId { get; set; }
        public List<IndustrialManagementTargetChildModel> listChild { get; set; } = new List<IndustrialManagementTargetChildModel>();
    }

    public partial class IndustrialManagementTargetChildModel
    {
        public Guid IndustrialManagementTargetId { get; set; } = Guid.Empty;
        public string ParentName { get; set; } = "";
        public Guid ParentTargetId { get; set; } = Guid.Empty;
        public Guid? GroupTargetId { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public string Unit { get; set; }
        public List<ChildModel> getChild { get; set; } = new List<ChildModel>();
    }

    public partial class ChildModel
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public string Unit { get; set; }
    }

    public partial class ReponseModel
    {
        public Guid IndustrialManagementTargetId { get; set; }
        public string ParentName { get; set; }  
        public string Name { get; set; }
        public string Unit { get; set; }
    }
}
