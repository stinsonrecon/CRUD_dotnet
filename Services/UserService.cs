using AutoMapper;
using CRUD.Models;
using CRUD.Repositories;
using CRUD.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRUD.Services
{
    public interface IUserService
    {
        Task<Response> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<Response> RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        IEnumerable<ApplicationUser> GetAll();
        ApplicationUser GetById(int id);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        public UserService(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<Response> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var resp = await _repo.Authenticate(model, ipAddress);
            return resp;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _repo.GetAll();
        }

        public ApplicationUser GetById(int id)
        {
            return _repo.GetById(id);
        }

        public async Task<Response> RefreshToken(string token, string ipAddress)
        {
            var resp = await _repo.RefreshToken(token, ipAddress);
            return resp;
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            return _repo.RevokeToken(token, ipAddress);
        }
    }
}
