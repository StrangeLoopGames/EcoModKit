/*
 * The WelcomeUserPlugin is an example of how to write a configurable plugin for Eco server. It allows the server administrator to configure a title and body for a 
 * popup dialog box that will open everytime a user connects to the server.
 * 
 * To learn more about how this plugin works check out the documentation below or visit the respective docs.play.eco api reference pages for each component
 * of the this plugin.
 */

using Eco.Core.Plugins;
using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.Players;
using Eco.Shared.Localization;
using Eco.Shared.Utils;
using System.Threading.Tasks;

namespace Eco.WelcomePlugin
{
    /// <summary>
    /// This class represents a Singleton instance of the <see cref="WelcomeUserPlugin"/>'s configuration options. Options populated inside this model
    /// will be exposed to the configuration system and Server UI if you are running the application from Windows. These settings can be adjusted in real time 
    /// if the matching <see cref="IConfigurablePlugin"/> instance supports it.
    /// 
    /// Properties of this model can be attributed with the <see cref="LocDescriptionAttribute"/> to define a helpful description for server UI users.
    /// </summary>
    [Localized]
    public class WelcomeUserConfig : Singleton<WelcomeUserConfig>
    {
        /// <summary>Represents the user defined welcome dialog's title</summary>
        [LocDescription("Represents the user defined welcome dialog's title")]
        public string WelcomeMessageTitle { get; set; }

        /// <summary>Represents the user defined welcome dialog's body</summary>
        [LocDescription("Represents the user defined welcome dialog's body")]
        public string WelcomeMessageBody { get; set; }
    }

    /// <summary>
    /// The User Welcome Plugin is an example of how to write a configurable plugin for Eco server. It allows the server administrator to configure a title and body for a message 
    /// displayed to the user when they connect to the server. All Modkit created plugins should inherit from the <see cref="IModKitPlugin"/> interface to be loaded automatically by
    /// Eco server. From there various features interfaces can be added to extend the functionality. A full list can be found on docs.play.eco.
    /// 
    /// This plugin inherits several additional feature interfaces of the plugin system for specific uses. These interfaces and uses are as follows
    /// * IInitializablePlugin - Allows the plugin to set it's status on startup, load our config, and register our event with the <see cref="UserManager"/>
    /// * IShutdownablePlugin - Allows the plugin to unregister its self from the <see cref="UserManager"/>
    /// * IConfigurablePlugin - Allows the plugin to be configured from the server UI.
    /// </summary>
    public class WelcomeUserPlugin : IModKitPlugin, IInitializablePlugin, IShutdownablePlugin, IConfigurablePlugin
    {
        /// <summary>Our private reference to our currently loaded configuration model</summary>
        PluginConfig<WelcomeUserConfig> config;
        /// <summary>Our public reference to our currently loaded configuration model. This property is a requirement of the <see cref="IConfigurablePlugin"/> interface.</summary>
        public IPluginConfig PluginConfig => this.config;
        /// <summary>Our thread safe configuration parameter changed event. This property is a requirement of the <see cref="IConfigurablePlugin"/> interface.</summary>
        public ThreadSafeAction<object, string> ParamChanged { get; set; } = new();
        /// <summary>This variable is used to store the local status of the public as displayed in the Server UI and other helpful places around the administrative tools of Eco server.</summary>
        string status = string.Empty;

        /// <summary>Retrieves the current status of the <see cref="IModKitPlugin"/>.</summary>
        public string GetStatus() => this.status;

        #region IInitializablePlugin/IShutdownablePlugin Interface
        /// <summary>Called on plugin Startup. Performs the initial status change, loads our configuration, and registers our event with the <see cref="UserManager"/></summary>
        public void Initialize(TimedTask timer)
        {
            this.status = "Ready.";
            this.config = new PluginConfig<WelcomeUserConfig>("WelcomeUser");   // Load our plugin configuration
            UserManager.OnUserLoggedIn.Add(this.OnUserLogin);                   // Register our OnUserLoggedIn event handler for showing players our welcome message.
        }

        /// <summary>Called on plugin shutdown. Removes our event handler for the <see cref="UserManager.OnUserLoggedIn"/> event.</summary>
        public Task ShutdownAsync()
        {
            UserManager.OnUserLoggedIn.Remove(this.OnUserLogin);                // Remove our OnUserLoggedIn event handler
            return Task.CompletedTask;
        }
        #endregion

        #region IConfigurablePlugin Interface
        /// <summary>Getter for retrieving our current configuration instance. This method is provided by the <see cref="IConfigurablePlugin"/> interface.</summary>
        public object GetEditObject() => this.config.Config;

        /// <summary>Called whenever the user changes a configuration option for this plugin. This method is provided by the <see cref="IConfigurablePlugin"/> interface.</summary>
        /// <param name="o">The object being modified</param>
        /// <param name="param">The string parameter that was modified.</param>
        public void OnEditObjectChanged(object o, string param) { }
        #endregion

        /// <summary>Called whenever a user logs into the server from the <see cref="UserManager.OnUserLoggedIn"/> event. Sends the newly connected user our welcome message as defined by the user.</summary>
        /// <param name="user">The new Eco user connected to the server.</param>
        void OnUserLogin(User user) => user.Player.OpenInfoPanel(this.config.Config.WelcomeMessageTitle, this.config.Config.WelcomeMessageBody, "Welcome");

        /// <summary>Custom ToString override for properly naming our plugin into the Eco server UI</summary>
        public override string ToString() => "Welcome User";
    }
}
