using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class EmployeeHub: Hub
    {
        public async Task NotifyEmployeeAdded(int employeeId, string surname, string name, string patronymic, string position, string login, int level)
        {
            await Clients.All.SendAsync("EmployeeCreated", employeeId, surname, name, patronymic, position, login, level);
        }

        public async Task NotifyEmployeeUpdated(int employeeId, string surname, string name, string patronymic, string position, string login, int level)
        {
            await Clients.All.SendAsync("EmployeeUpdated", employeeId, surname, name, patronymic, position, login, level);
        }

        public async Task NotifyEmployeeDeleted(int employeeId)
        {
            await Clients.All.SendAsync("EmployeeDeleted", employeeId);
        }
    }
}
