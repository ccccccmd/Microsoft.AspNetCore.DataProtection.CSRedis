using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.DataProtection.CSRedis.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var x = HttpContext.Session.GetString("admin");
            if (string.IsNullOrWhiteSpace(x))
            {
                HttpContext.Session.SetString("admin", Guid.NewGuid().ToString());
            }

            return Content(x);
        }
    }
}