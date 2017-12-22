using System.Web.Mvc;
using System.Web.Security;
using OktaAPI.Helpers;
using OktaAPIShared.Models;

namespace OktaCustomerUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string BasicLogin, string OIDCLogin)
        {
            //handle 2 ways to login
            if (BasicLogin != null)
            {
                return RedirectToAction("BasicLogin", model);
            }
            else//OIDCLogin
            {
                return RedirectToAction("OIDCLogin", model);
            }
        }
        
        [AllowAnonymous]
        public ActionResult BasicLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sessionResponse = APIHelper.SendBasicLogin(model);

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
                TempData["Message"] = "You have been logged in as " + name;
                TempData["IsError"] = false;
            }
            else
            {
                try
                {
                    TempData["Message"] = sessionResponse.errorSummary;
                    TempData["IsError"] = true;
                }
                catch
                {
                    // ignored
                }

            }

            //return View(model);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult OIDCLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var sessionResponse = APIHelper.SendBasicLogin(model);// This will return a Okta Session w/Session Token

            if (sessionResponse != null && !string.IsNullOrEmpty(sessionResponse.SessionToken))
            {
                var url = APIHelper.GetAuthorizationURL(sessionResponse.SessionToken);
                return Redirect(APIHelper.GetAuthorizationURL(sessionResponse.SessionToken));
            }
            else
            {
                try
                {
                    TempData["Message"] = sessionResponse.errorSummary;
                    TempData["IsError"] = true;
                }
                catch
                {
                    // ignored
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AuthCode(string code, string state)//state caode from okta should match what I sent in
        {
            var tokenresponse = APIHelper.GetToken(code);

            if (string.IsNullOrEmpty(tokenresponse.AccessToken) || string.IsNullOrEmpty(tokenresponse.IDToken))
            {
                try
                {
                    var errordesc = "Unknown Error - No Access or ID Tokens";
                    errordesc = tokenresponse.errorSummary;
                    TempData["Message"] = errordesc;
                    TempData["IsError"] = true;
                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                Helpers.LoginHelper.SetOIDCTokens(tokenresponse);
            }

            //1 server side - check for token on home page
            //2 then write logic to show login info
            //3 add page for security check - not anonymous
            
            //LAST
            //display credentials when accessing site if valid token - use javascript to detect valid okta session

            //redo cert
            //force https on login screen

            return RedirectToAction("Index", "Home");
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