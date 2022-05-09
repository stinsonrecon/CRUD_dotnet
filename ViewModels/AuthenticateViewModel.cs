using CRUD.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CRUD.ViewModels
{
    public class RevokeTokenRequest
    {
        public string Token { get; set; }
    }

    public class AuthenticateRequest
    {
        public string UserType { get; set; }
        public string DeviceId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }

        public int? DonViId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string NhomQuyenId { get; set; }
        public string chucNangDefault { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? ChangePassWordDate { get; set; }
        public List<string> ListChucNang { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(ApplicationUser user, string jwtToken, string refreshToken, List<string> listChucNang, string chucNangDefaultParam)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Email;
            RefreshToken = refreshToken;
            Email = user.Email;
            Username = user.UserName;
            Token = jwtToken;
            DonViId = user.DonViId;
            NhomQuyenId = user.NhomQuyenId;
            AccessFailedCount = user.AccessFailedCount;
            ChangePassWordDate = user.ChangePasswordDate;
            ListChucNang = listChucNang;
            chucNangDefault = chucNangDefaultParam;
        }
        public AuthenticateResponse(ApplicationUser user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Email;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
            Email = user.Email;
            Username = user.UserName;
            Token = jwtToken;
            DonViId = user.DonViId;
            NhomQuyenId = user.NhomQuyenId;
            AccessFailedCount = user.AccessFailedCount;
            ChangePassWordDate = user.ChangePasswordDate;
        }
    }
}
