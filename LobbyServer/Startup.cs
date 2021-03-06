using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LobbyServer
{
    public class Startup
    {
        public static List<string> AvailableChampions { get; set; }
        public static List<short> AvailablePorts { get; set; }
        public static string LeagueGameServerConsole { get; set; }
        public static string GameSettngLocation { get; set; }

        public Startup(IConfiguration configuration)
        {            
            Configuration = configuration;
            
            var lobbySection = Configuration.GetSection("LobbySettings");

            LeagueGameServerConsole = lobbySection.GetValue<string>("LeagueGameServerConsole");
            GameSettngLocation = lobbySection.GetValue<string>("GameSettngLocation");

            if (!File.Exists(LeagueGameServerConsole))
            {
                throw new Exception($"{LeagueGameServerConsole} is not a valid path for: LeagueGameServerConsole");
            }

            if(!Directory.Exists(GameSettngLocation))
            {
                throw new Exception($"{GameSettngLocation} is not a directoy for: GameSettngLocation");
            }

            var ScriptChampLocation = Path.Combine(lobbySection.GetValue<string>("LeagueSandboxContent"), @"LeagueSandbox-Scripts\Champions");
            AvailablePorts = new List<short>();
            foreach (var item in lobbySection.GetSection("AvailablePorts").AsEnumerable())
            {                
                if(short.TryParse(item.Value, out short port))
                {
                    AvailablePorts.Add(port);
                }
            }
            
            if (AvailablePorts.Count == 0)
            {
                AvailablePorts = new List<short>() { 5119 };
            }
            
            AvailableChampions = new List<string>();

            if (Directory.Exists(ScriptChampLocation))
            {
                foreach (var item in Directory.GetDirectories(ScriptChampLocation))
                {                    
                    var dirName = Path.GetFileName(item);
                    if (dirName.ToLower() == "global")
                        continue;
                    AvailableChampions.Add(dirName);
                } 
            }

            if (AvailableChampions.Count == 0)
                throw new ArgumentNullException(nameof(AvailableChampions));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
