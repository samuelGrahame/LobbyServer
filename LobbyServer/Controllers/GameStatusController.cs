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

        [HttpPost]
        public GameInfoDetailed Post(Guid id, [FromBody]Player player, string passwordPhrase = "")
        {
            if(player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            player.Validate();

            var game = LobbyList.AvailableGames?.FirstOrDefault(o => o.Id == id);

            if(game == null)
            {
                game = LobbyList.ActiveGameProcess?.FirstOrDefault(o => o.Game.Id == id)?.Game;
            }
            if (game == null)
            {

                throw new NullReferenceException(nameof(game));
            }
            if(string.IsNullOrWhiteSpace(game.PasswordPhrase) || string.CompareOrdinal(game.PasswordPhrase, passwordPhrase) == 0)
            {                                
                return game.GetInfoDetailed(player.BlowFishKey, player.Name);
            }
            else
            {
                throw new InvalidCredentialException();
            }
        }
    }
}
