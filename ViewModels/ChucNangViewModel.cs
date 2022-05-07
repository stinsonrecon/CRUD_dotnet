using System.Collections.Generic;

namespace CRUD.ViewModels
{
    public class ChucNangViewModel
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public int ChucNangChaId { get; set; }
        public string MoTa { get; set; }
        public int TrangThai { get; set; }
        public string LinkUrl { get; set; }
        public string Icon { get; set; }
        public string NhomQuyenId { get; set; }
        public int? Order { get; set; }
        public int? Type { get; set; }
        public string ClaimValue { get; set; }
    }
    public class ChucNangGetViewModel
    {
        public int Id { get; set; }
        public string TieuDe { get; set; }

        public string ClaimValue { get; set; }
        public int ChucNangChaId { get; set; }
        public string ChucNangChaTieuDe { get; set; }
        public string MoTa { get; set; }
        public int TrangThai { get; set; }
        public string LinkUrl { get; set; }
        public string Icon { get; set; }
        public int? Order { get; set; }
        public int? Type { get; set; }
    }
    public class ResponseChucNangViewModel : ResponseWithPaginationViewModel
    {
        public List<ChucNangGetViewModel> Data { get; set; }
        public ResponseChucNangViewModel(List<ChucNangGetViewModel> data, int statusCode, int totalRecord) : base(statusCode, totalRecord)
        {
            Data = data;
        }
    }
    public class ChucNangSearchViewModel : PaginationViewModel
    {
        public string TenCN { get; set; }
        public int? TrangThaiCN { get; set; }
        public int? Type { get; set; }
    }
}
