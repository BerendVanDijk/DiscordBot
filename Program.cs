using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot
{
    class Program
    {
        static async Task Main(string[] args)
        => await new DiscordBotMusicClient().InitializeAsync();
    }
}
