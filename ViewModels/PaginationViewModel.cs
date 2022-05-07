using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRUD.ViewModels
{
    public class PaginationViewModel
    {
        [Required]
        public int PageSize { get; set; }
        [Required]
        public int PageIndex { get; set; }
    }
}
