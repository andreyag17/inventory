using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace appInventory.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        [HttpGet]
        public ActionResult Menu()
        {
            if (Session["usuario"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }

        }
        [HttpGet]
        public ActionResult CerrarSession()
        {
            Session.Clear();
            Session.Abandon();

            HttpCookie cookie = new HttpCookie("ASP.NET_SessionId");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            return RedirectToAction("Login", "Usuario");
        }

        [HttpGet]
        public ActionResult PaginaPrincipal()
        {
            if (Session["usuario"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("Login", "Usuario");
            }

        }
        

    }
}
