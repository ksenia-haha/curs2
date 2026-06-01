using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class ReturnHub: Hub
    {
        public async Task NotifyReturnAdded(int returnId, string employeeName, string clientName, string exemplarIsbn, string status, string reason)
        {
            await Clients.All.SendAsync("ReturnCreated", returnId, employeeName, clientName, exemplarIsbn, status, reason);
        }

        public async Task NotifyReturnUpdated(int returnId, string employeeName, string clientName, string exemplarIsbn, string status, string reason)
        {
            await Clients.All.SendAsync("ReturnUpdated", returnId, employeeName, clientName, exemplarIsbn, status, reason);
        }

        public async Task NotifyReturnDeleted(int returnId)
        {
            await Clients.All.SendAsync("ReturnDeleted", returnId);
        }
    }
}
