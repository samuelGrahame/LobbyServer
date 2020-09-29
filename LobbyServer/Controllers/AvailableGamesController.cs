using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LobbyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AvailableGamesController : ControllerBase
    {        
        private readonly ILogger<AvailableGamesController> _logger;

        public AvailableGamesController(ILogger<AvailableGamesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<GameInfo> Get()
        {            
            if(LobbyList.AvailableGames != null)
            {
                foreach (var item in LobbyList.AvailableGames)
                {
                    yield return item.GetInfo();
                }
            }
            
        }
    }
}
