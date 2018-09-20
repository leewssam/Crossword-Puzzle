using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SEF_Assignment
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "RankingBoard",
                url: "RankingBoard",
                defaults: new { controller = "ManageClass", action = "CreateRanking", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                 "ManageClass",
                 "ManageClass/{action}/{id}",
                 new { controller = "ManageClass", action = "ManageClass", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "ChooseIdentity", id = UrlParameter.Optional }
            );


        }
    }
}
