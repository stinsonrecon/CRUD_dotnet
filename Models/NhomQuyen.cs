using System;
using System.ComponentModel.DataAnnotations;

namespace CRUD.Models
{
    public class NhomQuyen
    {
        [Key]
        public int Id { get; set; }
        public string TenNhomQuyen { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int NguoiTaoId { get; set; }
    }
}
