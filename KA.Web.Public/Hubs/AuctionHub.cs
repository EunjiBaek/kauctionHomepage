using KA.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KA.Web.Public.Hubs
{
    public class AuctionHub : Hub
    {
        readonly private IAuctionRepository _auctionRepository;
        public AuctionHub(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // await Clients.Group(groupName).SendAsync("Send", $"[{Context.ConnectionId}] 사용자가 [{groupName}] 그룹에 입장하였습니다.");
            await Clients.Group(groupName).SendAsync("Send", Context.ConnectionId);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"[{Context.ConnectionId}] 사용자가 [{groupName}] 그룹에 떠났습니다.");
        }

        public Task SendMessageToGroup(string groupName, string connectionID = "")
        {
            // return Clients.Group(groupName).SendAsync("UpdateInfo", $"[{Context.ConnectionId}] 님이 신규 응찰하였습니다.");
            // return Clients.Group(groupName).SendAsync("UpdateInfo", $"신규 응찰이 있어 현재가가 업데이트 되었습니다.");
            return Clients.Group(groupName).SendAsync("UpdateInfo", "refresh", connectionID);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("Message", user, message);
        }
    }
}
