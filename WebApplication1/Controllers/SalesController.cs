using System.Text;
using Domain;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using SQLitePCL;
using WebApplication1.Data;
using WebApplication1.Hubs;
using WebApplication1.Hubs;
using WebApplication1.Models;
using Sale = WebApplication1.Models.Sale;
using SaleAndExemplar = Domain.SaleAndExemplar;

namespace WebApplication1.Controllers
{
    public class SalesController : Controller
    {
        private readonly ISaleRepository _repository;
        private readonly IClientRepository _clientRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISaleAndExemplarRepository _saleAndExemplarRepository;
        private readonly IExemplarRepository _exemplarRepository;
        private readonly IHubContext<SaleHub> _hubContext;

        public SalesController( ISaleRepository repository, IClientRepository clientRepository, IEmployeeRepository employeeRepository, ISaleAndExemplarRepository saleAndExemplarRepository, IExemplarRepository exemplarRepository, IHubContext<SaleHub> hubContext)
        {
            _repository = repository;
            _clientRepository = clientRepository;
            _employeeRepository = employeeRepository;
            _saleAndExemplarRepository = saleAndExemplarRepository;
            _exemplarRepository = exemplarRepository;
            _hubContext = hubContext;
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

            var clientList = clients
                .OrderBy(c => c.Id == 5 ? 0 : 1) 
                .ThenBy(c => c.Surname)
                .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Surname} | {c.Name} | {c.PhoneNumber}",
            })
                .ToList();

            var employeeList = employees.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.Surname} | {e.Name} | {e.Patronymic}",
            });

            var exemplars = await _exemplarRepository.GetAvailableExemplarsAsync();
            var exemplarList = exemplars.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.Id} | {e.Edition.Name} | {e.Edition.Author} | {e.Edition.Publisher} | {e.EditionISBN}",
            });

            ViewBag.ClientList = clientList;
            ViewBag.EmployeeList = employeeList;
            ViewBag.ExemplarList = exemplarList;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Sale sale, List<int> selectedExemplarIds)
        {
            using var transaction = await _repository.BeginTransactionAsync();
            var domainSale = MapSale(sale);
            domainSale.Sum = 0;
            domainSale.Date = DateOnly.FromDateTime(DateTime.Today);

            await _repository.CreateAsync(domainSale);

            double totalSum = 0;

            foreach (var exemplarId in selectedExemplarIds)
            {
                var exemplar = await _exemplarRepository.GetByIntIdAsync(exemplarId);
                if (exemplar != null)
                {
                    var price = exemplar.Price; 
                    totalSum += price;

                    var se = new SaleAndExemplar
                    {
                        SaleId = domainSale.Id.Value,
                        ExemplarId = exemplarId,
                    };
                    await _saleAndExemplarRepository.CreateAsync(se);

                    await _exemplarRepository.UpdateStatusSoldAsynс(exemplarId);
                }
            }

            domainSale.Sum = totalSum;
            await _repository.UpdateAsync(domainSale);
           

            await transaction.CommitAsync();

            var client = await _clientRepository.GetByIdAsync(new Domain.Client { Id = domainSale.ClientId });
            var employee = await _employeeRepository.GetByIdAsync(new Domain.Employee { Id = domainSale.EmployeeId });

            string clientName = client != null ? $"{client.Surname} {client.Name}" : "Не указан";
            string employeeName = employee != null ? $"{employee.Surname} {employee.Name}" : "Не указан";

            await _hubContext.Clients.All.SendAsync("SaleCreated",
                domainSale.Id,
                clientName,     
                employeeName,  
                domainSale.Date,
                domainSale.Sum);

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
            await _hubContext.Clients.All.SendAsync("SaleDeleted", saleToDelete.Id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<bool> ConvertData(List<Domain.Sale> data, string outputPath)
        {
            if (!data.Any()) return false;
            if (string.IsNullOrWhiteSpace(outputPath) || !outputPath.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                return false;

            var builder = new StringBuilder();
            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine("<meta charset=\"UTF-8\">");
            builder.AppendLine("<title>Отчёт по продажам</title>");
            builder.AppendLine("<style>");
            builder.AppendLine("table { border-collapse: collapse; width: 100%; }");
            builder.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            builder.AppendLine("th { background-color: #8B5E3C; color: white; }");
            builder.AppendLine("tr:nth-child(even) { background-color: #f2f2f2; }");
            builder.AppendLine("</style>");
            builder.AppendLine("</head>");
            builder.AppendLine("<body>");
            builder.AppendLine("<h1>Отчёт по продажам</h1>");
            builder.AppendLine($"<p>Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");
            builder.AppendLine("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\">");

            builder.AppendLine("<thead>");
            builder.AppendLine("<tr>");
            builder.AppendLine("<th>ID</th>");
            builder.AppendLine("<th>Клиент</th>");
            builder.AppendLine("<th>Сотрудник</th>");
            builder.AppendLine("<th>Дата</th>");
            builder.AppendLine("<th>Сумма</th>");
            builder.AppendLine("</tr>");
            builder.AppendLine("</thead>");

            builder.AppendLine("<tbody>");
            foreach (var item in data)
            {
                builder.AppendLine("<tr>");
                builder.AppendLine($"<td style=\"text-align: center;\">{item.Id}</td>");

                string clientName = item.Client != null ? $"{item.Client.Surname} {item.Client.Name}" : "Не указан";
                builder.AppendLine($"<td>{clientName}</td>");

                string employeeName = item.Employee != null ? $"{item.Employee.Surname} {item.Employee.Name}" : "Не указан";
                builder.AppendLine($"<td>{employeeName}</td>");

                builder.AppendLine($"<td>{item.Date:dd.MM.yyyy}</td>");
                builder.AppendLine($"<td style=\"text-align: right;\">{item.Sum:F2} руб.</td>");
                builder.AppendLine("</tr>");
            }
            builder.AppendLine("</tbody>");
            builder.AppendLine("</table>");
            builder.AppendLine("</body>");
            builder.AppendLine("</html>");

            await System.IO.File.WriteAllTextAsync(outputPath, builder.ToString(), Encoding.UTF8);
            return true;
        }

        public async Task<IActionResult> Report()
        {
            var sales = await _repository.GetAllAsync();
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports", $"sales_report_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            var result = await ConvertData((List<Domain.Sale>)sales, outputPath);
            if (result)
            {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(outputPath);
                return File(fileBytes, "text/html", Path.GetFileName(outputPath));
            }

            return BadRequest("Ошибка при создании отчёта");
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

            var client = await _clientRepository.GetByIdAsync(new Domain.Client { Id = saleToEdit.ClientId });
            var employee = await _employeeRepository.GetByIdAsync(new Domain.Employee { Id = saleToEdit.EmployeeId });

            string clientName = $"{client.Surname} {client.Name}";
            string employeeName = $"{employee.Surname} {employee.Name}";

            await _hubContext.Clients.All.SendAsync("SaleUpdated",
                saleToEdit.Id,
                clientName,    
                employeeName,    
                saleToEdit.Date,
                saleToEdit.Sum);

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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var sale = await _repository.GetByIntIdAsync(id);
            
            var saleAndExemplars = await _saleAndExemplarRepository.GetBySaleIdAsync(id);
            var exemplars = saleAndExemplars.Select(se => se.Exemplar).ToList();

            ViewBag.Sale = sale;
            ViewBag.Exemplars = exemplars;
            ViewBag.ExemplarsCount = exemplars.Count;
            ViewBag.TotalSum = sale.Sum;

            return View();
        }

    }
}
