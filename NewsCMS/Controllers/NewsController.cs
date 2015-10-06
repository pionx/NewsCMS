using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewsCMS.Models;
using System.IO;

namespace NewsCMS.Controllers
{
    public class NewsController : Controller
    {
        private NewsDataBaseEntities db = new NewsDataBaseEntities();

        //
        // GET: /News/

        public ActionResult Index()
        {
            if (Session["userLogged"] != null)
            {
                ViewBag.userLogged = true;
                return View(db.News.ToList());
            }
            else
            {
                ViewBag.userLogged = false;
                return View(db.News.ToList());
            }
        }

        public ActionResult Manage()
        {
            if (Session["userLogged"] != null)
            {
                ViewBag.userLogged = true;
                return View(db.News.ToList());
            }
            else
            {
                ViewBag.userlogged = false;
                return View();
            }
        }

        //
        // GET: /News/View/5
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult Watch(int id = 0)
        {
            if (Session["userLogged"] != null)
            {
                ViewBag.userLogged = true;
            }
            else { ViewBag.userLogged = false; }

            News news = db.News.Find(id);
            List<Image> images = db.Images.Where(x => x.NewsID == id).ToList();

            ViewBag.news = db.News.ToList();

            if (news == null)
            {
                return HttpNotFound();
            }

            if (Session["userLogged"] != null)
            {
                ViewBag.userLogged = true;
            }
            else {
                ViewBag.userLogged = false;
            }
            ViewBag.images = images;
            return View(news);
            
        }

        //
        // GET: /News/Create

        public ActionResult Create()
        {
            if (Session["userLogged"] != null)
            {
                ViewBag.userLogged = true;
            }
            else { ViewBag.userLogged = false; }

            return View();
        }

        //
        // POST: /News/Create


        [HttpPost, ValidateInput(false)]
        public ActionResult Create(News news, IEnumerable<HttpPostedFileBase> files)
        {

            if (ModelState.IsValid)
            {
                string filename = "";
                byte[] bytes;
                int BytestoRead;
                int numBytesRead;
                Image image;

                //get next id
                var rows = db.News.ToList();

                var nextId = 0;
                if (rows.Count > 0)
                {
                    var lastRow = rows[rows.Count - 1];
                    var lastId = lastRow.ID;
                    nextId = lastId + 1;
                }
                else
                {
                    nextId++;
                }

                //save news
                db.News.Add(news);
                db.SaveChanges();

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        filename = Path.GetFileName(file.FileName);
                        bytes = new byte[file.ContentLength];
                        BytestoRead = (int)file.ContentLength;
                        numBytesRead = 0;
                        while (BytestoRead > 0)
                        {
                            int n = file.InputStream.Read(bytes, numBytesRead, BytestoRead);
                            if (n == 0) break;
                            numBytesRead += n;
                            BytestoRead -= n;
                        }

                        //save image
                        image = new Image();
                        image.ImageFile = bytes;
                        image.NewsID = nextId;
                        image.Ext = "?";

                        db.Images.Add(image);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("index");
            }

            return View();
        }

        //
        // GET: /News/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Session["userLogged"] != null)
            {
                ViewBag.userLogged = true;
            }
            else { ViewBag.userLogged = false; }

            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        //
        // POST: /News/Edit/5

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(News news, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();

                string filename = "";
                byte[] bytes;
                int BytestoRead;
                int numBytesRead;
                string ext;
                Image image;

                int id = news.ID;

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        filename = Path.GetFileName(file.FileName);
                        bytes = new byte[file.ContentLength];
                        BytestoRead = (int)file.ContentLength;
                        numBytesRead = 0;
                        ext = System.IO.Path.GetExtension(file.FileName);
                        while (BytestoRead > 0)
                        {
                            int n = file.InputStream.Read(bytes, numBytesRead, BytestoRead);
                            if (n == 0) break;
                            numBytesRead += n;
                            BytestoRead -= n;
                        }

                        //save image
                        image = new Image();
                        image.ImageFile = bytes;
                        image.NewsID = id;
                        image.Ext = ext;

                        db.Images.Add(image);
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Watch", "News", new { id = news.ID });
            }
            return View(news);
        }

        //
        // GET: /News/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Session["userLogged"] != null)
            {
                ViewBag.userLogged = true;
            }
            else { ViewBag.userLogged = false; }

            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        //
        // POST: /News/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            List<Image> images = db.Images.Where(i => i.NewsID == id).ToList();
            for (var i = 0; i < images.Count; i++) 
            {
                db.Images.Remove(images[i]);
            }
            db.SaveChanges();

            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        
    }
}