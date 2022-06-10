using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Entities;

namespace DiscordBot.Services
{
    public class MusicService
    {
        private LavaRestClient _lavaRestClient;
        private LavaSocketClient _lavaSocketClient;
        private DiscordSocketClient _client;
        private LavaPlayer _player;
        private readonly ulong BotID = 8032;
        public MusicService(LavaRestClient lavaRestClient, LavaSocketClient lavaSocketClient, DiscordSocketClient discordSocketClient)
        {
            _lavaRestClient = lavaRestClient;
            _lavaSocketClient = lavaSocketClient;
            _client = discordSocketClient;
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel,ITextChannel textChannel)
            => await _lavaSocketClient.ConnectAsync(voiceChannel,textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
            => await _lavaSocketClient.DisconnectAsync(voiceChannel);
        public async Task<string> PlayAsync(string query,ulong guildId, SocketVoiceChannel voiceChannel, ITextChannel textChannel)
        {
            int con=0;
            _player = _lavaSocketClient.GetPlayer(guildId);
            
            if (_player == null)
            {
                await ConnectAsync(voiceChannel, textChannel);
                 con = 1;
            }

            _player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);

            if (results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
                return "No matches found";
            
            var track = results.Tracks.FirstOrDefault();
            
            if (_player.IsPlaying)
            {
                await MoveAsync(voiceChannel);
                _player.Queue.Enqueue(track);
                return $"{track.Title} added to queue.";
            }
            else
            {
               await _player.PlayAsync(track);
                return $"Now playing {track.Title}";
            }
        }
        public async Task MoveAsync(SocketVoiceChannel voiceChannel)
        =>await _lavaSocketClient.MoveChannelsAsync(voiceChannel);

        public async Task StopAsync()
        {
            if (_player.IsPlaying)
            {
                await _player.StopAsync();
                
            }
            else
            return;
        }
        public async Task<string> PauseAsync()
        {
            if (_player.IsPlaying)
            {
                await _player.PauseAsync();
                return $"{_player.CurrentTrack.Title} is paused!";
            }
            else
                return null;
        }
        public async Task<string> ResumeAsync()
        {
            if (_player.IsPaused)
            {
                await _player.ResumeAsync();
                return $"{_player.CurrentTrack.Title} has resumed!";
            }
            else
                return null;
        }
        public async Task<string> SkipAsync()
        {
            if (_player.IsPlaying)
            {
                var previousTrack = _player.CurrentTrack.Title;
                await _player.SkipAsync();
                return $"{previousTrack} has been skipped!";
            }
            else
                return null;
        }
        
        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += TrackFinished;
            
            return Task.CompletedTask;
        }
        private async Task ClientReadyAsync()
        {
            await _lavaSocketClient.StartAsync(_client, new Configuration());

        }

        private static async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {

            if (!reason.ShouldPlayNext()) return;

            if (!player.Queue.TryDequeue(out var item)||!(item is LavaTrack nextTrack))
            {
                await player.TextChannel.SendMessageAsync("No more tracks in the queue.");
                return;
            }
            await player.PlayAsync(nextTrack);
        }

        private  Task LogAsync(LogMessage logmessage)
        {
            Console.WriteLine(logmessage.Message);
            return Task.CompletedTask;
        }

       
    }
}
