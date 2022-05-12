using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUD.Services;
using CRUD.ViewModels;
using CRUD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    [Route("api/chucnang")]
    [Authorize]
    public class ChucNangController : Controller
    {
        private readonly IChucNangService _service;
        public ChucNangController(IChucNangService service)
        {
            _service = service;
        }
        [HttpGet("{NhomQuyenId}")]
        public ActionResult<List<ChucNangViewModel>> GetAllChucNang(string NhomQuyenId)
        {
            var resp = _service.GetAllChucNang(NhomQuyenId);
            return Ok(resp);
        }

        [HttpGet("GetAll")]
        public ActionResult<ResponseWithPaginationViewModel> GetChucNangWithPaginate([FromQuery] ChucNangSearchViewModel request)
        {
            var resp = _service.GetChucNangWithPaginate(request);
            return Ok(resp);
        }

        [HttpPost("create")]
        public ActionResult<ResponsePostViewModel> CreateChucNang([FromBody] ChucNangGetViewModel request)
        {
            var resp = _service.CreateChucNang(request);
            return Ok(resp);
        }

        [HttpPut("update")]
        public ActionResult<ResponsePostViewModel> UpdateChucNang([FromBody] ChucNangGetViewModel chucNangGetViewModel)
        {
            var resp = _service.UpdateChucNang(chucNangGetViewModel);
            return Ok(resp);    
        }

        [HttpDelete("{id}")]
        public ActionResult<ResponsePostViewModel> DeleteChucNang(int id)
        {
            var resp = _service.DeleteChucNang(id);
            return Ok(resp);
        }
    }
}
