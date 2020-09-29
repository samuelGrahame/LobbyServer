using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyServer
{
    public class GameServerProcess
    {
        public short Port { get; set; }        
        public Process Process { get; set; }
        public Game Game { get; set; }
    }
}
