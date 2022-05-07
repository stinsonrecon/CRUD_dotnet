using CRUD.Data;
using CRUD.Models;
using CRUD.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUD.Controllers
{
    [Route("api/role")]
    public class ApplicationRoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;
        public ApplicationRoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            _context = context;
        }
        [HttpGet("GetAll")]
        public IActionResult GetRole()
        {
            List<ApplicationRoleViewModel> model = new List<ApplicationRoleViewModel>();
            model = roleManager.Roles.Select(r => new ApplicationRoleViewModel
            {
                RoleName = r.Name,
                Id = r.Id,
                Description = r.Description,
                ChucNangDefault = r.ChucNangDefault
            }).ToList();
            return Ok(model);
        }
        [HttpPost("{id}")]
        public async Task<ActionResult> CreateOrEditRole(string id, [FromBody] ApplicationRoleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                ApplicationRole applicationRole = (!String.IsNullOrEmpty(id) && id != "0") ?
                                                roleManager.Roles.FirstOrDefault(r => r.Id == id) :
                                                new ApplicationRole
                                                {
                                                    CreatedDate = DateTime.UtcNow
                                                };

                applicationRole.Name = model.RoleName;
                applicationRole.Description = model.Description;
                applicationRole.ChucNangDefault = model.ChucNangDefault;
                applicationRole.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                IdentityResult roleResult = !String.IsNullOrEmpty(id) && id != "0" ?
                                            await roleManager.UpdateAsync(applicationRole) :
                                            await roleManager.CreateAsync(applicationRole);

                _context.ChucNangNhom.RemoveRange(_context.ChucNangNhom.Where(x => x.NhomQuyenId == id));
                _context.SaveChanges();

                if (model.DsChucNang.Count > 0)
                {
                    foreach (var item in model.DsChucNang)
                    {
                        _context.ChucNangNhom.Add(
                            new ChucNangNhom
                            {
                                ChucNangId = item,
                                NhomQuyenId = !String.IsNullOrEmpty(id) && id != "0" ? id : applicationRole.Id
                            }
                        );
                        _context.SaveChanges();
                    }
                }

                if(applicationRole != null)
                {
                    var listUserRole = await userManager.GetUsersInRoleAsync(applicationRole.Name.ToString());

                    if(listUserRole.Count > 0)
                    {
                        foreach(var item in listUserRole)
                        {
                            IdentityResult roleResultRemove = await userManager.RemoveFromRoleAsync(item, applicationRole.Name.ToString());
                        }                    
                    }

                    foreach (var item in model.DsDonVi)
                    {
                        ApplicationUser user = await userManager.FindByIdAsync(item.Id);
                        if (user != null)
                        {
                            user.FirstName = item.FirstName;
                            user.LastName = item.LastName;
                            user.Email = item.Email;
                            user.UserName = item.UserName;
                            user.DonViId = item.DonViId;
                            user.PhoneNumber = item.PhoneNumber;
                            user.ChangePasswordDate = DateTime.Now;
                        }

                        IdentityResult result = await userManager.AddToRoleAsync(user, applicationRole.Name.ToString());
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
