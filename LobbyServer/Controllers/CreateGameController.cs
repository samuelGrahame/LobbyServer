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

        [HttpGet]
        public GameInfoDetailed Get(Player player, string passwordPhrase)
        {
            // reserve a port.
            var listOfAllPorts = LobbyList.AvailableGames.Select(o => o.Port)?.ToList();
            listOfAllPorts.AddRange(LobbyList.ActiveGameProcess.Select(o => o.Port)?.ToList());


            
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
