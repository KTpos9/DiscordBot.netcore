using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;

namespace DiscordBotDonnetCore
{
    public class SlashCommand : ApplicationCommandModule
    {
        [SlashCommand("test","test command")]
        public async Task Test(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("it works");
        }
    }
}
