using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameDevBot
{
    public class GameDevBot : ModuleBase
    {
        private CommandService _service;

        public GameDevBot(CommandService service)
        {
            _service = service;
        }

        [Command("hello")]
        [Summary("Say hello")]
        [Alias("hi")]
        public async Task Hello()
        {
            await ReplyAsync("Hello " + Context.Message.Author.Mention);
        }


        [Command("purge")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Alias("clear", "delete")]
        public async Task Purge([Remainder] int num = 0)
        {
            var userList = await Context.Guild.GetUsersAsync();
            var user = userList.Where(input => input.Username == Context.Message.Author.Username).FirstOrDefault() as SocketGuildUser;
            var userRoles = user.Roles;
            if (userRoles.Any(input => input.Name.ToUpper() == "ADMIN"))
            {
                if (num <= 100)
                {
                    var messagesToDelete = await Context.Channel.GetMessagesAsync(num + 1).Flatten();
                    await Context.Channel.DeleteMessagesAsync(messagesToDelete);
                    if (num == 1)
                    {
                       // await Context.Channel.SendMessageAsync(Context.User.Username + " deleted 1 message");
                    }
                    else
                    {
                       // await Context.Channel.SendMessageAsync(Context.User.Username + " deleted " + num + "messages");
                    }
                }
                else
                {
                 //   var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                //    await dmchannel.SendMessageAsync("You left blender");
                    await ReplyAsync("You cannot delete more than 100 messages");
                }
            }
            else
            {
                var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                await dmchannel.SendMessageAsync("You do not have permissions to use the command");
                //await ReplyAsync("You do not have permissions to use the command");
            }
        }
        [Command("help")]
        [Summary("Shows what a specific command does and what parameters it takes")]
        public async Task HelpAsync([Remainder, Summary("command to retrieve help for")] string command = null)
        {
            string prefix = "!";
            if (command == null)
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = "These are the commands available for this bot."
                };

                foreach (var module in _service.Modules) /* We are going to loop though the modules taken from the service we initiated earlier */
                {
                    string description = null;
                    foreach (var cmd in module.Commands) /* And now we loop though the commands per modules as well oh joy! */
                    {
                        var result = await cmd.CheckPreconditionsAsync(Context); /* Got to check if they pass */
                        if (result.IsSuccess)
                            description += $"{prefix}{cmd.Aliases.First()}\n"; /* If they do pass, we add yhat commands first alias (aka its actual name) to the description of this embed */
                    }

                    if (!string.IsNullOrWhiteSpace(description)) /* if the module wasn't empty, we go and add a field where we drop all the data into! */
                    {
                        builder.AddField(x =>
                        {
                            x.Name = module.Name;
                            x.Value = description;
                            x.IsInline = false;
                        });
                    }
                }

                /* Comment is for if you want to DM the user rather than message in the channel */
                var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                await dmchannel.SendMessageAsync("", false, builder.Build());
                // await ReplyAsync("", false, builder.Build());
            }
            else
            {
                var result = _service.Search(Context, command);

                if (!result.IsSuccess)
                {
                    await ReplyAsync($"Sorry, I couldnt find a command like **{command}**.*");
                    return;
                }

                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = $"Help for Command **{prefix}{command}**\n\nAliases: "
                };

                foreach (var match in result.Commands)
                {
                    var cmd = match.Command;

                    builder.AddField(x =>
                    {
                        x.Name = string.Join(", ", cmd.Aliases);
                        x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
            $"Remarks: {cmd.Remarks}";
                        x.IsInline = false;
                    });
                }
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("rank")]
        [Summary("Add or Remove roles to/from user in the discord server")]
        public async Task Rank(string role)
        {
            var userList = await Context.Guild.GetUsersAsync();
            var user = userList.Where(input => input.Username == Context.Message.Author.Username).FirstOrDefault() as SocketGuildUser;
            var userRoles = user.Roles;          
            // This is the roles Array
            // (Context.Channel as SocketGuildChannel).Guild.Roles;
            

            if (role != null)
            {
                if (role == "blender")
                {
                    var blender = Context.Guild.Roles.Where(input => input.Name.ToUpper() == "BLENDER").FirstOrDefault() as SocketRole;
                    if (user.Roles.Contains(blender))
                        
                    {
                        await user.RemoveRoleAsync(blender);
                        var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                        await dmchannel.SendMessageAsync("You left blender");
                    }

                    else
                    {
                        await user.AddRoleAsync(blender);
                        var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                        await dmchannel.SendMessageAsync("You joined blender");
                    }
                }
                if (role == "unity")
                {
                    var unity = Context.Guild.Roles.Where(input => input.Name.ToUpper() == "UNITY").FirstOrDefault() as SocketRole;

                    if (user.Roles.Contains(unity))
                    {
                        await user.RemoveRoleAsync(unity);
                        var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                        await dmchannel.SendMessageAsync("You left unity");
                    }
                    else
                    {
                        await user.AddRoleAsync(unity);
                        var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                        await dmchannel.SendMessageAsync("You joined unity");
                    }
                }
                if (role == "test")
                {
                    var test = Context.Guild.Roles.Where(input => input.Name.ToUpper() == "TEST").FirstOrDefault() as SocketRole;

                    if (user.Roles.Contains(test))
                    {
                        await user.RemoveRoleAsync(test);
                        var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                        await dmchannel.SendMessageAsync("You left unity");
                    }
                    else
                    {
                        await user.AddRoleAsync(test);
                        var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                        await dmchannel.SendMessageAsync("You joined unity");
                    }
                }
                if (role == null)
                {
                    List<string> roles = new List<string>();
                    Context.Guild.Roles.ToList().ForEach(c => roles.Add(c.ToString()));
                    var embed = new EmbedBuilder()
                    {
                        Title = "Roles for GameDev.tv Discord",
                        Description = string.Join(", ", roles),
                        Color = new Color(114, 33, 161)
                    };
                    await ReplyAsync("", false, embed);
                }
            }
        }

        [Command("ranks")]
        [Summary("Lists the roles on the server and displays them embedded in the channel.")]
        public async Task Ranks()
        {
          
            {
                List<string> roles = new List<string>();
                Context.Guild.Roles.ToList().ForEach(c => roles.Add(c.ToString()));
                var embed = new EmbedBuilder()
                {
                    Title = "Roles for GameDev.tv Discord",
                    Description = string.Join(", ", roles),
                    Color = new Color(114, 33, 161)
                };
                await ReplyAsync("", false, embed);
            }
        }

        [Command("addrank")]
        [Summary("Currently broken will only create a rank if the rank blender exists and will always create one if it does. Needs array check.")]
        public async Task AddRank(string crank)
        {
            var userList = await Context.Guild.GetUsersAsync();
            var user = userList.Where(input => input.Username == Context.Message.Author.Username).FirstOrDefault() as SocketGuildUser;
            var userRoles = user.Roles;

            if (userRoles.Any(input => input.Name.ToUpper() == "ADMIN"))
            {
                var blender = Context.Guild.Roles.Where(input => input.Name.ToUpper() == "BLENDER").FirstOrDefault() as SocketRole;

                if (Context.Guild.Roles.Contains(blender))           
                {                 
                    await ReplyAsync("Role already exists Created");
                }
                else
                {
                    await Context.Guild.CreateRoleAsync(crank.ToString());
                    await ReplyAsync("Role Created");
                }
            }
        }

        [Command("secret")]
        public async Task RevealSecret([Remainder]string arg = "")
        {
            if (!UserIsSecretOwner((SocketGuildUser)Context.User))
            {
                await Context.Channel.SendMessageAsync(":x: You need the SecretOwner role to do that. " + Context.User.Mention);
                return;
            }
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync("Yes");
        }

        private bool UserIsSecretOwner(SocketGuildUser user)
        {
            string targetRoleName = "SecretOwner";
            var result = from r in user.Guild.Roles
                         where r.Name == targetRoleName
                         select r.Id;
            ulong roleID = result.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetRole);
        }
    }
}
