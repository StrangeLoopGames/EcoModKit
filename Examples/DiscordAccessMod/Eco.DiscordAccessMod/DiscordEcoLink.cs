namespace Eco.DiscordAccessMod
{
    /// <summary>Represents a linked Discord/Eco user inside the plugin's configuration.</summary>
    public class DiscordEcoLink
    {
        /// <summary>Represents the user's Discord identifier.</summary>
        public ulong DiscordId { get; set; }
    
        /// <summary>Represents the user's Eco username.</summary>
        public string EcoUsername { get; set; }
    }
}
