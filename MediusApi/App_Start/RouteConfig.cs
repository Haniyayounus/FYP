using Microsoft.AspNetCore.Routing;

namespace MediusApi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = System.Web.Mvc.UrlParameter.Optional }
            );
        }
    }
}