using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_SoCongThuong.Models
{
    public class UserModel
    {
        public string Uuid { get; set; }
        public Guid Userid { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Avatar { get; set; }
        public string Birthday { get; set; }
        public string? Phonenumber { get; set; }
        public long JobtitleID { get; set; }
        public string Roles { get; set; }
        public string Jobtitle { get; set; }
        public string Departmemt { get; set; }
        public string DepartmemtID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int LevelUser { get; set; }
        public string AreaID { get; set; }
        public string Cccd { get; set; }
        public string GroupUserName { get; set; }
        public string DeptName { get; set; }
        public string RoleName { get; set; }
        public int CountLoginFail { get; set; } = 0;
        public DateTime? TimeLock { get; set; } = null;
    }
    public class AuthenticateRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PassWord { get; set; }
        public string RecaptchaToken { get; set; }
    }
    public class RevokeTokenRequest
    {
        public string Token { get; set; }
    }

    public class AuthenticateResponse
    {
        [JsonPropertyName("uuid")]
        [JsonProperty("uuid")]
        public string uuid { get; set; }
        [JsonPropertyName("user")]
        [JsonProperty("user")]
        public UserModel User { get; set; }

        [JsonPropertyName("access_token")]
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        [JsonProperty("refresh_token")]
        [Newtonsoft.Json.JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
    }
    public class RefreshToken
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public string userId { get; set; }

        public string username { get; set; }
    }

    public class DefaultUser
    {
        [JsonPropertyName("userId")]
        [JsonProperty("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("username")]
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonPropertyName("fullname")]
        [JsonProperty("fullname")]
        public string Fullname { get; set; }
        [JsonPropertyName("password")]
        [JsonProperty("password")]
        public string PassWord { get; set; }
        [JsonPropertyName("deptId")]
        [JsonProperty("deptId")]
        public Guid DeptId { get; set; }
        [JsonPropertyName("roleId")]
        [JsonProperty("roleId")]
        public Guid RoleId { get; set; }
        [JsonPropertyName("phone")]
        [JsonProperty("phone")]
        public string Phone { get; set; }
        //[JsonPropertyName("avatar")]
        //[JsonProperty("avatar")]
        //public string Avatar { get; set; }
        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonPropertyName("cccd")]
        [JsonProperty("cccd")]
        public string CCCD { get; set; }

        [JsonPropertyName("groupId")]
        [JsonProperty("groupId")]
        public Guid GroupUserId { get; set; }

        [JsonPropertyName("deptName")]
        [JsonProperty("deptName")]
        public string DeptName { get; set; }

        [JsonPropertyName("roleName")]
        [JsonProperty("roleName")]
        public string RoleName { get; set; }
        [JsonPropertyName("groupName")]
        [JsonProperty("groupName")]
        public string GroupUserName { get; set; }

        [JsonPropertyName("levelUser")]
        [JsonProperty("levelUser")]
        public int levelUser { get; set; }
        [JsonPropertyName("areaId")]
        [JsonProperty("areaId")]
        public Guid areaId { get; set; }
        [JsonPropertyName("status")]
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonPropertyName("countLoginFail")]
        [JsonProperty("countLoginFail")]
        public int CountLoginFail { get; set; } = 0;
        [JsonPropertyName("timeLock")]
        [JsonProperty("timeLock")]
        public DateTime? TimeLock { get; set; } = null;
        [JsonPropertyName("isDel")]
        [JsonProperty("isDel")]
        public bool? IsDel { get; set; } = false;
    }

    public class CreateUser
    {
        [JsonPropertyName("userId")]
        [JsonProperty("userId")]
        public Guid UserId { get; set; }
        [JsonPropertyName("username")]
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonPropertyName("fullname")]
        [JsonProperty("fullname")]
        public string Fullname { get; set; }
        [JsonPropertyName("password")]
        [JsonProperty("password")]
        public string PassWord { get; set; }
        [JsonPropertyName("deptId")]
        [JsonProperty("deptId")]
        public Guid DeptId { get; set; }
        [JsonPropertyName("roleId")]
        [JsonProperty("roleId")]
        public Guid RoleId { get; set; }
        [JsonPropertyName("phone")]
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonPropertyName("email")]
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonPropertyName("cccd")]
        [JsonProperty("cccd")]
        public string CCCD { get; set; }
        [JsonPropertyName("levelUser")]
        [JsonProperty("levelUser")]
        public int leveluser { get; set; }
        [JsonPropertyName("areaId")]
        [JsonProperty("areaId")]
        public Guid areaId { get; set; }
        [JsonPropertyName("groupId")]
        [JsonProperty("groupId")]
        public Guid GroupUserId { get; set; }
        public class ChangePasswordModel
        {
            [Required]
            public string UserName { get; set; }
            [Required]
            public string PassWordCurrent { get; set; }
            [Required]
            public string PassWord { get; set; }
            [Required]
            public string ConfirmPassword { get; set; }
        }
    }

    public class captchaResult
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        [JsonProperty("error-codes")]
        public List<string> errors { get; set; }
    }

    public class LoginFailedModel
    {
        public int CountLoginFail { get; set; }
        public DateTime? TimeLock { get; set; } = null;
        public string Error { get; set; }
    }

    public class TreeUserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Role { get; set; }
        public List<TreeUserModel> Children { get; set; }
        public TreeUserModel Parent { get; set; }
    }
}
