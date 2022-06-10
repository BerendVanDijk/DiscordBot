using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using DiscordBot.Services;

namespace DiscordBot
{
    class DiscordBotMusicClient
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _services;

        public DiscordBotMusicClient(DiscordSocketClient client = null, CommandService cmdService = null)
        {
            _client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Debug,
                ConnectionTimeout = 1000

            }); ;

            _cmdService = cmdService ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false,
                

            });

        }


        public async Task InitializeAsync()
        {

            await _client.LoginAsync(TokenType.Bot, "NTk1NDU1NDMzNDk0MjMzMTE1.GbmzaO.Be5pTI-dFaF-LnSU1eBiw2tbSe_7geh1U8Rah4");
            await _client.StartAsync();
            _client.Log += _client_Log_Async;
            _services = SetupServices();
            var cmdhandler = new CommandHandeler(_client,_cmdService,_services);
            await cmdhandler.InitializeAsync();
            await _services.GetRequiredService<MusicService>().InitializeAsync();

            await Task.Delay(-1);
        }

        private Task _client_Log_Async(LogMessage logMessage)
        {
            Console.WriteLine(logMessage.Message);
            return Task.CompletedTask;
        }

        private IServiceProvider SetupServices()
            => new ServiceCollection()
              .AddSingleton(_client)
              .AddSingleton(_cmdService)
              .AddSingleton<LavaRestClient>()
              .AddSingleton<LavaSocketClient>()
              .AddSingleton<MusicService>()
              .BuildServiceProvider();
     
              
        
    }
}
