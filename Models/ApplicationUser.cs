using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CRUD.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? DonViId { get; set; }
        public string NhomQuyenId { get; set; }
        public DateTime? ChangePasswordDate { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsActive { get; set; }
        public int? AccountType { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshToken { get; set; }
    }
}
