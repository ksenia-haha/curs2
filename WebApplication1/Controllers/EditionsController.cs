using Domain;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EditionsController : Controller
    {

        private readonly IEditionRepository _repository;
        private readonly IExemplarRepository _exemplarRepository;
        private readonly ISaleRepository _saleRepository;

        public EditionsController(IEditionRepository repository, IExemplarRepository exemplarRepository, ISaleRepository saleRepository)
        {
            _repository = repository;
            _exemplarRepository = exemplarRepository;
        }

        public async Task<IActionResult> Index()
        {
            var editions = await _repository.GetAllAsync();
            return View(editions);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Models.Edition edition)
        {
            await _repository.CreateAsync(MapEdition(edition));

            return RedirectToAction(nameof(Index));
        }
        private Domain.Edition MapEdition(WebApplication1.Models.Edition edition)
        {
            var e = new Domain.Edition();
            e.ISBN = edition.ISBN;
            e.Author = edition.Author;
            e.Publisher = edition.Publisher;
            e.Year = edition.Year;
            e.Genre = edition.Genre;
            //e.CountShop = edition.CountShop;
            //e.CountSklad = edition.CountSklad;
            e.Name = edition.Name;

            return e;
        }
        public IActionResult Create()
        {
            return View();
        }


        public async Task<IActionResult> Delete(Domain.DTO.EditionDTO edition)
        {
            var editionToDelete = new Domain.Edition { ISBN = edition.Id };

            await _repository.DeleteAsync(editionToDelete);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Domain.DTO.EditionDTO edition)
        {
            var editionToEdit = new Domain.Edition
            {
                ISBN = edition.Id,
                Name = edition.Name,
                Author = edition.Author,
                Genre = edition.Genre,
                Publisher = edition.Publisher,
                Year = edition.Year,
            };

            if (edition.Id == null)
            {
                ModelState.AddModelError("", "ID издательства не указан");
                return View(edition);
            }

            await _repository.UpdateAsync(editionToEdit);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id) 
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var edition = await _repository.GetByIdAsync(new Domain.Edition { ISBN = id });

            if (edition == null)
            {
                return NotFound();
            }

            var editionModel = new WebApplication1.Models.Edition
            {
                ISBN = edition.ISBN,
                Name = edition.Name,
                Author = edition.Author,
                Genre = edition.Genre,
                Publisher = edition.Publisher,
                Year = edition.Year
            };

            return View(editionModel);
        }


        public async Task<IActionResult> Details(Domain.DTO.EditionDTO edition)
        {

            var editionToDetail = new Domain.Edition
            {
                ISBN = edition.Id,
                Name = edition.Name,
                Author = edition.Author,
                Genre = edition.Genre,
                Publisher = edition.Publisher,
                Year = edition.Year,
                //CountShop = edition.CountShop,
                //CountSklad = edition.CountSklad,
                Exemplars = edition.Exemplars,
            };

            var Detail = await _repository.GetByIdAsync(editionToDetail);
            var exemplars = await _exemplarRepository.GetByEditionId(editionToDetail.ISBN);

            ViewBag.Edition = Detail;
            ViewBag.Exemplars = exemplars;
            ViewBag.ExemplarsCount = exemplars.Count();


            return View();
        }
    } 


}
