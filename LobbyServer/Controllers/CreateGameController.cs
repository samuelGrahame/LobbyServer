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
    public class CreateGameController : ControllerBase
    {        
        private readonly ILogger<CreateGameController> _logger;

        public CreateGameController(ILogger<CreateGameController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public GameInfoDetailed Post([FromBody]Player player, string passwordPhrase = "", int maxPlayerPerTeam = 5)
        {
            if(player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            player.Validate();

            // reserve a port.
            var listOfAllPorts = LobbyList.AvailableGames.Select(o => o.Port)?.ToList();
            listOfAllPorts.AddRange(LobbyList.ActiveGameProcess.Select(o => o.Port)?.ToList());

            var freePorts = Startup.AvailablePorts.Where(o => !listOfAllPorts.Contains(o))?.ToList();
            
            if(freePorts?.Count == 0)
            {
                throw new Exception("No Available ports to play a game");
            }

            var portToUse = freePorts.FirstOrDefault();
            var game = new Game()
            {
                Host = player,
                Id = Guid.NewGuid(),
                Port = portToUse,
                PasswordPhrase = passwordPhrase,
                MaxPlayersPerTeam = maxPlayerPerTeam
            };

            game.Join(player, passwordPhrase);

            LobbyList.AvailableGames.Add(game);
            
            return game.GetInfoDetailed();
        }
    }
}
