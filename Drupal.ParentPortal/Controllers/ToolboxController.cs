using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Drupal.ParentPortal.Controllers
{
    public class ToolboxController : Controller
    {        
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult GetGuid()
        {
            var g = Guid.NewGuid();
            return Json(g.ToString(), JsonRequestBehavior.AllowGet);
        }

    }
}
