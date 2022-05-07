using CRUD.Models;
using CRUD.ViewModels;
using System.Collections.Generic;

namespace CRUD.Repositories
{
    public interface IDonViRepository
    {
        List<DonViViewModel> GetDonVi();
        ResponseWithPaginationViewModel GetDonViWithPagination(ParamsGetDonViViewModel request);
        ResponsePostViewModel CreateDonVi(DonVi request);
        ResponsePostViewModel UpdateDonVi(DonVi request);
        ResponsePostViewModel DeleteDonVi(int id);
        DonVi GetDonViById(int id);
        List<DonViViewModel> GetBoPhanByIdDonVi(int id);
    }
}
