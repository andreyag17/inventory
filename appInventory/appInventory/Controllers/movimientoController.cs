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
        //Busca entre los movimientos siertos parametros
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
        //Muestra los detalles de un producto en especifico
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

        //Genera una entrada, suma de cantidad en productos y además muestra una fecha para que los usuarios administradores observen cuantas entradas se realizan
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

        //Genera una salida, resta de cantidad en productos y además muestra una fecha para que los usuarios administradores observen cuantas salidas se realizan
        // POST: movimiento/Create
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

        // POST: movimiento/Salida
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
