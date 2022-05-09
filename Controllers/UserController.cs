using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CRUD.Models;
using CRUD.Services;
using CRUD.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD.Controllers
{
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public async Task<object> Authenticate([FromBody] AuthenticateRequest model)
        {
            var resp = await _userService.Authenticate(model, IpAddress());

            if(resp == null)
            {
                return Ok(new { message = "Username or password is incorrect" });
            }

            if(resp.Data != null)
            {
                AuthenticateResponse resAuth = (AuthenticateResponse)resp.Data;
                SetTokenCookie(resAuth.RefreshToken);
            }

            return Ok(resp);
        }

        [HttpPost("refresh-token")]
        public async Task<object> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var resp = await _userService.RefreshToken(refreshToken, IpAddress());

            if (resp == null)
                return Unauthorized(new { message = "Invalid token" });

            if (resp.Data != null)
            {
                AuthenticateResponse resAuth = (AuthenticateResponse)resp.Data;
                SetTokenCookie(resAuth.RefreshToken);
            }

            return Ok(resp);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var response = _userService.RevokeToken(token, IpAddress());

            if (!response)
            {
                return NotFound(new { message = "Token not found" });
            }

            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();

            return Ok(user.RefreshToken);
        }

        // helper methods
        private void SetTokenCookie(string token)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", token, cookieOption);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
                
        }
    }
}
