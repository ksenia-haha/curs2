using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class ExemplarHub: Hub
    {
        public async Task NotifyExemplarAdded(int exemplarId, string editionIsbn, string section, string shelf, double price, string status)
        {
            await Clients.All.SendAsync("ExemplarCreated", exemplarId, editionIsbn, section, shelf, price, status);
        }

        public async Task NotifyExemplarUpdated(int exemplarId, string editionIsbn, string section, string shelf, double price, string status)
        {
            await Clients.All.SendAsync("ExemplarUpdated", exemplarId, editionIsbn, section, shelf, price, status);
        }

        public async Task NotifyExemplarDeleted(int exemplarId)
        {
            await Clients.All.SendAsync("ExemplarDeleted", exemplarId);
        }
    }
}
