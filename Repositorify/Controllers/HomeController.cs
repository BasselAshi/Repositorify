using Repositorify.Controllers.Operations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repositorify.Controllers
{
    public class HomeController : Controller
    {
        HomeServices Service = new HomeServices();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tag(string id)
        {
            var images = Service.GetImages(id);
            return View(images);
        }

        public JsonResult Tags_Read()
        {
            var tags = Service.GetTagViewModels();
            return Json(tags, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadImage(HttpPostedFileBase uploadImage, string tags)
        {
            var allowedExtensions = new List<string>
            {
                ".jpg", ".jpeg", ".png", ".gif"
            };
            var maxSize = 16 * 1024 * 1024; // in Bytes

            // Backend validation
            if (uploadImage == null)
            {
                // No file
                return Content(null);
            } else if (!allowedExtensions.Contains(Path.GetExtension(uploadImage.FileName)))
            {
                // Invalid extension
                return Content("-1");
            } else if (uploadImage.ContentLength > maxSize)
            {
                // Limit exceeded
                return Content("-2");
            }
            
            var serverPath = Server.MapPath("~/Uploads");
            var success = Service.SaveImage(serverPath, uploadImage, tags);
            return Content(success ? "0" : "-3");
        }
    }
}