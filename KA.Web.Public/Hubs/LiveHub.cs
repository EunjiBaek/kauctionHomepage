using KA.Entities.Helpers;
using KA.Entities.Models.Live;
using KA.Repositories;
using KA.Web.Public.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using NewtonJson = Newtonsoft.Json;

namespace KA.Web.Public.Hubs
{
    public interface ILiveService
    {
        Task CurrentLotStat(string data);

        Task ErrorNotification(string data);

        Task ConnNotification(string data);

        Task GroupNotification(string data);

        Task GroupUsers(IEnumerable<LiveUser> liveUsers);

        Task CurrentBidHst(string data);

        Task CurrentUserInfo(string data);
    }


    public class LiveHub : Hub<ILiveService>
    {
        //접속자리스트
        public static ConcurrentDictionary<string, LiveUser> LiveGroups = new ConcurrentDictionary<string, LiveUser>();

        //접속자수
        public static int ConnectionCnt => LiveGroups.Count;
        
        private readonly ILogger<LiveHub> _logger;        
        private readonly ILiveRepository _liveRepository;        

        public LiveHub(ILogger<LiveHub> logger, ILiveRepository liveRepository) {
            _logger = logger;
            _liveRepository = liveRepository;            
        }
                

        public override async Task OnConnectedAsync() 
        {
            _logger.LogInformation("Connection");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            _logger.LogInformation("DisConnection");

            var disLiveuser = new LiveUser();            
            LiveGroups.TryRemove(Context.ConnectionId, out disLiveuser);
            if (disLiveuser is not null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, disLiveuser.auc_num);                
                disLiveuser.disconnection_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");                
                _liveRepository.InertUserLogData(disLiveuser);
            }
            await Clients.Group("admin").ConnNotification(ConnectionCnt.ToString());
            await Clients.Group("admin").GroupNotification(GetGroupList());
            await base.OnDisconnectedAsync(ex);
        }


        public async Task AddToGroup(string auc_num)
        {
            try {            
                ClaimsPrincipal principal = Context.GetHttpContext().User as ClaimsPrincipal;                
                int www_seq = int.TryParse((principal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault() ?? new Claim(ClaimTypes.NameIdentifier, "0")).Value, out int result) ? result : 0;

                if (www_seq == 0)
                    throw new Exception("ERR_LOGOUT");

                var newLiveuser = new LiveUser()
                {
                    auc_num         = auc_num
                    , www_seq       = www_seq
                    , session_key   = Context.GetHttpContext().Request.Cookies["K-Auction.Token"]
                    , client_val    = Context.GetHttpContext().Request.Headers["User-Agent"].ToString().ToLower()
                    , ip_val        = Context.GetHttpContext().Connection.RemoteIpAddress.ToString()
                    , cache_date    = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    , connection_id = Context.ConnectionId
                };                
                LiveGroups.AddOrUpdate(Context.ConnectionId, newLiveuser, (k, v) => v = newLiveuser);
                _liveRepository.InertUserLogData(newLiveuser);

                await Groups.AddToGroupAsync(Context.ConnectionId, auc_num);

                await Clients.Caller.CurrentUserInfo(NewtonJson.JsonConvert.SerializeObject(newLiveuser, NewtonJson.Formatting.None, new NewtonJson.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }));
                await Clients.Group("admin").ConnNotification(ConnectionCnt.ToString());
                await Clients.Group("admin").GroupNotification(GetGroupList());
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                await Clients.Caller.ErrorNotification(ex.Message);
            }
        }


        public async Task AddToGroupAdmin(){
            await Groups.AddToGroupAsync(Context.ConnectionId, "admin");            
            await Clients.Group("admin").ConnNotification(ConnectionCnt.ToString());
            await Clients.Group("admin").GroupNotification(GetGroupList());
        }


        public async Task GetConnectionUsers(string auc_num) {

            var users = LiveGroups.Where(u => u.Value.auc_num.Equals(auc_num)).Select(u => u.Value);
            await Clients.Caller.GroupUsers(users);
        }


        public string GetGroupList() {

            var groups = from lg in LiveGroups.Values
                         group lg by lg.auc_num into g
                         select new
                         {
                             auc_num = g.Key
                            ,
                             user_cnt = g.Count()                             
                         };
            return NewtonJson.JsonConvert.SerializeObject(groups, NewtonJson.Formatting.None, new NewtonJson.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });            

        }


    }


}
