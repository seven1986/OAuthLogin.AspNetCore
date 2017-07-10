using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OAuthLogin.AspNetCore
{
    public class LoginBase
    {
        public HttpContext Context;

        public LoginBase(IHttpContextAccessor
                              contextAccessor)
        {
            this.Context = contextAccessor.HttpContext;
        }

        private const string code = "code";

        protected string AuthorizeCode
        {
            get
            {
                var result = Context.Request.Query[code].ToString();

                if (!string.IsNullOrEmpty(result)) return result;

                return string.Empty;
            }
        }

        /// <summary>
        /// 授权码回调地址
        /// </summary>
        protected string redirect_uri
        {
            get
            {
                return Context.Request.Scheme + "://" + Context.Request.Host.Value + Context.Request.Path.Value;
            }
        }

        protected string Serialize(object obj)
        {
           return JsonConvert.SerializeObject(obj);
        }

        protected JObject Deserialize(string objStr)
        {
            return JsonConvert.DeserializeObject<JObject>(objStr);
        }
    }

    public class AuthorizeResult
    {
        public int code { get; set; }

        public string error { get; set; }

        public JObject result { get; set; }

        public string token { get; set; }
    }
}
