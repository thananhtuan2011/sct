using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using EF_Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.ComponentModel;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static API_SoCongThuong.Models.CreateUser;

namespace API_SoCongThuong.Reponsitories.UserRepository
{
    public class UserRepo
    {
        public SoHoa_SoCongThuongContext _context;
        private IConfiguration _configuration;
        private readonly IConnectionMultiplexer _redisCache;
        public UserRepo(SoHoa_SoCongThuongContext context, IConfiguration configuration, IConnectionMultiplexer redisCache)
        {
            _context = context;
            _configuration = configuration;
            _redisCache = redisCache;
        }
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var user = _context.Users.Where(x => (x.IsDel == false && x.Status == 0 && x.UserName.Equals(model.UserName) && x.Password.Equals(DpsLibs.Common.EncDec.Encrypt(model.PassWord, _configuration.GetValue<string>("PASSWORD_ED"))))).FirstOrDefault();
            if (user != null)
            {
                if (user.CountLoginFail > 1 && user.CountLoginFail < 4)
                {
                    if (String.IsNullOrEmpty(model.RecaptchaToken) || !await validateCaptchaAsync(model.RecaptchaToken))
                    {
                        LoginFail(model.UserName);
                        return null;
                    }
                }
                else if (user.CountLoginFail > 3 && user.CountLoginFail < 8)
                {
                    if (user.TimeLock > DateTime.Now)
                        return null;
                }
                else if (user.CountLoginFail > 7)
                    return null;

                LoginSuccess(model.UserName);

                List<string> ls = new List<string>();
                string roles = "";
                var groupRoles = _context.GroupPermits.Where(x => x.GroupId == user.GroupUserId).ToList();
                foreach (var item in groupRoles)
                {
                    ls.Add(item.Code);
                }

                roles = string.Join(",", ls.ToArray());
                var _user = new UserModel
                {
                    Userid = user.UserId,
                    Username = user.UserName.ToString(),
                    Fullname = user.FullName.ToString(),
                    Phonenumber = user.Phone,
                    Email = user.Email,
                    Password = user.Password,
                    Cccd = user.Cccd,
                    Avatar = user.Avatar,
                    Roles = roles,
                    DeptName = _context.StateTitles.Where(x => x.StateTitlesId == user.RoleId).FirstOrDefault()?.StateTitlesName ?? "",
                    //GroupUserName = user.GroupUserName,
                    //DeptName = user.DeptName,
                    //RoleName = user.RoleName,
                    LevelUser = user.LevelUser,
                    AreaID = user.AreaId.ToString(),
                };

                var uuid = Guid.NewGuid().ToString();
                var jwtToken = generateJwtToken(_user, uuid);
                var refreshToken = generateRefreshToken(_user, uuid);

                return new AuthenticateResponse
                {
                    uuid = uuid,
                    User = _user,
                    AccessToken = jwtToken,
                    RefreshToken = refreshToken
                };
            }
            else
            {
                LoginFail(model.UserName);
                return null;
            }
        }

        private void LoginSuccess(string UserName)
        {
            var userLoginSuccess = _context.Users.Where(x => (x.IsDel == false && x.Status == 0 && x.UserName.Equals(UserName))).FirstOrDefault();
            if (userLoginSuccess != null)
            {
                userLoginSuccess.CountLoginFail = 0;
                userLoginSuccess.TimeLock = null;

                _context.SaveChanges();
            }
        }

        private void LoginFail(string Username)
        {
            var userLoginFail = _context.Users
                .Where(x => x.IsDel == false && x.Status == 0 && x.UserName.Equals(Username))
                .FirstOrDefault();

            if (userLoginFail != null)
            {
                if (userLoginFail.CountLoginFail < 5 && userLoginFail.TimeLock == null)
                {
                    userLoginFail.CountLoginFail++;

                    if (userLoginFail.CountLoginFail == 4)
                    {
                        userLoginFail.TimeLock = DateTime.UtcNow + TimeSpan.FromMinutes(2);
                    }
                }

                if (userLoginFail.CountLoginFail > 3 && userLoginFail.TimeLock < DateTime.UtcNow && userLoginFail.CountLoginFail < 8)
                {
                    userLoginFail.CountLoginFail++;
                    switch (userLoginFail.CountLoginFail)
                    {
                        case 5:
                            userLoginFail.TimeLock = DateTime.UtcNow + TimeSpan.FromMinutes(5);
                            break;
                        case 6:
                            userLoginFail.TimeLock = DateTime.UtcNow + TimeSpan.FromMinutes(15);
                            break;
                        case 7:
                            userLoginFail.TimeLock = DateTime.UtcNow + TimeSpan.FromHours(6);
                            break;
                        case 8:
                            userLoginFail.TimeLock = DateTime.UtcNow + TimeSpan.FromDays(36500);
                            userLoginFail.Status = 1;
                            break;
                    }
                }

                _context.SaveChanges();
            }
        }

