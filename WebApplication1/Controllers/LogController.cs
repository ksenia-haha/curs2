using System.Security.Claims;
using Domain;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class LogController : Controller
    {

        private readonly IEmployeeRepository _repository;

        public LogController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(); 
        }


        [HttpPost]
        public async Task<IActionResult> Login(Log model)
        {
            var employeeToCheck = new Domain.Employee
            {
                Login = model.Login,
                Password = model.Password,
            };

            var employeeToLogin = await _repository.Login(employeeToCheck);

            if (employeeToLogin != null)
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("EmployeeId", employeeToLogin.Id.ToString());
                HttpContext.Session.SetString("UserName", $"{employeeToLogin.Name} {employeeToLogin.Surname}");
                HttpContext.Session.SetInt32("UserLevel", employeeToLogin.Level);
                return RedirectToAction("Index", "Home");
            }

            //заглушка
            //if (model.Login == "admin" && model.Password == "123")
            //{
            //    var employee = await _repository.Login(employeeToLogin);

            //    return RedirectToAction("Index", "Home"); 
            //}

            return View(model);
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Log");
        }
    }
}
