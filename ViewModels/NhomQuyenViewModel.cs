using System;

namespace CRUD.ViewModels
{
    public class NhomQuyenViewModel
    {
        public int id { get; set; }
        public string TenNhomQuyen { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int NguoiTaoId { get; set; }
    }
}
