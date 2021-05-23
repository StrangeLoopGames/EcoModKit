namespace Eco
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems.Chat;
    using Eco.Mods.TechTree;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Networking;
    using Eco.Shared.Services;
    using Eco.Shared.Utils;
    using Eco.World.Blocks;

    public class EchoCommand : IChatCommandHandler
    {
        /// <summary>
        /// Chat command for server administrators for repeating a message back to the sender
        /// </summary>
        /// <param name="user">User sending the message</param>
        /// <param name="message">Message to be repeated</param>
        [ChatCommand("Repeats what you tell it back to you", ChatAuthorizationLevel.Admin)]
        public static void Echo(User user, string message = "")
        {
            if (message.Length == 0)
            {
                ChatManager.ServerMessageToPlayer(Localizer.DoStr("Invalid usage. You must specify a message."), user);
                return;
            }
            ChatManager.ServerMessageToPlayer(Localizer.DoStr(message), user);
        }
    }
}
