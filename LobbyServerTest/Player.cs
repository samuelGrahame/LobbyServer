using System;
using System.Collections.Generic;
using System.Text;

namespace LobbyServerTest
{
    public class Player
    {
        public string Name { get; set; }
        public string Champion { get; set; }
        public Team Team { get; set; }
        public string BlowFishKey { get; set; }

        public int GamePlayerId;
    }
}
