using Domain;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;
using WebApplication1.Data;
using WebApplication1.Models;
using Sale = WebApplication1.Models.Sale;

namespace WebApplication1.Controllers
{
    public class SalesController : Controller
    {
        private readonly ISaleRepository _repository;
        private readonly IClientRepository _clientRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public SalesController( ISaleRepository repository, IClientRepository clientRepository, IEmployeeRepository employeeRepository)
        {
            _repository = repository;
            _clientRepository = clientRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _repository.GetAllAsync();
            return View(sales);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var clients = await _clientRepository.GetAllAsync();
            var employees = await _employeeRepository.GetAllAsync();

            var clientList = clients.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Surname} | {c.Name} | {c.PhoneNumber}",
            });

            var employeeList = employees.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.Surname} | {e.Name} | {e.Patronymic}",
            });

            ViewBag.ClientList = clientList;
            ViewBag.EmployeeList = employeeList;

            //ViewBag.ClientList = new SelectList(clients, "Id", "Name", "Surname");
            //ViewBag.EmployeeList = new SelectList(employees, "Id", "Name", "Surname");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Sale sale)
        {
            await _repository.CreateAsync(MapSale(sale));

            return RedirectToAction(nameof(Index));
        }
        private Domain.Sale MapSale(WebApplication1.Models.Sale sale)
        {
            var s = new Domain.Sale();
            s.Id = sale.Id;
            s.ClientId = sale.ClientId;      
            s.EmployeeId = sale.EmployeeId; 
            s.Date = sale.Date;
            s.Sum = sale.Sum;

            return s;
        }

        public async Task<IActionResult> Delete(Sale sale)
        {
            var saleToDelete = new Domain.Sale { Id = sale.Id };

            await _repository.DeleteAsync(saleToDelete);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Sale sale)
        {
            var saleToEdit = new Domain.Sale
            {
                Id = sale.Id,
                ClientId = sale.ClientId,
                EmployeeId = sale.EmployeeId,
                Date = sale.Date,
                Sum = sale.Sum,
            };

            if (sale.Id == null || sale.Id == 0)
            {
                ModelState.AddModelError("", "ID продажи не указан");
                return View(sale);
            }

            await _repository.UpdateAsync(saleToEdit);
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
            var saleDomain = await _repository.GetByIdAsync(new Domain.Sale { Id = id });

            var saleWeb = new WebApplication1.Models.Sale
            {
                Id = saleDomain.Id,
                ClientId = saleDomain.ClientId,
                EmployeeId = saleDomain.EmployeeId,
                Date = saleDomain.Date,
                Sum = saleDomain.Sum,
            };

            var clients = await _clientRepository.GetAllAsync();
            var employees = await _employeeRepository.GetAllAsync();

            var clientList = clients.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Surname} | {c.Name} | {c.PhoneNumber}",
            });

            var employeeList = employees.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.Surname} | {e.Name} | {e.Patronymic}",
            });

            ViewBag.ClientList = clientList;
            ViewBag.EmployeeList = employeeList;

            return View(saleWeb);
        }

    }
}
