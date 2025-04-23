using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// Unity安全区域辅助工具。
    /// </summary>
    internal sealed class UnitySafeAreaHelper : SafeAreaHelperBase
    {
        public UnitySafeAreaHelper()
        {
        }

        public override void Dispose()
        {
        }

        public override void OnUpdate()
        {
            SetSafeArea(Screen.safeArea);
        }
    }
}
