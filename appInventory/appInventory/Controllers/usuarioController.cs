using appInventory.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using appInventory.Models.ViewModel;

public class UsuarioController : Controller
{

    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string Username, string Password)
    {
        using (inventoryEntities db = new inventoryEntities())
        {
            var user = db.usuario.FirstOrDefault(u =>
                u.nombreUsuario == Username && u.contrasennaUsuario == Password);
            if (user == null)
            {
                ViewBag.LoginError = "⚠️ Usuario o contraseña incorrectos.";
                return View();
            }
            else
            {
                Session["usuario"] = user;
                Session["rol"] = user.rol_Id;
                return RedirectToAction("paginaPrincipal", "Menu");
            }
        }
    }


}

