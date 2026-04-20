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

namespace Infrastructure.Repositories
{
    public class EditionRepository : IEditionRepository
    {
        private readonly AppDbContext _context;

        public EditionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Edition item)
        {
            ValidateEdition(item);
            var exists = await _context.Editions.AnyAsync(e => e.ISBN == item.ISBN);
            if (exists)
            {
                throw new EditionException("Такое издание уже есть");
            }

            await _context.Editions.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        private void ValidateEdition(Edition item)
        {
            if (item == null)
                throw new ArgumentNullException("Издание не может быть null");

            if (string.IsNullOrWhiteSpace(item.ISBN))
                throw new EditionException("ISBN не может быть пустым");

            if (string.IsNullOrWhiteSpace(item.Author))
                throw new EditionException("Поле 'Автор' не может быть пустой");

            if (string.IsNullOrWhiteSpace(item.Publisher))
                throw new EditionException("Поле 'Издательство' не может быть пустым");

            if (string.IsNullOrWhiteSpace(item.Name))
                throw new EditionException("Название не может быть пустым");

            if (string.IsNullOrWhiteSpace(item.Genre))
                throw new EditionException("Поле 'Жанр' не может быть пустым");
        }

        public async Task DeleteAsync(Edition item)
        {
            var edition = await _context.Editions.FirstOrDefaultAsync(e => e.ISBN == item.ISBN)
                ?? throw new EditionException("Такого издания нет в БД");
            _context.Editions.Remove(edition);
            await _context.SaveChangesAsync();

        }

        public async Task<IEnumerable<Edition>> GetAllAsync()
        {
            return await _context.Editions.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Edition>> GetByAuthor(Edition item)
        {
            if (string.IsNullOrWhiteSpace(item.Author))
                throw new EditionException("Автор не указан");

            var editions = await _context.Editions
                .AsNoTracking()
                .Where(e => e.Author == item.Author)
                .ToListAsync();

            if (!editions.Any())
                throw new EditionException($"Издания автора не найдены");

            return editions;
        }

        public async Task<Edition> GetByIdAsync(Edition item)
        {
            //ValidateEdition(item);
            var client = await _context.Editions.FirstOrDefaultAsync(e => e.ISBN == item.ISBN)
                ?? throw new EditionException("Такого издания нет в БД");
            return await _context.Editions.AsNoTracking().FirstOrDefaultAsync(e => e.ISBN == item.ISBN);
        }   

        public async Task<IEnumerable<Edition>> GetByTitle(Edition item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new EditionException("Название не указано");

            var editions = await _context.Editions
                .AsNoTracking()
                .Where(e => e.Name == item.Name)
                .ToListAsync();

            if (!editions.Any())
                throw new EditionException($"Издания с таким названием не найдены");

            return editions;
        }

        public async Task UpdateAsync(Edition item)
        {
            //ValidateEdition(item);
            var edition = await _context.Editions.FirstOrDefaultAsync(e => e.ISBN == item.ISBN)
                ?? throw new EditionException("Такого издания нет в БД");

            edition.ISBN = item.ISBN;
            edition.Author = item.Author;
            edition.Publisher = item.Publisher;
            edition.Year = item.Year;
            //edition.CountShop = item.CountShop;
            //edition.CountSklad = item.CountSklad;
            edition.Name = item.Name;
            edition.Genre = item.Genre;

            await _context.SaveChangesAsync();
        }
    }
}
