using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace OAuthLogin.AspNetCore
{
   public class Kakao: LoginBase
    {
        public Kakao(IHttpContextAccessor
                          contextAccessor) :base(contextAccessor) { }

        static string authorize_url = "https://kauth.kakao.com/oauth/authorize?client_id=" + LoginProvider.Kakao_client_id + "&redirect_uri={0}&response_type=code";

       static string oauth_url = "https://kauth.kakao.com/oauth/token?";

       static string user_id_url = "https://kapi.kakao.com/v1/user/me";

       static string user_info_url = "https://kapi.kakao.com/v1/api/talk/profile";

        public AuthorizeResult Authorize()
        {
            try
            {
                var code = AuthorizeCode;

                if (string.IsNullOrEmpty(code))
                {
                    Context.Response.Redirect(string.Format(authorize_url, redirect_uri), true);

                    return null;
                }

                if (!string.IsNullOrEmpty(code))
                {
                    var errorMsg = string.Empty;

                    var token = Accesstoken(code, ref errorMsg);

                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        var access_token = token.Value<string>("access_token");

                        var user = UserInfo(access_token,ref errorMsg);

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
            data.Add("client_id", LoginProvider.Kakao_client_id);
            data.Add("redirect_uri", redirect_uri);
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

        private JObject UserInfo(string token, ref string errMsg)
        {
            try
            {
                var result = string.Empty;

                using (var wc = new HttpClient())
                {
                    wc.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    var response = wc.PostAsync(user_info_url, null).Result;

                    result = response.Content.ReadAsStringAsync().Result;
                }

                var user = Deserialize(result);

                using (var wc = new HttpClient())
                {
                    wc.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    var response = wc.PostAsync(user_id_url, null).Result;

                    result = response.Content.ReadAsStringAsync().Result;
                }

                var userId = Deserialize(result);

                user["uid"] = userId["id"];

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
