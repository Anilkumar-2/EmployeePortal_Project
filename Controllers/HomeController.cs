namespace EmployeeManagementPortal.Controllers
{
    using EmployeeManagementPortal.Model;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmployeeManagementContext _context;

        public HomeController(ILogger<HomeController> logger, EmployeeManagementContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.FindFirst(ClaimTypes.Email)?.Value != null)
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.EmployeeName = _context.Employees.Where(x => x.Email == userEmail).First().Ename.ToString();

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login", "Account");
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
