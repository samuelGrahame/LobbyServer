using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace LobbyServer
{
    public class Game
    {        
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

        public GameInfoDetailed GetInfoDetailed()
        {
            return new GameInfoDetailed()
            {
                Id = Id,
                LobbyName = $"{Host.Name}'s lobby",
                RedTeam = new List<PlayerInfo>(Players.Where(o => o.Team == Team.Red).
                    Select(o => new PlayerInfo() { Name   = o.Name, Champion = o.Champion })),
                BlueTeam = new List<PlayerInfo>(Players.Where(o => o.Team == Team.Blue).
                    Select(o => new PlayerInfo() { Name = o.Name, Champion = o.Champion }))
            };
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
