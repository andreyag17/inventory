using appInventory.Models;
using appInventory.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace appInventory.Controllers
{
    public class movimientoController : Controller
    {
        private inventoryEntities db = new inventoryEntities();

        // GET: movimiento
        public ActionResult Index()
        {
            

            if (Session["usuario"] != null)
            {
                var movimiento = db.movimiento.Include(m => m.producto).Include(m => m.usuario);
                return View(movimiento.ToList());

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        //GET: buscador
        [HttpPost]
        public ActionResult BuscadorMovimiento(string nombreProducto, string tipoMovimiento, string Registradox)
        {
            var movimientos = db.movimiento.Include(p => p.usuario).AsQueryable();

            if (!string.IsNullOrEmpty(nombreProducto))
            {
                movimientos = movimientos.Where(p => p.producto.nombreProducto.Contains(nombreProducto));
            }

            if (!string.IsNullOrEmpty(tipoMovimiento.ToString()))
            {
                movimientos = movimientos.Where(p => p.tipoMovimiento.ToString().Contains(tipoMovimiento));
            }

            if (!string.IsNullOrEmpty(Registradox))
            {
                movimientos = movimientos.Where(p => p.usuario.nombre.Contains(Registradox));
            }
            //return View(movimientos.ToList());
            return View("Index", movimientos.ToList());
        }

        // GET: movimiento/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var movimiento = db.movimiento.Where(m => m.codigoProducto.ToLower() == id.ToLower()).ToList();
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // GET: movimiento/Create
        public ActionResult Create()
        {
            if (Session["usuario"] != null)
            {
                ViewBag.codigoProducto = new SelectList(db.producto, "codigoProducto", "nombreProducto");
                var modelo = new movimiento
                {
                    fechaIngreso = DateTime.Now
                };

                return View(modelo);

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        

        // GET: movimiento/Salida
        public ActionResult Salida()
        {
            if (Session["usuario"] != null)
            {
                ViewBag.codigoProducto = new SelectList(db.producto, "codigoProducto", "nombreProducto");
                var modelo = new movimiento
                {
                    fechaSalida = DateTime.Now
                };
                return View(modelo);

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // POST: movimiento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idCodigo,codigoProducto,fechaVencimiento,fechaIngreso,fechaSalida,creacionRegistro,cantidad,tipoMovimiento,usuarioId")] movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                movimiento.creacionRegistro = DateTime.Now;
                movimiento.tipoMovimiento = 1;
                usuario user = (usuario)Session["usuario"];
                movimiento.usuarioId = user.usuarioId;
                productoController prod = new productoController();
                prod.Actualizar(movimiento.codigoProducto, Convert.ToInt32(movimiento.cantidad));

                db.movimiento.Add(movimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.codigoProducto = new SelectList(db.producto, "codigoProducto", "nombreProducto", movimiento.codigoProducto);
            ViewBag.usuarioId = new SelectList(db.usuario, "usuarioId", "nombre", movimiento.usuarioId);
            return View(movimiento);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Salida ([Bind(Include = "idCodigo,codigoProducto,fechaVencimiento,fechaIngreso,fechaSalida,creacionRegistro,cantidad,tipoMovimiento,usuarioId")] movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                movimiento.creacionRegistro = DateTime.Now;
                movimiento.tipoMovimiento = 2;
                usuario user = (usuario)Session["usuario"];
                movimiento.usuarioId = user.usuarioId;
                productoController prod = new productoController();
                prod.Actualizar(movimiento.codigoProducto, Convert.ToInt32(-movimiento.cantidad));

                db.movimiento.Add(movimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.codigoProducto = new SelectList(db.producto, "codigoProducto", "nombreProducto", movimiento.codigoProducto);
            ViewBag.usuarioId = new SelectList(db.usuario, "usuarioId", "nombre", movimiento.usuarioId);
            return View(movimiento);
        }

        // GET: movimiento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movimiento movimiento = db.movimiento.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            ViewBag.codigoProducto = new SelectList(db.producto, "codigoProducto", "nombreProducto", movimiento.codigoProducto);
            ViewBag.usuarioId = new SelectList(db.usuario, "usuarioId", "nombre", movimiento.usuarioId);
            return View(movimiento);
        }

        // POST: movimiento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idCodigo,codigoProducto,fechaVencimiento,fechaIngreso,fechaSalida,creacionRegistro,cantidad,tipoMovimiento,usuarioId")] movimiento movimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.codigoProducto = new SelectList(db.producto, "codigoProducto", "nombreProducto", movimiento.codigoProducto);
            ViewBag.usuarioId = new SelectList(db.usuario, "usuarioId", "nombre", movimiento.usuarioId);
            return View(movimiento);
        }

        // GET: movimiento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            movimiento movimiento = db.movimiento.Find(id);
            if (movimiento == null)
            {
                return HttpNotFound();
            }
            return View(movimiento);
        }

        // POST: movimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            movimiento movimiento = db.movimiento.Find(id);
            db.movimiento.Remove(movimiento);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
