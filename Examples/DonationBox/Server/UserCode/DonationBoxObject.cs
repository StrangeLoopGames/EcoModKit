/*
 * This Donation box object demonstrates the use of the auto-generated UI for components, check the documentation and attributes assigned to the 
 * members and compare to the UI it creates to get a better understanding of how the system works. A donation box will allow authorized players
 * or the object's owner to adjust the donation amount and the bank account the donations will be placed into. Non authorized players
 * will be presented with a page containing information about how much and where their donation is going along with a "Donate" button that
 * will take money from their selected account and transfer it to the account set previously.
 */

namespace Eco.Example.DonationBox
{
    using System.ComponentModel;
    using Eco.Core.Controller;
    using Eco.Core.Items;
    using Eco.Core.Utils;
    using Eco.Gameplay.Auth;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.Economy;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Systems;
    using Eco.Gameplay.Systems.Messaging.Chat.Commands;
    using Eco.Gameplay.Utils;
    using Eco.Gameplay.Wires;
    using Eco.Shared.Localization;
    using Eco.Shared.Math;
    using Eco.Shared.Networking;
    using Eco.Shared.Serialization;
    using Eco.Simulation.Time;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Gameplay.Aliases;
    using Eco.Shared.Items;
    using Eco.Gameplay.GameActions;
    using Eco.Gameplay;
    using Eco.Gameplay.UI;
    using Eco.Shared.IoC;

    /// <summary>
    /// WorldObjectItem representing our Donation box object. This can be spawned into the world using the "/give Donation Box" command.
    /// </summary>
    [Serialized]
    [LocDisplayName("Donation Box")]
    [Category("Hidden")]
    public class DonationBoxItem : WorldObjectItem<DonationBoxObject>
    {
        public override LocString DisplayDescription => Localizer.DoStr("Toggle on any touching wires and electronic objects.");
    }

    [Serialized]
    [RequireComponent(typeof(DonationBoxComponent))]
    [RequireComponent(typeof(AttachmentComponent))]
    [RequireComponent(typeof(MustBeOwnedComponent))]
    [RequireComponent(typeof(PropertyAuthComponent))]
    public class DonationBoxObject : WorldObject
    {
        static DonationBoxObject()
        {
            AddOccupancyList(typeof(DonationBoxObject), new BlockOccupancy(Vector3i.Zero, typeof(BuildingWorldObjectBlock)));
        }

        public override LocString DisplayName => Localizer.DoStr("Donation Box");
    }

    /// <summary>
    /// Here we define our custom WorldObjectComponent instance. World Object Components allow us to define custom interactable behaviours for the
    /// world objects they are attached to. In this case we add UI elements to allow users to donate to another user's bank account.
    /// </summary>
    [Serialized, AutogenClass, LocDisplayName("Donation")]
    [Tag("Economy"), Category("Hidden")]
    public class DonationBoxComponent : WorldObjectComponent
    {
        /*
         * Here we define the properties of our componet. Using a mix of SyncToView, Autogen, and Eco
         * these properties will be shown to the interacting user as interactable UI options. Due to a potential bug in Eco 9.5
         * these properties must be setup with a getter/setter that calls the Changed network method to notify the client that their
         * change request was accepted. Under normal usage simply having the property with the required attributes would be enough.
         */
        [Serialized] private Currency currencyHandle;
        [SyncToView, Autogen, AutoRPC] 
        public Currency Currency                   
        { 
            get => this.currencyHandle; 
            set 
            { 
                if (value == this.currencyHandle) return;
                this.currencyHandle = value; 
                this.Changed(nameof(Currency));
            } 
        }

        float donationAmount;
        [SyncToView, Autogen, AutoRPC, Serialized] 
        public float DonationAmount
        {
            get => donationAmount;
            set
            {
                if (value == this.donationAmount) return;
                this.donationAmount = value;
                this.Changed(nameof(DonationAmount));
            }
        }

        BankAccount targetBankAccount;
        [Eco]
        public BankAccount TargetBankAccount
        {
            get => targetBankAccount;
            set
            {
                if (value == this.targetBankAccount) return;
                this.targetBankAccount = value;
                this.Changed(nameof(TargetBankAccount));
            }
        }

        // Initialize our donation box with a default donation amount of 1 currency unit
        public DonationBoxComponent()
        {
           this.DonationAmount = 1f;
        }

        /// <summary>
        /// Here we define our donation button that will be displayed to "guest" users of the Donation Box. A guest user is any user that is not the 
        /// object's owner or an authorized individual. 
        /// 
        /// By using a combination of RPC and AutoGen this method will appear as a clickable button matching the method's name. The OwnerHidden attribute
        /// ensures this button is not visible to the box's owner or an authorized individual. GuestEditable does the inverse of this and allows non authorized
        /// players to see the button.
        /// </summary>
        /// <param name="player">Player clicking the button.</param>
        [RPC, Autogen, OwnerHidden, GuestEditable] 
        public void Donate(Player player)
        {
            if (this.Authed(player))           { player.ErrorLocStr("Since you're authorized, you cannot contribute a donation."); return; }
            if (this.Parent.Owners == null)    { player.ErrorLocStr("Object does not have an owner, cannot be used."); return; }

            if (Transfers.TransferNow(new TransferData {
                Amount                = this.DonationAmount,
                SourceAccount         = player?.User.BankAccount,
                TargetAccount         = this.TargetBankAccount,
                Sender                = player?.User,
                Receiver              = this.Parent.Owners?.OneUser(),
                TransferType          = TransferType.DirectTransfer,
                TransferDescription   = Localizer.Do($"Paid donation at {this.Parent.UILink()}"),
                Currency              = this.Currency
            })) player.InfoBoxLocStr("Thank you for your donation!");
        }

        public override void OnCreate() => this.Currency = CurrencyManager.GetPlayerCurrency(this.Parent.NameOfCreator);

        /// <summary>Helper utility function to check if a user is authorized to interact with this object.</summary>
        /// <param name="player">Player to check authorization against</param>
        bool Authed(Player player)
        {
            IAlias @alias = player != null ? player.User : null;
            return ServiceHolder<IAuthManager>.Obj.IsAuthorized(this.Parent, alias, AccessType.FullAccess, null);
        }
    }
}
