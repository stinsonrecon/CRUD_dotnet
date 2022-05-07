using System;

namespace CRUD.ViewModels
{
    public class QuyenViewModel
    {
        public int id { get; set; }
        public string TenQuyen { get; set; }
        public string MoTa { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int NguoiTaoId { get; set; }
    }
    public class QuyenCuaNhomViewModel
    {
        public int id { get; set; }
        public int NhomQuyenId { get; set; }
        public int QuyenId { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public int NguoiTaoId { get; set; }
    }
}
