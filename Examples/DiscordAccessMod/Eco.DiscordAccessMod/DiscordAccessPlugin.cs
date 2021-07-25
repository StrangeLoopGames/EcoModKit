namespace Eco.DiscordAccessMod
{
    using Discord;
    using Discord.WebSocket;
    using Eco.Core.Plugins;
    using Eco.Core.Plugins.Interfaces;
    using Eco.Core.Utils;
    using Eco.Core.Utils.Async;
    using Eco.Gameplay.Players;
    using Eco.Plugins.Networking;
    using Eco.Shared.Authentication;
    using Eco.Shared.Localization;
    using Eco.Shared.Utils;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>Configuration for the <seealso cref="DiscordAccessPlugin"/> ModKit plugin.</summary>
    [Localized]
    public class DiscordAccessConfig : Singleton<DiscordAccessConfig>
    {
        /// <summary>Discord bot client token used to interact with Discord.</summary>
        [LocDescription("Discord bot client token used to interact with Discord. Changing this value will reload the Discord bot")]
        public string DiscordBotToken { get; set; }

        /// <summary>Complete list of Discord role ids that are allowed to access this Eco server.</summary>
        [LocDescription("Complete list of Discord role ids that are allowed to access this Eco server.")]
        public List<ulong> DiscordAccessRoles { get; set; } = new List<ulong>();

        /// <summary>Represents linked Discord/Eco accounts.</summary>
        [LocDescription("Represents linked Discord/Eco accounts.")]
        [Browsable(false)]
        public List<DiscordEcoLink> DiscordLinks { get; set; } = new List<DiscordEcoLink>();
    }
 
    /// <summary>
    /// <para>
    /// Provides Discord access control to your Eco server. This plugin spins up a Discord bot on your Eco server that keeps a record of 
    /// of each Discord server user and their matching Eco in game username. This is later used to check for their current Discord role for whitelisting
    /// them.
    /// </para>
    /// <para>
    /// Discord bots using this plugin must have the following Privilaged Gateway Intents enabled to properly function.
    /// <list type="bullet">
    /// <item>Presence Intent</item>
    /// <item>Server Members Intent</item>
    /// </list>
    /// </para>
    /// <para>
    /// This mod can be configured to allow access only to Twitch subscribers through your community Discord server by using the Twitch Subscriber role provided
    /// by Discord's built in Twitch integration.
    /// </para>
    /// </summary>
    /// <inheritdoc/>
    public class DiscordAccessPlugin : IModKitPlugin, IInitializablePlugin, IShutdownablePlugin, IConfigurablePlugin, IUserAuthorizer, IDisplayablePlugin
    {
        private string status;
        private DiscordSocketClient discordClient;
        private PluginConfig<DiscordAccessConfig> config;

        #region Configuration Management
        public IPluginConfig PluginConfig => this.config;
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new ThreadSafeAction<object, string>();
        public object GetEditObject() => this.config.Config;

        /// <summary>Restarts the Discord client when the plugin's configuration bot token changes.</summary>
        public void OnEditObjectChanged(object o, string param)
        {
            if (param == "DiscordBotToken") this.RestartDiscordClient();
        }
        #endregion

        #region Plugin Management
        public void Initialize(TimedTask timer)
        {
            this.status = "Initializing...";
            this.config = new PluginConfig<DiscordAccessConfig>("DiscordAccess");
            UserManager.Obj.AddUserAuthorizer(this);

            if (string.IsNullOrEmpty(NetworkManager.Config.Password))
                Log.WriteWarningLine(Localizer.DoStr("No server password detected. This mod cannot properly function without a server password assigned."));

            this.discordClient = new DiscordSocketClient();
            this.discordClient.Log += this.LogDiscordMessageAsync;
            this.discordClient.Ready += this.DiscordBotReadyAsync;
            this.discordClient.MessageReceived += this.DiscordBotMessageReceivedAsync;

            this.StartDiscordClient();
        }

        public async Task ShutdownAsync()
        {
            UserManager.Obj.RemoveUserAuthorizer(this);
            await this.discordClient.StopAsync();
        }

        public override string ToString() => Localizer.DoStr("Discord Access");
        public string GetDisplayText() => this.status;
        public string GetStatus() => this.status;
        #endregion

        #region Discord Bot Management

        /// <summary>Attempts to start our Discord client instance.</summary>
        private async Task StartDiscordClientAsync()
        {
            if (string.IsNullOrEmpty(this.config.Config.DiscordBotToken)) 
            {
                Log.WriteWarningLine(Localizer.DoStr("Failed to start Discord Access Mod. Bot token not supplied."));
                return;
            }

            await this.discordClient.LoginAsync(TokenType.Bot, this.config.Config.DiscordBotToken);
            await this.discordClient.StartAsync();
        }
        
        /// <inheritdoc cref="StartDiscordClientAsync"/>
        private void StartDiscordClient() => TaskUtils.RunWithExceptionLog("StartDiscordClient", async () => await this.StartDiscordClientAsync());
        
        /// <summary>Restarts the Discord client instance. Useful for when configuration changes occur.</summary>
        private async Task RestartDiscordClientAsync()
        {
            if (this.IsDiscordClientRunning()) 
                await this.discordClient.StopAsync();
            await this.StartDiscordClientAsync();
        }

        private void RestartDiscordClient() => TaskUtils.RunWithExceptionLog("RestartDiscordClient", async () => await this.RestartDiscordClientAsync());

        /// <summary>Returns true if the Discord client is currently running.</summary>
        private bool IsDiscordClientRunning() => this.discordClient.LoginState == LoginState.LoggedIn || this.discordClient.LoginState == LoginState.LoggingIn;

        /// <summary>Async task that passes the Discord.Net log messages through to Eco's logging System.</summary>
        /// <param name="message"><seealso cref="LogMessage"/> instance to log.</param>
        private Task LogDiscordMessageAsync(LogMessage message)
        {
            var localizedMessage = Localizer.DoStr($"Discord: {message.Message}");
            switch (message.Severity)
            {
                case LogSeverity.Debug:
                case LogSeverity.Verbose:
                case LogSeverity.Info:
                    Log.WriteLine(localizedMessage);
                    break;
                case LogSeverity.Warning:
                    Log.WriteWarningLine(localizedMessage);
                    break;
                case LogSeverity.Error:
                    Log.WriteErrorLine(localizedMessage);
                    break;
                case LogSeverity.Critical:
                    Log.WriteErrorLine(localizedMessage);
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        /// <summary>Handles the OnReady event from Discord.NET to update our plugin's status.</summary>
        private Task DiscordBotReadyAsync()
        {
            this.status = $"Ready. Connected as {this.discordClient.CurrentUser.Username}.";
            return Task.CompletedTask;
        }

        /// <summary>Handles incoming Discord messages from servers the Discord bot is in.</summary>
        /// <param name="message">Message received from Discord's servers.</param>
        private async Task DiscordBotMessageReceivedAsync(SocketMessage message)
        {
            var messageText = message.Content;
            var isCommand = messageText.StartsWith("!ecolink");
            if (!isCommand) return;

            string[] commandParts = messageText.Split(' ');
            if (commandParts.Length < 2) 
            {
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Invalid Usage: !ecolink [slg username/steam username]");
                return;
            }

            var ecoUsername = commandParts[1];
            await this.HandleLinkCommandAsync(message, ecoUsername);
        }

        /// <summary>Handles an incoming !ecolink command and creates/updates a Eco/Discord link object in our configuration object.</summary>
        /// <param name="message">Original Discord message received.</param>
        /// <param name="ecoUsername">Eco username supplied with the command.</param>
        private async Task HandleLinkCommandAsync(SocketMessage message, string ecoUsername)
        {
            var discordId = message.Author.Id;
            var searchQuery = this.config.Config.DiscordLinks.Where(l => l.DiscordId == discordId);
            if (searchQuery.Any())
            {
                var link = searchQuery.First();
                link.EcoUsername = ecoUsername;

                await this.config.SaveAsync();
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Eco linked username updated!");
            }
            else 
            {
                var link = new DiscordEcoLink
                {
                    DiscordId = discordId,
                    EcoUsername = ecoUsername,
                };

                this.config.Config.DiscordLinks.Add(link);
                await this.config.SaveAsync();
                await message.Channel.SendMessageAsync($"{message.Author.Mention} Eco link established!");
            }
        }
        #endregion

        /// <summary>Checks for an active Discord/Eco link and if the Discord user has any of the configured roles.</summary>
        /// <returns>Boolean true if the incoming <seealso cref="LoginSession"/> is allowed to connect.</returns>
        public bool AuthorizeSession(LoginSession session)
        {
            // Check if there is an established link
            var linkQuery = this.config.Config.DiscordLinks.Where(l => l.EcoUsername.ToLower() == session.username.ToLower());
            if (!linkQuery.Any()) return false;
            var link = linkQuery.First();

            // Check the user is still in the server and has the proper role
            SocketGuildUser discordUser = null;
            foreach (var guild in this.discordClient.Guilds)
            {
                discordUser = guild.GetUser(link.DiscordId);
                if (discordUser != null) break;
            }

            if (discordUser == null) return false;
            return discordUser.Roles.Where(r => this.config.Config.DiscordAccessRoles.Contains(r.Id)).Any();            
        }
    }
}
