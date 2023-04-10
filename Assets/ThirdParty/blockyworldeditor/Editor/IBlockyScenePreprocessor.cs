using UnityEngine.SceneManagement;

namespace PeartreeGames.Blocky2DEditor.Editor
{
    public interface IBlockyScenePreprocessor
    {
        public int Order { get; }
        public void ProcessScene(BlockyEditorWindow window, Scene scene);
        public void RevertScene(BlockyEditorWindow window, Scene scene);
    }
}