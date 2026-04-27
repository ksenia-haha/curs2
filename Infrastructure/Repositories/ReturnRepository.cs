using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace Infrastructure.Repositories
{
    public class ReturnRepository : IReturnRepository
    {
        private readonly AppDbContext _context;

        public ReturnRepository(AppDbContext context)
        {
            _context = context;
        }

        //Добавление

        public async Task CreateAsync(Return item)
        {
            var exists = await _context.Returns.AnyAsync(c => c.Id == item.Id);
            if (exists)
            {
                throw new ReturnException("Такой возврат уже есть");
            }

            await _context.Returns.AddAsync(item);
            await _context.SaveChangesAsync();
        }


        //удаление

        public async Task DeleteAsync(Return item)
        {
            var toReturn = await _context.Returns.FirstOrDefaultAsync(r => r.Id == item.Id)
                ?? throw new ReturnException("Такого возврата нет в БД");
            _context.Returns.Remove(toReturn);
            await _context.SaveChangesAsync();

        }

        //Получение всех записей

        public async Task<IEnumerable<Return>> GetAllAsync()
        {
            return await _context.Returns
                .AsNoTracking()
                .Include(r => r.Client)
                .Include(r => r.Employee)
                .Include(r => r.Exemplar)
                .ToListAsync();
        }

        //поиск по чему-то

        public async Task<Return> GetByIdAsync(Return item)
        {
            var toReturn = await _context.Returns.FirstOrDefaultAsync(r => r.Id == item.Id)
                ?? throw new ReturnException("Такого возврата нет в БД");
            return await _context.Returns
                //.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == item.Id);
        }


        //Обновление

        public async Task UpdateAsync(Return item)
        {
            var toReturn = await _context.Returns.FirstOrDefaultAsync(r => r.Id == item.Id)
                ?? throw new ReturnException("Такого возврата нет в БД");

            // var exemplar = await _context.Exemplars
            //.FirstOrDefaultAsync(e => e.Id == toReturn.ExemplarId);

            await _context.Returns.Where(e => e.Id == item.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.ClientId, item.ClientId)
                .SetProperty(i => i.ExemplarId, item.ExemplarId)
                .SetProperty(i => i.Status, item.Status)
                .SetProperty(i => i.EmployeeId, item.EmployeeId)
                .SetProperty(i => i.ClientId, item.ClientId));
                //.SetProperty(i => i.Sale,item.Sale));

            //toReturn.EmployeeId = item.EmployeeId;
            //toReturn.ClientId = item.ClientId;
            ////toReturn.SaleId = item.SaleId;
            //toReturn.ExemplarId = item.ExemplarId;
            //toReturn.Status = item.Status;

            //toReturn.Employee = item.Employee;
            //toReturn.Client = item.Client;
            //toReturn.Sale = item.Sale;
            //toReturn.Exemplar = item.Exemplar;
            var exemplar = await _context.Exemplars
                .FirstOrDefaultAsync(e => e.Id == toReturn.ExemplarId);
            if (exemplar.Status == Exemplar.ExemplarStatus.Sold && toReturn.Status == Return.ReturnStatus.Yes)
            {

                exemplar.Status = Exemplar.ExemplarStatus.InStock;
                await _context.SaveChangesAsync();
            }

    }

        public Task GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }


        //Task<IEnumerable<Return>> IRepository<Return>.GetAllAsync()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
