using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using static Domain.Exemplar;

namespace Infrastructure.Repositories
{
    public class ExemplarRepository : IExemplarRepository
    {
        private readonly AppDbContext _context;

        public ExemplarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Exemplar item)
        {
            ValidateExemplar(item);
            var exists = await _context.Exemplars.AnyAsync(e => e.Id == item.Id);
            if (exists)
            {
                throw new ExemplarException("Такой экземпляр уже есть");
            }

            if (!string.IsNullOrEmpty(item.EditionISBN))
            {
                var existingEdition = await _context.Editions
                    .FirstOrDefaultAsync(e => e.ISBN == item.EditionISBN);

                if (existingEdition != null)
                {
                    item.Edition = existingEdition; 
                }
                else
                {
                    throw new ExemplarException($"Издание с ISBN {item.EditionISBN} не найдено");
                }
            }

            await _context.Exemplars.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        private void ValidateExemplar(Exemplar item)
        {
            if (item == null)
                throw new ArgumentNullException("Экземпляр не может быть null");
            
            if (string.IsNullOrWhiteSpace(item.Section))
                throw new ExemplarException("Секция не может быть пустой");

            if (string.IsNullOrWhiteSpace(item.Shelf))
                throw new ExemplarException("Стеллаж не может быть пустым");

        }

        public async Task DeleteAsync(Exemplar item)
        {
            var exemplar = await _context.Exemplars.FirstOrDefaultAsync(e => e.Id == item.Id)
                ?? throw new ExemplarException("Такого экземпляра нет в БД");
            _context.Exemplars.Remove(exemplar);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Exemplar>> GetAllAsync()
        {
            return await _context.Exemplars.AsNoTracking().ToListAsync();
        }

        public async Task<Exemplar> GetByIdAsync(Exemplar item)
        {
            //ValidateExemplar(item);
            var exemplar = await _context.Exemplars.FirstOrDefaultAsync(e => e.Id == item.Id)
                ?? throw new ExemplarException("Такого экземпляра нет в БД");
            return await _context.Exemplars.AsNoTracking().FirstOrDefaultAsync(e => e.Id == item.Id);
        }

        public async Task<Exemplar> GetByIntIdAsync(int id)
        {
            var exemplar = await _context.Exemplars.FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new ExemplarException("Такого экземпляра нет в БД");
            return await _context.Exemplars.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Exemplar>> GetByEditionId(string id)
        {
            var exemplars = await _context.Exemplars
                .AsNoTracking()
                .Include(e => e.Edition)
                .Where(e => e.Edition.ISBN == id)
                .ToListAsync();

            return exemplars;
        }

        public async Task UpdateAsync(Exemplar item)
        {
            var exemplar = await _context.Exemplars.FirstOrDefaultAsync(e => e.Id == item.Id)
                ?? throw new ExemplarException("Такого экземпляра нет в БД");

            exemplar.Edition = item.Edition;
            exemplar.Id = item.Id;  
            exemplar.Shelf = item.Shelf;
            exemplar.Section = item.Section;
            exemplar.Status = item.Status;
            exemplar.Price = item.Price;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Exemplar>> GetAvailableExemplarsAsync()
        {
            return await _context.Exemplars
                .AsNoTracking()
                .Include(e => e.Edition)
                .Where(e => e.Status == ExemplarStatus.InStock)
                .ToListAsync();
        }

        public async Task UpdateStatusSoldAsynс(int exemplarId)
        {
            var exemplar = await _context.Exemplars.FirstOrDefaultAsync (e  => e.Id == exemplarId);
            exemplar.Status = ExemplarStatus.Sold;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Exemplar>> GetSoldExemplarsAsync()
        {
            return await _context.Exemplars
                .AsNoTracking()
                .Include(e => e.Edition)
                .Where(e => e.Status == ExemplarStatus.Sold)
                .ToListAsync();
        }

    }
}
