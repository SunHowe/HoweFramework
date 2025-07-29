using UnityEditor;
using UnityEngine;

namespace HoweFramework.Editor
{
    public static class YooAssetMenu
    {
        private const string MenuItemSimulateAssetBundle = "Game Framework/Simulate AssetBundle";
        private const string PlayerPrefsSimulateAssetBundle = "SimulateAssetBundle";

        [MenuItem(MenuItemSimulateAssetBundle)]
        private static void SimulateAssetBundle()
        {
            var isSimulateAssetBundle = PlayerPrefs.GetInt(PlayerPrefsSimulateAssetBundle, 0) == 1;
            PlayerPrefs.SetInt(PlayerPrefsSimulateAssetBundle, isSimulateAssetBundle ? 0 : 1);
        }

        [MenuItem(MenuItemSimulateAssetBundle, true)]
        private static bool IsSimulateAssetBundle()
        {
            bool isSimulateAssetBundle = PlayerPrefs.GetInt(PlayerPrefsSimulateAssetBundle, 0) == 1;
            Menu.SetChecked(MenuItemSimulateAssetBundle, isSimulateAssetBundle);
            return isSimulateAssetBundle;
        }
    }
}