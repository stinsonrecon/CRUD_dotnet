using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CRUD.Data;
using CRUD.Models;
using CRUD.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CRUD.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        public AccountController(SignInManager<ApplicationUser> signInManager,
                                UserManager<ApplicationUser> userManager,
                                RoleManager<ApplicationRole> roleManager,
                                ApplicationDbContext context,
                                IConfiguration configuration,
                                ILogger<HomeController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration; 
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("LoginApi")]
        [AllowAnonymous]
        public async Task<object> LoginApi([FromBody] LoginViewModel model)
        {
            _logger.LogInformation("Username login: " + model.Email);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            var checkActiveUser = _userManager.Users.FirstOrDefault(x => x.UserName == model.Email);

            if (result.Succeeded)
            {
                if (checkActiveUser != null && checkActiveUser.IsActive == true)
                {
                    var appUser = _userManager.Users.Where(x => x.UserName == model.Email).Select(u => new ApplicationUser
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        DonViId = u.DonViId,
                        UserName = u.UserName,
                        PhoneNumber = u.PhoneNumber,
                        ChangePasswordDate = u.ChangePasswordDate
                    }).FirstOrDefault();

                    var user = _userManager.Users.FirstOrDefault(u => u.Id == appUser.Id);

                    IList<string> listRole = await _userManager.GetRolesAsync(user);

                    if(listRole != null && listRole.Count > 0)
                    {
                        appUser.NhomQuyenId = _roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
                    }

                    var chucNangDefault = _roleManager.Roles.Single(r => r.Name == listRole.Single()).ChucNangDefault;

                    var listChucNangQuery = from cnn in _context.ChucNangNhom
                                            join cn in _context.ChucNang on cnn.ChucNangId equals cn.Id
                                            where cnn.NhomQuyenId == appUser.NhomQuyenId
                                            where cn.Type == 2
                                            select cn.ClaimValue;

                    var listChucNang = listChucNangQuery.ToList();

                    string tokenString = await GenerateJwtToken(model.Email, appUser, appUser.NhomQuyenId, listChucNang);

                    return new LoginResultViewModel(appUser, tokenString, listChucNang, chucNangDefault);
                }
                return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);
            }
            else
            {
                var user = _userManager.Users.Where(x => x.UserName == model.Email).FirstOrDefault();

                if(user != null)
                {
                    user.AccessFailedCount = user.AccessFailedCount + 1;

                    await _userManager.UpdateAsync(user);
                }

                LoginResultViewModel loginResult = new LoginResultViewModel();

                loginResult.Token = null;
                loginResult.Username = model.Email;
                loginResult.AccessFailedCount = user.AccessFailedCount;

                return loginResult;
            }

            throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
        }

        private async Task<string> GenerateJwtToken(string userName,
                                                    ApplicationUser user,
                                                    string permissions,
                                                    List<string> listChucNang)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            if(listChucNang.Count() > 0)
            {
                foreach(var chucNang in listChucNang)
                {
                    if(chucNang != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, chucNang));
                    }
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];

                rngCryptoServiceProvider.GetBytes(randomBytes);

                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}
