using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace CustomElements
{
    public class BlockEditorElement : VisualElement
    {
        public Block Block { get; set; }

        public BlockEditorElement()
        {
            this.hierarchy.Add(new IMGUIContainer(OnGUI));
        }

        private void OnGUI()
        {
            if (Block != null)
                BlockEditor.DrawBlockEditor(Block);
        }

        public new class UxmlFactory : UxmlFactory<BlockEditorElement, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription someNumberAttr = new UxmlIntAttributeDescription { name = "some-number" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var blockElement = (BlockEditorElement)ve;
            }
        }
    }
}
