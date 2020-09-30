using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace LobbyServerTest
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://localhost:5001")
            };

            var player = new Player()
            {
                Team = Team.Blue,
                BlowFishKey = "17BLOhi6KZsTtldTsizvHg=="
            };

            var champions = 
                JsonConvert.DeserializeObject<List<string>>(
                    await 
                        (await client.GetAsync("Champions")).
                            Content.ReadAsStringAsync());

            Console.WriteLine("What is your player name?");

            player.Name = Console.ReadLine();

            if(champions?.Count == 0)
            {
                Console.WriteLine("No Champions to choose from!");
                return;
            }
            
            bool hasChosen = false;
            while(!hasChosen)
            {
                Console.WriteLine("Please choose a champion");
                for (int i = 0; i < champions.Count; i++)
                {
                    Console.WriteLine($"{i}. {champions[i]}");
                }
                var index = -1;
                int.TryParse(Console.ReadLine(), out index);
               // index = 14;

                if(index > -1 && index <= champions.Count - 1)
                {
                    Console.WriteLine($"{champions[index]} has been chosen");
                    hasChosen = true;
                    player.Champion = champions[index];
                }
                else
                {
                    Console.WriteLine("Invalid index for champion");
                }                
            }

            GameInfoDetailed startInfo = null;

            Console.WriteLine("Would you like to create a game?");
            if(Console.ReadLine().ToLower().Trim() == "yes")
            {
                Console.WriteLine("Would you like to create lobby password protected?");
                var password = "";
                if(Console.ReadLine().ToLower().Trim() == "yes")
                {
                    password = Console.ReadLine();
                }
                var gameInfo = await PostData<GameInfoDetailed, Player>(client, $"CreateGame?passwordPhrase={HttpUtility.UrlEncode(password + "")}", player);

                if (gameInfo == null)
                {
                    Console.WriteLine("Unable to create game!");
                }
                else
                {
                    Console.WriteLine($"Game has been created - with id: {gameInfo.Id}");
                }

                Console.WriteLine("Would you like to start the server?");
                while (Console.ReadLine().ToLower().Trim() != "yes")
                {
                    Console.WriteLine("Would you like to start the server?");
                }

                startInfo = await PostData<GameInfoDetailed, Player>(client, $"StartGame?id={gameInfo.Id}&passwordPhrase={HttpUtility.UrlEncode(password + "")}", player);
                if (startInfo == null)
                {
                    Console.WriteLine("Unable to start game!");
                }
                else
                {
                    Console.WriteLine($"Game has been started - Your player id is: {startInfo.MyPlayerId}");
                }
            }
            else
            {
                Guid gameToJoin = default;

                bool hasChosen2 = false;
                string passwordTry = "";

                while (!hasChosen2)
                {
                    Console.WriteLine("Please choose a game you would like to join.");

                    var games =
                JsonConvert.DeserializeObject<List<GameInfo>>(
                    await
                        (await client.GetAsync("AvailableGames")).
                            Content.ReadAsStringAsync());

                    for (int i = 0; i < games.Count; i++)
                    {
                        Console.WriteLine($"{i}. {games[i].LobbyName}{(games[i].PasswordProtected ? " (Password Required)" : "")} Lobby: {games[i].TotalSlots - games[i].TotalAvailable} / {games[i].TotalSlots}");
                    }
                    var index = -1;
                    int.TryParse(Console.ReadLine(), out index);
                    // index = 14;

                    if (index > -1 && index <= games.Count - 1)
                    {
                        if(games[index].PasswordProtected)
                        {
                            Console.WriteLine("What is the password?");
                            passwordTry = Console.ReadLine();
                        }

                        Console.WriteLine($"attempting to join {games[index]}");
                        hasChosen2 = true;
                        gameToJoin = games[index].Id;
                    }
                    else
                    {
                        Console.WriteLine("Invalid index for champion");
                    }
                }

                var gameInfo = await PostData<GameInfoDetailed, Player>(client, $"JoinGame?id={gameToJoin}&passwordPhrase={HttpUtility.UrlEncode(passwordTry + "")}", player);

                if (gameInfo == null)
                {
                    Console.WriteLine("Unable to Join game!");
                }
                else
                {
                    Console.WriteLine($"You have joined game: {gameInfo.Id}");
                }

                Console.WriteLine("Waiting for game to start.");

                bool hasStarted = false;
                while(!hasStarted)
                {
                    startInfo = await PostData<GameInfoDetailed, Player>(client, $"GameStatus?id={gameToJoin}&passwordPhrase={HttpUtility.UrlEncode(passwordTry + "")}", player);
                    if(startInfo != null && startInfo.MyPlayerId > 0)
                    {
                        hasStarted = true;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }                    
                }
            }

            
            if(startInfo != null)
            {
                //"8394" "LoLLauncher.exe" "" "165.228.159.115 5119 17BLOhi6KZsTtldTsizvHg== 1"

                var argsLine = $"\"8394\" \"LoLLauncher.exe\" \"\" \"127.0.0.1 {startInfo.Port} {player.BlowFishKey} {startInfo.MyPlayerId}\"";

                var exeLoc = @"C:\LeagueFake\League of Legends_UNPACKED\League of Legends_UNPACKED\League-of-Legends-4-20\RADS\solutions\lol_game_client_sln\releases\0.0.1.68\deploy\League of Legends.exe";
                var processInfo = new ProcessStartInfo(
                    exeLoc,
                    argsLine);

                var process = new Process();
                process.StartInfo = processInfo;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeLoc);
                process.Start();

                Console.ReadKey();
            }
           
        }

        public static async Task<ReturnData> PostData<ReturnData, PostData>(HttpClient httpClient, string query, PostData postData) where ReturnData : class
        {
            var result = await httpClient.PostAsync(query,
                new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ReturnData>(content);
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

        public class GameInfo
        {
            public Guid Id { get; set; }
            public string LobbyName { get; set; }
            public int TotalAvailable { get; set; }
            public int TotalSlots { get; set; }
            public bool PasswordProtected { get; set; }
        }
    }
}
