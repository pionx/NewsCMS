using NewsCMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewsCMS.Controllers
{
    public class ImageController : Controller
    {
        private NewsDataBaseEntities db = new NewsDataBaseEntities();

        public ActionResult GetImage(int id = 0)
        {
            if (id != 0)
            {
                if (Session["userLogged"] != null)
                {
                    ViewBag.userLogged = true;
                }
                else { ViewBag.userLogged = false; }

                Image image = db.Images.Find(id);
                return View(image);
            }
            return RedirectToAction("Index", "News");
        }


        public ActionResult AddImage(int id = 0)
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


        public ActionResult UploadImages(int NewsId = 0)
        {
            if (NewsId != 0)
            {
                
                string filename = "";
                byte[] bytes;
                int BytestoRead;
                int numBytesRead;
                string ext;
                Image image;

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
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
                    image.NewsID = NewsId;
                    image.Ext = ext;

                    db.Images.Add(image);
                    
                }
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet); 
            }
            else {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);                
            }
        }

        public ActionResult Delete(int id = 0)
        {
            if (id != 0)
            {
                if (Session["userLogged"] != null)
                {
                    ViewBag.userLogged = true;
                }
                else { ViewBag.userLogged = false; }

                List<Image> images = db.Images.Where(i => i.NewsID == id).ToList();
                ViewBag.News = db.News.Find(id);
                
                return View(images);
            }
            else {
                return RedirectToAction("Index", "News");
            }

        }

        [HttpPost]
        public ActionResult deleteImages(List<string> imagesId)
        {
            List<Image> images = new List<Image>();
            List<int> ids = new List<int>();


            for (var i = 0; i < imagesId.Count; i++) {
                ids.Add(Convert.ToInt32(imagesId[i]));
            }
            
            for (var i = 0; i < ids.Count; i++) {
                var id = ids[i];
                images.Add (db.Images.Where(r => r.ID == id).First());
            }

            for (var i = 0; i < images.Count; i++)
            {
                db.Images.Remove(images[i]);
            }

            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}
