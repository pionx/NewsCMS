using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewsCMS.Models;

namespace NewsCMS.Controllers
{
    public class UserController : Controller
    {
        private NewsDataBaseEntities db = new NewsDataBaseEntities();

        //
        // GET: /User/

        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //
        // GET: /User/Details/5

        public ActionResult Details(int id = 0)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(User user)
        {
            var userExist = (from u in db.Users
                                where u.UserName == user.UserName && u.Password == user.Password
                                select u).Count();

            if (userExist > 0)
            {
                Session["userName"] = user.UserName;
                Session["userLogged"] = true;

                return RedirectToAction("Index", "News");
            }
            else {
                ViewBag.errorCredenciales = "Datos Erroneos";
            }

            return View();

        }

        public ActionResult LogOut()
        {
            Session["userLogged"] = null;

            return RedirectToAction("Index", "News");
        }
        
    }
}