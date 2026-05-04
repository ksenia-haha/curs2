using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class SaleHub: Hub
    {
        public async Task NotifySaleAdded(int saleId, string clientName, string employeeName,  string date, double sum)
        {
            await Clients.All.SendAsync("SaleCreated", saleId, clientName, employeeName, date, sum);
        }

        public async Task NotifySaleUpdated(int saleId, string clientName, string employeeName, string date, double sum)
        {
            await Clients.All.SendAsync("SaleUpdated", saleId, clientName, employeeName, date, sum);
        }

        public async Task NotifySaleDeleted(int saleId)
        {
            await Clients.All.SendAsync("SaleDeleted", saleId);
        }
    }
}
