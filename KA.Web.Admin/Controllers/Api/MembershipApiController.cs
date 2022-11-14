using KA.Entities.Helpers;
using KA.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace KA.Web.Admin.Controllers.Api
{
    // [Route("api/[controller]")]
    [ApiController]
    public class MembershipApiController : ControllerBase
    {
        private readonly ICommonRepository _commonRepository;

        public MembershipApiController(ICommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
        }

        [HttpGet]
        [Route("api/Membership/GetClauseList")]
        public JObject GetClauseList()
        {
            return JsonHelper.GetApiResult("00", _commonRepository.GetCodeList("MEM_CLAUSE"));
        }
    }
}
