using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Lavalink;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext;

namespace DiscordBotDonnetCore
{
    public class MusicSlashCommands : ApplicationCommandModule
    {
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

            var node = lava.ConnectedNodes.Values.First();

            if (channel.Type != ChannelType.Voice)
            {
                await ctx.CreateResponseAsync("Not a valid voice channel.");
                return;
            }
            //if(node.ConnectedGuilds == null)
            //{
            //    await node.ConnectAsync(channel);
            //}
            await node.ConnectAsync(channel);
            #endregion
            var guild = ctx.Member.VoiceState.Guild;
            LavalinkGuildConnection conn = node.GetGuildConnection(guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            var loadResult = await node.Rest.GetTracksAsync(url);

            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.CreateResponseAsync($"Track search failed for {url}.");
                return;
            }
            var track = loadResult.Tracks.First();


            await conn.PlayAsync(track);

            await ctx.CreateResponseAsync($"Now playing {track.Title}!");
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
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.CreateResponseAsync("There are no tracks loaded.");
                return;
            }

            await conn.PauseAsync();
        }
        [SlashCommand("dis","disconnect the bot from this channel")]
        public async Task Disconnect(InteractionContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.CreateResponseAsync("You are not in a voice channel.");
                return;
            }

            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.CreateResponseAsync("Lavalink is not connected.");
                return;
            }

            await conn.StopAsync();
            await conn.DisconnectAsync();
            await ctx.CreateResponseAsync("Bot Disconnected");
        }
    }
}
