using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using nCommon;
using nCommon.OAuth;

namespace nOAuth.Client.Controllers
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
    }

    public class HomeController : Controller
    {
        public IAuthorizationState Authorization { get; private set; }
        public UserAgentClient Client { get; set; }

        public HomeController()
        {
            var a = Guid.NewGuid().ToString("N");
            var authServer = new AuthorizationServerDescription()
            {
                AuthorizationEndpoint = new Uri("http://localhost:4251/OAuth/Authorize"),
                TokenEndpoint = new Uri("http://localhost:4251/OAuth/Token"),
            };
            Client = new UserAgentClient(authServer, "samplewebapiconsumer", "samplesecret");
            Authorization = new AuthorizationState { Callback = new Uri("http://localhost:4494/") };
        }

        public ActionResult Index()
        {
            try
            {
                var valueString = string.Empty;
                if (Session["AccessToken"] == null)
                {
                    this.Client.ProcessUserAuthorization(Request.Url, this.Authorization);

                    if (!string.IsNullOrEmpty(this.Authorization.AccessToken))
                    {
                        Session["AccessToken"] = this.Authorization.AccessToken;
                        Session["RefreshToken"] = this.Authorization.RefreshToken;
                        valueString = CallAPI(this.Authorization.AccessToken);
                    }
                }
                else
                {
                    valueString = CallAPI(Session["AccessToken"].ToString());
                }
                ViewBag.Values = valueString;
            }
            catch (ProtocolException ex)
            {
            }

            return View();
        }

        /// <summary>
        /// Password Auth方式授权
        /// </summary>
        /// <returns></returns>
        public ActionResult PasswordAuth()
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var data = client.UploadValues("http://localhost:4251/OAuth/Token", new NameValueCollection
            {
                {"grant_type","password"},
                {"username","zhoucw"},
                {"password","111111"},
                {"client_id","rograndanylearntest_5cdf2af64a9e460aa96e4ac631ed16d9"},
                {"client_secret","cd3ebe9fd4ca4f7b866c3f9b205ae338"}
            });
            var str = Encoding.UTF8.GetString(data);
            var token = str.ToObject<Token>();
            ViewBag.token = str;
            ViewBag.accesstoken = token.access_token;
            ViewBag.refreshToken = token.refresh_token;
            return View();
        }

        /// <summary>
        /// 使用accesstoken获取resource 1
        /// </summary>
        /// <param name="accesstoken"></param>
        /// <returns></returns>
        public ActionResult GetResource(string accesstoken)
        {
            WebClient c = new WebClient();
            c.Headers[HttpRequestHeader.Authorization] = string.Format("Bearer {0}", accesstoken);
            var str = c.DownloadString("http://localhost:4501/api/values");
            ViewBag.str = str;
            return View();
        }

        /// <summary>
        /// 使用refresh_token获取accesstoken
        /// </summary>
        /// <param name="refreshtoken"></param>
        /// <returns></returns>
        public ActionResult Refresh(string refreshtoken)
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var data = client.UploadValues("http://localhost:4251/OAuth/Token", new NameValueCollection
            {
                {"grant_type","refresh_token"},
                {"client_id","rograndanylearntest_5cdf2af64a9e460aa96e4ac631ed16d9"},
                {"client_secret","cd3ebe9fd4ca4f7b866c3f9b205ae338"},
                {"refresh_token",refreshtoken}
            });
            var str = Encoding.UTF8.GetString(data);
            ViewBag.token = str;
            return View();
        }

        /// <summary>
        /// 使用accesstoken获取resource 2
        /// </summary>
        /// <param name="accesstoken"></param>
        /// <returns></returns>
        private string CallAPI(string accesstoken)
        {
            var webClient = new WebClient();
            webClient.Headers["Content-Type"] = "application/json";
            webClient.Headers["X-JavaScript-User-Agent"] = "Google APIs Explorer";
            this.Client.AuthorizeRequest(webClient, accesstoken);
            var valueString = webClient.DownloadString("http://localhost:4501/api/values");
            return valueString;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult GetValues()
        {
            bool isOK = false;
            bool requiresAuth = false;
            string redirectURL = "";
            if (Session["AccessToken"] == null)
            {
                this.Authorization.Scope.AddRange(OAuthUtilities.SplitScopes("read_user read_course publish_comment publish_appraise publish_question publish_note"));
                Uri authorizationUrl = this.Client.RequestUserAuthorization(this.Authorization);
                requiresAuth = true;
                redirectURL = authorizationUrl.AbsoluteUri;
                isOK = true;
            }
            else
            {
                requiresAuth = false;
            }
            return new JsonResult()
            {
                Data = new
                {
                    OK = isOK,
                    RequiresAuth = requiresAuth,
                    RedirectURL = redirectURL
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
