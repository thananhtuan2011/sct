using API_SoCongThuong.Models;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using EF_Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace API_SoCongThuong.Reponsitories.CommonImportRepository
{
    public class CommonImportRepo
    {
        public SoHoa_SoCongThuongContext _context;
        public CommonImportRepo(SoHoa_SoCongThuongContext context)
        {
            _context = context;
        }

        public List<ImportResult> GetCommentsByTableName(string TableName)
        {
        //    var query = @"
        //    select * from sys_table t INNER JOIN Sys_Columns c on t.table_name = c.table_name where t.table_name = {0}
        //";
        //    var data = _context.TableResults.FromSqlRaw(query, TableName).Select(x => new TableResult
        //    {
        //        ColumnName= x.ColumnName,
        //        Comment = Regex.Matches(x.Comment, @"(?<=@).*?(?=@)", RegexOptions.Singleline, TimeSpan.FromSeconds(5)).Count > 0 ? Regex.Matches(x.Comment, @"(?<=@).*?(?=@)", RegexOptions.Singleline, TimeSpan.FromSeconds(5))[0].ToString().Replace("{", "").Replace("}", "") : "",
        //        IsNull = x.IsNull,
        //        Exist = Regex.Matches(x.Comment, @"(?<=$).*?(?=$)", RegexOptions.Singleline, TimeSpan.FromSeconds(5)).Count > 0 ? true : false,
        //        Ref = Regex.Matches(x.Comment, @"(?<=#).*?(?=#)", RegexOptions.Singleline, TimeSpan.FromSeconds(5)).Count > 0 ? Regex.Matches(x.Comment, @"(?<=#).*?(?=#)", RegexOptions.Singleline, TimeSpan.FromSeconds(5))[0].ToString().Replace("{", "").Replace("}", "") : "",
        //    }).ToList(); ;
        //    return data;

            var result = (from t in _context.SysTables
                          join s in _context.SysColumns on t.TableKey equals s.TableKey
                          where t.TableKey.Equals(TableName)
                          select new ImportResult
                          {
                              Column = s.ColumnKey,
                              ColumnName = s.ColumnName + " (" + s.DataType + ")",
                              IsNull = s.IsNull,
                              Exist = s.IsExist,
                              Ref = s.RefName,
                              RefId = s.RefId,
                              RefTable = s.RefTable,
                              Url = t.Url
                          }).ToList();

            return result;
        }


        public List<ImportResult> GetItemByUrl(string Url)
        {
            var result = (from t in _context.SysTables
                          where t.Url.Equals(Url)
                          select new ImportResult
                          {
                              TableName = t.TableKey
                          }).ToList();

            return result;
        }

        public List<ImportResult> GetTableName()
        {
            var query = @"
    //        SELECT
    //            t.name AS TableName,
				//epp.value AS FullTableName,
    //            '' AS ColumnName,
    //            '' AS Comment,
    //            '' AS Ref,
    //            CAST(0 AS BIT) AS IsNull,
    //            CAST(0 AS BIT) AS Exist
    //        FROM
    //            sys.tables t
				//INNER JOIN sys.extended_properties epp ON t.object_id = epp.major_id and epp.name =  'name_import'
    //            INNER JOIN sys.columns c ON t.object_id = c.object_id
    //            LEFT OUTER JOIN sys.extended_properties ep ON ep.major_id = c.object_id AND ep.minor_id = c.column_id AND ep.name = 'MS_Description'
    //        WHERE
    //            t.is_ms_shipped = 0 AND
    //            t.name <> 'sysdiagrams' AND
    //            ep.value IS NOT NULL group by t.name , epp.value
    //    ";
            //        var data = _context.TableResults.FromSqlRaw(query).ToList();
            //        return data;
            var result = (from t in _context.SysTables
                          select new ImportResult
                          {
                              TableName= t.TableKey,
                              FullTableName = t.TableName

                          }).ToList();
            return result;
        }

        public int checkTonTai(string tableName, string columnName, string name)
        {
            var query = @$"SELECT * FROM {tableName} WHERE {columnName} LIKE @Name";
            var parameter = new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name };
            var data = _context.TableResults.FromSqlRaw(query, parameter).Count();
            return data;
        }

        public string SaveDataToTable(List<string> columns, string tableName, Dictionary<string, object> data, IConfiguration _configuration)
        {
            // tạo câu lệnh insert vào database
            string columnsString = string.Join(", ", columns);
            string valuesString = string.Join(", ", columns.Select(c => $"@{c}"));
            string sql = $"INSERT INTO {tableName}({columnsString}) VALUES({valuesString})";

            // thực thi câu lệnh insert
            using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
            using (var cmd = new SqlCommand(sql, conn))
            {
                foreach (var col in columns)
                {
                    cmd.Parameters.AddWithValue($"@{col}", data[col] == null ? DBNull.Value : data[col]);
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            if (tableName.Equals("CateManageAncolLocalBussines"))
            {
                sql = $"SELECT top 1 CateManageAncolLocalBussinessId FROM CateManageAncolLocalBussines WHERE 1=1 order by CreateTime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }

            if (tableName.Equals("ProcessAdministrativeProcedures"))
            {
                sql = $"SELECT top 1 ProcessAdministrativeProceduresId FROM ProcessAdministrativeProcedures WHERE 1=1 order by CreateTime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }

            if (tableName.Equals("ParticipateSupportFair"))
            {
                sql = $"SELECT top 1 ParticipateSupportFairId FROM ParticipateSupportFair WHERE 1=1 order by CreateTime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }

            if (tableName.Equals("IndustrialPromotionProject"))
            {
                sql = $"SELECT top 1 IndustrialPromotionProjectId FROM IndustrialPromotionProject WHERE 1=1 order by CreateTime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }

            if (tableName.Equals("ChemicalSafetyCertificate"))
            {
                sql = $"SELECT top 1 ChemicalSafetyCertificateId FROM ChemicalSafetyCertificate WHERE 1=1 order by CreateTime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }

            if (tableName.Equals("RegulationConformityAM"))
            {
                sql = $"SELECT top 1 RegulationConformityAMId FROM RegulationConformityAM WHERE 1=1 order by CreateTime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }


            if (tableName.Equals("CateRetail"))
            {
                sql = $"SELECT top 1 CateRetailId FROM CateRetail WHERE 1=1 order by importtime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }

            if (tableName.Equals("ConsumerServiceRevenue"))
            {
                sql = $"SELECT top 1 ConsumerServiceRevenueId FROM ConsumerServiceRevenue WHERE 1=1 order by importtime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }

            if (tableName.Equals("CateCriteriaNumberSeven"))
            {
                sql = $"SELECT top 1 CateCriteriaNumberSevenId FROM CateCriteriaNumberSeven WHERE 1=1 order by importtime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }

            if (tableName.Equals("RuralDevelopmentPlan"))
            {
                sql = $"SELECT top 1 RuralDevelopmentPlanId FROM RuralDevelopmentPlan WHERE 1=1 order by CreateTime desc";
                using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
            return "";
        }

        public Type GetTypeOfColumn(string tableName, string columnName, IConfiguration _configuration)
        {
            // kết nối đến database và lấy kiểu dữ liệu của cột
            using (var conn = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
            using (var cmd = new SqlCommand($"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'", conn))
            {
                conn.Open();
                string dataTypeName = (string)cmd.ExecuteScalar();

                // map tên kiểu dữ liệu với kiểu Type tương ứng trong C#
                // ví dụ: nvarchar -> typeof(string), int -> typeof(int), ...
                switch (dataTypeName)
                {
                    case "nvarchar":
                    case "varchar":
                    case "text":
                    case "uniqueidentifier":
                        return typeof(string);
                    case "int":
                        return typeof(int);
                    case "float":
                    case "decimal":
                        return typeof(double);
                    case "bit":
                        return typeof(bool);
                    case "datetime":
                    case "datetime2":
                        return typeof(DateTime);
                    default:
                        throw new Exception($"Unknown data type: {dataTypeName}");
                }
            }
        }
        public string GetPrimaryKeyId(string tableName,string primaryKeyid, string primaryKeyColumnName, string primaryKeyValue, IConfiguration _configuration, Guid userid, string tableparent)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetValue<string>("ConnectionStrings:DefaultConnection")))
            {
                connection.Open();
                tableName = tableName.Replace("\r", "").Replace("\n", "");
                SqlCommand command = new SqlCommand($"SELECT {primaryKeyid} FROM {tableName} WHERE {primaryKeyColumnName} = @Value", connection);
                command.Parameters.AddWithValue("@Value", primaryKeyValue);

                if (tableparent.Equals("Petroleum_Business_Store") && tableName.Equals("Business"))
                {
                    string sql = $"INSERT INTO Petroleum_Business(PetroleumBusinessName,CreateUserId) VALUES(@PetroleumBusinessName,@CreateUserId); SELECT top 1 PetroleumBusinessId FROM Petroleum_Business WHERE PetroleumBusinessName is not null order by CreateTime desc";
                    using (var cmd = new SqlCommand(sql, connection))
                    {

                        cmd.Parameters.AddWithValue($"@PetroleumBusinessName", command.ExecuteScalar()?.ToString());
                        cmd.Parameters.AddWithValue($"@CreateUserId", userid);
                        return cmd.ExecuteScalar()?.ToString();
                    }
                }

                if (tableparent.Equals("Cigarette_Business_Store") && tableName.Equals("Business"))
                {
                    string sql = $"INSERT INTO Cigarette_Business(CigaretteBusinessName,CreateUserId) VALUES(@CigaretteBusinessName,@CreateUserId); SELECT top 1 CigaretteBusinessId FROM Cigarette_Business WHERE CigaretteBusinessName is not null order by CreateTime desc";
                    using (var cmd = new SqlCommand(sql, connection))
                    {

                        cmd.Parameters.AddWithValue($"@CigaretteBusinessName", command.ExecuteScalar()?.ToString());
                        cmd.Parameters.AddWithValue($"@CreateUserId", userid);
                        return cmd.ExecuteScalar()?.ToString();
                    }
                }

                if (tableparent.Equals("Alcohol_Bussiness_Detail") && tableName.Equals("Business"))
                {
                    string sql = $"INSERT INTO Alcohol_Business(AlcoholBusinessName,CreateUserId) VALUES(@AlcoholBusinessName,@CreateUserId); SELECT top 1 AlcoholBusinessId FROM Alcohol_Business WHERE AlcoholBusinessName is not null order by CreateTime desc";
                    using (var cmd = new SqlCommand(sql, connection))
                    {

                        cmd.Parameters.AddWithValue($"@AlcoholBusinessName", command.ExecuteScalar()?.ToString());
                        cmd.Parameters.AddWithValue($"@CreateUserId", userid);
                        return cmd.ExecuteScalar()?.ToString();
                    }
                }

                return command.ExecuteScalar()?.ToString();
            }
        }
    }
}
