using CRUD.Models;
using CRUD.Services;
using CRUD.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CRUD.Controllers
{
    [Route("api/donvi")]
    [Authorize]
    public class DonViController : Controller
    {
        private readonly IDonViService _service;
        public DonViController(IDonViService service)
        {
            _service = service;
        }
        [HttpGet]
        public ActionResult<List<DonViViewModel>> GetDonVi()
        {
            var resp = _service.GetDonVi();
            return Ok(resp);
        }
        [HttpGet("GetAll")]
        public ActionResult<ResponseWithPaginationViewModel> GetDonViWithPagination([FromQuery] ParamsGetDonViViewModel request)
        {
            var resp = _service.GetDonViWithPagination(request);
            return Ok(resp);
        }
        [HttpPost("create")]
        public ActionResult<ResponsePostViewModel> CreateDonVi([FromBody] DonVi request)
        {
            var resp = _service.CreateDonVi(request);
            return Ok(resp);
        }
        [HttpPut("update")]
        public ActionResult<ResponsePostViewModel> UpdateDonVi([FromBody] DonVi request)
        {
            var resp = _service.UpdateDonVi(request);
            return Ok(resp);
        }
        [HttpDelete("delete/{id}")]
        public ActionResult<ResponsePostViewModel> DeleteDonVi(int id)
        {
            var resp = _service.DeleteDonVi(id);
            return Ok(resp);
        }
        [HttpGet("GetById/{id}")]
        public ActionResult<DonVi> GetDonViById(int id)
        {
            var resp = _service.GetDonViById(id);
            return Ok(resp);
        }
        [HttpGet("GetBoPhanById/{id}")]
        public ActionResult<List<DonViViewModel>> GetBoPhanByIdDonVi(int id)
        {
            var resp = _service.GetBoPhanByIdDonVi(id);
            return resp;
        }
    }
}
