using CRUD.ViewModels;
using CRUD.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using CRUD.Data;
using System;

namespace CRUD.Repositories
{
    public class DonViRepository : IDonViRepository
    {
        private readonly ILogger<DonViRepository> _logger;
        private readonly ApplicationDbContext _context;
        public DonViRepository(ILogger<DonViRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public ResponsePostViewModel CreateDonVi(DonVi request)
        {
            try
            {
                var donViItem = new DonVi
                {
                    TenDonVi = request.TenDonVi,
                    MaDonVi = request.MaDonVi,
                    DiaChi = request.DiaChi,
                    SoDienThoai = request.SoDienThoai,
                    Email = request.Email,
                    MoTa = request.MoTa,
                    DonViChaId = request.DonViChaId,
                    NguoiTaoId = request.NguoiTaoId,
                    ThoiGianTao = DateTime.Now,
                    LaPhongBan = request.LaPhongBan,
                    TrangThai = request.TrangThai,
                };

                _context.DonVi.Add(donViItem);
                _context.SaveChanges();
                return new ResponsePostViewModel("Add successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        public ResponsePostViewModel DeleteDonVi(int id)
        {
            try
            {
                var donViItem = _context.DonVi.FirstOrDefault(d => d.Id == id);
                if(donViItem == null)
                {
                    return new ResponsePostViewModel("Not found", 404);
                }

                _context.DonVi.Remove(donViItem);
                _context.SaveChanges();
                return new ResponsePostViewModel("Delete successfully", 200);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }

        public List<DonViViewModel> GetBoPhanByIdDonVi(int id)
        {
            try
            {
                var donVi = from x in _context.DonVi
                            where x.TrangThai == 1
                            where x.LaPhongBan == 1 && x.DonViChaId == id
                            select new DonViViewModel
                            {
                                Id = x.Id,
                                TenDonVi = x.TenDonVi,
                                MaDonVi = x.MaDonVi,
                                DiaChi = x.DiaChi,
                                SoDienThoai = x.SoDienThoai,
                                Email = x.Email,
                                MoTa = x.MoTa,
                                DonViChaId = x.DonViChaId,
                                NguoiTaoId = x.NguoiTaoId,
                                ThoiGianTao = x.ThoiGianTao,
                                TrangThai = x.TrangThai
                            };
                var response = donVi.ToList();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return null;
            }
        }

        public List<DonViViewModel> GetDonVi()
        {
            try
            {
                var donVi = from x in _context.DonVi
                            where x.TrangThai == 1
                            where x.LaPhongBan == 1
                            select new DonViViewModel
                            {
                                Id = x.Id,
                                TenDonVi = x.TenDonVi,
                                MaDonVi = x.MaDonVi,
                                DiaChi = x.DiaChi,
                                SoDienThoai = x.SoDienThoai,
                                Email = x.Email,
                                MoTa = x.MoTa,
                                DonViChaId = x.DonViChaId,
                                NguoiTaoId = x.NguoiTaoId,
                                ThoiGianTao = x.ThoiGianTao,
                                TrangThai = x.TrangThai
                            };
                var response = donVi.ToList();
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return null;
            }
        }

        public DonVi GetDonViById(int id)
        {
            return _context.DonVi.FirstOrDefault(x => x.Id == id);
        }

        public ResponseWithPaginationViewModel GetDonViWithPagination(ParamsGetDonViViewModel request)
        {
            try
            {
                var donVi = from x in _context.DonVi
                                select new DonViGetViewModel
                                {
                                    Id = x.Id,
                                    TenDonVi = x.TenDonVi,
                                    MaDonVi = x.MaDonVi,
                                    DiaChi = x.DiaChi,
                                    SoDienThoai = x.SoDienThoai,
                                    Email = x.Email,
                                    MoTa = x.MoTa,
                                    DonViChaId = x.DonViChaId,
                                    NguoiTaoId = x.NguoiTaoId,
                                    ThoiGianTao = x.ThoiGianTao,
                                    TrangThai = x.TrangThai
                                };
                if (!String.IsNullOrEmpty(request.TenDonVi))
                {
                    donVi = donVi.Where(s => s.TenDonVi.Contains(request.TenDonVi));
                }
                if (request.TrangThai == 1)
                {
                    donVi = donVi.Where(s => s.TrangThai == request.TrangThai);
                }
                if (request.TrangThai == 0)
                {
                    donVi = donVi.Where(s => s.TrangThai == request.TrangThai || s.TrangThai == null);
                }
                donVi = donVi.OrderBy(s => s.TenDonVi);
                var totalRecord = donVi.ToList().Count();
                if (request.PageIndex > 0)
                {
                    donVi = donVi.Skip(request.PageSize * (request.PageIndex - 1)).Take(request.PageSize);
                }
                var response = donVi.ToList();
                return new ResponseDonViViewModel(response, 200, totalRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return null;
            }
        }

        public ResponsePostViewModel UpdateDonVi(DonVi request)
        {
            try
            {
                var donViItem = _context.DonVi.FirstOrDefault(x => x.Id == request.Id);
                if(donViItem == null)
                {
                    return new ResponsePostViewModel("Not found", 404);
                }

                donViItem.TenDonVi = request.TenDonVi;
                donViItem.MaDonVi = request.MaDonVi;
                donViItem.DiaChi = request.DiaChi;
                donViItem.SoDienThoai = request.SoDienThoai;
                donViItem.Email = request.Email;
                donViItem.MoTa = request.MoTa;
                donViItem.DonViChaId = request.DonViChaId;
                donViItem.NguoiTaoId = request.NguoiTaoId;
                donViItem.LaPhongBan = request.LaPhongBan;
                donViItem.TrangThai = request.TrangThai;

                _context.SaveChanges();
                return new ResponsePostViewModel("Update successfully", 200);
            }
            catch(Exception ex)
            {
                _logger.LogError("Error: ", ex);
                return new ResponsePostViewModel(ex.ToString(), 500);
            }
        }
    }
}
