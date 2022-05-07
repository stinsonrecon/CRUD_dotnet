using CRUD.ViewModels;
using System.Collections.Generic;

namespace CRUD.Services
{
    public interface IChucNangService
    {
        List<ChucNangViewModel> GetAllChucNang(string NhomQuyenId);
        ResponseWithPaginationViewModel GetChucNangWithPaginate(ChucNangSearchViewModel request);
        ResponsePostViewModel CreateChucNang(ChucNangGetViewModel request);
        ResponsePostViewModel UpdateChucNang(ChucNangGetViewModel request);
        ResponsePostViewModel DeleteChucNang(int id);
    }
}
