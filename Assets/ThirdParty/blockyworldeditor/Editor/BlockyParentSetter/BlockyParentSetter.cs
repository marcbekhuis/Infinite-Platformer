using UnityEngine;

namespace PeartreeGames.Blocky2DEditor.Editor
{
    public abstract class BlockyParentSetter : ScriptableObject
    {
        public abstract Transform GetParent(BlockyObject block);
        public virtual void SetVisualization(Vector3Int target, int gridHeight) {}
    }
}