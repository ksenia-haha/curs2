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
    public class SaleAndExemplarRepository : ISaleAndExemplarRepository
    {
        private readonly AppDbContext _context;

        public SaleAndExemplarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(SaleAndExemplar item)
        {
            var exists = await _context.SalesAndExemplars.AnyAsync(se => se.Id == item.Id);
            if (exists)
            {
                throw new SaleAndExemplarException("Такая продажа уже есть");
            }

            // Находим продажу
                var existingSale = await _context.Sales
                    .FirstOrDefaultAsync(s => s.Id == item.SaleId);

                if (existingSale != null)
                {
                    item.Sale = existingSale; 
                }
                else
                {
                    throw new SaleException($"Продажа с ID {item.SaleId} не найдена");
                }

            // Находим экземпляр
            var existingExemplar = await _context.Exemplars
                .FirstOrDefaultAsync(e => e.Id == item.ExemplarId);

            if (existingExemplar != null)
            {
                item.Exemplar = existingExemplar; 
            }
            else
            {
                throw new ExemplarException($"Экземпляр с ID {item.ExemplarId} не найден");
            }


            await _context.SalesAndExemplars.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SaleAndExemplar item)
        {
            var sale = await _context.SalesAndExemplars.FirstOrDefaultAsync(se => se.Id == item.Id)
                ?? throw new SaleAndExemplarException("Такой продажи нет в БД");
            _context.SalesAndExemplars.Remove(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SaleAndExemplar>> GetAllAsync()
        {
            return await _context.SalesAndExemplars.
                AsNoTracking()
                .Include(se => se.Sale)  
                .Include(se => se.Exemplar)
                .ToListAsync();
        }

        public async Task<IEnumerable<SaleAndExemplar>> GetByExemplarId(int id)
        {
            var sales = await _context.SalesAndExemplars
                .AsNoTracking()
                .Include(se => se.Exemplar)
                .Where(se => se.Exemplar.Id == id)
                .ToListAsync();


            return sales;
        }

        public async Task<IEnumerable<SaleAndExemplar>> GetBySaleIdAsync(int id)
        {
            var sales = await _context.SalesAndExemplars
                .AsNoTracking()
                .Include(se => se.Sale)
                .Where(se => se.Sale.Id == id)
                .ToListAsync();


            return sales;
        }

        public async Task<SaleAndExemplar> GetByIdAsync(SaleAndExemplar item)
        {
            var sale = await _context.SalesAndExemplars.FirstOrDefaultAsync(se => se.Id == item.Id)
                ?? throw new SaleAndExemplarException("Такой продажи нет в БД");
            return await _context.SalesAndExemplars.AsNoTracking().FirstOrDefaultAsync(se => se.Id == item.Id);
        }

        public async Task UpdateAsync(SaleAndExemplar item)
        {
            var sale = await _context.SalesAndExemplars.FirstOrDefaultAsync(se => se.Id == item.Id)
                ?? throw new SaleAndExemplarException("Такой продажи нет в БД");

            sale.SaleId = item.SaleId;
            sale.ExemplarId = item.ExemplarId;
            sale.Sale = item.Sale;
            sale.Exemplar= item.Exemplar;

            await _context.SaveChangesAsync();
        }


    }
}
