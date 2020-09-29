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
    }
}
