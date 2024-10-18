using API_SoCongThuong.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace API_SoCongThuong.Classes
{
    public class CusAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        public string Roles { get; set; } //Role string to get from controller

        //public void OnAuthorization(AuthorizationFilterContext context) // -> Độ phức tạp kognitif = 24 > 15 nên cần được tối ưu
        //{
        //    if (string.IsNullOrEmpty(Roles))
        //    {
        //        context.Result = new UnauthorizedResult();
        //        return;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            IHeaderDictionary _d = context.HttpContext.Request.Headers;

        //            if (!_d.ContainsKey(HeaderNames.Authorization))
        //                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };

        //            string _bearer_token, _data;

        //            _bearer_token = _d[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        //            var handler = new JwtSecurityTokenHandler();

        //            var tokenS = handler.ReadToken(_bearer_token) as JwtSecurityToken;
        //            if (tokenS == null)
        //            {
        //                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
        //                return;
        //            }

        //            var checkData = tokenS.Claims.Where(x => x.Type == "user").FirstOrDefault();
        //            if (checkData == null)
        //            {
        //                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
        //                return;
        //            }

        //            _data = checkData.Value;
        //            if (string.IsNullOrEmpty(_data))
        //            {
        //                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status400BadRequest };
        //                return;
        //            }

        //            var data = JsonConvert.DeserializeObject<UserModel>(_data);
        //            if (data == null)
        //            {
        //                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
        //                return;
        //            }

        //            if (DateTime.UtcNow > tokenS.ValidTo)
        //            {
        //                context.Result = new JsonResult(new { message = "Expired" }) { StatusCode = StatusCodes.Status408RequestTimeout };
        //            }
        //            else
        //            {
        //                //Kiểm tra quyền ở đây
        //                var requiredPermissions = Roles.Split(","); // nhận nhiều mã quyền từ controller, xóa "," để lấy từng quyền
        //                List<string> roles = data.Roles.Split(",").ToList();
        //                foreach (var x in requiredPermissions)
        //                {
        //                    if (roles.Contains(x))
        //                        return; //User Authorized
        //                }
        //                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
        //                return;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
        //        }
        //    }
        //}

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (string.IsNullOrWhiteSpace(Roles))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                var bearerToken = GetBearerToken(context.HttpContext.Request.Headers);
                var userModel = ValidateTokenAndGetUserModel(bearerToken);

                if (!IsAuthorized(userModel.Roles, Roles))
                {
                    context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
                    return;
                }
            }
            catch (Exception ex)
            {
                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }

        private string GetBearerToken(IHeaderDictionary headers)
        {
            if (!headers.ContainsKey(HeaderNames.Authorization))
            {
                throw new UnauthorizedAccessException();
            }

            return headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        }

        private UserModel ValidateTokenAndGetUserModel(string bearerToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadJwtToken(bearerToken);

            if (tokenS == null || DateTime.UtcNow > tokenS.ValidTo)
            {
                throw new UnauthorizedAccessException();
            }

            var userModel = JsonConvert.DeserializeObject<UserModel>(tokenS.Claims.FirstOrDefault(c => c.Type == "user")?.Value);

            if (userModel == null || string.IsNullOrWhiteSpace(userModel.Roles))
            {
                throw new UnauthorizedAccessException();
            }

            return userModel;
        }

        private bool IsAuthorized(string userRoles, string requiredRoles)
        {
            var requiredPermissions = requiredRoles.Split(",");
            var roles = userRoles.Split(",");

            return requiredPermissions.All(rp => roles.Contains(rp));
        }
    }
}
