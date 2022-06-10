using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace DiscordBot.Modules
{
    public class Music : ModuleBase<SocketCommandContext>
    {
        private MusicService _musicService;
        public Music(MusicService musicService)
        {
            _musicService = musicService;
        }
        [Command("Join")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync($"{user.Mention} you need to connect to a voice channel!");
                return;
            }
            else
            {;
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Now connected to voice channel: {user.VoiceChannel.Name}");
            }
        }
        [Command("Leave")]
        public async Task Leave()
        {
            var user = Context.User as SocketGuildUser;

            if (user.VoiceChannel is null)
            {
                await ReplyAsync($"{user.Mention} you are not connected to a voice channel!");
                return;
            }
            else
            {
                await _musicService.LeaveAsync(user.VoiceChannel);
                await ReplyAsync($"Disconnecting from: {user.VoiceChannel.Name}");
            }
        }
        [Command("Play")]
        public async Task Play([Remainder] string query )
        {
            var user = Context.User as SocketGuildUser;
            if (user.VoiceChannel is null)
            {
                await ReplyAsync($"{user.Mention} you are not connected to a voice channel!");
                return;
            }
            else
            {
                var result = await _musicService.PlayAsync(query, Context.Guild.Id,user.VoiceChannel,Context.Channel as ITextChannel);
                await ReplyAsync(result);
            }
            
        }
        [Command("Stop")]
        public async Task Stop()
        {
            await _musicService.StopAsync();
        }
        [Command("Pause")]
        public async Task Pause()
        {
            var result = await _musicService.PauseAsync();
            await ReplyAsync(result);
        }
        [Command("Resume")]
        public async Task Resume()
        {
            var result = await _musicService.ResumeAsync();
            await ReplyAsync(result);
        }
        [Command("Skip")]
        public async Task Skip()
        {
            var result = await _musicService.SkipAsync();
            await ReplyAsync(result);
        }
        [Command("Move")]
        public async Task Move()
        {
            var user = Context.User as SocketGuildUser;
            await _musicService.MoveAsync(user.VoiceChannel);
        }
    }
}
