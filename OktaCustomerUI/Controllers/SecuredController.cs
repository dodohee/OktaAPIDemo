using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OktaCustomerUI.Controllers
{
    
    public class SecuredController : Controller
    {
        [OktaAuthorization]
        public ActionResult Index()
        {
            return View();
        }
    }
}
