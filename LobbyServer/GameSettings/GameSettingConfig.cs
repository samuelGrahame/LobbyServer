using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LobbyServer.GameSettings
{
    public partial class GameSettingConfig
    {
        [JsonProperty("players")]
        public Player[] Players { get; set; }

        [JsonProperty("game")]
        public Game Game { get; set; }

        [JsonProperty("gameInfo")]
        public GameInfo GameInfo { get; set; }
    }

    public partial class Game
    {
        [JsonProperty("map")]
        public long Map { get; set; }

        [JsonProperty("dataPackage")]
        public string DataPackage { get; set; }
    }

    public partial class GameInfo
    {
        [JsonProperty("MANACOSTS_ENABLED")]
        public bool ManacostsEnabled { get; set; }

        [JsonProperty("COOLDOWNS_ENABLED")]
        public bool CooldownsEnabled { get; set; }

        [JsonProperty("CHEATS_ENABLED")]
        public bool CheatsEnabled { get; set; }

        [JsonProperty("MINION_SPAWNS_ENABLED")]
        public bool MinionSpawnsEnabled { get; set; }

        [JsonProperty("CONTENT_PATH")]
        public string ContentPath { get; set; }

        [JsonProperty("IS_DAMAGE_TEXT_GLOBAL")]
        public bool IsDamageTextGlobal { get; set; }
    }

    public partial class Player
    {
        [JsonProperty("playerId")]
        public long PlayerId { get; set; }

        [JsonProperty("blowfishKey")]
        public string BlowfishKey { get; set; }

        [JsonProperty("rank")]
        public string Rank { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("champion")]
        public string Champion { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("skin")]
        public long Skin { get; set; }

        [JsonProperty("summoner1")]
        public string Summoner1 { get; set; }

        [JsonProperty("summoner2")]
        public string Summoner2 { get; set; }

        [JsonProperty("ribbon")]
        public long Ribbon { get; set; }

        [JsonProperty("icon")]
        public long Icon { get; set; }

        [JsonProperty("runes")]
        public Dictionary<string, long> Runes { get; set; }
    }
}
