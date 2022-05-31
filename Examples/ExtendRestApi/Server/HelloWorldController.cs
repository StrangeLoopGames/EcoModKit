using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eco.WebServer.Web.Controllers
{
     /// <summary>
    /// Example Controller to extend the Eco WebServer with a custom Endpoint.
    /// More details are available at the official ASP.NET Core MVC Documentation from Microsoft.
    /// </summary>
    [AllowAnonymous, Route("api/v1/helloworld")]
    public class HelloWorldController : Controller
    {
        [HttpGet("")]
        public String getAll()
        {
            return "Hello World!";
        }
    }
}