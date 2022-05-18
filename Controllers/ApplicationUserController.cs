using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using CRUD.Data;
using CRUD.Models;
using CRUD.Statics;
using CRUD.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRUD.Controllers
{
    [Route("api/user")]
    public class ApplicationUserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationDbContext _context;
        public ApplicationUserController(UserManager<ApplicationUser> userManager,
                                         RoleManager<ApplicationRole> roleManager,
                                         SignInManager<ApplicationUser> signInManager,
                                         ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            _context = context;
        }
        [HttpGet("getAll")]
        public async Task<List<UserViewModel>> GetAllUser()
        {
            List<UserViewModel> model = new List<UserViewModel>();
            model = userManager.Users.Where(x => (x.IsDelete == false || x.IsDelete == null)).Select(u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                DonViId = u.DonViId,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                IsDelete = u.IsDelete,
                IsActive = u.IsActive,
                AccountType = u.AccountType
            }).OrderBy(x => x.UserName).ToList();

            foreach (var m in model)
            {
                var user = userManager.Users.FirstOrDefault(u => u.Id == m.Id);
                IList<string> listRole = await userManager.GetRolesAsync(user);
                if (listRole != null && listRole.Count > 0)
                {
                    m.ApplicationRolesId = roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
                    m.RoleName = roleManager.Roles.Single(r => r.Name == listRole.Single()).Name;
                }
            }
            return model;
        }

        [HttpPost("create/{id}")]
        public async Task<HttpResponseMessage> CreateOrEditUser(string id, [FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if(id == "-1")
            {
                ApplicationUser user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.UserName,
                    DonViId = model.DonViId,
                    PhoneNumber = model.PhoneNumber,
                    ChangePasswordDate = DateTime.Now.AddDays(90),
                    AccountType = model.AccountType,
                    IsActive = model.IsActive
                };

                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRolesId);

                    if(applicationRole != null)
                    {
                        IdentityResult roleResult = await userManager.AddToRoleAsync(user, applicationRole.Name.ToString());

                        if (roleResult.Succeeded)
                        {
                            return new HttpResponseMessage(HttpStatusCode.OK);
                        }
                    }

                    List<SelectListItem> userClaims = model.UserClaims.Where(c => c.Selected).ToList();

                    foreach(var claim in userClaims)
                    {
                        await userManager.AddClaimAsync(user, new Claim(claim.Value, claim.Value));
                    }
                }
            }
            else
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);

                if(user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.DonViId = model.DonViId;
                    user.PhoneNumber = model.PhoneNumber;
                    user.ChangePasswordDate = DateTime.Now;
                    user.IsActive = model.IsActive;
                    user.AccountType = model.AccountType;

                    string existingRole = "";
                    string existingRoleId = ""; 

                    IList<string> listRole = await userManager.GetRolesAsync(user);

                    if(listRole != null && listRole.Count > 0)
                    {
                        existingRole = listRole.Single();
                        existingRoleId = roleManager.Roles.Single(r => r.Name == existingRole).Id;
                    }

                    IdentityResult result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        if(existingRoleId != model.ApplicationRolesId)
                        {
                            if(existingRole != "")
                            {
                                IdentityResult roleResult = await userManager.RemoveFromRoleAsync(user, existingRole);

                                if (roleResult.Succeeded)
                                {
                                    ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRolesId);

                                    if(applicationRole != null)
                                    {
                                        IdentityResult newRoleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);

                                        if (newRoleResult.Succeeded)
                                        {
                                            return new HttpResponseMessage(HttpStatusCode.OK);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ApplicationRole applicationRole = await roleManager.FindByIdAsync(model.ApplicationRolesId);

                                if(applicationRole != null)
                                {
                                    IdentityResult newRoleResult = await userManager.AddToRoleAsync(user, applicationRole.Name);

                                    if (newRoleResult.Succeeded)
                                    {
                                        return new HttpResponseMessage(HttpStatusCode.OK);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpDelete("delete/{id}")]
        public async Task<HttpResponseMessage> DeleteUser(string id)
        {
            if(id != null)
            {
                ApplicationUser user = await userManager.FindByIdAsync(id);

                if(user != null)
                {
                    user.IsDelete = true;
                    user.IsActive = false;
                }

                IdentityResult result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet("checkpassword")]
        public async Task<bool> CheckPassword(string Id, string Password)
        {
            var user = await userManager.FindByIdAsync(Id);

            var result = await userManager.CheckPasswordAsync(user, Password);

            return result;
        }

        [HttpGet("changepassword")]
        public async Task<HttpResponseMessage> ChangePassword(string id, string PasswordNew)
        {
            if(PasswordNew == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            else
            {
                var user = await userManager.FindByIdAsync(id);

                IdentityResult result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    IdentityResult result2 = await userManager.AddPasswordAsync(user, PasswordNew);

                    var userFirst = _context.ApplicationUSer.FirstOrDefault(x => x.Id == id);

                    userFirst.ChangePasswordDate = DateTime.Now;

                    _context.SaveChanges();

                    if(result2.Succeeded)
                    {
                        return new HttpResponseMessage(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}
