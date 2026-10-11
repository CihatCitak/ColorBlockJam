using Game.Core.Utilities;
using UnityEditor;

namespace Game.PlayerSystem
{
    internal static class PlayerDataMenu
    {
        private const string MenuPath = "ColorBlock/Reset Player Data";

        [MenuItem(MenuPath)]
        private static void ResetPlayerData()
        {
            if (EditorUtility.DisplayDialog("Reset Player Data", "Delete the saved level, coins and lives?", "Reset", "Cancel"))
                JsonFileUtility.Delete(PlayerDataStore.FileName);
        }

        [MenuItem(MenuPath, true)]
        private static bool CanResetPlayerData()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode;
        }
    }
}
