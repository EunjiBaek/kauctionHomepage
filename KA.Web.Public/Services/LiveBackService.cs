using KA.Entities.Models.Live;
using KA.Repositories;
using KA.Web.Public.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NewtonJson = Newtonsoft.Json;

namespace KA.Web.Public.Services
{
    public class LiveBackService : BackgroundService
    {
        private readonly ILogger<LiveBackService> _logger;
        private readonly IHubContext<LiveHub, ILiveService> _liveHub;
        private readonly ILiveRepository _liveRepository;
        private IMemoryCache _cache;

        public LiveBackService(ILogger<LiveBackService> logger, IHubContext<LiveHub, ILiveService> liveHub, ILiveRepository liveRepository, IMemoryCache memoryCache) {
            _logger = logger;
            _liveHub = liveHub;
            _liveRepository = liveRepository;
            _cache = memoryCache;
        }   


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogError("The {Type} is stopping due to a host shutdown, queued items might not be processed anymore.", nameof(LiveBackService));

            return base.StopAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            LiveResult _jsonResult = new LiveResult();
            string _szData = string.Empty;
                        
            ConcurrentDictionary<string, LiveStat> _liveStats = new ConcurrentDictionary<string, LiveStat>();

            //현재 진행중 LIVE 정보
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var groups = from g in LiveHub.LiveGroups.Values
                                 group g by g.auc_num;

                    foreach (var grp in groups)
                    {
                        try
                        {
                            _logger.LogInformation($"Worker({grp.Key}) running at: {DateTime.Now}");

                            initLiveResult(ref _jsonResult);


                            var _ds = _liveRepository.GetLiveData(auc_kind: 1, auc_num: Convert.ToInt32(grp.Key));
                            _jsonResult.data = _ds;
                            
                            if (_ds.Tables.Count > 0 && _ds.Tables[0].Rows.Count > 0)
                            {
                                LiveResult liveResult = new LiveResult();
                                initLiveResult(ref liveResult);
                                liveResult.data = _liveRepository.GetDataSet("usp_Live_Auc_Bidding_Hst_SelectForPage", new
                                {
                                    auc_kind = 1
                                        ,
                                    auc_num = Convert.ToInt32(grp.Key)
                                        ,
                                    lot_num = Convert.ToInt32(_ds.Tables[0].Rows[0]["lot_num"])
                                        ,
                                    page_no = 1
                                        ,
                                    page_size = 5
                                    ,
                                    is_change = false
                                });
                                _szData = NewtonJson.JsonConvert.SerializeObject(liveResult, NewtonJson.Formatting.None, new NewtonJson.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                                await _liveHub.Clients.Group(grp.Key).CurrentBidHst(_szData);
                                await _liveHub.Clients.Group("admin").CurrentBidHst(_szData);
                            }

                        }
                        catch (Exception ex)
                        {
                            _jsonResult.resultCd = "99";
                            _jsonResult.resultMsg = ex.Message;
                        }
                        finally
                        {
                            _szData = NewtonJson.JsonConvert.SerializeObject(_jsonResult, NewtonJson.Formatting.None, new NewtonJson.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
                            await _liveHub.Clients.Group(grp.Key).CurrentLotStat(_szData);
                            await _liveHub.Clients.Group("admin").CurrentLotStat(_szData);
                        }
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                    await _liveHub.Clients.Group("admin").ErrorNotification(ex.Message);
                }
                finally
                {
                    await Task.Delay(700);
                }
            }
        }


        private void initLiveResult(ref LiveResult liveResult) {
            liveResult = null;
            liveResult = new LiveResult() {
                resultCd = "00",
                resultMsg = "성공",
                data = null
            };
        }
    }
}
