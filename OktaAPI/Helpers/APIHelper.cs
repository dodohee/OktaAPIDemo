using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OktaAPIShared.Models;

namespace OktaAPI.Helpers
{
    public class APIHelper
    {
        private static string _apiUrlBase;
        private static string _oktaOAuthHeaderAuth;
        private static string _oktaOAuthIssuerId;
        private static string _oktaOAuthClientId;
        private static string _oktaOAuthRedirectUri;
        private static string _oktaToken;
        private static HttpClient _client = new HttpClient();

        static APIHelper()
        {
            _apiUrlBase = WebConfigurationManager.AppSettings["okta:BaseUrl"];
            _oktaOAuthIssuerId = WebConfigurationManager.AppSettings["okta:OAuthIssuerId"];
            _oktaOAuthClientId = WebConfigurationManager.AppSettings["okta:OAuthClientId"];
            _oktaOAuthRedirectUri = WebConfigurationManager.AppSettings["okta:OAuthRedirectUri"];
            _oktaToken = WebConfigurationManager.AppSettings["okta:OAuthToken"];

            var oktaOAuthSecret = WebConfigurationManager.AppSettings["okta:OauthClientSecret"];
            _oktaOAuthHeaderAuth = Base64Encode($"{_oktaOAuthClientId}:{oktaOAuthSecret}");
        }

        public static IEnumerable<BaseCustomer> GetAllCustomers()
        {
            var sJsonResponse = JsonHelper.Get($"https://{_apiUrlBase}/api/v1/users?limit=100", _oktaToken);

            var result = JsonConvert.DeserializeObject<IEnumerable<BaseCustomer>>(sJsonResponse);

            return result;
        }

        public static Customer GetCustomerById(string Id)
        {
            var sJsonResponse = JsonHelper.Get($"https://{_apiUrlBase}/api/v1/users/{Id}", _oktaToken);

            var result = JsonConvert.DeserializeObject<Customer>(sJsonResponse);

            return result;
        }

        public static OktaSessionResponse UpdateCustomer(Customer oCustomer)
        {
            var uc = new CustomerUpdate(oCustomer);

            var sJsonResponse = JsonHelper.Post($"https://{_apiUrlBase}/api/v1/users/{oCustomer.Id}", JsonHelper.JsonContent(uc), _oktaToken);

            return JsonConvert.DeserializeObject<OktaSessionResponse>(sJsonResponse);
        }

        public static OktaSessionResponse AddNewCustomer(Customer oCustomer)
        {
            var oNewCustomer = new CustomerAdd(oCustomer);

            var sJsonResponse = JsonHelper.Post($"https://{_apiUrlBase}/api/v1/users?activate=true", JsonHelper.JsonContent(oNewCustomer), _oktaToken);

            return JsonConvert.DeserializeObject<OktaSessionResponse>(sJsonResponse);
        }

        public static TokenIntrospectionResponse IntrospectToken(string token)
        {
            return GetObjectFromAPI<TokenIntrospectionResponse>(HttpMethod.Post, $"https://{_apiUrlBase}/oauth2/{_oktaOAuthIssuerId}/v1/introspect?token={token}&token_type_hint=access_token", _oktaOAuthHeaderAuth);
        }

        public static string GetAuthorizationURL(string oktaSessionId)
        {
            return string.Format("https://{0}/oauth2/{1}/v1/authorize?response_type=code&client_id={2}&redirect_uri={3}/Account/AuthCode&scope=Read&state=af0ifjsldkj&nonce=n-0S6_WzA2Mj&sessionToken={4}",
                _apiUrlBase,
                _oktaOAuthIssuerId,
                _oktaOAuthClientId,
                _oktaOAuthRedirectUri,
                oktaSessionId);
        }

        public static OktaSessionResponse GetSession(LoginViewModel login)
        {
            //create simple class to lowecase model for json - case sensitive
            var ologin = new OktaAPIShared.Models.Login
            {
                username = login.UserName,
                password = login.Password
            };

            var sJsonResponse = JsonHelper.Post($"https://{_apiUrlBase}/api/v1/authn", JsonHelper.JsonContent(ologin));

            var result = JsonConvert.DeserializeObject<OktaSessionResponse>(sJsonResponse);
            
            return result;
        }
        
        private static T GetObjectFromAPI<T>(HttpMethod method, string uri, object model)
        {
            return GetObjectFromAPI<T>(method, uri, model, null);
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, string uri, string authHeader)
        {
            return GetObjectFromAPI<T>(method, uri, null, authHeader);
        }

        private static T GetObjectFromAPI<T>(HttpMethod method, string uri, object model, string authHeader)
        {
            var result = default(T);
            var request = CreateBaseRequest(method, uri, model, authHeader);
            var response = _client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                if ("text/html".Equals(response.Content.Headers.ContentType.MediaType))
                {
                    var results = response.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine(results);
                }
                else
                {
                    result = response.Content.ReadAsAsync<T>().Result;
                }
            }
            return result;
        }
        
        private static T GetObjectFromAPI<T>(HttpMethod method, string uri)
        {
            return GetObjectFromAPI<T>(method, uri, null);
        }

        private static T[] GetObjectsFromAPI<T>(HttpMethod method, string uri)
        {
            T[] results = { };
            var request = CreateBaseRequest(method, uri);
            var response = _client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                var apiObjects = response.Content.ReadAsAsync<IEnumerable<T>>().Result;
                results = apiObjects.ToArray<T>();
            }

            return results;
        }

        private static HttpRequestMessage CreateBaseRequest(HttpMethod method, string uri, object model = null, string authHeader = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(uri),
                Method = method
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //request.Headers.Authorization = authHeader != null ? new AuthenticationHeaderValue("Basic", authHeader) : new AuthenticationHeaderValue("SSWS", _oktaToken);//_oktaApiKey

            if (authHeader != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            }
            
            if (model != null)
            {
                string json = JsonHelper.JsonContent(model);
                
                request.Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );
            }
            else
            {
                if (method == HttpMethod.Post || method == HttpMethod.Put)
                {
                    request.Content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");
                }
            }

            return request;
        }

        private static string Base64Encode(string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        //public static OIDCTokenResponse GetOIDCToken(string url)
        //{
        //    return GetObjectFromAPI<OIDCTokenResponse>(HttpMethod.Post, url, _oktaOAuthHeaderAuth);
        //}

        //public static string GetTokenURL(string oidcCode)
        //{
        //    return $"https://{_apiUrlBase}/oauth2/{_oktaOAuthIssuerId}/v1/token?grant_type=authorization_code&code={oidcCode}&redirect_uri={_oktaOAuthRedirectUri}/Home/AuthCode";
        //}

        //public static void RevokeToken(string token)
        //{
        //    GetObjectFromAPI<Object>(HttpMethod.Post, $"https://{_apiUrlBase}/oauth2/{_oktaOAuthIssuerId}/v1/revoke?token={token}&token_type_hint=access_token", _oktaOAuthHeaderAuth);
        //}

        //public static OktaSessionResponse GetSession(Authentication authentication)
        //{
        //    var response = new OktaSessionResponse();
        //    var authResponse = GetObjectFromAPI<AuthenticationResponse>(HttpMethod.Post, $"https://{_apiUrlBase}/api/v1/authn", authentication);

        //    if (authResponse != null)
        //    {
        //        //response.SessionToken = authResponse.SessionToken;
        //    }

        //    return response;
        //}
    }
}