﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Web_API.Models;
using Web_API.Models.Enums;

namespace Web_API.Controllers
{
    public class DiaAPIController : ApiController
    {
        VideoRentalDb db;
        public DiaAPIController()
        {
            db = new VideoRentalDb();
        }

        //get danh sach dia
        [Route("api/dia")]
        public IHttpActionResult GetAll()
        {
            var lst = db.Dias.OrderBy(x=>x.MaTieuDe).ToList();
            if (lst.Count == 0)
            {
                return NotFound();
            }

            List<Dia> result = new List<Dia>();

            lst.ForEach(item =>
            {
                Dia dia = new Dia();
                dia.MaDia = item.MaDia;
                dia.MaTieuDe = item.MaTieuDe;
                dia.TinhTrangThue = item.TinhTrangThue;
                result.Add(dia);
            });

            return Json(result);
        }

        [Route("api/dia/{maDia}")]
        public IHttpActionResult GetDia(int maDia)
        {
            Dia dia =  db.Dias.Find(maDia);
            if (dia == null)
            {
                return NotFound();
            }

            return Json(dia);
        }

        //them dia
        [Route("api/dia/{maTieuDe}/{soLuong}")]
        [HttpPost]
        public IHttpActionResult Post(int maTieuDe, int soLuong)
        {
            if (maTieuDe < 0 || soLuong < 0) return NotFound();
            var tieuDe = db.TieuDes.Find(maTieuDe);
            /*
            var list = new List<Dia>();
            for (int i = 0; i < soLuong; i++)
            {
                var model = new Dia
                {
                    MaTieuDe = tieuDe.MaTieuDe,
                    TinhTrangThue = TinhTrangThueCollection.CoSan
                };
                list.Add(model);
            }
            */
            var model = new Dia
            {
                MaTieuDe = tieuDe.MaTieuDe,
                TinhTrangThue = TinhTrangThueCollection.CoSan
            };

            var result = db.Dias.Add(model);
            db.SaveChanges();
            return Json(result);
        }

        //xoa dia
        [Route("api/dia/{maDia}")]
        [HttpDelete]
        public IHttpActionResult Delete(int maDia)
        {
            string err = null;
            try
            {
                db.Dias.Remove(db.Dias.Find(maDia));
                db.SaveChanges();
            }
            catch (Exception)
            {
                err = "Lỗi";
                return Json(err);
            }
            return Json(err);
        }

        //get so luong dia cua mot tieu de
        [Route("api/dia/tieude/{maTieuDe}/count")]
        public IHttpActionResult GetCountDiaByTieuDe(int maTieuDe)
        {
            if(maTieuDe < 0)
            {
                string err = "Không tìm thấy tiêu đề cần tìm";
                return Json(err);
            }
            var result = db.Dias.Where(x => x.MaTieuDe == maTieuDe).Count();
            return Json(result);
        }

        //get 
        [Route("api/dia/tieude/{maTieuDe}/{limit}/{offset}")]
        public IHttpActionResult GetDiaByTieuDes(int maTieuDe, int limit, int offset)
        {
            string err = null;
            var tieuDe = db.TieuDes.Find(maTieuDe);
            if (tieuDe == null)
            {
                err = "Không tìm thấy tiêu đề cần tìm";
                return Json(err);
            }
            if (limit < 0 || offset < 0)
            {
                err = "Lỗi";
                return Json(err);
            }
            var result = db.Dias.Where(x => x.MaTieuDe == maTieuDe).ToList().Skip(offset).Take(limit).OrderBy(x=>x.MaTieuDe).ToList();
            
            return Json(result);
        }
    }
}
