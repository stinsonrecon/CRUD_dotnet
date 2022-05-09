using AutoMapper;
using CRUD.Repositories;
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
    public class ChucNangService : IChucNangService
    {
        private readonly IMapper _mapper;
        private readonly IChucNangRepository _repo;
        public ChucNangService(IMapper mapper, IChucNangRepository repo)
        {
            _mapper = mapper;
            _repo = repo;
        }
        public ResponsePostViewModel CreateChucNang(ChucNangGetViewModel request)
        {
            return _repo.CreateChucNang(request);
        }

        public ResponsePostViewModel DeleteChucNang(int id)
        {
            return _repo.DeleteChucNang(id);
        }

        public List<ChucNangViewModel> GetAllChucNang(string NhomQuyenId)
        {
            return _repo.GetAllChucNang(NhomQuyenId);
        }

        public ResponseWithPaginationViewModel GetChucNangWithPaginate(ChucNangSearchViewModel request)
        {
            return _repo.GetChucNangWithPaginate(request);
        }

        public ResponsePostViewModel UpdateChucNang(ChucNangGetViewModel request)
        {
            return _repo.UpdateChucNang(request);
        }
    }
}