        private string generateJwtToken(UserModel user, string uuid)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:Key"));
            var data = new
            {
                userId = user.Userid,
                username = user.Username,
                Fullname = user?.Fullname,
                Password = user.Password,
                Avatar = user?.Avatar,
                Birthday = user?.Birthday,
                Phonenumber = user?.Phonenumber,
                GroupUserName = user.GroupUserName,
                DeptName = user.DeptName,
                RoleName = user.RoleName,
                Cccd = user?.Cccd,
                Jobtitle = user?.Jobtitle,
                Departmemt = user?.Departmemt,
                Email = user?.Email,
                Roles = user.Roles,
                LevelUser = user.LevelUser,
                AreaId = user.AreaID.ToString(),
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new[] { new Claim("uuid", uuid), new Claim("user", JsonConvert.SerializeObject(data)) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string generateRefreshToken(UserModel user, string uuid)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Jwt:Key"));
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("uuid", uuid));
            claims.Add(new Claim("userId", user.Userid.ToString()));
            claims.Add(new Claim("username", user.Username));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task Insert(User model)
        {
            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<User> findByUsername(string username)
        {
            return await _context.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
        }

        public List<User> FindAll()
        {
            var result = _context.Users.ToList();
            return result;
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public async Task Update(User _user)
        {
            User db = await _context.Users.Where(d => d.UserId == _user.UserId).FirstOrDefaultAsync();
            db.UserName = _user.UserName.Trim();
            db.FullName = _user.FullName.Trim();
            //db.Password = DpsLibs.Common.EncDec.Encrypt(_user.Password, _configuration.GetValue<string>("PASSWORD_ED"));
            db.RoleId = _user.RoleId;
            db.DeptId = _user.DeptId;
            db.GroupUserId = _user.GroupUserId;
            db.Phone = _user.Phone;
            db.Email = _user.Email;
            db.Cccd = _user.Cccd;
            db.LevelUser = _user.LevelUser;
            db.AreaId = _user.AreaId;
            db.UpdateUserId = _user.UpdateUserId;
            db.Status = _user.Status;
            db.IsDel = _user.IsDel;
            db.CountLoginFail = _user.CountLoginFail;
            db.TimeLock = _user.TimeLock;
            _context.Update(db);
            await _context.SaveChangesAsync();
        }


        public async Task UpdatePassword(AuthenticateRequest model)
        {
            var user = _context.Users.Where(x => (x.IsDel == false && x.Status == 0 && x.UserName.Equals(model.UserName))).FirstOrDefault();
            if (user != null)
            {
                user.Password = DpsLibs.Common.EncDec.Encrypt(model.PassWord, _configuration.GetValue<string>("PASSWORD_ED"));
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePasswords(ChangePasswordModel model)
        {
            var user = _context.Users.Where(x => (x.IsDel == false && x.Status == 0 && x.UserName.Equals(model.UserName))).FirstOrDefault();

            if (user != null)
            {
                user.Password = DpsLibs.Common.EncDec.Encrypt(model.PassWord, _configuration.GetValue<string>("PASSWORD_ED"));
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public ChangePasswordModel CheckPasswordCurrent(ChangePasswordModel model)
        {
            model.PassWordCurrent = DpsLibs.Common.EncDec.Encrypt(model.PassWordCurrent, _configuration.GetValue<string>("PASSWORD_ED"));
            return model;
        }

        public async Task setCurrentRefreshToken(string Uuid, string username)
        {
            UuiRefreshToken ss = new UuiRefreshToken();
            ss.Uuid = Guid.Parse(Uuid);
            ss.UserName = username;
            await _context.UuiRefreshTokens.AddAsync(ss);
            await _context.SaveChangesAsync();
        }

        public async Task addToWhitelist(string Uuid, string refreshtoken)
        {
            var db = _redisCache.GetDatabase();
            await db.StringSetAsync(Uuid.ToString(), refreshtoken);
        }

        public async Task<string> getFromWhitelist(string Uuid)
        {
            try
            {
                var db = _redisCache.GetDatabase();
                var rf = await db.StringGetAsync(Uuid.ToString());
                return rf;
            }
            catch (Exception)
            {

                return "";
            }
        }

        public async Task removeFromWhitelist(string Uuid)
        {
            var db = _redisCache.GetDatabase();
            await db.KeyDeleteAsync(Uuid.ToString());
        }

        public IQueryable<UuiRefreshToken> findUuidByUsername(string username)
        {
            return _context.UuiRefreshTokens.Where(x => x.UserName == username).Select(x => new UuiRefreshToken()
            {
                Uuid = x.Uuid,
                UserName = x.UserName
            });
        }

        public string findUserNameById(Guid Id)
        {
            var result = _context.Users.Where(x => x.UserId == Id).FirstOrDefault();

            return null;
        }

        private async Task<bool> validateCaptchaAsync(string captchares)
        {
            ErrorModel error = new ErrorModel();
            object jres = new object();
            if (String.IsNullOrEmpty(captchares))
            {
                return false;
            }
            string secret_key = _configuration.GetValue<string>("RecaptchaSecretKey");
            if (String.IsNullOrEmpty(secret_key))
            {
                return false;
            }

            var content = new FormUrlEncodedContent(new[]
              {
                new KeyValuePair<string, string>("secret",  secret_key),
                new KeyValuePair<string, string>("response", captchares)
              });

            HttpClient client = new HttpClient();
            var res = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);

            captchaResult captchaRes = JsonConvert.DeserializeObject<captchaResult>(res.Content.ReadAsStringAsync().Result);

            if (!captchaRes.success)
            {
                //error.message = HelperClass.GetBackendMessage("474"); //_474
                return false;
            }
            //error.message = HelperClass.GetBackendMessage("722"); //_722
            return true;
        }

        public List<TreeUserModel> GetUserTree()
        {
            List<TreeUserModel> result = new List<TreeUserModel>();
            List<StateUnit> listParent = _context.StateUnits.Where(x => x.IsDel == false && x.ParentId == Guid.Empty).ToList();
            List<StateUnit> allList = _context.StateUnits.Where(x => x.IsDel == false).ToList();

            foreach (var item in listParent)
            {
                List<TreeUserModel> subList = listParent.Where(x => x.StateUnitsId == item.StateUnitsId).Select(x => new TreeUserModel()
                {
                    Id = x.StateUnitsId,
                    Name = x.StateUnitsName,
                    Children = GetUser(x.StateUnitsId).Concat(GetChild(allList, x.StateUnitsId)).ToList()
                }).ToList();

                result.AddRange(subList.Where(x => x.Children.Any()));
            }
            return result;
        }

        private List<TreeUserModel> GetChild(List<StateUnit> allList, Guid parentId)
        {
            List<StateUnit> temp = allList;

            var data = allList.Where(x => x.ParentId == parentId).Select(n => new TreeUserModel()
            {
                Id = n.StateUnitsId,
                Name = n.StateUnitsName,
                Children = temp.Where(x => x.ParentId == n.StateUnitsId).Any() ? GetUser(n.StateUnitsId).Concat(GetChild(temp, n.StateUnitsId)).ToList() : GetUser(n.StateUnitsId)
            }).ToList();

            return data.Where(x => x.Children.Any()).ToList();
        }

        private List<TreeUserModel> GetUser(Guid StateUnitId)
        {
            List<TreeUserModel> result = (from u in _context.Users
                                          where u.IsDel == false && u.DeptId == StateUnitId
                                          join r in _context.StateTitles
                                             on u.RoleId equals r.StateTitlesId into re
                                          from res in re.DefaultIfEmpty()
                                          orderby res.Piority
                                          select new TreeUserModel()
                                          {
                                              Id = u.UserId,
                                              Name = u.FullName,
                                              Role = res.StateTitlesName
                                          }).ToList();
            return result;
        }
    }
}