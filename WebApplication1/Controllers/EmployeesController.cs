using Domain;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using Employee = WebApplication1.Models.Employee;

namespace WebApplication1.Controllers
{
    public class EmployeesController : Controller
    {

        private readonly IEmployeeRepository _repository;
        private readonly ISaleRepository _saleRepository;

        public EmployeesController(IEmployeeRepository repository, ISaleRepository saleRepository)
        {
            _repository = repository;
            _saleRepository = saleRepository;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _repository.GetAllAsync();
            return View(employees);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var accessLevels = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Продавец" },
                new SelectListItem { Value = "2", Text = "Администратор" }
            };

            ViewBag.AccessLevels = accessLevels;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            

            await _repository.CreateAsync(MapEmployee(employee));
            return RedirectToAction(nameof(Index));
        }
        private Domain.Employee MapEmployee(WebApplication1.Models.Employee employee)
        {
            var e = new Domain.Employee();
            e.Id = employee.Id;
            e.Surname = employee.Surname;
            e.Name = employee.Name;
            e.Patronymic = employee.Patronymic;
            e.Position = employee.Position;
            e.Password = employee.Password;
            e.Login = employee.Login;
            e.Level = employee.Level;

            return e;
        }

        public async Task<IActionResult> Delete(Employee employee)
        {
            var employeeToDelete = new Domain.Employee { Id = employee.Id };

            await _repository.DeleteAsync(employeeToDelete);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            var employeeToEdit = new Domain.Employee
            {
                Id = employee.Id,
                Surname = employee.Surname,
                Name = employee.Name,
                Patronymic = employee.Patronymic,
                Position = employee.Position,
                Login = employee.Login,
                Password = employee.Password,
                Level = employee.Level,

            };

            if (employee.Id == null || employee.Id == 0)
            {
                ModelState.AddModelError("", "ID сотрудника не указан");
                return View(employee);
            }

            await _repository.UpdateAsync(employeeToEdit);
            return RedirectToAction(nameof(Index));
        }

        //[HttpGet]
        //public IActionResult Edit()
        //{ 
        //    return View();
        //}

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employeeDomain = await _repository.GetByIdAsync(new Domain.Employee { Id = id });

            var employeeWeb = new WebApplication1.Models.Employee
            {
                Id = employeeDomain.Id,
                Surname = employeeDomain.Surname,
                Name = employeeDomain.Name,
                Patronymic = employeeDomain.Patronymic,
                Position = employeeDomain.Position,
                Login = employeeDomain.Login,
                Password = employeeDomain.Password,
                Level = employeeDomain.Level,
            };
            return View(employeeWeb);
        }

        public async Task<IActionResult> Details(int id)
        {
            var employee = await _repository.GetByIdAsync(new Domain.Employee { Id = id });
            var sales = await _saleRepository.GetByEmployeeId(id);

            ViewBag.Employee = employee;
            ViewBag.Sales = sales;
            ViewBag.TotalSum = sales.Sum(s => s.Sum);
            ViewBag.SalesCount = sales.Count();

            return View();
        }
    }
}
