using API_SoCongThuong.Logger;
using API_SoCongThuong.Models;
using EF_Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace API_SoCongThuong.Classes
{
    public class Ulities
    {

        //public static UserModel GetUserByHeader(IHeaderDictionary pHeader)
        //{
        //    try
        //    {
        //        if (pHeader == null) return null;
        //        if (!pHeader.ContainsKey(HeaderNames.Authorization)) return null;

        //        IHeaderDictionary _d = pHeader;
        //        string _bearer_token, _data, _uuid;
        //        _bearer_token = _d[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        //        var handler = new JwtSecurityTokenHandler();
        //        var tokenS = handler.ReadToken(_bearer_token) as JwtSecurityToken;
        //        _data = tokenS.Claims.Where(x => x.Type == "user").FirstOrDefault().Value;
        //        if (string.IsNullOrEmpty(_data))
        //            return null;
        //        _uuid = tokenS.Claims.Where(x => x.Type == "uuid").FirstOrDefault().Value;
        //        var data = JsonConvert.DeserializeObject<UserModel>(_data);
        //        data.Uuid = _uuid;
        //        return data;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public static string GetIPAddress(HttpContext context)
        {
            var data = GetIpClient(context.Request.Headers);
            //var ipAddress = context.Connection.RemoteIpAddress;
            //if (ipAddress != null && ipAddress.IsIPv4MappedToIPv6)
            //{
            //    ipAddress = ipAddress.MapToIPv4();
            //}
            //return ipAddress != null ? ipAddress.ToString() : "";
            return data ?? "";
        }

        public static string GetIpClient(IHeaderDictionary pHeader)
        {
            pHeader.TryGetValue("X-Forwarded-For", out var headerValue);
            return headerValue;
        }
        public static UserModel GetUserByHeader(IHeaderDictionary pHeader)
        {
            try
            {
                if (pHeader == null || !pHeader.TryGetValue(HeaderNames.Authorization, out var headerValue) || string.IsNullOrEmpty(headerValue))
                {
                    return null;
                }

                var bearerToken = headerValue.ToString().Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(bearerToken);

                var userData = tokenS.Claims.FirstOrDefault(x => x.Type == "user")?.Value;
                if (string.IsNullOrEmpty(userData))
                {
                    return null;
                }

                var uuid = tokenS.Claims.FirstOrDefault(x => x.Type == "uuid")?.Value;
                var data = JsonConvert.DeserializeObject<UserModel>(userData);
                data.Uuid = uuid;
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static UploadResult UploadFile(upLoadFileModel upLoad, IConfiguration config, string ContentType = "application/octet-stream")
        {
            UploadResult rs = new UploadResult();
            string MinioSSL = config.GetValue<string>("MinioConfig:MinioSSL");
            string MinioServer = config.GetValue<string>("MinioConfig:MinioServer");
            string MinioAccessKey = config.GetValue<string>("MinioConfig:MinioAccessKey");
            string MinioSecretKey = config.GetValue<string>("MinioConfig:MinioSecretKey");
            if (string.IsNullOrEmpty(MinioSSL)) throw new ArgumentNullException(nameof(MinioSSL));
            if (string.IsNullOrEmpty(MinioServer)) throw new ArgumentNullException(nameof(MinioServer));
            if (string.IsNullOrEmpty(MinioAccessKey)) throw new ArgumentNullException(nameof(MinioAccessKey));
            if (string.IsNullOrEmpty(MinioSecretKey)) throw new ArgumentNullException(nameof(MinioSecretKey));
            try
            {
                MinioClient minio;
                if (!bool.Parse(MinioSSL))
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey);
                }
                else
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey).WithSSL();
                }
                bool found = minio.BucketExistsAsync("sct").Result;
                if (!found)
                { minio.MakeBucketAsync("sct"); }
                MemoryStream filestream = new MemoryStream(upLoad.bs);
                var task = Task.Run(async () => await minio.PutObjectAsync("sct", upLoad.Linkfile + "/" + upLoad.FileName, filestream, filestream.Length, ContentType));
                task.GetAwaiter().GetResult();
                rs.status = true;
                rs.statusCode = 1;
                rs.link = "/" + "sct" + "/" + upLoad.Linkfile + "/" + upLoad.FileName;
                return rs;
            }
            catch (MinioException ex)
            {
                rs.status = false;
                rs.statusCode = 2;
                rs.link = "";
                rs.msg = "Upload thất bại";
                rs.error = ex.Message;
                return rs;
            }
        }

        public static bool RemoveFileMinio(string FileName, IConfiguration config)
        {
            try
            {
                MinioClient minio;
                string MinioSSL = config.GetValue<string>("MinioConfig:MinioSSL");
                string MinioServer = config.GetValue<string>("MinioConfig:MinioServer");
                string MinioAccessKey = config.GetValue<string>("MinioConfig:MinioAccessKey");
                string MinioSecretKey = config.GetValue<string>("MinioConfig:MinioSecretKey");
                if (string.IsNullOrEmpty(MinioSSL)) throw new ArgumentNullException(nameof(MinioSSL));
                if (string.IsNullOrEmpty(MinioServer)) throw new ArgumentNullException(nameof(MinioServer));
                if (string.IsNullOrEmpty(MinioAccessKey)) throw new ArgumentNullException(nameof(MinioAccessKey));
                if (string.IsNullOrEmpty(MinioSecretKey)) throw new ArgumentNullException(nameof(MinioSecretKey));
                if (!bool.Parse(MinioSSL))
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey);
                }
                else
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey).WithSSL();
                }

                var task = Task.Run(async () => await minio.RemoveObjectAsync("sct", FileName));
                task.GetAwaiter().GetResult();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static UploadResult CopyFolderMinio(IConfiguration config, string folderName, string destFolderName, bool copyItem = false)
        {
            UploadResult rs = new UploadResult();
            string MinioSSL = config.GetValue<string>("MinioConfig:MinioSSL");
            string MinioServer = config.GetValue<string>("MinioConfig:MinioServer");
            string MinioAccessKey = config.GetValue<string>("MinioConfig:MinioAccessKey");
            string MinioSecretKey = config.GetValue<string>("MinioConfig:MinioSecretKey");
            if (string.IsNullOrEmpty(MinioSSL)) throw new ArgumentNullException(nameof(MinioSSL));
            if (string.IsNullOrEmpty(MinioServer)) throw new ArgumentNullException(nameof(MinioServer));
            if (string.IsNullOrEmpty(MinioAccessKey)) throw new ArgumentNullException(nameof(MinioAccessKey));
            if (string.IsNullOrEmpty(MinioSecretKey)) throw new ArgumentNullException(nameof(MinioSecretKey));
            try
            {
                MinioClient minio;
                if (!bool.Parse(MinioSSL))
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey);
                }
                else
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey).WithSSL();
                }
                bool found = minio.BucketExistsAsync("sct").Result;
                if (!found)
                { minio.MakeBucketAsync("sct"); }
                IObservable<Item> observable = minio.ListObjectsAsync("sct", folderName, true);
                IDisposable subscription = observable.Subscribe(
                item =>
                {
                    if (copyItem)
                    {
                        var task = Task.Run(async () => await minio.CopyObjectAsync("sct", item.Key, "sct", destFolderName + (("/" + item.Key).ToString().Replace(folderName, ""))));
                        task.GetAwaiter().GetResult();
                    }
                    else
                    {
                        var task = Task.Run(async () => await minio.CopyObjectAsync("sct", item.Key, "sct", destFolderName + "/" + item.Key));
                        task.GetAwaiter().GetResult();
                    }
                },
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnComplete: {0}"));
                rs.status = true;
                rs.statusCode = 1;
                return rs;
            }
            catch (MinioException ex)
            {
                rs.status = false;
                rs.statusCode = 2;
                rs.msg = "Copy folder thất bại";
                rs.error = "Error occurred: " + ex.Message;
                return rs;
            }
        }
        public static UploadResult RemoveFolderMinio(IConfiguration config, string folderName)
        {
            UploadResult rs = new UploadResult();
            string MinioSSL = config.GetValue<string>("MinioConfig:MinioSSL");
            string MinioServer = config.GetValue<string>("MinioConfig:MinioServer");
            string MinioAccessKey = config.GetValue<string>("MinioConfig:MinioAccessKey");
            string MinioSecretKey = config.GetValue<string>("MinioConfig:MinioSecretKey");
            if (string.IsNullOrEmpty(MinioSSL)) throw new ArgumentNullException(nameof(MinioSSL));
            if (string.IsNullOrEmpty(MinioServer)) throw new ArgumentNullException(nameof(MinioServer));
            if (string.IsNullOrEmpty(MinioAccessKey)) throw new ArgumentNullException(nameof(MinioAccessKey));
            if (string.IsNullOrEmpty(MinioSecretKey)) throw new ArgumentNullException(nameof(MinioSecretKey));
            try
            {
                MinioClient minio;
                if (!bool.Parse(MinioSSL))
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey);
                }
                else
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey).WithSSL();
                }
                bool found = minio.BucketExistsAsync("sct").Result;
                if (!found)
                { minio.MakeBucketAsync("sct"); }
                IObservable<Item> observable = minio.ListObjectsAsync("sct", folderName, true);
                IDisposable subscription = observable.Subscribe(
                item =>
                {
                    var task = Task.Run(async () => await minio.RemoveObjectAsync("sct", item.Key));
                    task.GetAwaiter().GetResult();
                },
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnComplete: {0}"));
                rs.status = true;
                rs.statusCode = 1;
                return rs;
            }
            catch (MinioException ex)
            {
                rs.status = false;
                rs.statusCode = 3;
                rs.msg = "Xóa folder thất bại";
                rs.error = "Error occurred: " + ex.Message;
                return rs;
            }
        }
        public static UploadResult CheckFileExists(IConfiguration config, string file)
        {
            UploadResult rs = new UploadResult();
            string MinioSSL = config.GetValue<string>("MinioConfig:MinioSSL");
            string MinioServer = config.GetValue<string>("MinioConfig:MinioServer");
            string MinioAccessKey = config.GetValue<string>("MinioConfig:MinioAccessKey");
            string MinioSecretKey = config.GetValue<string>("MinioConfig:MinioSecretKey");
            if (string.IsNullOrEmpty(MinioSSL)) throw new ArgumentNullException(nameof(MinioSSL));
            if (string.IsNullOrEmpty(MinioServer)) throw new ArgumentNullException(nameof(MinioServer));
            if (string.IsNullOrEmpty(MinioAccessKey)) throw new ArgumentNullException(nameof(MinioAccessKey));
            if (string.IsNullOrEmpty(MinioSecretKey)) throw new ArgumentNullException(nameof(MinioSecretKey));
            try
            {
                MinioClient minio;
                if (!bool.Parse(MinioSSL))
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey);
                }
                else
                {
                    minio = new MinioClient(MinioServer, MinioAccessKey, MinioSecretKey).WithSSL();
                }
                bool found = minio.BucketExistsAsync("sct").Result;
                if (!found)
                {
                    minio.MakeBucketAsync("sct");
                }
                var task = Task.Run(async () => await minio.StatObjectAsync("sct", file));
                task.GetAwaiter().GetResult();
                rs.status = true;
                rs.statusCode = 1;
                return rs;
            }
            catch (MinioException ex)
            {
                rs.status = false;
                rs.statusCode = 4;
                rs.msg = "File không tồn tại";
                rs.error = "Error occurred: " + ex.Message;
                return rs;
            }
        }

        public class UploadResult
        {
            public bool status { get; set; }
            public string link { get; set; }
            public int statusCode { get; set; }
            public string msg { get; set; }
            public string error { get; set; }
        }
        public class upLoadFileModel
        {
            public byte[] bs { get; set; }
            public string Linkfile { get; set; }
            public string FileName { get; set; }
        }


        /// <summary>
        /// Đọc file excel thành datatable
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sheetname"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static DataTable ReaddataFromXLSFile(string filename, out string error, int sheet = 0)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; //ko có sẽ lỗi
            DataTable dt = new DataTable();
            using (var excelPack = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    try
                    {
                        excelPack.Load(stream);
                        ExcelWorksheet worksheet = excelPack.Workbook.Worksheets[sheet];
                        int colCount = worksheet.Dimension.End.Column;  //get Column Count
                        int rowCountAll = worksheet.Dimension.End.Row;     //get row count
                        var rowCount = GetLastUsedRow(worksheet);
                        for (int col = 1; col <= colCount; col++)
                        {
                            if (dt.Columns.Contains(worksheet.Cells[1, col].Value?.ToString().Trim()))
                            {
                                dt.Columns.Add(worksheet.Cells[1, col].Value?.ToString().Trim() + " " + col);
                            }
                            else
                            {
                                dt.Columns.Add(worksheet.Cells[1, col].Value?.ToString().Trim());
                            }

                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            DataRow dr = dt.NewRow();
                            for (int col = 1; col <= colCount; col++)
                            {
                                dr[col - 1] = worksheet.Cells[row, col].Value?.ToString().Trim();
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        return dt;
                    }
                }
                error = "";
                return dt;
            }
        }

        public static int GetLastUsedRow(ExcelWorksheet sheet)
        {
            var row = sheet.Dimension.End.Row;
            while (row >= 1)
            {
                var range = sheet.Cells[row, 1, row, sheet.Dimension.End.Column];
                if (range.Any(c => !string.IsNullOrEmpty(c.Text)))
                {
                    break;
                }
                row--;
            }
            return row;
        }

        /// <summary>
        /// Đọc file excel thành datatable
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sheetname"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static List<object> ReadListColumnsFromXLSFile(string filename, out string error)
        {
            List<object> lstCols = new List<object>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; //ko có sẽ lỗi
            using (var excelPack = new ExcelPackage())
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    try
                    {
                        excelPack.Load(stream);
                        ExcelWorksheet worksheet = excelPack.Workbook.Worksheets[0];
                        int colCount = worksheet.Dimension.End.Column;  //get Column Count
                        int rowCountAll = worksheet.Dimension.End.Row;     //get row count
                        var rowCount = GetLastUsedRow(worksheet);


                        for (int col = 1; col <= colCount; col++)
                        {
                            var data = new
                            {
                                idCol = col,
                                nameCol = worksheet.Cells[1, col].Value?.ToString().Trim(),
                                idRowMap = 0,
                                nameMap = "",
                                type = 0
                            };
                            lstCols.Add(data);
                        }
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        return lstCols;
                    }
                }
                error = "";
                return lstCols;
            }
        }

        public static DateTime ConvertTimeZone(IHeaderDictionary _header, DateTime CurrentTime)
        {
            int _timeZone = 0;
            if (_header != null)
                _timeZone = int.Parse(_header["TimeZone"].ToString());
            _timeZone /= -60;
            var _currentLocalTime = CurrentTime.AddHours(_timeZone);
            return _currentLocalTime;
        }

        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[]  { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                                            "đ",
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                                            "í","ì","ỉ","ĩ","ị",
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                                            "ý","ỳ","ỷ","ỹ","ỵ",
                                          };
            string[] arr2 = new string[]  { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                            "d",
                                            "e","e","e","e","e","e","e","e","e","e","e",
                                            "i","i","i","i","i",
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                                            "u","u","u","u","u","u","u","u","u","u","u",
                                            "y","y","y","y","y",
                                          };
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        public T TrimModel<T>(T model)
        {
            var properties = model.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(string))
                {
                    var currentValue = (string)prop.GetValue(model);
                    if (currentValue != null)
                    {
                        var trimmedValue = currentValue.Trim();
                        prop.SetValue(model, trimmedValue);
                    }
                }
            }

            return model;
        }

        public static SystemLog WriteLog(IConfiguration _configuration, HttpContext HttpContext, string Username, string acctionType, string actionName, string contentLog)
        {
            var ipAddress = Ulities.GetIPAddress(HttpContext);
            string ActionName = "";
            if (acctionType == "CREATE")
            {
                ActionName = $"Thêm mới {actionName}";
            }
            else if (acctionType == "UPDATE")
            {
                ActionName = $"Cập nhật {actionName}";
            }
            else if (acctionType == "DELETE")
            {
                ActionName = $"Xóa {actionName}";
            }
            else if (acctionType == "LOGOUT")
            {
                ActionName = "Đăng xuất";
            }
            var data = new SystemLog()
            {
                ApplicationCode = _configuration.GetValue<string>("ApplicationCode"),
                ServiceCode = _configuration.GetValue<string>("ModuleCode"),
                SessionId = "",
                IpPortParentNode = "",
                IpPortCurrentNode = ipAddress,
                ReponseConent = "",
                RequestConent = "",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(7),
                Duration = 0,
                ErrorCode = 0,
                ErrorDescription = "",
                TransactionStatus = "",
                ActionType = acctionType,
                ActionName = ActionName,
                UserName = Username,
                Account = "",
                ContentLog = contentLog
            };
            return data;

        }
    }
}
