using System.Collections.Generic;
using UnityEngine;

namespace Assets.Editor.Blocks
{
    public class BlockEditorModel : ScriptableObject
    {
        public List<Block> Blocks;
        public List<BlockSet> BlockSets;
    }
}