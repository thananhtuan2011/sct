using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CommonImportRepository;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using EF_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static API_SoCongThuong.Classes.Ulities;

namespace API_SoCongThuong.Controllers
{
    [Route("api/import")]
    [ApiController]
    public class CommonImportController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        private CommonImportRepo _repo;
        public CommonImportController(IWebHostEnvironment hostingEnvironment, SoHoa_SoCongThuongContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _repo = new CommonImportRepo(context);
        }

        #region ImportFile
        /// <summary>
        /// Upload file excel  
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("ImportExcel")]
        [HttpPost]
        public object ImportFile([FromForm] string data)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                var _ = JsonConvert.DeserializeObject<List<ImportModel>>(data);
                var Files = Request.Form.Files;
                if (Files[0].Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Files[0].CopyTo(ms);
                        upLoadFileModel up = new upLoadFileModel()
                        {
                            bs = ms.ToArray(),
                            FileName = Files[0].FileName.Replace(" ", ""),
                            Linkfile = "Import"
                        };
                        string _targetPath = Path.Combine(_hostingEnvironment.ContentRootPath, "data/import/");

                        if (!Directory.Exists(_targetPath))
                            Directory.CreateDirectory(_targetPath);

                        string _fileName = _targetPath + Files[0].FileName.Replace(" ", "");
                        System.IO.File.WriteAllBytes(_fileName, ms.ToArray());
                        string error = "";
                        var data_import = ReaddataFromXLSFile(_fileName, out error);

                        // Khởi tạo danh sách các string
                        List<string> columnsList = new List<string>();

                        // Lấy danh sách các cột trong DataTable và chuyển chúng thành string
                        //int dem = 0;
                        //foreach (DataColumn column in data_import.Columns)
                        //{
                        //    var dtc = _.Where(x => x.nameCol.Equals(column.ColumnName));
                        //    if (dtc.Any())
                        //    {
                        //        columnsList.Add(column.ColumnName);
                        //    }
                        //    dem++;
                        //}
                        foreach (var item in _)
                        {
                            columnsList.Add(item.nameCol);
                        }

                        // Khởi tạo danh sách các mảng string
                        List<string[]> rowsList = new List<string[]>();

                        // Lấy danh sách các hàng trong DataTable và chuyển chúng thành mảng string
                        int dem2 = 0;
                        foreach (DataRow row in data_import.Rows)
                        {
                            dem2++;
                            string[] rowData = new string[data_import.Columns.Count];
                            for (int i = 0; i < _.Count(); i++)
                            {
                                //kiểm tra kiểm dữ liệu


                                if (string.IsNullOrEmpty(row[_[i].idCol - 1].ToString()) && _[i].isNull == false)
                                {
                                    rowData[i] = "2" + "%%" + dem2;
                                }
                                else if (_[i].reftable != null)
                                {
                                    //kiểm tra dữ liệu ref và không được để trống đồng thời kiểm tra xem text có tồn tại trong bản ref hay không?
                                    try
                                    {
                                        var d = _repo.checkTonTai(_[i].reftable, _[i].refCol, row[_[i].idCol - 1].ToString());
                                        if (d == 0)
                                        {
                                            rowData[i] = "4" + "%%" + dem2;
                                        }
                                        else
                                        {
                                            rowData[i] = "1" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        rowData[i] = "5" + "%%" + dem2;
                                    }

                                }
                                else if (_[i].exist)
                                {
                                    var d = _repo.checkTonTai(_[i].type, _[i].idRowMap, row[_[i].idCol - 1].ToString());
                                    if (d == 0)
                                    {
                                        rowData[i] = "3" + "%%" + dem2;
                                    }
                                    else
                                    {
                                        rowData[i] = "1" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                    }
                                }
                                else
                                {
                                    // kiểm tra data type
                                    var coltype = _repo._context.SysColumns.Where(x => x.ColumnKey.Equals(_[i].idRowMap)).FirstOrDefault();
                                    if (coltype == null)
                                    {
                                        rowData[i] = "1" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                    }
                                    else
                                    {
                                        if (coltype.DataType == "text")
                                        {
                                            if (row[_[i].idCol - 1].ToString() is string str)
                                            {
                                                rowData[i] = "1" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                            }
                                            else
                                            {

                                                rowData[i] = "6" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                            }
                                        }
                                        else if (coltype.DataType == "datetime")
                                        {
                                            if (DateTime.TryParseExact(row[_[i].idCol - 1].ToString().ToLower(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dti))
                                            {
                                                rowData[i] = "1" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                            }
                                            else
                                            {
                                                if (coltype.IsNull == true && string.IsNullOrEmpty(row[_[i].idCol - 1].ToString()))
                                                {
                                                    rowData[i] = "1" + "%%" + dem2;
                                                }
                                                else
                                                {
                                                    rowData[i] = "7" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                                }
                                            }
                                        }
                                        else if (coltype.DataType == "int")
                                        {
                                            int number;
                                            if (int.TryParse(row[_[i].idCol - 1].ToString(), out number))
                                            {
                                                rowData[i] = "1" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                            }
                                            else
                                            {
                                                rowData[i] = "8" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                            }
                                        }
                                        else
                                        {
                                            rowData[i] = "1" + row[_[i].idCol - 1].ToString() + "%%" + dem2;
                                        }
                                    }

                                }

                            }
                            rowsList.Add(rowData);
                        }

                        Console.WriteLine("]");
                        var dt = new
                        {
                            columns = columnsList,
                            rows = rowsList
                        };
                        model.status = 1;
                        model.data = dt;
                        return Ok(model);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }
        #endregion


        #region ImportFile
        /// <summary>
        /// Upload file excel  
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("ImportExcelSave")]
        [HttpPost]
        public object ImportFileSave([FromForm] string data, [FromForm] string remove)
        {
            BaseModels<object> model = new BaseModels<object>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var _ = JsonConvert.DeserializeObject<List<ImportModel>>(data);
                var list_remove = JsonConvert.DeserializeObject<List<string>>(remove);

                var Files = Request.Form.Files;
                if (Files[0].Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Files[0].CopyTo(ms);
                        upLoadFileModel up = new upLoadFileModel()
                        {
                            bs = ms.ToArray(),
                            FileName = Files[0].FileName.Replace(" ", ""),
                            Linkfile = "Import"
                        };
                        string _targetPath = Path.Combine(_hostingEnvironment.ContentRootPath, "data/import/");

                        if (!Directory.Exists(_targetPath))
                            Directory.CreateDirectory(_targetPath);

                        string _fileName = _targetPath + Files[0].FileName.Replace(" ", "");
                        System.IO.File.WriteAllBytes(_fileName, ms.ToArray());
                        string error = "";
                        var data_import = ReaddataFromXLSFile(_fileName, out error);


                        // Lấy danh sách các hàng trong DataTable và chuyển chúng thành mảng string
                        int dem = 0;
                        foreach (DataRow row in data_import.Rows)
                        {
                            dem++;
                            string[] rowData = new string[data_import.Columns.Count];
                            Dictionary<string, object> values = new Dictionary<string, object>();
                            List<string> columns = new List<string>();
                            string nameTable = "";
                            if (!list_remove.Contains(dem.ToString()))
                            {
                                for (int i = 0; i < _.Count(); i++)
                                {
                                    columns.Add(_[i].idRowMap);
                                    var name = _[i].idRowMap;
                                    if (values.ContainsKey(_[i].idRowMap))
                                    {
                                        name = _[i].idRowMap + "1";
                                    }

                                    // kiểm tra data type
                                    var coltype = _repo._context.SysColumns.Where(x => x.ColumnKey.Equals(_[i].idRowMap)).FirstOrDefault();
                                    if (_[i].reftable != "" && _[i].reftable != null)
                                    {
                                        values[name] = _repo.GetPrimaryKeyId(_[i].reftable, _[i].refId, _[i].refCol, row[_[i].idCol - 1].ToString(), _configuration, loginData.Userid, _[i].type);
                                    }
                                    else if (coltype != null && coltype.DataType == "datetime")
                                    {
                                        if (coltype.IsNull == true && string.IsNullOrEmpty(row[_[i].idCol - 1].ToString()))
                                        {
                                            values[name] = null;
                                        }
                                        else
                                        {
                                            DateTime dateValue = DateTime.ParseExact(row[_[i].idCol - 1].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            values[name] = dateValue;
                                        }
                                    }
                                    else
                                    {
                                        values[name] = row[_[i].idCol - 1].ToString();
                                    }
                                    nameTable = _[i].type;
                                }
                                if (!values.ContainsKey("CreateUserId"))
                                {
                                    columns.Add("CreateUserId");
                                    values["CreateUserId"] = loginData.Userid;
                                }
                                List<string> nganhnghe = new List<string>();
                                List<string> danhsachcodong = new List<string>();
                                List<string> danhsachgopvon = new List<string>();
                                if (nameTable.Equals("CateManageAncolLocalBussines"))
                                {
                                    nganhnghe = values["nganhnghekinhdoanh"].ToString().Split(";").ToList();
                                    values.Remove("nganhnghekinhdoanh");
                                    columns.Remove("nganhnghekinhdoanh");

                                    danhsachcodong = values["danhsachcodong"].ToString().Split(",").ToList();
                                    values.Remove("danhsachcodong");
                                    columns.Remove("danhsachcodong");

                                    danhsachgopvon = values["danhsachgopvon"].ToString().Split(",").ToList();
                                    values.Remove("danhsachgopvon");
                                    columns.Remove("danhsachgopvon");
                                }

                                //lưu dữ liệu bảng chính
                                var id = _repo.SaveDataToTable(columns, nameTable, values, _configuration);

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("CateManageAncolLocalBussines"))
                                {
                                    foreach (var item in nganhnghe)
                                    {
                                        values = new Dictionary<string, object>();
                                        columns = new List<string>();
                                        string idd = _repo.GetPrimaryKeyId("Industry", "IndustryId", "IndustryName", item.ToString().Trim(), _configuration, loginData.Userid, nameTable);
                                        if (!string.IsNullOrEmpty(idd))
                                        {
                                            columns.Add("TypeOfProfessionId");
                                            values["TypeOfProfessionId"] = idd;
                                            columns.Add("CateManageAncolLocalBussinessId");
                                            values["CateManageAncolLocalBussinessId"] = id;
                                            _repo.SaveDataToTable(columns, "CateManageAncolLocalBussines_TypeOfProfession", values, _configuration);
                                        }
                                    }

                                    foreach (var item in danhsachcodong)
                                    {
                                        values = new Dictionary<string, object>();
                                        columns = new List<string>();
                                        columns.Add("Fullname");
                                        values["Fullname"] = item;
                                        columns.Add("Type");
                                        values["Type"] = 1;
                                        columns.Add("CateManageAncolLocalBussinessId");
                                        values["CateManageAncolLocalBussinessId"] = id;
                                        _repo.SaveDataToTable(columns, "CateManageAncolLocalBussines_Detail", values, _configuration);
                                    }

                                    foreach (var item in danhsachgopvon)
                                    {
                                        values = new Dictionary<string, object>();
                                        columns = new List<string>();
                                        columns.Add("Fullname");
                                        values["Fullname"] = item;
                                        columns.Add("Type");
                                        values["Type"] = 2;
                                        columns.Add("CateManageAncolLocalBussinessId");
                                        values["CateManageAncolLocalBussinessId"] = id;
                                        _repo.SaveDataToTable(columns, "CateManageAncolLocalBussines_Detail", values, _configuration);
                                    }
                                }

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("ProcessAdministrativeProcedures"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    int step = 0;
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[3].ToString()) == dem)
                                        {
                                            step++;
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "ProcessAdministrativeProcedures_Step";
                                            columns.Add("ProcessAdministrativeProceduresId");
                                            values["ProcessAdministrativeProceduresId"] = id;

                                            columns.Add("Step");
                                            values["Step"] = step;

                                            columns.Add("ImplementingAgencies");
                                            values["ImplementingAgencies"] = row2[0].ToString();

                                            columns.Add("ProcessingTime");
                                            values["ProcessingTime"] = row2[1].ToString(); ;

                                            columns.Add("ContentImplementation");
                                            values["ContentImplementation"] = row2[2].ToString(); ;
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("ParticipateSupportFair"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[7].ToString()) == dem)
                                        {
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "ParticipateSupportFairDetail";
                                            columns.Add("ParticipateSupportFairId");
                                            values["ParticipateSupportFairId"] = id;


                                            string idd = _repo.GetPrimaryKeyId("Business", "BusinessId", "BusinessNameVi", row2[0].ToString().Trim(), _configuration, loginData.Userid, nameTable);
                                            if (string.IsNullOrEmpty(idd))
                                            {
                                                idd = "00000000-0000-0000-0000-000000000000";
                                            }

                                            columns.Add("BusinessId");
                                            values["BusinessId"] = idd;

                                            columns.Add("BusinessCode");
                                            values["BusinessCode"] = row2[1].ToString();

                                            columns.Add("BusinessNameVi");
                                            values["BusinessNameVi"] = row2[0].ToString(); ;

                                            columns.Add("NganhNghe");
                                            values["NganhNghe"] = row2[6].ToString();

                                            columns.Add("DiaChi");
                                            values["DiaChi"] = row2[2].ToString();

                                            columns.Add("NguoiDaiDien");
                                            values["NguoiDaiDien"] = row2[3].ToString();

                                            columns.Add("Huyen");
                                            values["Huyen"] = row2[4].ToString();

                                            columns.Add("Xa");
                                            values["Xa"] = row2[5].ToString();
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("IndustrialPromotionProject"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[5].ToString()) == dem)
                                        {
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "IndustrialPromotionProjectDetail";
                                            columns.Add("IndustrialPromotionProjectId");
                                            values["IndustrialPromotionProjectId"] = id;


                                            string idd = _repo.GetPrimaryKeyId("Business", "BusinessId", "BusinessNameVi", row2[0].ToString().Trim(), _configuration, loginData.Userid, nameTable);
                                            if (string.IsNullOrEmpty(idd))
                                            {
                                                idd = "00000000-0000-0000-0000-000000000000";
                                            }

                                            columns.Add("BusinessId");
                                            values["BusinessId"] = idd;

                                            columns.Add("BusinessCode");
                                            values["BusinessCode"] = row2[1].ToString();

                                            columns.Add("BusinessNameVi");
                                            values["BusinessNameVi"] = row2[0].ToString(); ;

                                            columns.Add("NganhNghe");
                                            values["NganhNghe"] = row2[4].ToString();

                                            columns.Add("DiaChi");
                                            values["DiaChi"] = row2[2].ToString();

                                            columns.Add("NguoiDaiDien");
                                            values["NguoiDaiDien"] = row2[3].ToString();
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("CateRetail"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[4].ToString()) == dem)
                                        {
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "CateRetail_Detail";
                                            columns.Add("CateRetailId");
                                            values["CateRetailId"] = id;


                                            string idd = _repo.GetPrimaryKeyId("Category", "CategoryId", "CategoryName", row2[0].ToString().Trim(), _configuration, loginData.Userid, nameTable);
                                            if (string.IsNullOrEmpty(idd))
                                            {
                                                idd = "00000000-0000-0000-0000-000000000000";
                                            }

                                            columns.Add("Criteria");
                                            values["Criteria"] = idd;

                                            columns.Add("PerformLastmonth");
                                            values["PerformLastmonth"] = row2[1].ToString();

                                            columns.Add("EstimateReportingMonth");
                                            values["EstimateReportingMonth"] = row2[2].ToString(); ;

                                            columns.Add("CumulativeToReportingMonth");
                                            values["CumulativeToReportingMonth"] = row2[3].ToString();
                                            columns.Add("Type");
                                            values["Type"] = 1;
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("ConsumerServiceRevenue"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[4].ToString()) == dem)
                                        {
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "ConsumerServiceRevenue_Detail";
                                            columns.Add("ConsumerServiceRevenueId");
                                            values["ConsumerServiceRevenueId"] = id;


                                            string idd = _repo.GetPrimaryKeyId("Category", "CategoryId", "CategoryName", row2[0].ToString().Trim(), _configuration, loginData.Userid, nameTable);
                                            if (string.IsNullOrEmpty(idd))
                                            {
                                                idd = "00000000-0000-0000-0000-000000000000";
                                            }

                                            columns.Add("Criteria");
                                            values["Criteria"] = idd;

                                            columns.Add("PerformLastmonth");
                                            values["PerformLastmonth"] = row2[1].ToString();

                                            columns.Add("EstimateReportingMonth");
                                            values["EstimateReportingMonth"] = row2[2].ToString(); ;

                                            columns.Add("CumulativeToReportingMonth");
                                            values["CumulativeToReportingMonth"] = row2[3].ToString();
                                            columns.Add("Type");
                                            values["Type"] = 1;
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("CateCriteriaNumberSeven"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[10].ToString()) == dem)
                                        {
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "CateCriteriaNumberSevenDetail";
                                            columns.Add("CateCriteriaNumberSevenId");
                                            values["CateCriteriaNumberSevenId"] = id;


                                            string idd = _repo.GetPrimaryKeyId("District", "DistrictId", "DistrictName", row2[0].ToString().Trim(), _configuration, loginData.Userid, nameTable);
                                            if (string.IsNullOrEmpty(idd))
                                            {
                                                idd = "00000000-0000-0000-0000-000000000000";
                                            }

                                            columns.Add("DistrictId");
                                            values["DistrictId"] = idd;

                                            columns.Add("NumberOfWard");
                                            values["NumberOfWard"] = row2[1].ToString();

                                            columns.Add("NumberOfQualifyingWard");
                                            values["NumberOfQualifyingWard"] = row2[2].ToString(); ;

                                            columns.Add("NumberOfWardWithMarket");
                                            values["NumberOfWardWithMarket"] = row2[3].ToString();

                                            columns.Add("NumberOfWardCommercialInfrastructure");
                                            values["NumberOfWardCommercialInfrastructure"] = row2[4].ToString();

                                            columns.Add("NumberOfWardNewCountryside");
                                            values["NumberOfWardNewCountryside"] = row2[5].ToString();

                                            columns.Add("NumberOfWardCommercialInfrastructure_Plan");
                                            values["NumberOfWardCommercialInfrastructure_Plan"] = row2[6].ToString();

                                            columns.Add("NumberOfWardNewCountryside_Plan");
                                            values["NumberOfWardNewCountryside_Plan"] = row2[7].ToString();

                                            columns.Add("NumberOfWardCommercialInfrastructure_Estimate");
                                            values["NumberOfWardCommercialInfrastructure_Estimate"] = row2[8].ToString();

                                            columns.Add("NumberOfWardNewCountryside_Estimate");
                                            values["NumberOfWardNewCountryside_Estimate"] = row2[9].ToString();
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("ChemicalSafetyCertificate"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[5].ToString()) == dem)
                                        {
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "ChemicalSafetyCertificate_ChemicalInfo";
                                            columns.Add("ChemicalSafetyCertificateId");
                                            values["ChemicalSafetyCertificateId"] = id;

                                            columns.Add("TradeName");
                                            values["TradeName"] = row2[0].ToString();

                                            columns.Add("NameOfChemical");
                                            values["NameOfChemical"] = row2[1].ToString(); ;

                                            columns.Add("CASCode");
                                            values["CASCode"] = row2[2].ToString();

                                            columns.Add("ChemicalFormula");
                                            values["ChemicalFormula"] = row2[3].ToString();

                                            columns.Add("Content");
                                            values["Content"] = row2[4].ToString();

                                            columns.Add("Mass");
                                            values["Mass"] = row2[5].ToString();
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }

                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("RegulationConformityAM"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[2].ToString()) == dem)
                                        {
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "RegulationConformityAM_Product";
                                            columns.Add("RegulationConformityAMId");
                                            values["RegulationConformityAMId"] = id;

                                            columns.Add("ProductName");
                                            values["ProductName"] = row2[0].ToString();

                                            columns.Add("Note");
                                            values["Note"] = row2[1].ToString();
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }


                                if (!string.IsNullOrEmpty(id) && nameTable.Equals("RuralDevelopmentPlan"))
                                {
                                    data_import = ReaddataFromXLSFile(_fileName, out error, 1);
                                    foreach (DataRow row2 in data_import.Rows)
                                    {
                                        if (int.Parse(row2[3].ToString()) == dem)
                                        {
                                            rowData = new string[data_import.Columns.Count];
                                            values = new Dictionary<string, object>();
                                            columns = new List<string>();
                                            nameTable = "RuralDevelopmentPlan_Stage";
                                            columns.Add("RuralDevelopmentPlanId");
                                            values["RuralDevelopmentPlanId"] = id;


                                            string idd = _repo.GetPrimaryKeyId("Stage", "StageId", "StageName", row2[0].ToString().Trim(), _configuration, loginData.Userid, nameTable);
                                            if (string.IsNullOrEmpty(idd))
                                            {
                                                idd = "00000000-0000-0000-0000-000000000000";
                                            }

                                            columns.Add("StageId");
                                            values["StageId"] = idd;

                                            columns.Add("Year");
                                            values["Year"] = row2[1].ToString();

                                            columns.Add("Budget");
                                            values["Budget"] = row2[2].ToString();
                                            _repo.SaveDataToTable(columns, nameTable, values, _configuration);
                                        }

                                    }
                                }

                            }


                        }
                        var dt = new
                        {
                        };
                        model.status = 1;
                        model.data = dt;
                        return Ok(model);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }
        #endregion

        #region lấy danh sách columns
        /// <summary>
        /// Upload file excel  
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("GetListColumns")]
        [HttpPost]
        public object GetListColumns()
        {
            try
            {
                var Files = Request.Form.Files;
                if (Files[0].Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Files[0].CopyTo(ms);
                        string _targetPath = Path.Combine(_hostingEnvironment.ContentRootPath, "data/import/");

                        if (!Directory.Exists(_targetPath))
                            Directory.CreateDirectory(_targetPath);

                        string _fileName = _targetPath + Files[0].FileName.Replace(" ", "");
                        System.IO.File.WriteAllBytes(_fileName, ms.ToArray());
                        string error = "";
                        var data_import = ReadListColumnsFromXLSFile(_fileName, out error);
                        return data_import;
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }
        #endregion

        #region Danh sách danh mục
        [Route("list-danh-muc")]
        [HttpGet]
        public IActionResult LoadDanhSachDanhMuc()
        {
            BaseModels<object> model = new BaseModels<object>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var data = from r in _repo.GetTableName()
                           select new
                           {
                               id = r.TableName,
                               name = r.FullTableName
                           };
                model.status = 1;
                model.data = data;
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }
        #endregion

        #region Danh sách danh mục
        [Route("get-danh-muc-by-id")]
        [HttpGet]
        public IActionResult LoadDanhSachDanhMucById(string id)
        {
            BaseModels<object> model = new BaseModels<object>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var data = from r in _repo.GetCommentsByTableName(id)
                           select new
                           {
                               id = r.Column,
                               name = r.ColumnName,
                               isNull = r.IsNull,
                               refCol = r.Ref,
                               exist = r.Exist,
                               refId = r.RefId,
                               refTable = r.RefTable,
                               url = r.Url
                           };
                if (id == "AdministrativeProcedures")
                {
                    data = data.Where(x => x.url.Equals("ReportAdministrativeProcedures/list"));
                }

                if (id == "IndustrialPromotionProject")
                {
                    data = data.Where(x => x.url.Equals("ParticipateTradePromotion/list"));
                }
                model.status = 1;
                model.data = data;
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }
        #endregion

        #region Danh sách danh mục
        [Route("get-item-by-url")]
        [HttpGet]
        public IActionResult LoadItemMucByUrl(string id)
        {
            BaseModels<object> model = new BaseModels<object>();

            try
            {
                //Lấy Token, lấy model
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                var data = from r in _repo.GetItemByUrl(id)
                           select new
                           {
                               id = r.TableName
                           };
                model.status = 1;
                model.data = data;
                return Ok(model);
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.error = new ErrorModel()
                {
                    Code = ErrCode_Const.EXCEPTION_API,
                    Msg = ex.Message
                };
                return BadRequest(model);
            }
        }
        #endregion


        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            string keyword = "";
            if (!string.IsNullOrEmpty(query.SearchValue))
            {
                keyword = query.SearchValue.Trim().ToLower();
            }

            try
            {
                if (keyword.Equals("financialplantargets") || keyword.Equals("cateproject"))
                {
                    // Đường dẫn đến tập tin zip đã tồn tại
                    string zipFilePath = $@"Upload/Import/{keyword}.zip";

                    // Kiểm tra xem tập tin có tồn tại không
                    if (System.IO.File.Exists(zipFilePath))
                    {
                        // Mở tập tin và đọc dữ liệu
                        var fileData = System.IO.File.ReadAllBytes(zipFilePath);

                        // Trả về tập tin zip
                        return File(fileData, "application/octet-stream", "file.zip");
                    }

                }
                else
                {
                    using (var workbook = new XLWorkbook(@$"Upload/Import/{keyword}.xlsx"))
                    {
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            stream.Flush();
                            stream.Position = 0;

                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                        }
                    }
                }
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
