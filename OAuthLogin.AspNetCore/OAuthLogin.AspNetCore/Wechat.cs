using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace OAuthLogin.AspNetCore
{
    public class Wechat:LoginBase
    {
        public Wechat(IHttpContextAccessor
                  contextAccessor) :base(contextAccessor) { }

        static string authorize_url = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=" +LoginProvider.Wechat_client_id + "&redirect_uri={0}&response_type=code&scope=snsapi_userinfo#wechat_redirect";

        static string oauth_url = "https://api.weixin.qq.com/sns/oauth2/access_token";

        static string user_info_url = "https://api.weixin.qq.com/sns/userinfo";

        static string user_info_url_params = "access_token={0}&openid={1}&lang=zh_CN";

        public AuthorizeResult Authorize()
        {
            try
            {
                var code = AuthorizeCode;

                if (string.IsNullOrEmpty(code))
                {
                    Context.Response.Redirect(string.Format(authorize_url,redirect_uri), true);

                    return null;
                }

                if (!string.IsNullOrEmpty(code))
                {
                    var errorMsg = string.Empty;

                    var token = Accesstoken(code, ref errorMsg);

                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        var access_token = token.Value<string>("access_token");

                        var uid = token.Value<string>("openid");

                        var user = UserInfo(access_token, uid, ref errorMsg);

                        if (string.IsNullOrEmpty(errorMsg))
                        {
                            return new AuthorizeResult() { code = 0, result = user, token = access_token };
                        }

                        return new AuthorizeResult() { code = 3, error = errorMsg, token = access_token };
                    }

                    return new AuthorizeResult() { code = 2, error = errorMsg };
                }
            }

            catch (Exception ex)
            {
                return new AuthorizeResult() { code = 1, error = ex.Message };
            }

            return null;
        }

        private JObject Accesstoken(string code, ref string errMsg)
        {
            var data = new SortedDictionary<string, string>();
            data.Add("appid", LoginProvider.Wechat_client_id);
            data.Add("secret", LoginProvider.Wechat_client_secret);
            data.Add("grant_type", "authorization_code");
            data.Add("code", code);

            var Params = string.Join("&", data.Select(x => x.Key + "=" + x.Value).ToArray());

            using (var wb = new HttpClient())
            {
                try
                {
                    var response = wb.PostAsync(oauth_url, new StringContent(Params)).Result;

                    var result = response.Content.ReadAsStringAsync().Result;

                    return Deserialize(result);
                }

                catch (Exception ex)
                {
                    errMsg = ex.Message;

                    return null;
                }
            }
        }

        private JObject UserInfo(string token, string uid,ref string errMsg)
        {
            try
            {
                var result = string.Empty;

                using (var wc = new HttpClient())
                {
                    var content = new StringContent(string.Format(user_info_url_params, token, uid));

                    var response = wc.PostAsync(user_info_url, content).Result;

                    result = response.Content.ReadAsStringAsync().Result;
                }

                var user = Deserialize(result);

                user.Add("uid", uid);

                return user;

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;

                return null;
            }
        }
    }
}
