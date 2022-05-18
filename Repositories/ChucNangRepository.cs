using CRUD.Data;
using CRUD.ViewModels;
using CRUD.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CRUD.Repositories
{
    public interface IChucNangRepository
    {
        List<ChucNangViewModel> GetAllChucNang(string NhomQuyenId);
        ResponseWithPaginationViewModel GetChucNangWithPaginate(ChucNangSearchViewModel request);
        ResponsePostViewModel CreateChucNang(ChucNangGetViewModel request);
        ResponsePostViewModel UpdateChucNang(ChucNangGetViewModel request);
        ResponsePostViewModel DeleteChucNang(int id);
    }
    public class ChucNangRepository : IChucNangRepository
    {
        private readonly ILogger<ChucNangRepository> _logger;
        private readonly ApplicationDbContext _context;
        public ChucNangRepository(ILogger<ChucNangRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public ResponsePostViewModel CreateChucNang(ChucNangGetViewModel request)
        {
            try
            {
                var chucNang = new ChucNang
                {
                    TieuDe = request.TieuDe,
                    ClaimValue = request.ClaimValue,
                    ChucNangChaId = request.ChucNangChaId,
                    MoTa = request.MoTa,
                    TrangThai = request.TrangThai,
                    LinkUrl = request.LinkUrl,
                    Icon = request.Icon,
                    Order = request.Order,
                    Type = request.Type
                };

                _context.ChucNang.Add(chucNang);
                _context.SaveChanges();

                return new ResponsePostViewModel("Create successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        public ResponsePostViewModel DeleteChucNang(int id)
        {
            try
            {
                var chucNangItem = _context.ChucNang.FirstOrDefault(x => x.Id == id);
                if (chucNangItem == null)
                {
                    return new ResponsePostViewModel("Not found", 404);
                }

                var chucNangChild = _context.ChucNang.FirstOrDefault(x => x.ChucNangChaId == chucNangItem.Id);
                if (chucNangChild != null)
                {
                    return new ResponsePostViewModel("Failed to delete", 500);
                }

                _context.ChucNang.Remove(chucNangItem);
                _context.SaveChanges();

                return new ResponsePostViewModel("Delete successfully", 200);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return new ResponsePostViewModel(ex.ToString(), 500);    
            }
        }

        public List<ChucNangViewModel> GetAllChucNang(string NhomQuyenId)
        {
            try
            {
                var chucNangList = from cn in _context.ChucNang
                                   join cnn in _context.ChucNangNhom on cn.Id equals cnn.ChucNangId
                                   join role in _context.ApplicationRole on cnn.NhomQuyenId equals role.Id
                                   where cn.TrangThai == 1
                                   orderby cn.Order
                                   select new ChucNangViewModel
                                   {
                                       Id = cn.Id,
                                       TieuDe = cn.TieuDe,
                                       ChucNangChaId = cn.ChucNangChaId,
                                       MoTa = cn.MoTa,
                                       TrangThai = cn.TrangThai,
                                       LinkUrl = cn.LinkUrl,
                                       Icon = cn.Icon,
                                       NhomQuyenId = role.Id,
                                       Type = cn.Type,
                                       ClaimValue = cn.ClaimValue
                                   };

                if(NhomQuyenId != null && NhomQuyenId != "-1")
                {
                    chucNangList = chucNangList.Where(x => x.NhomQuyenId == NhomQuyenId);
                }

                var chucNang = chucNangList.ToList();

                var chucNangMap = new List<ChucNangViewModel>();

                for(int i = 0; i < chucNang.Count(); i++)
                {
                    if(chucNang[i].ChucNangChaId == 0)
                    {
                        chucNangMap.Add(chucNang[i]);
                    }
                    for(int j = 0; j < chucNang.Count(); j++)
                    {
                        if(chucNang[j].ChucNangChaId == chucNang[i].Id)
                        {
                            chucNangMap.Add(chucNang[j]);
                        }
                    }
                }

                return chucNangMap;
            }
            catch(Exception ex)
            {
                _logger.LogError("Error: ", ex);    
                return null;
            }
        }

        public ResponseWithPaginationViewModel GetChucNangWithPaginate(ChucNangSearchViewModel request)
        {
            try
            {
                var chucNangList = from cn in _context.ChucNang
                                   select new ChucNangGetViewModel
                                   {
                                       Id = cn.Id,
                                       TieuDe = cn.TieuDe,
                                       ClaimValue = cn.ClaimValue,
                                       ChucNangChaId = cn.ChucNangChaId,
                                       ChucNangChaTieuDe = _context.ChucNang.FirstOrDefault(x => x.Id == cn.ChucNangChaId).TieuDe,
                                       MoTa = cn.MoTa,
                                       TrangThai = cn.TrangThai,
                                       LinkUrl = cn.LinkUrl,
                                       Icon = cn.Icon,
                                       Order = cn.Order,
                                       Type = cn.Type
                                   };

                if(request.Type != null)
                {
                    chucNangList = chucNangList.Where(x => x.Type == request.Type);
                }
                if(request.TrangThaiCN != null)
                {
                    chucNangList = chucNangList.Where(x => x.TrangThai == request.TrangThaiCN);
                }
                if (!String.IsNullOrEmpty(request.TenCN))
                {
                    chucNangList = chucNangList.Where(x => x.TieuDe.Contains(request.TenCN));
                }
                
                chucNangList = chucNangList.OrderBy(x => x.Order);

                var totalRecord = chucNangList.ToList().Count();

                if(request.PageIndex > 0)
                {
                    chucNangList = chucNangList.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }

                var resp = chucNangList.ToList();

                return new ResponseChucNangViewModel(resp, 200, totalRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return null;
            }
        }

        public ResponsePostViewModel UpdateChucNang(ChucNangGetViewModel request)
        {
            try
            {
                var chucNangItem = _context.ChucNang.FirstOrDefault(x => x.Id == request.Id);

                if(chucNangItem == null)
                {
                    return new ResponsePostViewModel("Not found", 404);
                }

                chucNangItem.TieuDe = request.TieuDe;
                chucNangItem.ClaimValue = request.ClaimValue;
                chucNangItem.ChucNangChaId = request.ChucNangChaId;
                chucNangItem.MoTa = request.MoTa;
                chucNangItem.TrangThai = request.TrangThai;
                chucNangItem.LinkUrl = request.LinkUrl;
                chucNangItem.Icon = request.Icon;
                chucNangItem.Order = request.Order;
                chucNangItem.Type = request.Type;

                _context.SaveChanges();

                return new ResponsePostViewModel("Update successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }
    }
}
