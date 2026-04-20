using Domain;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;
using Exemplar = WebApplication1.Models.Exemplar;

namespace WebApplication1.Controllers
{
    public class ExemplarsController : Controller
    {

        private readonly IExemplarRepository _repository;
        private readonly IEditionRepository _editionRepository;

        public ExemplarsController(IExemplarRepository repository, IEditionRepository editionRepository)
        {
            _repository = repository;
            _editionRepository = editionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var exemplars = await _repository.GetAllAsync();
            return View(exemplars);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var editions = await _editionRepository.GetAllAsync();

            var editionList = editions.Select(e => new SelectListItem
            {
                Value = e.ISBN,
                Text = $"{e.ISBN} | {e.Name} | {e.Author}",
            });

            ViewBag.EditionList = editionList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Exemplar exemplar)
        {
            await _repository.CreateAsync(MapExemplar(exemplar));

            return RedirectToAction(nameof(Index));
        }
        private Domain.Exemplar MapExemplar(WebApplication1.Models.Exemplar exemplar)
        {
            var e = new Domain.Exemplar();
            e.Id = exemplar.Id;
            e.EditionISBN = exemplar.EditionISBN;
            e.Section = exemplar.Section;
            e.Shelf = exemplar.Shelf;
            e.Status = exemplar.Status;

            return e;
        }


        public async Task<IActionResult> Delete(Exemplar exemplar)
        {
            var exemplarToDelete = new Domain.Exemplar { Id = exemplar.Id };

            await _repository.DeleteAsync(exemplarToDelete);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Exemplar exemplar)
        {
            var exemplarToEdit = new Domain.Exemplar
            {
                Id = exemplar.Id,
                EditionISBN = exemplar.EditionISBN,
                Section = exemplar.Section,
                Shelf = exemplar.Shelf,
                Status = exemplar.Status,
            };

            if (exemplar.Id == null || exemplar.Id == 0)
            {
                ModelState.AddModelError("", "ID экземпляра не указан");
                return View(exemplar);
            }

            await _repository.UpdateAsync(exemplarToEdit);
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
            var exemplarDomain = await _repository.GetByIdAsync(new Domain.Exemplar { Id = id });

            var exemplarWeb = new WebApplication1.Models.Exemplar
            {
                Id = exemplarDomain.Id,
                EditionISBN = exemplarDomain.EditionISBN,
                Section = exemplarDomain.Section,
                Shelf = exemplarDomain.Shelf,
            };

            var editions = await _editionRepository.GetAllAsync();

            var editionList = editions.Select(e => new SelectListItem
            {
                Value = e.ISBN,
                Text = $"{e.ISBN} | {e.Name} | {e.Author}",
            });

            ViewBag.EditionList = editionList;

            return View(exemplarWeb);
        }

    }
}
