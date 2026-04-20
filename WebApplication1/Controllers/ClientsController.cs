using Domain.Interfaces;
using Domain;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using Client = WebApplication1.Models.Client;


namespace WebApplication1.Controllers
{
    public class ClientsController : Controller
    {

        private readonly IClientRepository _repository;
        private readonly ISaleRepository _saleRepository;

        public ClientsController(IClientRepository repository, ISaleRepository saleRepository)
        {
            _repository = repository;
            _saleRepository = saleRepository;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _repository.GetAllAsync();
            return View(clients);
        }
         

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {  
            await _repository.CreateAsync(MapClient(client));

            return RedirectToAction(nameof(Index));
        }
        private Domain.Client MapClient(WebApplication1.Models.Client client)
        {
            var c = new Domain.Client();
            c.Address = client.Address;
            c.Id = client.Id;
            c.Surname = client.Surname; 
            c.Name = client.Name;
            c.Patronymic = client.Patronymic;
            c.PhoneNumber = client.PhoneNumber;

            return c;
        }
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Delete(Client client)
        {
            var clientToDelete = new Domain.Client { Id = client.Id };

            await _repository.DeleteAsync(clientToDelete);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Client client)
        {
            var clientToEdit = new Domain.Client {
                Id = client.Id,
                Surname = client.Surname,
                Name = client.Name,
                Patronymic = client.Patronymic,
                Address = client.Address,
                PhoneNumber = client.PhoneNumber,

            };

            if (client.Id == null || client.Id == 0)
            {
                ModelState.AddModelError("", "ID клиента не указан");
                return View(client);
            }

            await _repository.UpdateAsync(clientToEdit);
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
            var clientDomain = await _repository.GetByIdAsync(new Domain.Client { Id = id });

            var clientWeb = new WebApplication1.Models.Client
            {
                Id = clientDomain.Id,
                Surname = clientDomain.Surname,
                Name = clientDomain.Name,
                Patronymic = clientDomain.Patronymic,
                Address = clientDomain.Address,
                PhoneNumber = clientDomain.PhoneNumber
            };
            return View(clientWeb);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = await _repository.GetByIdAsync(new Domain.Client { Id = id });
            var sales = await _saleRepository.GetByClientId(id);

            ViewBag.Client = client;
            ViewBag.Sales = sales;
            ViewBag.TotalSum = sales.Sum(s => s.Sum);
            ViewBag.SalesCount = sales.Count();


            return View();
        }

    }
}
