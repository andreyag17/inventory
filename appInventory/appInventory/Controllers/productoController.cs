using appInventory.Models;
using appInventory.Models.ViewModel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls.WebParts;

namespace appInventory.Controllers
{
    public class productoController : Controller
    {
        private inventoryEntities db = new inventoryEntities();

        // GET: producto
        public ActionResult Index(string mensaje)
        {
            if (Session["usuario"] != null)
            {

                if (mensaje != null)
{
                    ViewBag.EditarMensaje = mensaje;

                    mensaje = null;
                }
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
                            .FirstOrDefault(m => m.idCodigo == id);

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
        // Crea los productos, agrega estos a Lista de inventario
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
        // Crea producto para agregarlo a Lista de inventario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "codigoProducto,nombreProducto,cantidad,categoriaId")] producto producto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.producto.Add(producto);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { mensaje = "✅ Creado correctamente." });
                }
                ViewBag.categoriaId = new SelectList(db.categoria, "categoriaId", "nombreCategoria", producto.categoriaId);
                return View(producto);
            }
            catch (Exception)
            {
                ViewBag.crearError = "⚠️ Error al crear.";
                throw;
            }
        }
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
        //Guarda los cambios realizados en Editar producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(producto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var producto = db.producto.Find(model.codigoProducto);
                    producto.nombreProducto = model.nombreProducto;
                    producto.categoriaId = model.categoriaId;
                    db.SaveChanges();

                    return RedirectToAction("Index", new { mensaje = "✅ Editado correctamente." });
                }
                ViewBag.categoriaId = new SelectList(db.categoria, "categoriaId", "nombreCategoria", model.categoriaId);
                return View(model);
            }
            catch (Exception)
            {
                ViewBag.EditarError = "⚠️ Error al guardar.";
                throw;
            }
        }

        //Put: Actualizar/Cantidad
        // Actualiza la cantidad que se ingreso desde ingreso de movimiento
        [HttpPut]
        public void Actualizar(string id, int cantidad)
        {
            producto prod = db.producto.Find(id);
            prod.cantidad += cantidad;
            db.Entry(prod).State = EntityState.Modified;
            db.SaveChanges();
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
