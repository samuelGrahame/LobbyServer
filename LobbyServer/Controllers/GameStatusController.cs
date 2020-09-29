using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LobbyServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameStatusController : ControllerBase
    {        
        private readonly ILogger<GameStatusController> _logger;

        public GameStatusController(ILogger<GameStatusController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public GameInfoDetailed Get(Guid id, Player player, string passwordPhrase)
        {
            if(player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            player.Validate();

            var game = LobbyList.AvailableGames?.First(o => o.Id == id);

            if (game == null)
            {
                throw new NullReferenceException(nameof(game));
            }
            if(string.CompareOrdinal(game.Host.BlowFishKey, player.BlowFishKey) == 0)
            {
                // we need to create
                game.Start();
                return game.GetInfoDetailed();
            }
            else
            {
                throw new InvalidCredentialException();
            }
        }
    }
}
