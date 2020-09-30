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
    public class StartGameController : ControllerBase
    {        
        private readonly ILogger<StartGameController> _logger;

        public StartGameController(ILogger<StartGameController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public GameInfoDetailed Post(Guid id, [FromBody] Player player, string passwordPhrase = "")
        {
            if(player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            player.Validate();

            var game = LobbyList.AvailableGames?.FirstOrDefault(o => o.Id == id);

            if (game == null)
            {
                throw new NullReferenceException(nameof(game));
            }
            if(!string.IsNullOrWhiteSpace(passwordPhrase) && string.CompareOrdinal(game.PasswordPhrase, passwordPhrase) != 0)
            {
                throw new InvalidCredentialException();
            }
            if(string.CompareOrdinal(game.Host.BlowFishKey, player.BlowFishKey) == 0)
            {
                // we need to create
                game.Start();
                return game.GetInfoDetailed(player.BlowFishKey, player.Name);
            }
            else
            {
                throw new InvalidCredentialException();
            }
        }
    }
}
