using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using appInventory.Models;
using appInventory.Models.ViewModel;

namespace appInventory.Controllers
{
    public class gestionUsuariosController : Controller
    {
        private inventoryEntities db = new inventoryEntities();

        // GET: gestionUsuarios
        public ActionResult Index()
        {

            if (Session["usuario"] != null)
            {
                var usuarios = db.usuario.Include(u => u.rol);
                return View(usuarios.ToList());

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }
        }

        // GET: gestionUsuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        //GET: buscador
        [HttpPost]
        public ActionResult Buscador(string usuarioId, string nombre, string rol)
        {
            var usuario = db.usuario.Include(p => p.rol).AsQueryable();

            if (!string.IsNullOrEmpty(usuarioId))
            {
                usuario = usuario.Where(p => p.usuarioId.ToString().Contains(usuarioId));
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                usuario = usuario.Where(p => p.nombre.Contains(nombre));
            }

            if (!string.IsNullOrEmpty(rol))
            {
                usuario = usuario.Where(p => p.rol.rolNombre.Contains(rol));
            }
            //return View(productos.ToList());
            return View("Index", usuario.ToList());
        }

        // GET: gestionUsuarios/Create
        public ActionResult Create()
        {
            ViewBag.rol_Id = new SelectList(db.rol, "rol_Id", "rolNombre");
            return View();
        }

        // POST: gestionUsuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "usuarioId,nombre,apellido1,apellido2,correoElectronico,nombreUsuario,contrasennaUsuario,rol_Id")] usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.usuario.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.rol_Id = new SelectList(db.rol, "rol_Id", "rolNombre", usuario.rol_Id);
            return View(usuario);
        }

        // GET: gestionUsuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuario.Find(id);
            if (usuario == null)
            {
                ViewBag.LoginError = "⚠️ Usuario o contraseña incorrectos.";
                return View();

            }
            ViewBag.rol_Id = new SelectList(db.rol, "rol_Id", "rolNombre", usuario.rol_Id);
            return View(usuario);
            //Haora
        }

        // POST: gestionUsuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(listaUsuario model)
        {
            if (ModelState.IsValid)
            {
                var usuario = db.usuario.Find(model.usuarioId);
                usuario.nombre = model.nombre;
                usuario.apellido1 = model.apellido1;
                usuario.apellido2 = model.apellido2;
                usuario.correoElectronico = model.correoElectronico;
                usuario.nombreUsuario = model.nombreUsuario;
                usuario.rol_Id = model.rol_Id;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: gestionUsuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: gestionUsuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            usuario usuario = db.usuario.Find(id);
            db.usuario.Remove(usuario);
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
