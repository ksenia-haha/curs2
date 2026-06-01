using System.Diagnostics;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IExemplarRepository _exemplarRepository;
        private readonly IEditionRepository _editionRepository;
        private readonly IReturnRepository _returnRepository;

        public HomeController(IExemplarRepository exemplarRepository, ISaleRepository saleRepository, IEditionRepository editionRepository, IReturnRepository returnRepository)
        {
            _exemplarRepository = exemplarRepository;
            _saleRepository = saleRepository;
            _editionRepository = editionRepository;
            _returnRepository = returnRepository;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _saleRepository.GetAllAsync();
            var editions = await _editionRepository.GetAllAsync();
            var exemplars = await _exemplarRepository.GetAvailableExemplarsAsync();
            var currentDate = DateTime.Now;

            var monthSales = sales
                .Where(s => s.Date.Year == currentDate.Year && s.Date.Month == currentDate.Month)
                .ToList();

            var monthSalesCount = monthSales.Count();
            var monthRevenue = monthSales.Sum(s => s.Sum);

            var editionCount = editions.Count();
            var exemplarCount = exemplars.Count();

            ViewBag.MonthSales = monthSalesCount;
            ViewBag.MonthRevenue = monthRevenue;
            ViewBag.EditionsCount = editionCount;
            ViewBag.ExemplarCount = exemplarCount;


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
