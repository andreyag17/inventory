using appInventory.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;

namespace appInventory.Controllers
{
    public class productoController : Controller
    {
        private inventoryEntities db = new inventoryEntities();

        // GET: producto
        public ActionResult Index()
        {
            if (Session["usuario"] != null)
            {
                var producto = db.producto.Include(p => p.categoria);
                return View(producto.ToList());

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        public ActionResult Ingreso()
        {
            if (Session["usuario"] != null)
            {
                var producto = db.producto.Include(p => p.categoria);
                return RedirectToAction("Create", "movimiento");

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        //No sirve y no se porque
        public ActionResult SalidaM()
        {
            if (Session["usuario"] != null)
            {
                var producto = db.producto.Include(p => p.categoria);
                return RedirectToAction("Salida", "movimiento");

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        //GET: buscador
        [HttpPost]
        public ActionResult Buscador(string codProducto, string nomProducto, string tipoProducto)
        {
            var productos = db.producto.Include(p => p.categoria).AsQueryable();

            if (!string.IsNullOrEmpty(codProducto))
            {
                productos = productos.Where(p => p.codigoProducto.Contains(codProducto));
            }

            if (!string.IsNullOrEmpty(nomProducto))
            {
                productos = productos.Where(p => p.nombreProducto.Contains(nomProducto));
            }

            if (!string.IsNullOrEmpty(tipoProducto))
            {
                productos = productos.Where(p => p.categoria.nombreCategoria.Contains(tipoProducto));
            }
            //return View(productos.ToList());
            return View("Index", productos.ToList());
        }



        public ActionResult Details(int? id)
        {
            {
                {
                    if (Session["usuario"] != null)
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }

                        var movimiento = db.movimiento
                            .Include(m => m.producto)
                            .Include(m => m.usuario)
                            .FirstOrDefault(m => m.idCodigo == id); // Usa FirstOrDefault con Include

                        if (movimiento == null)
                        {
                            return HttpNotFound();
                        }

                        return View(movimiento);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Usuario");
                    }
                }
            }
        }

        // GET: producto/Create
        public ActionResult Create()
        {
            {
                if (Session["usuario"] != null)
                {
                    ViewBag.categoriaId = new SelectList(db.categoria, "categoriaId", "nombreCategoria");
                    return View();

                }
                else
                {
                    return RedirectToAction("Login", "Usuario");
                }
            }
        }

        // POST: producto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "codigoProducto,nombreProducto,cantidad,categoriaId")] producto producto)
        {

            if (ModelState.IsValid)
            {
                db.producto.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.categoriaId = new SelectList(db.categoria, "categoriaId", "nombreCategoria", producto.categoriaId);
            return View(producto);

        }

        // GET: producto/Edit/5
        public ActionResult Edit(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            producto producto = db.producto.Find(id);
            if (producto == null)
            {
                ViewBag.Error = "⚠️ Producto no encontrado.";
                return View();
            }
            ViewBag.categoriaId = new SelectList(db.categoria, "categoriaId", "nombreCategoria", producto.categoriaId);
            return View(producto);



        }

        // POST: producto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "codigoProducto,nombreProducto,cantidad,categoriaId")] producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.categoriaId = new SelectList(db.categoria, "categoriaId", "nombreCategoria", producto.categoriaId);
            return View(producto);
        }

        //Put: Actualizar/Cantidad
        [HttpPut]
        public void Actualizar(string id, int cantidad)
        {
            producto prod = db.producto.Find(id);
            prod.cantidad += cantidad;
            db.Entry(prod).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: producto/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            producto producto = db.producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            producto producto = db.producto.Find(id);
            db.producto.Remove(producto);
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
