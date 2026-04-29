using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class ClientHub : Hub
    {
        public async Task NotifyClientAdded(int clientId, string surname, string name, string patronymic, string phoneNumber, string address)
        {
            await Clients.All.SendAsync("ClientCreated", clientId, surname, name, patronymic, phoneNumber, address);
        }

        public async Task NotifyClientUpdated(int clientId, string surname, string name, string patronymic, string phoneNumber, string address)
        {
            await Clients.All.SendAsync("ClientUpdated", clientId, surname, name, patronymic, phoneNumber, address);
        }

        public async Task NotifyClientDeleted(int clientId)
        {
            await Clients.All.SendAsync("ClientDeleted", clientId);
        }
    }
}
