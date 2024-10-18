using API_SoCongThuong.Classes;
using API_SoCongThuong.Models;
using API_SoCongThuong.Reponsitories.StatisticalImportExportDistrictRepository;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Wordprocessing;
using EF_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace API_SoCongThuong.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticalImportExportDistrictController : ControllerBase
    {
        private StatisticalImportExportDistrictRepo _repo;
        public StatisticalImportExportDistrictController(SoHoa_SoCongThuongContext context)
        {
            _repo = new StatisticalImportExportDistrictRepo(context);
        }

        [Route("LoadData")]
        [HttpPost]
        public IActionResult ReportAdministrativeProcedures([FromBody] QueryStatisticalImportExportDistrictBody query)
        {
            BaseModels<ReturnStatisticalImportExportDistrictData> model = new BaseModels<ReturnStatisticalImportExportDistrictData>();
            try
            {
                //Get query body data:
                var DistrictId = Guid.Parse(query.Filter["DistrictId"]);

                DateTime? MinDate = null;
                if (query.Filter != null && query.Filter.ContainsKey("MinDate"))
                {
                    MinDate = DateTime.ParseExact(query.Filter["MinDate"], "dd/MM/yyyy", null).Date;
                }

                DateTime? MaxDate = null;
                if (query.Filter != null && query.Filter.ContainsKey("MaxDate"))
                {
                    MaxDate = DateTime.ParseExact(query.Filter["MaxDate"], "dd/MM/yyyy", null).Date;
                }

                var Type = int.Parse(query.Filter["Type"]);

                //Get Data:
                var Result = _repo.FindData(Type, DistrictId, MinDate, MaxDate);

                //Return Data:
                model.status = 1;
                model.data = Result;

                return Ok(model);
            }
            catch (Exception ex)
            {
                //Catch Error:
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
