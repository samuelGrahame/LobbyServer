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
    public class JoinGameController : ControllerBase
    {        
        private readonly ILogger<JoinGameController> _logger;

        public JoinGameController(ILogger<JoinGameController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public GameInfoDetailed Post(Guid id, [FromBody]Player player, string passwordPhrase  = "")
        {
            var game = LobbyList.AvailableGames?.First(o => o.Id == id);

            if(game == null)
            {
                throw new NullReferenceException(nameof(game));
            }            
            game.Join(player, passwordPhrase);

            return game.GetInfoDetailed();
        }
    }
}
