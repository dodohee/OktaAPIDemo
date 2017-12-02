using System.Web.Mvc;
using System.Web.Security;
using OktaAPI.Helpers;
using OktaAPIShared.Models;

namespace OktaCustomerUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sessionResponse = APIHelper.GetSession(model);

            if (sessionResponse != null && !string.IsNullOrEmpty(sessionResponse.SessionToken))
            {
                var name = "Unknown";
                try
                {
                    name = $"{sessionResponse._embedded.user.profile.firstName} {sessionResponse._embedded.user.profile.lastName}";
                }
                catch
                {
                    // ignored
                }

                FormsAuthentication.SetAuthCookie(name, false);
                //return Redirect(APIHelper.GetAuthorizationURL(sessionResponse.SessionToken));

                TempData["Message"] = "You have been logged in as " + name;
            }

            //return View(model);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
    }
}