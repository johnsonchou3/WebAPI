using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPI.Handler;

namespace WebAPI
{
    /// <summary>
    /// Webapi config 文件, 控制request respond 會經過的地方
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 在此註冊想要經過的handler 及controller
        /// </summary>
        /// <param name="config">加入路徑</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務

            // Web API 路由
            config.MessageHandlers.Add(new CookieHandler());
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
