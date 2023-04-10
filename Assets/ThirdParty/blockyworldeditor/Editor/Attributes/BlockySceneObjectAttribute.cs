using UnityEngine;

namespace PeartreeGames.Blocky2DEditor.Editor
{
    public class BlockySceneObjectAttribute : PropertyAttribute
    {
        public readonly string backingPropertyName;

        public BlockySceneObjectAttribute(string backingPropertyName)
        {
            this.backingPropertyName = backingPropertyName;
        }
    }
}