using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.EcoModKit.VoxelEngine.Editor.Controls
{
    public class BlockElement : BindableElement, INotifyValueChanged<Block>
    {
        TextElement t;

        public new class UxmlFactory : UxmlFactory<BlockElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            private UxmlBlockAttributeDescription block = new UxmlBlockAttributeDescription { name = "block" };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get { yield break; } }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                BlockElement textElement = (BlockElement)ve;
                textElement.block = block.GetValueFromBag(bag, cc);
            }
        }

        Block block;
        private readonly Label label;
        private readonly Image preview;

        public Block value
        {
            get => block;
            set
            {
                if (block != value)
                {
                    using (ChangeEvent<Block> changeEvent = ChangeEvent<Block>.GetPooled(block, value))
                    {
                        changeEvent.target = this;
                        ((INotifyValueChanged<Block>)this).SetValueWithoutNotify(value);
                        SendEvent(changeEvent);
                    }
                }
            }
        }

        public BlockElement()
        {
            var root = new VisualElement() { name = "ListItem" };
            root.Add(new Image() { name = "Preview" });
            root.Add(new Label() { name = "Name" });
            base.hierarchy.Add(root);


            label = root.Q("Name") as Label;
            preview = root.Q("Preview") as Image;
        }

        public void SetValueWithoutNotify(Block newValue)
        {
            block = newValue;
            Refresh();
        }

        public void Refresh()
        {
            label.text = block.Name;
        }
    }
}
