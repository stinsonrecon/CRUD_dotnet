using AutoMapper;
using CRUD.Models;
using CRUD.Repositories;
using CRUD.ViewModels;
using System.Collections.Generic;

namespace CRUD.Services
{
    public interface IDonViService
    {
        List<DonViViewModel> GetDonVi();
        ResponseWithPaginationViewModel GetDonViWithPagination(ParamsGetDonViViewModel request);
        ResponsePostViewModel CreateDonVi(DonVi request);
        ResponsePostViewModel UpdateDonVi(DonVi request);
        ResponsePostViewModel DeleteDonVi(int id);
        DonVi GetDonViById(int id);
        List<DonViViewModel> GetBoPhanByIdDonVi(int id);
    }
    public class DonViService : IDonViService
    {
        private readonly IDonViRepository _repo;
        private readonly IMapper _mapper;
        public DonViService(IDonViRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;   
        }
        public ResponsePostViewModel CreateDonVi(DonVi request)
        {
            return _repo.CreateDonVi(request);
        }

        public ResponsePostViewModel DeleteDonVi(int id)
        {
            return _repo.DeleteDonVi(id);
        }

        public List<DonViViewModel> GetBoPhanByIdDonVi(int id)
        {
            return _repo.GetBoPhanByIdDonVi(id);
        }

        public List<DonViViewModel> GetDonVi()
        {
            return _repo.GetDonVi();
        }

        public DonVi GetDonViById(int id)
        {
            return _repo.GetDonViById(id);
        }

        public ResponseWithPaginationViewModel GetDonViWithPagination(ParamsGetDonViViewModel request)
        {
            return _repo.GetDonViWithPagination(request);
        }

        public ResponsePostViewModel UpdateDonVi(DonVi request)
        {
            return _repo.UpdateDonVi(request);
        }
    }
}
