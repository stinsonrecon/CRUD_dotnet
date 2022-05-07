using System;
using System.Collections.Generic;
using System.Text;
using CRUD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRUD.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<ApplicationUser> ApplicationUSer { get; set; }
        public DbSet<ChucNang> ChucNang { get; set; }
        public DbSet<ChucNangNhom> ChucNangNhom { get; set; }
        public DbSet<Quyen> Quyen { get; set; }
        public DbSet<NhomQuyen> NhomQuyen { get; set; }
        public DbSet<VaiTro> VaiTro { get; set; }
        public DbSet<DonVi> DonVi { get; set; }    

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
