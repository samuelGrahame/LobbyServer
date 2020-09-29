using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyServer
{
    public class Player
    {        
        public string Name { get; set; }
        public string Champion { get; set; }
        public Team Team { get; set; }
        public string BlowFishKey { get; set; }

        public int GamePlayerId;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ArgumentNullException(nameof(Name));
            }

            if (string.IsNullOrWhiteSpace(Champion))
            {
                throw new ArgumentNullException(nameof(Champion));
            }

            if (!Startup.AvailableChampions.Any(o => string.CompareOrdinal(Champion, o) == 0))
            {
                throw new Exception($"{nameof(Champion)} is not in the list of available champions");
            }

            if (string.IsNullOrWhiteSpace(BlowFishKey))
            {
                throw new ArgumentNullException(nameof(BlowFishKey));
            }
        }
    }
}
