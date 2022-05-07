using Microsoft.AspNetCore.Identity;
using System;

namespace CRUD.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IpAddress { get; set; }
        public string ChucNangDefault { get; set; }
    }
}
