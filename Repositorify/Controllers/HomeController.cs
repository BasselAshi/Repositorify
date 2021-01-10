using Repositorify.Controllers.Operations;
using System;
using System.Collections.Generic;
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

        public JsonResult Tags_Read(string text)
        {
            var tags = Service.GetTagViewModels();
            return Json(tags, JsonRequestBehavior.AllowGet);
        }
    }
}