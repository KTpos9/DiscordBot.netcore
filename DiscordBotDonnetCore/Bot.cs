using DSharpPlus;
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
using System.Text.RegularExpressions;
using DSharpPlus.SlashCommands;
using Serilog;
using Serilog.Extensions.Logging;

namespace DiscordBotDonnetCore
{
    public class Bot
    {
        private Regex r = new Regex(@"(จ|j).*(อ|o|0).*(ย|y)");
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public SlashCommandsExtension slash { get; private set; }
        public async Task RunAsync()
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
            .WriteTo.File("log.txt", outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

            var logFactory = new LoggerFactory().AddSerilog();

            var json = string.Empty;

            using (var fs = File.OpenRead(@"C:\Users\kitti\source\repos\DiscordBot.netCore\DiscordBotDonnetCore\config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Debug,
                LoggerFactory = logFactory
            };
            #region LAVALINK
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1", // From your server configuration.
                Port = 2333 // From your server configuration
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass", // From your server configuration.
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            #endregion

            Client = new DiscordClient(config);
            Client.Ready += Client_Ready;

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
                //await File.AppendAllTextAsync(@"Desktop\UserID", authorID);
                if (authorID == "697130665992257607")
                {
                    if (r.IsMatch(message) == true)
                    {
                        await e.Message.DeleteAsync();
                    }
                    #region old implimentation code
                    //if (message.IndexOf("จ") > -1 && (message.IndexOf("อ") > -1 || message.IndexOf("o") > -1 || message.IndexOf("0") > -1 || message.IndexOfAny(toneMark) > -1)
                    //    && message.IndexOf("ย") > -1 || (message.IndexOf("j") > -1 && message.IndexOf("y") > -1) || message.IndexOfAny(toneMark) > -1 ||
                    //    message.IndexOf("test") > -1)
                    //{
                    //    await e.Message.DeleteAsync();
                    //}
                    //else if (message.StartsWith("จ") || message.StartsWith("อ") || message.StartsWith("ย") ||
                    //        message.StartsWith("j") || message.StartsWith("o") || message.StartsWith("y") || message.IndexOfAny(toneMark) > -1)
                    //{
                    //    await e.Message.DeleteAsync();
                    //}
                    #endregion
                }
            };
            Client.MessageUpdated += async (s, e) =>
            {
                string message = e.Message.Content.ToLower().Trim();
                string authorID = e.Author.Id.ToString();
                if (authorID == "697130665992257607")
                {
                    if (r.IsMatch(message) == true)
                    {
                        await e.Message.DeleteAsync();
                    }
                }
            };
            #endregion
            
            var lavalink = Client.UseLavalink();//
            
            slash = Client.UseSlashCommands();
            slash.RegisterCommands<SlashCommand>();
            slash.RegisterCommands<MusicSlashCommands>(959281646622900244);
            slash.RegisterCommands<MusicSlashCommands>(822847772201713684);

            await Client.ConnectAsync();//
            await lavalink.ConnectAsync(lavalinkConfig);//

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<Commands>();
            Commands.RegisterCommands<MusicCommands>();

            await Task.Delay(-1);
        }

        private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            Console.WriteLine("Bot ready!");
            return Task.CompletedTask;
        }
    }
}