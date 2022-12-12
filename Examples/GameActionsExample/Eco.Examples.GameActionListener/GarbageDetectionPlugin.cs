/*
 * 
 * 
 */

using Eco.Core.Plugins.Interfaces;
using Eco.Core.Utils;
using Eco.Gameplay.GameActions;
using Eco.Gameplay.Systems.Messaging.Notifications;
using Eco.Shared.Localization;
using System.Threading.Tasks;

namespace Eco.Examples.GameActionListener
{
    /// <summary>
    /// The Garbage Detection Plugin is an example of how to use the <see cref="IGameActionAware"/> interface to detect <see cref="GameAction"/> instances being processed by Eco Server. 
    /// <see cref="IGameActionAware"/> allows an object to listen for all actions moving through the system and modify the authorization where applicable. For more information on the
    /// <see cref="IModKitPlugin"/> side of this example see the "ModkitPlugin" example codebase.
    /// </summary>
    public class GarbageDetectionPlugin : IModKitPlugin, IGameActionAware, IInitializablePlugin, IShutdownablePlugin
    {
        /// <summary>Retrieves the current status of the <see cref="IModKitPlugin"/>.</summary>
        public string GetStatus() => "Ready!";

        #region IInitializablePlugin/IShutdownablePlugin Interface
        /// <summary>Called on plugin instantiation to register our selves with the <see cref="ActionUtil"/> listeners.</summary>
        public void Initialize(TimedTask timer)
        {
            ActionUtil.AddListener(this); // Registers our instance with the list of listeners found in the ActionUtil class. This allows us to receive actions being performed.
        }

        /// <summary>Called on plugin shutdown to remove our selves from the <see cref="ActionUtil"/> listeners.</summary>
        public Task ShutdownAsync()
        {
            ActionUtil.RemoveListener(this); // Remove our instance from the list of listeners found in the ActionUtil class.
            return Task.CompletedTask;
        }
        #endregion

        #region IGameActionAware Interface
        /// <summary>
        /// Called by the game actions system to notify us of any <see cref="GameAction"/> being performed. 
        /// A complet list of <see cref="GameAction"/>s that can be processed can be found at https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.GameActions.html.
        /// </summary>
        /// <param name="action">The <see cref="GameAction"/> being performed. This variable can be compared against the interfaces and defined action models to find out what is being processed.</param>
        public void ActionPerformed(GameAction action)
        {
            if (action is DropGarbage)
            {
                var user = (action as IUserGameAction)?.Citizen;
                NotificationManager.ServerMessageToAll(Localizer.Do($"{user?.Name} droped garbage! How rude!"));
            }
        }

        /// <summary>
        /// Called by the game actions system to allow us to override the authorization of the <see cref="GameAction"/> being processed.
        /// Not all game actions support authorization override. A complete list of available game actions and their details can be found at 
        /// https://docs.play.eco/api/server/eco.gameplay/Eco.Gameplay.GameActions.html
        /// </summary>
        /// <param name="action">The <see cref="GameAction"/> being performed. This variable can be compared against the interfaces and defined action models to find out what is being processed.</param>
        /// <returns>The <see cref="LazyResult"/> containing the modified authorization result.</returns>
        public LazyResult ShouldOverrideAuth(GameAction action) => LazyResult.Succeeded; // Don't change the authorization behaviour of this action
        #endregion
    }
}
