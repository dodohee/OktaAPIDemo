using System.Web;
using System;
using OktaAPIShared.Models;
using OktaAPI.Helpers;


namespace OktaCustomerUI.Helpers
{
    public static class LoginHelper
    {
        public static readonly string ACCESS_TOKEN = "ACCESS_TOKEN";//auth token for introspection and access at okta level
        public static readonly string ID_TOKEN = "ID_TOKEN";//id token for app to parse and look at claims
        
        public static void SetOIDCTokens(OIDCTokenResponse tokenresponse)
        {
            var accesscookie = new HttpCookie(ACCESS_TOKEN);
            accesscookie.Value = tokenresponse.AccessToken;
            accesscookie.Expires = DateTime.Now.AddDays(1);
            HttpContext.Current.Response.Cookies.Add(accesscookie);

            var idcookie = new HttpCookie(ID_TOKEN);
            idcookie.Value = tokenresponse.IDToken;
            idcookie.Expires = DateTime.Now.AddDays(1);
            HttpContext.Current.Response.Cookies.Add(idcookie);
        }

        public static TokenIntrospectionResponse GetOIDCIntrospectionDetails()
        {
            TokenIntrospectionResponse tir = null;

            var accesscookie = new HttpCookie(ACCESS_TOKEN);
            accesscookie = HttpContext.Current.Request.Cookies[ACCESS_TOKEN];

            if (accesscookie != null)
            {
                string accesstoken = accesscookie.Value;

                if (!string.IsNullOrEmpty(accesstoken))
                {
                    tir = APIHelper.IntrospectToken(accesstoken);
                }
                
                if (!tir.Active)
                {
                    tir = null;
                }
            }

            return tir;
        }
    }
}