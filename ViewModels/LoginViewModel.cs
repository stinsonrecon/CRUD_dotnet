using CRUD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRUD.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]  
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string Token { get; set; }
    }

    public class LoginResultViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public int? DonViId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string NhomQuyenId { get; set; }
        public string ChucNangDefault { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? ChangePasswordDate { get; set; }
        public List<string> ListChucNang { get; set; }
        public string Message { get; set; }

        public LoginResultViewModel()
        { }
        public LoginResultViewModel(ApplicationUser user, string token, List<string> listChucNang, string chucNangDefaultParam, string message = "")
        {
            Id = user.Id;
            DonViId = user.DonViId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Username = user.UserName;
            Token = token;
            NhomQuyenId = user.NhomQuyenId;
            AccessFailedCount = user.AccessFailedCount;
            ChangePasswordDate = user.ChangePasswordDate;
            ListChucNang = listChucNang;
            ChucNangDefault = chucNangDefaultParam;
            Message = message;
        }
    }
}
