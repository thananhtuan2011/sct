using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.CategoryRepository;
using EF_Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private CategoryRepo _repo;
        public ConfigController(SoHoa_SoCongThuongContext context)
        {
            _repo = new CategoryRepo(context);
        }

        [HttpGet("{id}")]
        public IActionResult getItemByTypeCode(Guid id)
        {
            BaseModels<ConfigModel> model = new BaseModels<ConfigModel>();
            try
            {
                var categoryTypeCode = _repo._context.CategoryTypes
                    .Where(x => x.CategoryTypeId == id)
                    .Select(x => x.CategoryTypeCode)
                    .FirstOrDefault();

                var errorModel = new ErrorModel();
                if (string.IsNullOrEmpty(categoryTypeCode))
                {
                    errorModel = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(new { status = 0, error = errorModel });
                }

                var data = _repo.FindConfigById(categoryTypeCode);

                if (data.Any())
                {
                    var result = new ListConfigModel();
                    result.ListConfig = data.ToList();
                    return Ok(new { status = 1, items = result });
                } else
                {
                    var result = new ListConfigModel();
                    return Ok(new { status = 1, items = result });
                }
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
        public async Task<IActionResult> Update(ListConfigModel data)
        {
            BaseModels<Category> model = new BaseModels<Category>();
            try
            {
                UserModel loginData = Ulities.GetUserByHeader(HttpContext.Request.Headers);
                if (loginData == null)
                {
                    return BadRequest(ErrMsg_Const.GetMsg(ErrCode_Const.JWT_INVALID_TOKEN));
                }

                //Check Id
                Guid id = Guid.Parse(HttpContext.Request.RouteValues["id"] as string ?? "00000000-0000-0000-0000-000000000000");
                if (id == Guid.Empty)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(model);
                }

                //Check TypeCode
                string typeCode = _repo._context.CategoryTypes.Where(x => x.CategoryTypeId == id).Select(x => x.CategoryTypeCode).FirstOrDefault();
                if (typeCode == null)
                {
                    model.status = 0;
                    model.error = new ErrorModel()
                    {
                        Code = ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY,
                        Msg = "Không có dữ liệu này trên DB",
                    };
                    return NotFound(model);
                }

                //Prepare List Ids
                List<Guid> DbIds = _repo._context.Categories.Where(x => x.CategoryTypeCode == typeCode).Select(x => x.CategoryId).ToList();
                List<Guid> ClientIds = data.ListConfig.Where(x => x.CategoryId != Guid.Empty).Select(x => x.CategoryId).ToList();

                //Xóa Item bị xóa ở Client
                List<Guid> DelIds = DbIds.Except(ClientIds).ToList();
                if (DelIds.Any())
                {
                    var DelData = _repo._context.Categories.Where(x => DelIds.Contains(x.CategoryId)).ToList();
                    if (DelData.Any())
                    {
                        foreach (var item in DelData)
                        {
                            item.IsDel = true;
                            item.UpdateTime = DateTime.Now;
                            item.UpdateUserId = loginData.Userid;
                        }
                    }
                    await _repo.UpdateListConfig(DelData);
                }

                //Thêm Item mới
                List<ConfigModel> newConfig = data.ListConfig.Where(x => x.CategoryId == Guid.Empty).ToList();
                if (newConfig.Any())
                {
                    List<Category> AddData = new List<Category>();
                    foreach (var item in newConfig)
                    {
                        Category config = new Category();
                        config.CategoryCode = item.CategoryCode.Trim();
                        config.CategoryName = item.CategoryName.Trim();
                        config.CategoryTypeCode = item.CategoryTypeCode.Trim();
                        config.Piority = item.Priority;
                        config.IsAction = item.IsAction;
                        config.IsDel = false;
                        config.CreateTime = DateTime.Now;
                        config.CreateUserId = loginData.Userid;
                        AddData.Add(config);
                    }
                    await _repo.InsertListConfig(AddData);
                }

                //Cập nhật Item cũ được chỉnh sửa ở client
                List<ConfigModel> updateConfig = data.ListConfig.Where(x => x.CategoryId != Guid.Empty).ToList();
                if (updateConfig.Any())
                {

                    List<Category> UpdateData = _repo._context.Categories.Where(x => ClientIds.Contains(x.CategoryId)).ToList();
                    foreach (var item in UpdateData)
                    {
                        var UpdateItem = updateConfig.Where(x => x.CategoryId == item.CategoryId).FirstOrDefault();
                        if (UpdateItem != null)
                        {
                            item.CategoryCode = UpdateItem.CategoryCode.Trim();
                            item.CategoryName = UpdateItem.CategoryName.Trim();
                            item.CategoryTypeCode = UpdateItem.CategoryTypeCode.Trim();
                            item.Piority = UpdateItem.Priority;
                            item.IsAction = UpdateItem.IsAction;
                            item.IsDel = false;
                            item.UpdateTime = DateTime.Now;
                            item.UpdateUserId = loginData.Userid;
                        }
                    }
                    await _repo.UpdateListConfig(UpdateData);
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
    }
}

