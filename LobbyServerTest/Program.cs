using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

            var gameInfo = await PostData<GameInfoDetailed, Player>(client, "CreateGame", player);

            if(gameInfo == null)
            {
                Console.WriteLine("Unable to create game!");
            }
            else
            {
                Console.WriteLine($"Game has been created - with id: {gameInfo.Id}");
            }

            var startInfo = await PostData<GameInfoDetailed, Player>(client, $"StartGame?id={gameInfo.Id}", player);
            if (startInfo == null)
            {
                Console.WriteLine("Unable to start game!");
            }
            else
            {
                Console.WriteLine($"Game has been started - Your player id is: {startInfo.MyPlayerId}");
            }

            //"8394" "LoLLauncher.exe" "" "165.228.159.115 5119 17BLOhi6KZsTtldTsizvHg== 1"

            var argsLine = $"\"8394\" \"LoLLauncher.exe\" \"\" \"127.0.0.1 {startInfo.Port} {player.BlowFishKey} {startInfo.MyPlayerId}\"";


            var processInfo = new ProcessStartInfo(
                @"C:\LeagueFake\League of Legends_UNPACKED\League of Legends_UNPACKED\League-of-Legends-4-20\RADS\solutions\lol_game_client_sln\releases\0.0.1.68\deploy\League of Legends.exe",
                argsLine);

            var process = new Process();
            process.StartInfo = processInfo;
            process.Start();

            Console.ReadKey();
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
    }
}
