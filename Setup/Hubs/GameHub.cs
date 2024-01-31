using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Setup.Hubs
{
    public class GameHub : Hub
    {
        public async Task NotifyGameChanged()
        {
            await Clients.All.SendAsync("GameChanged");
        }
    }
}
