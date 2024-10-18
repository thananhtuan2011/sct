using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories;
using DocumentFormat.OpenXml.Office2019.Drawing.Diagram11;
using EF_Core.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using ClosedXML.Excel;
using API_SoCongThuong.Logger;
using Newtonsoft.Json;
using System.Data;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommitManagerController : ControllerBase
    {
        private CommitManagerRepo _repo;
        private IConfiguration _configuration;
        private readonly ILogger<AsyncLogger> _logger;
        public SoHoa_SoCongThuongContext _context;
        private AsyncLogger _asyncLogger;

        public CommitManagerController(SoHoa_SoCongThuongContext context, IConfiguration configuration, ILogger<AsyncLogger> logger)
        {
            _repo = new CommitManagerRepo(context);

            _logger = logger;
            _context = context;
            _asyncLogger = new AsyncLogger(_logger, _context);
            _configuration = configuration;
        }

        [Route("find")]
        [HttpPost]
        public IActionResult ListItems_New([FromBody] QueryRequestBody query)//query truyền lên
        {

            BaseModels<CommitManagerModel> model = new BaseModels<CommitManagerModel>();
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                Func<CommitManagerModel, object> _orderByExpression = x => x.MaHoSo; //Khởi tạo mặc định sắp xếp dữ liệu
                Dictionary<string, Func<CommitManagerModel, object>> _sortableFields = new Dictionary<string, Func<CommitManagerModel, object>>
                {
                    { "MaHoSo", x => x.MaHoSo },
                    { "TenThuTuc" , x => x.TenThuTuc },
                    { "TenToChuc", x => x.TenToChuc },
                    { "CoSo" , x => x.CoSo },
                    { "NguoiLamCamKet" , x => x.NguoiLamCamKet },
                    { "SoDienThoai", x => x.SoDienThoai }

                };//Khởi tạo các trường để sắp xếp

                IQueryable <CommitManagerListitemModel> listItems = (from item in _repo._context.CommitManagerListItems
                                 where !item.IsDel
                                 join cate in _repo._context.Categories on item.LoaiHinh equals cate.CategoryId
                                 select new CommitManagerListitemModel()
                                 {
                                     Id = item.Id,
                                     CommitManagerId = item.CommitManagerId,
                                     LoaiHinh = item.LoaiHinh,
                                     TenMatHang = item.TenMatHang,
                                     TenLoaiHinh = cate.CategoryName
                                 });

                IQueryable < CommitManagerModel > _data = _repo._context.CommitManagers.Where(x => !x.IsDel).Select(x => new CommitManagerModel
                {
                    CommitManagerId = x.CommitManagerId,
                    MaHoSo = x.MaHoSo,
                    TenThuTuc = x.TenThuTuc,
                    TenToChuc = x.TenToChuc,
                    CoSo = x.CoSo,
                    Huyen = x.Huyen,
                    DiaChi = x.DiaChi,
                    SoDienThoai = x.SoDienThoai,
                    NgayNhanHoSo = x.NgayNhanHoSo,
                    NgayCamKet = x.NgayCamKet,
                    NguoiLamCamKet = x.NguoiLamCamKet,
                    GhiChu = x.GhiChu,
                    ListItems = listItems.Where(item => item.CommitManagerId == x.CommitManagerId).Select(item => item).ToList()
                }).ToList().AsQueryable();
                    
                //    .Select(x => new CommitManagerModel
                //{

                //});
                if (query.Sort != null
                   && !string.IsNullOrEmpty(query.Sort.ColumnName)
                   && _sortableFields.ContainsKey(query.Sort.ColumnName))
                {
                    _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                    _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
                }
                if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
                {
                    _keywordSearch = query.SearchValue.Trim().ToLower();
                    _data = _data.Where(x =>
                       /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                        || */
                       x.MaHoSo.ToLower().Contains(_keywordSearch) || x.TenThuTuc.ToLower().Contains(_keywordSearch) || x.CoSo.ToLower().Contains(_keywordSearch) || x.TenToChuc.ToLower().Contains(_keywordSearch)
                       || x.NguoiLamCamKet.ToLower().Contains(_keywordSearch) || x.SoDienThoai.Contains(_keywordSearch)
                       );  //Lấy table đã select tìm kiếm theo keyword
                }

                if (query.Filter != null && query.Filter.ContainsKey("Huyen") && !string.IsNullOrEmpty(query.Filter["Huyen"]))
                {
                    _data = _data.Where(x => x.Huyen.ToString() == query.Filter["Huyen"]);
                }

                if (query.Filter != null && query.Filter.ContainsKey("TuNgayNhanHoSo") && !string.IsNullOrEmpty(query.Filter["TuNgayNhanHoSo"]))
                {
                    _data = _data.Where(x => x.NgayNhanHoSo >= DateTime.ParseExact(query.Filter["TuNgayNhanHoSo"], "dd/MM/yyyy", null));
                }
                if (query.Filter != null && query.Filter.ContainsKey("DenNgayNhanHoSo") && !string.IsNullOrEmpty(query.Filter["DenNgayNhanHoSo"]))
                {
                    _data = _data.Where(x => x.NgayNhanHoSo <= DateTime.ParseExact(query.Filter["DenNgayNhanHoSo"], "dd/MM/yyyy", null));
                }

                if (query.Filter != null && query.Filter.ContainsKey("TuNgayCamKet") && !string.IsNullOrEmpty(query.Filter["TuNgayCamKet"]))
                {
                    _data = _data.Where(x => x.NgayCamKet >= DateTime.ParseExact(query.Filter["TuNgayCamKet"], "dd/MM/yyyy", null));
                }
                if (query.Filter != null && query.Filter.ContainsKey("DenNgayCamKet") && !string.IsNullOrEmpty(query.Filter["DenNgayCamKet"]))
                {
                    _data = _data.Where(x => x.NgayCamKet <= DateTime.ParseExact(query.Filter["DenNgayCamKet"], "dd/MM/yyyy", null));
                }
                int _countRows = _data.Count(); //Đếm số dòng của table đã select được
                if (_countRows == 0)    //nếu table = 0 thì trả về không có dữ liệu
                {
                    return NotFound("Không có dữ liệu");
                }
               
  
         
                if (query.Panigator.More)    //query more = true
                {
                    model.status = 1;
                    model.items = _data.ToList();
                    model.total = _countRows;
                    return Ok(model);
                }
                if (_orderBy_ASC) //Sắp xếp dữ liệu theo acs
                {
                    model.items = _data
                        .OrderBy(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }
                else //Sắp xếp dữ liệu theo desc
                {
                    model.items = _data
                        .OrderByDescending(_orderByExpression)
                        .Skip((query.Panigator.PageIndex - 1) * query.Panigator.PageSize)
                        .Take(query.Panigator.PageSize)
                        .ToList();
                }

             
                model.status = 1;
                model.total = _countRows;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(CommitManagerModel data)
        {
            BaseModels<CommitManagerModel> model = new BaseModels<CommitManagerModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                //int count = _repo._context.CommitManagers.Where(x => !x.IsDel && x.CommitManagerId != data.CommitManagerId && x.Code == data.Code).Count();
                //if (count > 0)
                //{
                //    model.status = 0;
                //    model.error = new ErrorModel()
                //    {
                //        Code = ErrCode_Const.EXCEPTION_API,
                //        Msg = "Mã nhóm hồ sơ đã tồn tại!"
                //    };
                //    return Ok(model);
                //}
                SystemLog datalog = new SystemLog();
                CommitManager? _data = _repo._context.CommitManagers.Where(x => !x.IsDel && x.CommitManagerId == data.CommitManagerId).FirstOrDefault();
                if (_data != null)
                {
                    data.UpdateTime = DateTime.Now;
                    data.UpdateUserId = loginData.Userid;
                    await _repo.Update(data);
                    datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.UPDATE, NameCategory_Const.COMMIT_MANAGER, Action_Status.SUCCESS);
                    _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                }

                model.status = 1;
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
        [HttpPost()]
        public async Task<IActionResult> create(CommitManagerModel data)
        {
            BaseModels<CommitManagerModel> model = new BaseModels<CommitManagerModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                data.CreateTime = DateTime.Now;
                data.CreateUserId = loginData.Userid;
                await _repo.Insert(data);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.CREATE, NameCategory_Const.COMMIT_MANAGER, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                model.status = 1;
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


        [HttpPut("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            BaseModels<CommitManagerModel> model = new BaseModels<CommitManagerModel>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }
                SystemLog datalog = new SystemLog();
                CommitManager item = new CommitManager();
                item.CommitManagerId = id;
                item.IsDel = true;
                await _repo.Delete(item);
                datalog = Ulities.WriteLog(_configuration, HttpContext, loginData.Username, ActionType_Const.DELETE, NameCategory_Const.COMMIT_MANAGER, Action_Status.SUCCESS);
                _asyncLogger.LogInfo(JsonConvert.SerializeObject(datalog));
                model.status = 1;
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

        private List<CommitManagerModel> FindData([FromBody] QueryRequestBody query)
        {
            string _keywordSearch = ""; //Keyword tìm kiếm
            bool _orderBy_ASC = true;  //Khởi tạo sắp xếp dữ liệu acs hoặc desc khi tìm kiếm
            Func<CommitManagerModel, object> _orderByExpression = x => x.MaHoSo; //Khởi tạo mặc định sắp xếp dữ liệu
            Dictionary<string, Func<CommitManagerModel, object>> _sortableFields = new Dictionary<string, Func<CommitManagerModel, object>>
                {
                    { "MaHoSo", x => x.MaHoSo },
                    { "TenThuTuc" , x => x.TenThuTuc },
                    { "TenToChuc", x => x.TenToChuc },
                    { "CoSo" , x => x.CoSo },
                    { "NguoiLamCamKet" , x => x.NguoiLamCamKet },
                    { "SoDienThoai", x => x.SoDienThoai }

                };//Khởi tạo các trường để sắp xếp

            IQueryable<CommitManagerListitemModel> listItemProduct = (from item in _repo._context.CommitManagerListItems
                                                                where !item.IsDel
                                                                join cate in _repo._context.Categories on item.LoaiHinh equals cate.CategoryId
                                                                where cate.CategoryCode == "TYPE_ITEMS_1"
                                                                select new CommitManagerListitemModel()
                                                                {
                                                                    Id = item.Id,
                                                                    CommitManagerId = item.CommitManagerId,
                                                                    LoaiHinh = item.LoaiHinh,
                                                                    TenMatHang = item.TenMatHang,
                                                                    TenLoaiHinh = cate.CategoryName
                                                                });
            IQueryable<CommitManagerListitemModel> listItemBusiness = (from item in _repo._context.CommitManagerListItems
                                                                      where !item.IsDel
                                                                      join cate in _repo._context.Categories on item.LoaiHinh equals cate.CategoryId
                                                                      where cate.CategoryCode == "TYPE_ITEMS_2"
                                                                      select new CommitManagerListitemModel()
                                                                      {
                                                                          Id = item.Id,
                                                                          CommitManagerId = item.CommitManagerId,
                                                                          LoaiHinh = item.LoaiHinh,
                                                                          TenMatHang = item.TenMatHang,
                                                                          TenLoaiHinh = cate.CategoryName
                                                                      });
            IQueryable<CommitManagerModel> _data = (from cm in _repo._context.CommitManagers
                                                    where !cm.IsDel
                                                    join d in _repo._context.Districts on cm.Huyen equals d.DistrictId
                                                    select new CommitManagerModel()
                                                    {
                                                        CommitManagerId = cm.CommitManagerId,
                                                        MaHoSo = cm.MaHoSo,
                                                        TenThuTuc = cm.TenThuTuc,
                                                        TenToChuc = cm.TenToChuc,
                                                        CoSo = cm.CoSo,
                                                        Huyen = cm.Huyen,
                                                        DiaChi = cm.DiaChi,
                                                        SoDienThoai = cm.SoDienThoai,
                                                        NgayNhanHoSo = cm.NgayNhanHoSo,
                                                        NgayCamKet = cm.NgayCamKet,
                                                        NguoiLamCamKet = cm.NguoiLamCamKet,
                                                        GhiChu = cm.GhiChu,
                                                        TenHuyen = d.DistrictName,
                                                        ListItemProduct = listItemProduct.Where(item => item.CommitManagerId == cm.CommitManagerId).Select(item => item).ToList(),
                                                        ListItemBusiness = listItemBusiness.Where(item => item.CommitManagerId == cm.CommitManagerId).Select(item => item).ToList()
                                                    });
     
            if (query.Sort != null
               && !string.IsNullOrEmpty(query.Sort.ColumnName)
               && _sortableFields.ContainsKey(query.Sort.ColumnName))
            {
                _orderBy_ASC = ("desc".Equals(query.Sort.Direction.ToLower()) ? false : true);    //Sắp xếp asc hoặc desc
                _orderByExpression = _sortableFields[query.Sort.ColumnName]; //Trường cần sắp xếp
            }
            if (query.SearchValue != null && query.SearchValue != "") //Kiểm tra điều kiện tìm kiếm
            {
                _keywordSearch = query.SearchValue.Trim().ToLower();
                _data = _data.Where(x =>
                   /* x.DistrictId.ToString().ToLower().Contains(_keywordSearch)
                    || */
                   x.MaHoSo.ToLower().Contains(_keywordSearch) || x.TenThuTuc.ToLower().Contains(_keywordSearch) || x.CoSo.ToLower().Contains(_keywordSearch) || x.TenToChuc.ToLower().Contains(_keywordSearch)
                   || x.NguoiLamCamKet.ToLower().Contains(_keywordSearch) || x.SoDienThoai.Contains(_keywordSearch)
                   );  //Lấy table đã select tìm kiếm theo keyword
            }

            if (query.Filter != null && query.Filter.ContainsKey("Huyen") && !string.IsNullOrEmpty(query.Filter["Huyen"]))
            {
                _data = _data.Where(x => x.Huyen.ToString() == query.Filter["Huyen"]);
            }

            if (query.Filter != null && query.Filter.ContainsKey("TuNgayNhanHoSo") && !string.IsNullOrEmpty(query.Filter["TuNgayNhanHoSo"]))
            {
                _data = _data.Where(x => x.NgayNhanHoSo >= DateTime.ParseExact(query.Filter["TuNgayNhanHoSo"], "dd/MM/yyyy", null));
            }
            if (query.Filter != null && query.Filter.ContainsKey("DenNgayNhanHoSo") && !string.IsNullOrEmpty(query.Filter["DenNgayNhanHoSo"]))
            {
                _data = _data.Where(x => x.NgayNhanHoSo <= DateTime.ParseExact(query.Filter["DenNgayNhanHoSo"], "dd/MM/yyyy", null));
            }

            if (query.Filter != null && query.Filter.ContainsKey("TuNgayCamKet") && !string.IsNullOrEmpty(query.Filter["TuNgayCamKet"]))
            {
                _data = _data.Where(x => x.NgayCamKet >= DateTime.ParseExact(query.Filter["TuNgayCamKet"], "dd/MM/yyyy", null));
            }
            if (query.Filter != null && query.Filter.ContainsKey("DenNgayCamKet") && !string.IsNullOrEmpty(query.Filter["DenNgayCamKet"]))
            {
                _data = _data.Where(x => x.NgayCamKet <= DateTime.ParseExact(query.Filter["DenNgayCamKet"], "dd/MM/yyyy", null));
            }
            int _countRows = _data.Count(); //Đếm số dòng của table đã select được
            
            return _data.ToList();
        }

        [HttpPost("Export")]
        public IActionResult Export([FromBody] QueryRequestBody query)
        {
            //Query data
            var data = FindData(query);
           // var _data = OkObjectResult(dat)
            if (data == null)
            {
                return BadRequest();
            }

            try
            {
                using (var workbook = new XLWorkbook(@"Upload/Templates/Quanlycamket.xlsx"))
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Worksheet(1);
                    int index = 6;
                    int row = 1;

               //     worksheet.Cell(1, 1).Value = "THỐNG KÊ SỐ LƯỢNG CHỢ, SIÊU THỊ, TRUNG TÂM THƯƠNG MẠI - TỈNH BẾN TRE";

                    //Thêm dữ liệu vào file:
                    foreach (var item in data)
                    {
                        if (row == 1)
                        {
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.MaHoSo;
                            worksheet.Cell(index, 3).Value = item.TenThuTuc;
                            worksheet.Cell(index, 4).Value = item.TenToChuc;
                            worksheet.Cell(index, 5).Value = item.TenHuyen;
                            worksheet.Cell(index, 6).Value = item.DiaChi;
                            worksheet.Cell(index, 7).Value = item.SoDienThoai;
                            worksheet.Cell(index, 8).Value = item.NgayNhanHoSo.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 9).Value = item.NgayCamKet.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 10).Value = item.NguoiLamCamKet;
                            worksheet.Cell(index, 13).Value = item.GhiChu;
                            worksheet.Cell(index, 14).Value = item.CoSo;
                            if (item.ListItemProduct.Count > 0)
                            {
                                var str = "";
                                foreach (var itp in item.ListItemProduct)
                                {
                                    str += itp.TenMatHang;
                                    str += "\r\n";
                                }
                                worksheet.Cell(index, 11).Value = str;
                            }
                            if (item.ListItemBusiness.Count > 0)
                            {
                                var str = "";
                                foreach (var itp in item.ListItemBusiness)
                                {
                                    str += itp.TenMatHang;
                                    str += "\r\n";
                                }
                                worksheet.Cell(index, 12).Value = str;
                            }
                            index++;
                            row++;
                        }
                        else
                        {
                            var addrow = worksheet.Row(index - 1);
                            addrow.InsertRowsBelow(1);
                            worksheet.Cell(index, 1).Value = row;
                            worksheet.Cell(index, 2).Value = item.MaHoSo;
                            worksheet.Cell(index, 3).Value = item.TenThuTuc;
                            worksheet.Cell(index, 4).Value = item.TenToChuc;
                            worksheet.Cell(index, 5).Value = item.TenHuyen;
                            worksheet.Cell(index, 6).Value = item.DiaChi;
                            worksheet.Cell(index, 7).Value = item.SoDienThoai;
                            worksheet.Cell(index, 8).Value = item.NgayNhanHoSo.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 9).Value = item.NgayCamKet.ToString("dd/MM/yyyy");
                            worksheet.Cell(index, 10).Value = item.NguoiLamCamKet;
                            worksheet.Cell(index, 13).Value = item.GhiChu;
                            worksheet.Cell(index, 14).Value = item.CoSo;
                            if (item.ListItemProduct.Count > 0)
                            {
                                var str = "";
                                foreach (var itp in item.ListItemProduct)
                                {
                                    str += itp.TenMatHang;
                                    str += "\r\n";
                                }
                                worksheet.Cell(index, 11).Value = str;
                            }
                            if (item.ListItemBusiness.Count > 0)
                            {
                                var str = "";
                                foreach (var itp in item.ListItemBusiness)
                                {
                                    str += itp.TenMatHang;
                                    str += "\r\n";
                                }
                                worksheet.Cell(index, 12).Value = str;
                            }
                            index++;
                            row++;
                        }
                    }
                    var delrow = worksheet.Row(index);
                    delrow.Delete();
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Flush();
                        stream.Position = 0;

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
