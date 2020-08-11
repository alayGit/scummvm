using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ScummVMBrowser
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "API",
                url: "Api/{action}/{gameId}",
                defaults: new { controller = "Api", action = "{action}", gameId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TestAPI",
                url: "TestApi/{action}",
                defaults: new { controller = "TestApi", action = "{action}" }
            );

            routes.MapRoute(
                name: "FEW",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
