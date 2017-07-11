# OAuthLogin.AspNetCore [![Build status](https://ci.appveyor.com/api/projects/status/7oa50i7lt9fboq5m?svg=true)](https://ci.appveyor.com/project/seven1986/oauthlogin-aspnetcore) [![NuGet](https://img.shields.io/nuget/v/OAuthLogin.AspNetCore.svg)](https://www.nuget.org/packages/OAuthLogin.AspNetCore)  [![Join the chat at https://gitter.im/OAuthLogin/OAuthLogin](https://img.shields.io/gitter/room/OAuthLogin/OAuthLogin.svg?style=flat-square)](https://gitter.im/OAuthLogin/OAuthLogin?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Average time to resolve an issue](http://isitmaintained.com/badge/resolution/seven1986/OAuthLogin.AspNetCore.svg)](http://isitmaintained.com/project/seven1986/OAuthLogin.AspNetCore "Average time to resolve an issue") [![GitHub license](https://img.shields.io/badge/license-Apache%202-blue.svg)](https://raw.githubusercontent.com/seven1986/OAuthLogin/master/LICENSE)

Installation
-------------

OAuthLogin.AspNetCore is available as a NuGet package. You can install it using the NuGet Package Console window:


```
PM> Install-Package OAuthLogin.AspNetCore
```


---

If you are looking for the ASP.NET version please head to [OAuthLogin](https://github.com/seven1986/OAuthLogin) project.

---

Usage
------

第一步：配置appsettings.json文件
```json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "Credentials": {
    "QQ": {
      "client_id": "",
      "client_secret": ""
    },
    "Wechat": {
      "client_id": "",
      "client_secret": ""
    },
    "Weibo": {
      "client_id": "",
      "client_secret": ""
    },
    "FaceBook": {
      "client_id": "",
      "client_secret": ""
    },
    "KaKao": {
      "client_id": "",
      "client_secret": ""
    }
  },
}
```

第二步：在Startup.cs配置微博、微信、QQ、facebook、Kakao的client_id、client_secret

```csharp
 // This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    // for HttpContext
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    var Credentials = Configuration.GetSection("Credentials").Get<CredentialsSetting>();

    LoginProvider.UseWeibo(Credentials.Weibo.client_id, Credentials.Weibo.client_secret);

    LoginProvider.UseQQ(Credentials.QQ.client_id, Credentials.QQ.client_secret);

    LoginProvider.UseFaceBook(Credentials.FaceBook.client_id, Credentials.FaceBook.client_secret);

    LoginProvider.UseWechat(Credentials.Wechat.client_id, Credentials.Wechat.client_secret);

    LoginProvider.UseKakao(Credentials.KaKao.client_id); 
}
```


第三步：添加Controllers/OAuthController.cs
```csharp
public class OAuthController : Controller
    {
        IHttpContextAccessor _contextAccessor;

        public OAuthController(IHttpContextAccessor
                  contextAccessor) { this._contextAccessor = contextAccessor; }

        public IActionResult QQ()
        {
            var res = new QQ(_contextAccessor).Authorize();

            if (res != null && res.code == 0)
            {
                return RedirectToLogin(new
                {
                    channel = "qq",
                    code = 0,
                    user = new
                    {
                        uid = res.result.Value<string>("openid"),
                        name = res.result.Value<string>("nickname"),
                        img = res.result.Value<string>("figureurl"),
                        token = res.token
                    }
                });
            }

            return View();
        }

        public IActionResult Wechat()
        {
            var res = new Wechat(_contextAccessor).Authorize();

            if (res != null && res.code == 0)
            {
                return RedirectToLogin(new
                {
                    channel = "wechat",
                    code = 0,
                    user = new
                    {
                        uid = res.result.Value<string>("uid"),
                        name = res.result.Value<string>("nickname"),
                        img = res.result.Value<string>("headimgurl"),
                        token = res.token
                    }
                });
            }

            return View();
        }

        public IActionResult Weibo()
        {
            var res = new Weibo(_contextAccessor).Authorize();

            if (res != null && res.code == 0)
            {
                return RedirectToLogin(new
                {
                    channel = "weibo",
                    code = 0,
                    user = new
                    {
                        uid = res.result.Value<string>("idstr"),
                        name = res.result.Value<string>("name"),
                        img = res.result.Value<string>("profile_image_url"),
                        token = res.token
                    }
                });
            }

            return View();
        }

        public IActionResult Facebook()
        {
            var res = new Facebook(_contextAccessor).Authorize();

            if (res != null && res.code == 0)
            {
                return RedirectToLogin(new
                {
                    channel = "facebook",
                    code = 0,
                    user = new
                    {
                        uid = res.result.Value<string>("id"),
                        name = res.result.Value<string>("name"),
                        img = res.result["picture"]["data"].Value<string>("url"),
                        token = res.token
                    }
                });
            }

            return View();
        }

        public IActionResult Kakao()
        {
            var res = new Kakao(_contextAccessor).Authorize();

            if (res != null && res.code == 0)
            {
                return RedirectToLogin(new
                {
                    channel = "kakao",
                    code = 0,
                    user = new
                    {
                        uid = res.result.Value<string>("uid"),
                        name = res.result.Value<string>("nickname"),
                        img = res.result.Value<string>("thumbnail_image"),
                        token = res.token
                    }
                });
            }

            return View();
        }

        RedirectResult RedirectToLogin(object _entity)
        {
            var OAuthResult = JsonConvert.SerializeObject(_entity);

            // 跳转的页面，union参数后面是编码后的用户数据
            var url = "/login?union=" + WebUtility.UrlEncode(OAuthResult);

            return Redirect(url);
        }
    }
```

第四步：添加5个空页面,页面里不要有代码
```
1，Views/OAuth/QQ.cshtml

2，Views/OAuth/Facebook.cshtml

3，Views/OAuth/Wechat.cshtml

4，Views/OAuth/Webo.cshtml

5，Views/OAuth/Facebook.cshtml
```
