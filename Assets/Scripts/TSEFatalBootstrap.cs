using UnityEngine;

namespace TSEFatal
{
    public static class TSEFatalBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartGame()
        {
            if (Object.FindAnyObjectByType<GameFlow>() != null) return;

            var game = new GameObject("TSE Fatal - Game Flow");
            game.AddComponent<GameFlow>();
            Object.DontDestroyOnLoad(game);
        }
    }
}
