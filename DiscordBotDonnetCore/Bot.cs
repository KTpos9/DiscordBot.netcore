﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Net.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;

namespace DiscordBotDonnetCore
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public Action<MemberEditModel> VoiceChannel { get; private set; }
        char[] toneMark = { '่', '้', '๋', '๊', 'ี', 'ิ', 'ื', 'ุ', 'ู', '์', 'ํ', '็' };
        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead(@"C:\Users\kitti\source\repos\DiscordBotDonnetCore\DiscordBotDonnetCore\config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
            };
            #region LAVALINK
            //var endpoint = new ConnectionEndpoint
            //{
            //    Hostname = "127.0.0.1", // From your server configuration.
            //    Port = 2333 // From your server configuration
            //};

            //var lavalinkConfig = new LavalinkConfiguration
            //{
            //    Password = "youshallnotpass", // From your server configuration.
            //    RestEndpoint = endpoint,
            //    SocketEndpoint = endpoint
            //};
            #endregion

            Client = new DiscordClient(config);
            //Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commandsConfig = new CommandsNextConfiguration {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = true,
            };
            #region autoDelete
            Client.MessageCreated += async (s, e) => {
                string message = e.Message.Content.ToLower().Trim();
                string authorID = e.Author.Id.ToString();
                await File.AppendAllTextAsync(@"Desktop\UserID", authorID);
                
                if(authorID == "697130665992257607")
                {
                    if (message.IndexOf("จ") > -1 && (message.IndexOf("อ") > -1 || message.IndexOf("o") > -1 || message.IndexOf("0") > -1 || message.IndexOfAny(toneMark) > -1)
                        && message.IndexOf("ย") > -1 || (message.IndexOf("j") > -1 && message.IndexOf("y") > -1) || message.IndexOfAny(toneMark) > -1 ||
                        message.IndexOf("test") > -1)
                    {
                        await e.Message.DeleteAsync();
                    }
                    else if (message.IndexOf("มึงว่า") > -1 && (message.IndexOf("สมมติว่า") > -1))
                    {
                        await e.Message.DeleteAsync();
                    }
                    else if (message.StartsWith("จ") || message.StartsWith("อ") || message.StartsWith("ย") ||
                            message.StartsWith("j") || message.StartsWith("o") || message.StartsWith("y") || message.IndexOfAny(toneMark) > -1)
                    {
                        await e.Message.DeleteAsync();
                    }
                    await e.Message.DeleteAsync();
                }
            };
            Client.MessageUpdated += async (s, e) =>
            {
                string message = e.Message.Content.ToLower().Trim();
                string authorID = e.Author.Id.ToString();
                if (authorID == "697130665992257607")
                {
                    if (message.IndexOf("จ") > -1 && (message.IndexOf("อ") > -1 || message.IndexOf("o") > -1 || message.IndexOf("0") > -1 || message.IndexOfAny(toneMark) > -1)
                        && message.IndexOf("ย") > -1 || (message.IndexOf("j") > -1 && message.IndexOf("y") > -1) ||
                        message.IndexOf("test") > -1)
                    {
                        await e.Message.DeleteAsync();
                    }
                }
            };
            #endregion
            Commands = Client.UseCommandsNext(commandsConfig);
            var lavalink = Client.UseLavalink();
            Commands.RegisterCommands<MusicCommands>();

            await Client.ConnectAsync();
            //await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

    }
}