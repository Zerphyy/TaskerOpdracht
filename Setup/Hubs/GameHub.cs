using Microsoft.AspNetCore.SignalR;

namespace Setup.Hubs
{
    public class GameHub : Hub
    {
        public async Task NotifyGameChanged()
        {
            await Clients.All.SendAsync("GameListChanged");
        }
        public async Task NotifyPlayerMoved(string gameState)
        {
            await Clients.All.SendAsync("PlayerMoved", gameState);
        }
    }
}
