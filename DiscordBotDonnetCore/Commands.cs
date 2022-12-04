using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotDonnetCore
{
    class Commands : BaseCommandModule
    {
        [Command("kword")]
        public async Task KWord(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("krai");
        }
        [Command("vote")]
        public async Task Vote(CommandContext ctx, [RemainingText] DiscordMember user)
        {
            try
            {
                var userCounts = user.VoiceState.Channel.Users.Count();
                //setting up embed message
                var voteEmbed = new DiscordEmbedBuilder
                {
                    Title = $"Do you want to kick {user.Username}?",
                    ImageUrl = user.AvatarUrl
                };
                var voteKickMesage = await ctx.Channel.SendMessageAsync(voteEmbed).ConfigureAwait(false);
                var thumbsUpEmoji = DiscordEmoji.FromName(ctx.Client, ":+1:");
                var thumbsDownEmoji = DiscordEmoji.FromName(ctx.Client, ":-1:");
                await voteKickMesage.CreateReactionAsync(thumbsUpEmoji).ConfigureAwait(false);

                var interactivity = ctx.Client.GetInteractivity();
                var result = await interactivity.CollectReactionsAsync(voteKickMesage, TimeSpan.FromSeconds(6));

                if (result.Count >= userCounts)
                {
                    //kick user
                    Action<MemberEditModel> action;
                    action = (member) => { member.VoiceChannel = null; };
                    await user.ModifyAsync(action);
                    await ctx.Channel.SendMessageAsync($"{user.Username} disconnected");
                }
            }
            catch (NullReferenceException)
            {
                await ctx.Channel.SendMessageAsync("User is not in a voice channel");
            }
        }
        [Command("member")]
        public async Task Member(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"{ctx.Member.VoiceState.Channel.Users.Count()}");
        }
    }
}
