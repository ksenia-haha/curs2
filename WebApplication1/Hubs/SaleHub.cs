using Microsoft.AspNetCore.SignalR;

// Доделать

namespace WebApplication1.Hubs
{
    public class SaleHub: Hub
    {
        public async Task NotifySaleAdded(int returnId, int employeeId, int clientId, int exemplarId, string status)
        {
            await Clients.All.SendAsync("ReturnCreated", returnId, employeeId, clientId, exemplarId, status);
        }

        public async Task NotifySaleUpdated(int returnId, int employeeId, int clientId, int exemplarId, string status)
        {
            await Clients.All.SendAsync("ReturnUpdated", returnId, employeeId, clientId, exemplarId, status);
        }

        public async Task NotifySaleDeleted(int returnId)
        {
            await Clients.All.SendAsync("ReturnDeleted", returnId);
        }
    }
}
