using Eco.Core.Controller;
using Eco.Core.PropertyHandling;
using Eco.Gameplay.Objects;
using Eco.Shared.Localization;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;

namespace Eco.Mods.Components
{
    [Serialized]
    [AutogenClass]
    [LocDisplayName("Component Interface")]
    [LocDescription("An example interface for a component.")]
    [NoIcon]
    [CreateComponentTabLoc("Interface")]
    public partial class ComponentInterfaceExample : WorldObjectComponent
    {
        // A read only property formated into a LocString

        [LocDisplayName("Readonly Property"), LocDescription("This property is readonly.")]
        [SyncToView, Notify, Autogen] public string ReadonlyPropertyDisplay => Localizer.DoStr($"This property is set to {ChangingProperty}.");  // formats the value into a string
        // Called when a user trys to set the value.  Only updates value to ensure ui is up to date.
        [RPC] public void SetReadonlyPropertyDisplay(string value) => this.FirePropertyChanged(nameof(ReadonlyPropertyDisplay));

        [Serialized] private float _ChangingProperty = 0;
        public float ChangingProperty 
        { 
            get => _ChangingProperty;
            set 
            {
                if (_ChangingProperty == value) return;
                // Fires all displays that use this value to update the client
                this.FirePropertyChanged(nameof(ReadonlyPropertyDisplay));

                _ChangingProperty = value;
            } 
        }

        // A read only property as raw

        [Serialized] private float _ChangingProperty_2 = 0;

        [LocDisplayName("Readonly Raw Property"), LocDescription("This property is readonly from its raw value format.")]
        [SyncToView, Notify, Autogen] public float ChangingProperty_2
        {
            get => _ChangingProperty_2;
            set
            {
                if (_ChangingProperty_2 == value) return;
                // Fires all displays that use this value to update the client
                this.FirePropertyChanged(nameof(ChangingProperty_2));

                _ChangingProperty_2 = value;
            }
        }
        [RPC] public void SetChangingProperty_2(string value) => this.FirePropertyChanged(nameof(ChangingProperty_2));

        // A static property

        [LocDisplayName("Static Property"), LocDescription("This property is static and wont update.")]
        [SyncToView, Notify, Autogen] public string StaticPropertyDisplay => Localizer.DoStr($"This property is set to {Property}.");  // formats the value into a string
        // Called when a user trys to set the value.  Does nothing to ensure its read only.
        [RPC] public void SetStaticPropertyDisplay(string value) => this.FirePropertyChanged(nameof(StaticPropertyDisplay));

        public float Property => 18;

        // A editable property

        [Serialized] private string _Words = Localizer.DoStr("Potato");

        [LocDisplayName("Editable Words"), LocDescription("This property can be changed per your will.")]
        [SyncToView, Notify, Autogen]
        public string Words
        {
            get => _Words;
            set
            {
                if (_Words == value) return;
                // Fires all displays that use this value to update the client
                this.FirePropertyChanged(nameof(Words));

                _Words = value;
            }
        }
        [RPC] public void SetWords(string value) => Words = value;

        public override void Tick()
        {
            ChangingProperty += 1;
            ChangingProperty_2 -= 1.5f;
            base.Tick();
        }
    }
}