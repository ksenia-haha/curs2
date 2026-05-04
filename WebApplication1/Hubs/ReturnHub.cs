using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class ReturnHub: Hub
    {
        public async Task NotifyReturnAdded(int returnId, int employeeId, int clientId, int exemplarId, string status)
        {
            await Clients.All.SendAsync("ReturnCreated", returnId, employeeId, clientId, exemplarId, status);
        }

        public async Task NotifyReturnUpdated(int returnId, int employeeId, int clientId, int exemplarId, string status)
        {
            await Clients.All.SendAsync("ReturnUpdated", returnId, employeeId, clientId, exemplarId, status);
        }

        public async Task NotifyReturnDeleted(int returnId)
        {
            await Clients.All.SendAsync("ReturnDeleted", returnId);
        }
    }
}
