namespace API_SoCongThuong.Models.PermissionModel
{
    public class PermissionModel
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; } = "";
        public int Status { get; set; }
        public int Priority { get; set; }
        public int CountGroupUser { get; set; } = 0;
        public bool IsAdmin { get; set; }
    }
    public class removeGroupListItems
    {
        public List<Guid> ids { get; set; }
    }

    public class GroupRoleModel
    {
        public long IdPermit { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? IdGroup { get; set; }
        public short? Position { get; set; }
        public string Code { get; set; }
        public string CodeGroup { get; set; }
        public bool Permitted { get; set; }
    }

    public class UserGroupRoles
    {
        public List<string> Code { get; set; }
        public Guid IdGroup { get; set; }
    }

}
