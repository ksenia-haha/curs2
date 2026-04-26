using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebApplication1.Data;

namespace Infrastructure.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _context;

        public SaleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Sale item)
        {
            ValidateSale(item);
            var exists = await _context.Sales.AnyAsync(s => s.Id == item.Id);
            if (exists)
            {
                throw new SaleException("Такая продажа уже есть");
            }

            // Находим клиента
                var existingClient = await _context.Clients
                    .FirstOrDefaultAsync(s => s.Id == item.ClientId);

                if (existingClient != null)
                {
                    item.Client = existingClient; // Привязываем клиента
                }
                else
                {
                    // Если нет
                    throw new ClientException($"Клиент с ID {item.ClientId} не найден");
                }

            // Находим сотрудника 
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(s => s.Id == item.EmployeeId);

            if (existingEmployee != null)
            {
                item.Employee = existingEmployee; // Привязываем сотрудника
            }
            else
            {
                // Если нет
                throw new EmployeeException($"Сотрудник с ID {item.EmployeeId} не найден");
            }


            await _context.Sales.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        private void ValidateSale(Sale item)
        {
            if (item == null)
                throw new ArgumentNullException("Продажа не может быть null");

            if (string.IsNullOrWhiteSpace(item.Date))
                throw new SaleException("Поле 'Дата' не может быть пустым");

        }

        public async Task DeleteAsync(Sale item)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == item.Id)
                ?? throw new SaleException("Такой продажи нет в БД");
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _context.Sales.
                AsNoTracking()
                .Include(s => s.Client)  
                .Include(s => s.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetByClientId(int id)
        {

            var sales = await _context.Sales
                .AsNoTracking()
                .Include(s => s.Employee)
                .Where(s => s.Client.Id == id)
                .ToListAsync();


            return sales;
        }

        public async Task<IEnumerable<Sale>> GetByEmployeeId(int id)
        {
            var sales = await _context.Sales
                .AsNoTracking()
                .Include(s => s.Client)
                .Where(s => s.Employee.Id == id)
                .ToListAsync();


            return sales;
        }

        public async Task<Sale> GetByIdAsync(Sale item)
        {
            //ValidateSale(item);
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == item.Id)
                ?? throw new SaleException("Такой продажи нет в БД");
            return await _context.Sales.AsNoTracking().FirstOrDefaultAsync(s => s.Id == item.Id);
        }

        public async Task<Sale> GetByIntIdAsync(int id)
        {
            var sale = await _context.Sales
                .AsNoTracking()
                .Include(s => s.Client)
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
                throw new SaleException("Такой продажи нет в БД");

            return (sale);
        }

        public async Task UpdateAsync(Sale item)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == item.Id)
                ?? throw new SaleException("Такой продажи нет в БД");

            sale.Sum = item.Sum;
            //sale.Id = item.Id;
            sale.Sum = item.Sum;
            sale.Client = item.Client;
            sale.Employee = item.Employee;

            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
