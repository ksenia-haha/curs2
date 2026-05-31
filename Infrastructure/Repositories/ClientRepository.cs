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
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        //Добавление

        public async Task CreateAsync(Client item)
        {
            ValidateClient(item);
            var exists = await _context.Clients.AnyAsync(c => c.Id == item.Id);
            if (exists)
            {
                throw new ClientException("Такой клиент уже есть");
            }

            var phoneExists = await _context.Clients.AnyAsync(c => c.PhoneNumber == item.PhoneNumber);
            if (phoneExists)
            {
                throw new ClientException("Клиент с таким номером телефона уже есть");
            }

            await _context.Clients.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        private void ValidateClient(Client item)
        {
            if (item == null)
                throw new ArgumentNullException ("Клиент не может быть null");

            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ClientException("Имя не может быть пустым");

            if (string.IsNullOrWhiteSpace(item.Surname))
                throw new ClientException("Фамилия не может быть пустой");

            if (string.IsNullOrWhiteSpace(item.Patronymic))
                throw new ClientException("Отчество не может быть пустым");

            if (string.IsNullOrWhiteSpace(item.PhoneNumber))
                throw new ClientException("Номер телефона не может быть пустым");
        }

        //удаление

        public async Task DeleteAsync(Client item)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == item.Id)
                ?? throw new ClientException("Такого клиента нет в БД");
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

        }

        //Получение всех записей

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients.AsNoTracking().ToListAsync();
        }

        //поиск по чему-то

        public async Task<Client> GetByIdAsync(Client item)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == item.Id)
                ?? throw new ClientException("Такого клиента нет в БД");
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == item.Id);
        }


        public async Task<Client> GetByPhone(Client item)
        {
            ValidateClient(item);

            if (string.IsNullOrWhiteSpace(item.PhoneNumber))
                throw new ClientException("Номер телефона не указан");

            return await _context.Clients
                //.AsNoTracking()
                .FirstOrDefaultAsync(c => c.PhoneNumber == item.PhoneNumber)
                ?? throw new ClientException("Клиент с таким номером телефона не найден");
        }

        //Обновление

        public async Task UpdateAsync(Client item)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == item.Id)
                ?? throw new ClientException("Такого клиента нет в БД");

            client.Surname = item.Surname;
            client.Name = item.Name;
            client.Patronymic = item.Patronymic;
            client.PhoneNumber = item.PhoneNumber;

            await _context.SaveChangesAsync();
        }

        public Task GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
