using Eco.Gameplay.Players;
using Eco.Gameplay.Systems.Messaging.Chat;
using Eco.Gameplay.Systems.Messaging.Chat.Commands;
using Eco.Shared.Localization;

namespace EcoChat
{
    [ChatCommandHandler]
    public class EchoCommand
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
                ChatManager.SendMessage(user, user, Localizer.DoStr("Invalid usage. You must specify a message."));
                return;
            }
            ChatManager.SendMessage(user, user, Localizer.DoStr(message));
        }
    }
}