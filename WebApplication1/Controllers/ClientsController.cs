using Domain;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Data;
using WebApplication1.Hubs;
using WebApplication1.Models;
using Client = WebApplication1.Models.Client;
using WebApplication1.Hubs;


namespace WebApplication1.Controllers
{
    public class ClientsController : Controller
    {

        private readonly IClientRepository _repository;
        private readonly ISaleRepository _saleRepository;
        private readonly IHubContext<ClientHub> _hubContext;

        public ClientsController(IClientRepository repository, ISaleRepository saleRepository, IHubContext<ClientHub> hubContext)
        {
            _repository = repository;
            _saleRepository = saleRepository;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _repository.GetAllAsync();
            return View(clients);
        }
         

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            var domainClient = MapClient(client);
            await _repository.CreateAsync(domainClient);

            await _hubContext.Clients.All.SendAsync("ClientCreated",
                    domainClient.Id,
                    domainClient.Surname,
                    domainClient.Name,
                    domainClient.Patronymic,
                    domainClient.PhoneNumber);

            return RedirectToAction(nameof(Index));
        }
        private Domain.Client MapClient(WebApplication1.Models.Client client)
        {
            var c = new Domain.Client();
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
            await _hubContext.Clients.All.SendAsync("ClientDeleted", clientToDelete.Id);
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
                PhoneNumber = client.PhoneNumber,

            };

            if (client.Id == null || client.Id == 0)
            {
                ModelState.AddModelError("", "ID клиента не указан");
                return View(client);
            }

            await _repository.UpdateAsync(clientToEdit);

            await _hubContext.Clients.All.SendAsync("ClientUpdated",
                    client.Id, client.Surname, client.Name, client.Patronymic, client.PhoneNumber);

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
