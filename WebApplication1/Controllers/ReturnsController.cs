using Domain;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Data;
using WebApplication1.Hubs;
using WebApplication1.Models;
using Return = WebApplication1.Models.Return;


namespace WebApplication1.Controllers
{
    public class ReturnsController : Controller
    {

        private readonly IReturnRepository _repository;
        private readonly IClientRepository _clientRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IExemplarRepository _exemplarRepository;
        private readonly IHubContext<ReturnHub> _hubContext;

        public ReturnsController(IReturnRepository repository, ISaleRepository saleRepository, IClientRepository clientRepository, IEmployeeRepository employeeRepository, IExemplarRepository exmplarRepository, IHubContext<ReturnHub> hubContext)
        {
            _repository = repository;
            _clientRepository = clientRepository;
            _employeeRepository = employeeRepository;
            _exemplarRepository = exmplarRepository;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var returns = await _repository.GetAllAsync();
            return View(returns);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var clients = await _clientRepository.GetAllAsync();
            var employees = await _employeeRepository.GetAllAsync();
            var exemplars = await _exemplarRepository.GetSoldExemplarsAsync();

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

            var exemplarList = exemplars.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.EditionISBN} | {e.Id}",
            });

            ViewBag.ClientList = clientList;
            ViewBag.EmployeeList = employeeList;
            ViewBag.ExemplarList = exemplarList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Return toReturn)
        {
            var domainReturn = MapReturn(toReturn);
            await _repository.CreateAsync(domainReturn);

            var client = await _clientRepository.GetByIdAsync(new Domain.Client { Id = toReturn.ClientId });
            var employee = await _employeeRepository.GetByIdAsync(new Domain.Employee { Id = toReturn.EmployeeId });
            var exemplar = await _exemplarRepository.GetByIdAsync(new Domain.Exemplar { Id = toReturn.ExemplarId });

            string clientName = $"{client.Name} {client.Surname}";
            string employeeName = $"{employee.Name} {employee.Surname}";
            string exemplarIsbn = exemplar?.EditionISBN;


            await _hubContext.Clients.All.SendAsync("ReturnCreated",
                domainReturn.Id,
                clientName,     
                employeeName,  
                exemplarIsbn,    
                (int)toReturn.Status,
                domainReturn.Reason);


            return RedirectToAction(nameof(Index));
        }
        private Domain.Entities.Return MapReturn(WebApplication1.Models.Return toReturn)
        {
            var r = new Domain.Entities.Return();
            r.Id = toReturn.Id;
            r.EmployeeId = toReturn.EmployeeId;
            r.ClientId = toReturn.ClientId;
            r.ExemplarId = toReturn.ExemplarId;
            r.Status = toReturn.Status;
            r.Reason = toReturn.Reason;

            return r;
        }

        public async Task<IActionResult> Delete(Return toReturn)
        {
            var toReturnToDelete = new Domain.Entities.Return { Id = toReturn.Id };

            await _repository.DeleteAsync(toReturnToDelete);
            await _hubContext.Clients.All.SendAsync("ReturnDeleted", toReturnToDelete.Id);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Return toReturn)
        {
            var toReturnToEdit = new Domain.Entities.Return {
                Id = toReturn.Id,
                EmployeeId = toReturn.EmployeeId,
                ClientId = toReturn.ClientId,
                ExemplarId = toReturn.ExemplarId,
                Status = toReturn.Status,
                Reason = toReturn.Reason,
            };

            if (toReturn.Id == null || toReturn.Id == 0)
            {
                ModelState.AddModelError("", "ID возврата не указан");
                return View(toReturn);
            }

            await _repository.UpdateAsync(toReturnToEdit);

            var client = await _clientRepository.GetByIdAsync(new Domain.Client { Id = toReturn.ClientId });
            var employee = await _employeeRepository.GetByIdAsync(new Domain.Employee { Id = toReturn.EmployeeId });
            var exemplar = await _exemplarRepository.GetByIdAsync(new Domain.Exemplar { Id = toReturn.ExemplarId });

            string clientName = $"{client.Name} {client.Surname}";
            string employeeName = $"{employee.Name} {employee.Surname}";
            string exemplarIsbn = exemplar?.EditionISBN;

            await _hubContext.Clients.All.SendAsync("ReturnUpdated",
                toReturn.Id,
                clientName,
                employeeName,
                exemplarIsbn,
                (int)toReturn.Status,
                toReturn.Reason);


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
            var toReturnDomain = await _repository.GetByIdAsync(new Domain.Entities.Return { Id = id });

            var toReturnWeb = new WebApplication1.Models.Return
            {
                Id = toReturnDomain.Id,
                EmployeeId = toReturnDomain.EmployeeId,
                ClientId = toReturnDomain.ClientId,
                ExemplarId = toReturnDomain.ExemplarId,
                //SaleId = toReturnDomain.SaleId,
                Status = toReturnDomain.Status,
                Reason = toReturnDomain.Reason,
            };

            var clients = await _clientRepository.GetAllAsync();
            var employees = await _employeeRepository.GetAllAsync();
            var exemplars = await _exemplarRepository.GetAllAsync();

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

            var exemplarList = exemplars.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.EditionISBN} | {e.Id}",
            });

            ViewBag.ClientList = clientList;
            ViewBag.EmployeeList = employeeList;
            ViewBag.ExemplarList = exemplarList;


            return View(toReturnWeb);
        }

        //public async Task<IActionResult> Details(int id)
        //{
        //    var toReturn = await _repository.GetByIdAsync(new Domain.Entities.Return { Id = id });
        //    var sales = await _saleRepository.GetByClientId(id);

        //    ViewBag.Client = client;
        //    ViewBag.Sales = sales;
        //    ViewBag.TotalSum = sales.Sum(s => s.Sum);
        //    ViewBag.SalesCount = sales.Count();


        //    return View();
        //}

    }
}
