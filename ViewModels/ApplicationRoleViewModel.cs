using CRUD.Models;
using System.Collections.Generic;

namespace CRUD.ViewModels
{
    public class ApplicationRoleViewModel
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int NumberOfUsers { get; set; }
        public string ChucNangDefault { get; set; }
        public List<int> DsChucNang { get; set; }
        public List<ApplicationUser> DsDonVi { get; set; }
    }
}
