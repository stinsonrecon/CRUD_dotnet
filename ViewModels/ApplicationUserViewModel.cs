﻿using System;

namespace CRUD.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? DonViId { get; set; }
        public string NhomQuyenId { get; set; }
        public DateTime? ChangePassWordDate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
