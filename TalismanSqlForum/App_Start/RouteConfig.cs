using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TalismanSqlForum
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ForumMessages",
                url: "{id_list}/{id}/Message",
                defaults: new {controller = "ForumMessages",action = "Index", id = UrlParameter.Optional }
                );
            
            routes.MapRoute(
                name: "ForumThemes",
                url: "{id}/Themes",
                defaults: new { controller = "ForumThemes", action ="Index", id = UrlParameter.Optional }
                );
            routes.MapRoute(
                name: "ForumThemes_Create",
                url: "{id}/Add",
                defaults: new {controller = "ForumThemes",action = "Create", id = UrlParameter.Optional }
                );
            routes.MapRoute(
                name: "Notifications",
                url: "Notifications/{username}",
                defaults: new {controller ="Notifications", action = "Index", username = UrlParameter.Optional }
                );
            routes.MapRoute(
                name: "Notifications_new",
                url: "Notify",
                defaults: new { controller = "Notifications", action = "New"}
                );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "ForumList", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
