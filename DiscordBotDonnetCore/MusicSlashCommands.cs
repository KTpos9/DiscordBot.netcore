using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Lavalink;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace DiscordBotDonnetCore
{
    [SlashModuleLifespan(SlashModuleLifespan.Singleton)]
    public class MusicSlashCommands : ApplicationCommandModule
    {
        private LavalinkGuildConnection Conn { get; set; }
        private LavalinkNodeConnection Node { get; set; }
        private bool EnableLoop { get; set; } = false;
        private LavalinkTrack Track { get; set; }

        [SlashCommand("play","play song from given URL")]
        public async Task Play(InteractionContext ctx, [Option("URL","Song URL")] string url)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }
            #region join
            // bot join the voice channel
            var channel = ctx.Member.VoiceState.Channel;
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.CreateResponseAsync("The Lavalink connection is not established");
                return;
            }

            this.Node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync("Not a valid voice channel.");
                return;
            }
            //if(node.ConnectedGuilds == null)
            //{
            //    await node.ConnectAsync(channel);
            //}
            await this.Node.ConnectAsync(channel);
            #endregion
            var guild = ctx.Member.VoiceState.Guild;
            this.Conn = this.Node.GetGuildConnection(guild);

            if (this.Conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            var loadResult = await this.Node.Rest.GetTracksAsync(url);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.CreateResponseAsync($"Track search failed for {url}.");
                return;
            }
            Track = loadResult.Tracks.First();


            await this.Conn.PlayAsync(Track);

            await ctx.CreateResponseAsync($"Now playing {Track.Title}!");
        }
        [SlashCommand("pause","pause the song that are currently playing")]
        public async Task Pause(InteractionContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            this.Node = lava.ConnectedNodes.Values.First();
            this.Conn = this.Node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (this.Conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            if (this.Conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync("There are no tracks loaded.");
                return;
            }

            await this.Conn.PauseAsync();
            await ctx.CreateResponseAsync("track paused");
        }

        [SlashCommand("resume","resume the paused track")]
        public async Task Resume(InteractionContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            this.Node = lava.ConnectedNodes.Values.First();
            this.Conn = this.Node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (this.Conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            if (this.Conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync("There are no tracks loaded.");
                return;
            }

            await this.Conn.ResumeAsync();
            await ctx.CreateResponseAsync("track resumed");
        }

        [SlashCommand("dis","disconnect the bot from this channel")]
        public async Task Disconnect(InteractionContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }
            //not necessery for singleton 
            var lava = ctx.Client.GetLavalink();
            this.Node = lava.ConnectedNodes.Values.First();
            this.Conn = this.Node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (this.Conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            await this.Conn.StopAsync();
            await this.Conn.DisconnectAsync();
            await ctx.CreateResponseAsync("Bot Disconnected");
        }

        [SlashCommand("seek", "seek the track to specify time")]
        public async Task Seek(InteractionContext ctx, [Option("time", "time")] double time)
        {
            if (this.Conn == null) return;
            await this.Conn.SeekAsync(TimeSpan.FromMinutes(time));
            await ctx.CreateResponseAsync("seeked");
        }


        [SlashCommand("loop","loop the song")]
        public async Task Loop(InteractionContext ctx)
        {
            //not necessery for singleton 
            var lava = ctx.Client.GetLavalink();
            this.Node = lava.ConnectedNodes.Values.First();
            this.Conn = this.Node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }
            if (this.Conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync("There are no tracks loaded.");
                return;
            }
            EnableLoop = true;
            if (EnableLoop)
            {
                this.Conn.PlaybackFinished += Conn_PlaybackFinished;
                await ctx.CreateResponseAsync("loop enabled!");
            }
            else
            {
                EnableLoop = false;


                await ctx.CreateResponseAsync("loop disabled");
            }
        }

        private async Task Conn_PlaybackFinished(LavalinkGuildConnection sender, DSharpPlus.Lavalink.EventArgs.TrackFinishEventArgs e)
        {
            await this.Conn.PlayAsync(Track);
        }
    }
}
