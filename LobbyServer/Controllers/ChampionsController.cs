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
    public class ChampionsController : ControllerBase
    {        
        private readonly ILogger<ChampionsController> _logger;

        public ChampionsController(ILogger<ChampionsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return Startup.AvailableChampions;
        }
    }
}
