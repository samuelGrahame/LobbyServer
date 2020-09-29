using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace LobbyServer
{
    public class Game
    {
        public bool HasStarted { get; set; }
        public Guid Id { get; set; }
        public int MaxPlayersPerTeam { get; set; } = 5;

        public Player Host { get; set; }
        public string PasswordPhrase { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();

        public short Port { get; set; }

        public bool IsFull => IsRedTeamFull && IsRedTeamFull;
        public bool IsRedTeamFull => Players.Count(o => o.Team == Team.Red) >= MaxPlayersPerTeam;
        public bool IsBlueTeamFull => Players.Count(o => o.Team == Team.Blue) >= MaxPlayersPerTeam;

        public List<Player> RedTeam => Players.Where(o => o.Team == Team.Red)?.ToList();
        public List<Player> BlueTeam => Players.Where(o => o.Team == Team.Blue)?.ToList();

        /// <summary>
        /// Returns Exceptions if cant join.
        /// ArgumentNullException - if password is null/empty
        /// InvalidCredentialException - if invalid credentials/password
        /// Exception - generic - if team lobby is full
        /// </summary>
        /// <param name="player"></param>
        /// <param name="_passwordPhrase"></param>
        public void Join(Player player, string _passwordPhrase)
        {
            if(!string.IsNullOrWhiteSpace(PasswordPhrase))
            {
                if (string.IsNullOrWhiteSpace(_passwordPhrase))
                {
                    throw new ArgumentNullException(nameof(_passwordPhrase));
                }

                if(string.CompareOrdinal(_passwordPhrase, PasswordPhrase) != 0)
                {
                    throw new InvalidCredentialException(nameof(_passwordPhrase));
                }
            }

            player.Validate();

            var team = WhatTeamShouldIJoin();
            if(team == Team.None)
            {
                throw new Exception("game lobby is full");
            }

            if(player.Team == Team.None || player.Team == team)
            {
                player.Team = team;
            }
            else
            {
                if(Players.Count(o => o.Team == player.Team) >= MaxPlayersPerTeam)
                {
                    throw new Exception("game lobby is full");
                }
            }
            Players.Add(player);
        }

        public Team WhatTeamShouldIJoin()
        {
            if (!IsRedTeamFull)
                return Team.Red;

            if (!IsBlueTeamFull)
                return Team.Blue;

            return Team.None;
        }

        public GameInfo GetInfo()
        {
            return new GameInfo()
            {
                Id = Id,
                LobbyName = $"{Host.Name}'s lobby",
                TotalSlots = MaxPlayersPerTeam * 2,
                TotalAvailable = (MaxPlayersPerTeam * 2) - Players.Count
            };
        }

        public GameInfoDetailed GetInfoDetailed(string blowFishKey = "")
        {
            return new GameInfoDetailed()
            {
                Id = Id,
                LobbyName = $"{Host.Name}'s lobby",
                RedTeam = new List<PlayerInfo>(Players.Where(o => o.Team == Team.Red).
                    Select(o => new PlayerInfo() { Name   = o.Name, Champion = o.Champion })),
                BlueTeam = new List<PlayerInfo>(Players.Where(o => o.Team == Team.Blue).
                    Select(o => new PlayerInfo() { Name = o.Name, Champion = o.Champion })),
                Port = HasStarted ? Port : (short)0,
                MyPlayerId = HasStarted && !string.IsNullOrWhiteSpace(blowFishKey)  ? Players.FirstOrDefault(o => 
                    string.CompareOrdinal(o.BlowFishKey, blowFishKey) == 0)?.GamePlayerId ?? 0 : 0
            };
        }

        public GameServerProcess Start()
        {
            // lets create a game process
            var gameServerProcess = new GameServerProcess()
            {
                Game = this,
                Port = Port
            };
            
            CreateGameSettings();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(
                Startup.LeagueGameServerConsole,
                $"-config \"{GameSettingPath()}\" -port \"{Port}\"")
                { 
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };                        
            process.Exited += new EventHandler((s, ev) => {
                LobbyList.ActiveGameProcess.Remove(gameServerProcess);
            });

            process.Start();

            gameServerProcess.Process = process;                       

            LobbyList.ActiveGameProcess.Add(gameServerProcess);
            LobbyList.AvailableGames.Remove(this);

            this.HasStarted = true;

            return gameServerProcess;
        }

        private void CreateGameSettings()
        {
            //create game file
            var config = new GameSettings.GameSettingConfig()
            {
                Game = new GameSettings.Game()
                {
                    DataPackage = "LeagueSandbox-Scripts",
                    Map = 1
                },
                GameInfo = new GameSettings.GameInfo()
                {
                    ManacostsEnabled = true,
                    CooldownsEnabled = true,
                    MinionSpawnsEnabled = true,
                    ContentPath = "../../../../Content",
                    IsDamageTextGlobal = false
                }
            };
            config.Players = new GameSettings.Player[Players.Count];
            // Assign Players
            for (int i = 0; i < Players.Count; i++)
            {
                var player = Players[i];
                player.GamePlayerId = i + 1;
                var configPlayer = config.Players[i];
                configPlayer.BlowfishKey = player.BlowFishKey;
                configPlayer.Champion = player.Champion;
                configPlayer.Icon = 0;
                configPlayer.Rank = "DIAMOND";
                configPlayer.Name = player.Name;
                configPlayer.PlayerId = player.GamePlayerId;
                configPlayer.Ribbon = 2;
                configPlayer.Skin = 0;
                configPlayer.Summoner1 = "SummonerHeal";
                configPlayer.Summoner2 = "SummonerFlash";
                configPlayer.Runes = new Dictionary<string, long>() {
                    { "1", 5245 },
                      {"2", 5245 },
                      {"3", 5245},
                      {"4", 5245},
                      {"5", 5245},
                      {"6", 5245},
                      {"7", 5245},
                      {"8", 5245},
                      {"9", 5245},
                      {"10", 5317},
                      {"11", 5317},
                      {"12", 5317},
                      {"13", 5317},
                      {"14", 5317},
                      {"15", 5317},
                      {"16", 5317},
                      {"17", 5317},
                      {"18", 5317},
                      {"19", 5289},
                      {"20", 5289},
                      {"21", 5289},
                      {"22", 5289},
                      {"23", 5289 },
                      {"24", 5289},
                      {"25", 5289},
                      {"26", 5289},
                      {"27", 5289},
                      {"28", 5335},
                      {"29", 5335},
                      {"30", 5335}
                    };
                configPlayer.Team = player.Team.ToString("G").ToUpper();
            }

            System.IO.File.WriteAllText(GameSettingPath(), JsonConvert.SerializeObject(config));

        }

        private string GameSettingPath()
        {
            return Path.Combine(Startup.GameSettngLocation, Id.ToString() + ".json");
        }
    }

    public class GameInfo
    {
        public Guid Id { get; set; }
        public string LobbyName { get; set; }
        public int TotalAvailable { get; set; }
        public int TotalSlots { get; set; }
    }

    public class GameInfoDetailed
    {
        public int MyPlayerId { get; set; }
        public short Port { get; set; }
        public Guid Id { get; set; }
        public string LobbyName { get; set; }
        public List<PlayerInfo> RedTeam { get; set; }
        public List<PlayerInfo> BlueTeam { get; set; }
    }

    public class PlayerInfo
    {
        public string Name { get; set; }
        public string Champion { get; set; }
    }
}
