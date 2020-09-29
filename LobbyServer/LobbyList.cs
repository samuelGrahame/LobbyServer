using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyServer
{
    public static class LobbyList
    {
        public static List<Game> AvailableGames { get; set; } = new List<Game>();
        public static List<GameServerProcess> ActiveGameProcess { get; set; } = new List<GameServerProcess>();
    }
}
