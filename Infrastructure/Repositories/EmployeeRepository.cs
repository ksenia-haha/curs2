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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Employee item)
        {
            ValidateEmployee(item);
            var exists = await _context.Employees.AnyAsync(e => e.Id == item.Id);
            if (exists)
            {
                throw new EmployeeException("Такой сотрудник уже есть");
            }

            await _context.Employees.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        private void ValidateEmployee(Employee item)
        {
            if(item == null)
                throw new ArgumentNullException("Клиент не может быть null");

            if (string.IsNullOrWhiteSpace(item.Name))
                throw new EmployeeException("Имя не может быть пустым");

            if (string.IsNullOrWhiteSpace(item.Surname))
                throw new EmployeeException("Фамилия не может быть пустой");

            if (string.IsNullOrWhiteSpace(item.Patronymic))
                throw new EmployeeException("Отчество не может быть пустым");

            if (string.IsNullOrWhiteSpace(item.Position))
                throw new EmployeeException("Позиция не может быть пустой");

            if (string.IsNullOrWhiteSpace(item.Login))
                throw new EmployeeException("Логин не может быть пустым");

            if (string.IsNullOrWhiteSpace(item.Password))
                throw new EmployeeException("Пароль не может быть пустым");
        }

        public async Task DeleteAsync(Employee item)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == item.Id)
                ?? throw new EmployeeException("Такого сотрудника нет в БД");
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees.AsNoTracking().ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(Employee item)
        {
            //ValidateEmployee(item);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == item.Id)
                ?? throw new EmployeeException("Такого сотрудника нет в БД");
            return await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == item.Id);
        }

        public async Task<IEnumerable<Employee>> GetByPosition(Employee item)
        {
            if (string.IsNullOrWhiteSpace(item.Position))
                throw new EmployeeException("Позиция не указано");

            var employees = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Position == item.Position)
                .ToListAsync();

            if (!employees.Any())
                throw new EmployeeException($"Сотрудники на такой позиции не найдены");

            return employees;
        }

        public async Task<IEnumerable<Employee>> SearchBySurname(Employee item)
        {
            if (string.IsNullOrWhiteSpace(item.Surname))
                throw new EmployeeException("Фамилия не указана");

            var employees = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Surname == item.Surname)
                .ToListAsync();

            if (!employees.Any())
                throw new EmployeeException($"Сотрудники с такой фамилией не найдены");

            return employees;
        }

        public async Task<Employee> Login(Employee item)
        {
            if (string.IsNullOrWhiteSpace(item.Login))
                throw new EmployeeException("Логин не указан");

            var employee = await _context.Employees.
                FirstOrDefaultAsync(e => e.Login == item.Login && e.Password == e.Password);

            if (employee == null)
                throw new EmployeeException($"Неверный логин или пароль");

            return employee;
        }

        public async Task UpdateAsync(Employee item)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == item.Id)
                ?? throw new EmployeeException("Такого издания нет в БД");

            employee.Surname = item.Surname;
            employee.Name = item.Name;
            employee.Patronymic = item.Patronymic;
            employee.Position = item.Position;
            employee.Login = item.Login;
            employee.Password = item.Password;
            employee.Level = item.Level;

            await _context.SaveChangesAsync();
        }
    }
}
