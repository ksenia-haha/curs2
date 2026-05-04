using Microsoft.AspNetCore.SignalR;

namespace WebApplication1.Hubs
{
    public class EditionHub: Hub
    {
        public async Task NotifyEditionAdded(int isbn, string name, string author, string genre, string publisher, int year)
        {
            await Clients.All.SendAsync("EditionCreated", isbn, name, author, genre, publisher, year);
        }

        public async Task NotifyEditionUpdated(int isbn, string name, string author, string genre, string publisher, int year)
        {
            await Clients.All.SendAsync("EditionUpdated", isbn, name, author, genre, publisher, year);
        }

        public async Task NotifyEditionDeleted(int isbn)
        {
            await Clients.All.SendAsync("EditionDeleted", isbn);
        }
    }
}
