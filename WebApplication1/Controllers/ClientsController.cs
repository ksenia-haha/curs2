using System.Text;
using Domain;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Data;
using WebApplication1.Hubs;
using WebApplication1.Hubs;
using WebApplication1.Models;
using Client = WebApplication1.Models.Client;


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

        public async Task<bool> ConvertData(List<Domain.Client> data, string outputPath)
        {
            if (!data.Any()) return false;
            if (string.IsNullOrWhiteSpace(outputPath) || !outputPath.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                return false;

            var builder = new StringBuilder();
            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine("<html>");
            builder.AppendLine("<head>");
            builder.AppendLine("<meta charset=\"UTF-8\">");
            builder.AppendLine("<title>Отчёт по клиентам</title>");
            builder.AppendLine("<style>");
            builder.AppendLine("table { border-collapse: collapse; width: 100%; }");
            builder.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            builder.AppendLine("th { background-color: #8B5E3C; color: white; }");
            builder.AppendLine("tr:nth-child(even) { background-color: #f2f2f2; }");
            builder.AppendLine("</style>");
            builder.AppendLine("</head>");
            builder.AppendLine("<body>");
            builder.AppendLine("<h1>Отчёт по клиентам</h1>");
            builder.AppendLine($"<p>Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");
            builder.AppendLine($"<p>Всего клиентов: {data.Count}</p>");
            builder.AppendLine("<table border=\"1\" cellpadding=\"5\" cellspacing=\"0\">");

            builder.AppendLine("<thead>");
            builder.AppendLine("<tr>");
            builder.AppendLine("<th>ID</th>");
            builder.AppendLine("<th>Фамилия</th>");
            builder.AppendLine("<th>Имя</th>");
            builder.AppendLine("<th>Отчество</th>");
            builder.AppendLine("<th>Номер телефона</th>");
            builder.AppendLine("</tr>");
            builder.AppendLine("</thead>");

            builder.AppendLine("<tbody>");

            var groupedData = data.OrderBy(c => c.Surname);

            foreach (var item in groupedData)
            {
                builder.AppendLine("<tr>");
                builder.AppendLine($"<td style=\"text-align: center;\">{item.Id}</td>");
                builder.AppendLine($"<td>{item.Surname ?? "—"}</td>");
                builder.AppendLine($"<td>{item.Name ?? "—"}</td>");
                builder.AppendLine($"<td>{item.Patronymic ?? "—"}</td>");
                builder.AppendLine($"<td>{item.PhoneNumber ?? "—"}</td>");

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
            var clients = await _repository.GetAllAsync();
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports", $"clients_report_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            var result = await ConvertData((List<Domain.Client>)clients, outputPath);
            if (result)
            {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(outputPath);
                return File(fileBytes, "text/html", Path.GetFileName(outputPath));
            }

            return BadRequest("Ошибка при создании отчёта");
        }

    }
}
