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
using System.Web.Services.Description;


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

        public ActionResult BuscadorMovimiento(string nombreProducto, string tipoMovimiento, string Registradox, DateTime? Fecha)
        {
            //var movimientos = db.movimiento.Include(p => p.usuario).AsQueryable();
            var movimientos = db.movimiento.Include(p => p.usuario).Include(p => p.producto).ToList();
            if (!string.IsNullOrEmpty(nombreProducto))
            {
                movimientos = movimientos.Where(p => p.producto.nombreProducto.Contains(nombreProducto)).ToList();
            }

            if (!string.IsNullOrEmpty(tipoMovimiento))
            {
                movimientos = movimientos.Where(p => p.tipoMovimiento.ToString().Contains(tipoMovimiento)).ToList();
            }

            if (!string.IsNullOrEmpty(Registradox))
            {
                movimientos = movimientos.Where(p => p.usuario.nombre.Contains(Registradox)).ToList();
            }

            if (Fecha.HasValue)
            {
                DateTime fechaBuscar = Fecha.Value.Date;
                movimientos = movimientos
                    .Where(p =>
                        (p.fechaIngreso.HasValue && p.fechaIngreso.Value.Date == fechaBuscar) ||
                        (p.fechaSalida.HasValue && p.fechaSalida.Value.Date == fechaBuscar) ||
                        (p.creacionRegistro.HasValue && p.creacionRegistro.Value.Date == fechaBuscar))
                    .ToList();
            }
            return View("Index", movimientos);
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
                movimiento.sobrante = movimiento.cantidad;
                db.movimiento.Add(movimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.codigoProducto = new SelectList(db.producto, "codigoProducto", "nombreProducto", movimiento.codigoProducto);
            ViewBag.usuarioId = new SelectList(db.usuario, "usuarioId", "nombre", movimiento.usuarioId);
            return View(movimiento);
        }


        // GET: movimiento/Salida
        //Genera una salida, resta de cantidad en productos y además muestra una fecha para que los usuarios administradores observen cuantas salidas se realizan

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
        public ActionResult Salida([Bind(Include = "idCodigo,codigoProducto,fechaVencimiento,fechaIngreso,fechaSalida,creacionRegistro,cantidad,tipoMovimiento,usuarioId")] movimiento movimientop)
        {

            if (ModelState.IsValid)
            {
                //var movimientos = db.movimiento.Include(p => p.usuario).Include(p => p.producto).ToList();
                var productos = db.producto.AsQueryable();
                int cantidad = 0;
                int cantidadSobrante = 0;
                int cantidadSalida = Convert.ToInt32(movimientop.cantidad);
                productos = productos.Where(p => p.codigoProducto == movimientop.codigoProducto && p.cantidad >= movimientop.cantidad);
                if (productos.Count() > 0)
                {
                    var movimientoList = db.movimiento.ToList();
                    movimientoList = movimientoList.Where(p => p.codigoProducto == movimientop.codigoProducto && p.sobrante > 0 && p.tipoMovimiento == 1).ToList();

                    foreach (var item in movimientoList)
                    {
                        if (cantidadSalida != 0)
                        {
                            if (cantidadSalida >= item.sobrante)
                            {
                                cantidadSalida -=  Convert.ToInt32(item.sobrante);
                                cantidad = Convert.ToInt32(item.sobrante);
                                cantidadSobrante = 0;
                            }
                            else
                            {
                                cantidad = cantidadSalida;
                                cantidadSalida = 0;
                                cantidadSobrante = Convert.ToInt32(item.sobrante) - cantidad;

                            }

                            var mov = db.movimiento.FirstOrDefault(p => p.codigoProducto == item.codigoProducto && p.sobrante > 0 && p.tipoMovimiento == 1);
                            mov.sobrante = cantidadSobrante;
                            db.SaveChanges();

                            movimientop.creacionRegistro = DateTime.Now;
                            movimientop.tipoMovimiento = 2;
                            movimientop.fechaVencimiento = mov.fechaVencimiento;
                            movimientop.fechaIngreso = mov.fechaIngreso;
                            movimientop.cantidad = cantidad;
                            usuario user = (usuario)Session["usuario"];
                            movimientop.usuarioId = user.usuarioId;
                            productoController prod = new productoController();
                            prod.Actualizar(movimientop.codigoProducto, Convert.ToInt32(-cantidad));


                            db.movimiento.Add(movimientop);
                            db.SaveChanges();
                        }

                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.SalidaError = "⚠️ No hay cantidad suficiente para este producto";

                }
            }

            ViewBag.codigoProducto = new SelectList(db.producto, "codigoProducto", "nombreProducto", movimientop.codigoProducto);
            ViewBag.usuarioId = new SelectList(db.usuario, "usuarioId", "nombre", movimientop.usuarioId);
            //return View(movimiento);
            return View(movimientop);

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
