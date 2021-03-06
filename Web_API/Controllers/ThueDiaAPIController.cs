﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_API.Models;

namespace Web_API.Controllers
{
    public class ThueDiaAPIController : ApiController
    {
        VideoRentalDb db;
        PhiTreAPIController phiTreController;
        DatHangController datHangController;
        int soNgayChoThueMoiDia;

        public ThueDiaAPIController()
        {
            db = new VideoRentalDb();
            phiTreController = new PhiTreAPIController();
            datHangController = new DatHangController();
        }

        //cho thue
        [Route("api/muondia/{maKhachHang}/{maDia}")]
        public IHttpActionResult PostThueDia(int maKhachHang, int maDia)
        {
            string err = null;
            var dia = db.Dias.Find(maDia);
            var tieuDe = db.TieuDes.Find(dia.MaTieuDe);
            var soNgayChoThueMoiDia = db.DanhMucs.Find(tieuDe.MaDanhMuc).ThoiGianThue;
            var kh = db.KhachHangs.Where(x => x.MaKhachHang == maKhachHang).FirstOrDefault();
            if (dia == null || kh == null)
            {
                err = "Lỗi";
                return Json(err);
            }
            var model = new DsChoThue
            {
                MaKhachHang = maKhachHang,
                MaDia = maDia,
                NgayThue = DateTime.Now,
                NgayPhaiTra = DateTime.Now.AddDays(soNgayChoThueMoiDia),
                NgayThucTra = new DateTime(1753, 1, 1),
                Dia = dia,
                KhachHang = kh
            };
            db.DsChoThue.Add(model);
            dia.TinhTrangThue = Models.Enums.TinhTrangThueCollection.DangThue;
            db.Entry(dia).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(model);
        }
        //tra dia
        [Route("api/muondia/{maDia}")]
        public IHttpActionResult PutThueDia(int maDia)
        {
            if (maDia < 0) return NotFound();
            var model = db.DsChoThue.Where(x => x.MaDia == maDia).OrderByDescending(x => x.NgayThue).FirstOrDefault();
            model.NgayThucTra = DateTime.Now;
            phiTreController.ThemPhiTre(model);
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // datHangController.KiemTraTinhTrangDatHang(model.MaDia);//nếu đĩa đang được đặt hàng => thay đổi trạng thái đĩa
            datHangController.KiemTraTinhTrangDatHang(maDia);
            return Ok();
        }

        [Route("api/thue/{maKhachHang}/{limit}/{offset}")]
        public IHttpActionResult GetLimit(int maKhachHang, int limit, int offset)
        {
            string err = null;
            var kh = db.KhachHangs.Find(maKhachHang);
            if (kh == null)
            {
                err = "Không tìm thấy khách hàng cần tìm";
                return Json(err);
            }
            if (limit < 0 || offset < 0)
            {
                err = "Lỗi";
                return Json(err);
            }
            DateTime defaultDate = DateTime.Parse("1753-01-01 00:00:00.000");
            var result = db.DsChoThue.Where(x => x.MaKhachHang == kh.MaKhachHang && x.NgayThucTra.CompareTo(defaultDate) == 0)
                .ToList().Skip(offset).Take(limit).OrderByDescending(x=>x.NgayThue).ToList();
            return Json(result);
        }

        [Route("api/thue/{maKhachHang}/count")]
        public IHttpActionResult GetCountByMaKh(int maKhachHang)
        {
            string err = null;
            var kh = db.KhachHangs.Find(maKhachHang);
            if (kh == null)
            {
                err = "Không tìm thấy khách hàng cần tìm";
                return Json(err);
            }
            DateTime defaultDate = DateTime.Parse("1753-01-01 00:00:00.000");
            var result = db.DsChoThue.Where(x => x.MaKhachHang == kh.MaKhachHang && x.NgayThucTra.CompareTo(defaultDate) == 0).Count();
            return Json(result);
        }



    }
}
