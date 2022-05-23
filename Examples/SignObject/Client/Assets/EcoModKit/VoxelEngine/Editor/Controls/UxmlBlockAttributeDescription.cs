using UnityEngine.UIElements;

namespace Assets.EcoModKit.VoxelEngine.Editor.Controls
{
    internal class UxmlBlockAttributeDescription : TypedUxmlAttributeDescription<Block>
    {
        public UxmlBlockAttributeDescription()
        {
            base.type = "Block";
            base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
            base.defaultValue = null;
        }

        public override Block GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
        {
            return GetValueFromBag(bag, cc, (string val, Block defaultVal) => defaultVal, base.defaultValue);
        }

        public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref Block value)
        {
            return TryGetValueFromBag(bag, cc, (string val, Block defaultVal) => defaultVal, base.defaultValue, ref value);
        }
    }
}