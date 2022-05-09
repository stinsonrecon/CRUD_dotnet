using CRUD.Models;
using CRUD.ViewModels;
using CRUD.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace CRUD.Repositories
{
    public interface IUserRepository
    {
        Task<Response> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<Response> RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        IEnumerable<ApplicationUser> GetAll();
        ApplicationUser GetById(int id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(SignInManager<ApplicationUser> signInManager,
                              UserManager<ApplicationUser> userManager,
                              RoleManager<ApplicationRole> roleManager,
                              ApplicationDbContext context,
                              IConfiguration configuration,
                              ILogger<UserRepository> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<Response> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (result.Succeeded)
            {
                var checkActiveUser = _userManager.Users.FirstOrDefault(x => x.UserName == model.Username);

                if(checkActiveUser != null && checkActiveUser.IsActive == true)
                {
                    var appUser = _userManager.Users.Where(x => x.UserName == model.Username).Select(u => new ApplicationUser
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

                    var user = _userManager.Users.FirstOrDefault(x => x.Id == appUser.Id);

                    IList<string> listRole = await _userManager.GetRolesAsync(user);

                    if(listRole != null && listRole.Count > 0)
                    {
                        appUser.NhomQuyenId = _roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
                    }

                    var chucNangDefault = _roleManager.Roles.Single(r => r.Name == listRole.Single()).ChucNangDefault;

                    var listChucNangQuery = from cnn in _context.ChucNangNhom
                                            join cn in _context.ChucNang on cnn.ChucNangId equals cn.Id
                                            where cnn.NhomQuyenId == appUser.NhomQuyenId
                                            select cn.ClaimValue;

                    var listChucNang = listChucNangQuery.ToList();

                    string tokenString = await GenerateJwtToken(model.Username, appUser, appUser.NhomQuyenId, listChucNang);

                    var refreshToken = generateRefreshToken(ipAddress);

                    checkActiveUser.RefreshToken.Add(refreshToken);

                    // save refresh token
                    _context.Update(checkActiveUser);
                    _context.SaveChanges();

                    return new Response(
                        "DANG NHAP THANH CONG",
                        new AuthenticateResponse(appUser, tokenString, refreshToken.Token, listChucNang, chucNangDefault),
                        "00",
                        true
                    );
                }
                return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);
            }
            return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _userManager.Users;
        }

        public ApplicationUser GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public async Task<Response> RefreshToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(x => x.RefreshToken.Any(t => t.Token == token));

            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = user.RefreshToken.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            user.RefreshToken.Add(newRefreshToken);

            _context.Update(user);
            _context.SaveChanges();

            var appUser = _userManager.Users.Where(x => x.UserName == user.UserName).Select(u => new ApplicationUser
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

            IList<string> listRole = await _userManager.GetRolesAsync(appUser);

            if (listRole != null && listRole.Count > 0)
            {
                appUser.NhomQuyenId = _roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
            }

            var chucNangDefault = _roleManager.Roles.Single(r => r.Name == listRole.Single()).ChucNangDefault;

            var listChucNangQuery = from cnn in _context.ChucNangNhom
                                    join cn in _context.ChucNang on cnn.ChucNangId equals cn.Id
                                    where cnn.NhomQuyenId == appUser.NhomQuyenId
                                    select cn.ClaimValue;

            var listChucNang = listChucNangQuery.ToList();

            string tokenString = await GenerateJwtToken(user.UserName, appUser, appUser.NhomQuyenId, listChucNang);

            return new Response(
                "Lay token thanh cong",
                new AuthenticateResponse(user, tokenString, newRefreshToken.Token, listChucNang, chucNangDefault),
                "00",
                true
            );
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshToken.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshToken.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            return true;
        }

        private async Task<string> GenerateJwtToken(string username, 
                                                    ApplicationUser user,
                                                    string permission,
                                                    List<string> listChucNang)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
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

        // helper methods
        private string generateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(this._configuration["JwtKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),

                Expires = DateTime.UtcNow.AddMinutes(15),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];

                rngCryptoServiceProvider.GetBytes(randomBytes);

                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"])).AddMinutes(5),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}
