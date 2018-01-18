using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;

namespace GameDevBot
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        string token = "";


        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            //client = new DiscordSocketClient();
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance          
            });
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            // Get the token Key from the file
            try
            {
                StreamReader sr = new StreamReader("token.txt");
                token = sr.ReadLine();
            }
            catch (Exception e)
            {
                Console.Write("Exception " + e.Message);
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.Log += Log;
            client.UserJoined += UserJoined;

            await Task.Delay(-1);
        }

        public async Task UserJoined(SocketGuildUser user)
        {           
            var channel = client.GetChannel(331158539038883840) as SocketTextChannel; 
            await channel.SendMessageAsync("Welcome " + user.Mention + "! Dont be a D1ck");
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

        }

        public async Task HandleCommand(SocketMessage msgParam)
        {
            var msg = msgParam as SocketUserMessage;
            char prefix = '!';
            if (msg == null) return;

            int argPos = 0;

            if(!msg.HasCharPrefix(prefix, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos)) return;
            // Create Command Context
            var context = new CommandContext(client, msg);
            //Execute the command. (Result does not indicate a return value,
            // rather an object stating if the command executed succesfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Log(LogMessage msg)
        {
            var c = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.White; // Comment this out and uncomment the below for coloured consoles for errors.
            //switch (msg.Severity)
            //{
            //    case LogSeverity.Critical:
            //        Console.ForegroundColor = ConsoleColor.Green;
            //        break;
            //    case LogSeverity.Error:
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        break;
            //    case LogSeverity.Warning:
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        break;
            //    case LogSeverity.Error:
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        break;
            //    case LogSeverity.Verbose:
            //        Console.ForegroundColor = ConsoleColor.Green;
            //        break;
            //    case LogSeverity.Debug:
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        break;
            //}
            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message}");
            Console.ForegroundColor = c;

            return Task.CompletedTask;
        }
    }
}
