using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace WebAPIDiff
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services
      var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
      jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();


      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
        name: "Diffs",
        routeTemplate: "v1/{controller}/{diffId}/{side}",
        defaults: new {controller = "Diff", diffId = RouteParameter.Optional, side = RouteParameter.Optional},
        constraints: new { side = "^left$|^right$|^$" }
        );

      config.Formatters.Remove(config.Formatters.XmlFormatter);
    }
  }
}
