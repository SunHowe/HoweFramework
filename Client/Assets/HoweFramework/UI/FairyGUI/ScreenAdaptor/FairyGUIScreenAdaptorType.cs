namespace HoweFramework
{
    /// <summary>
    /// 屏幕适配器类型。
    /// </summary>
    public enum FairyGUIScreenAdaptorType
    {
        /// <summary>
        /// 不使用适配器。
        /// </summary>
        None,

        /// <summary>
        /// 固定尺寸界面适配器，并始终保持水平居中
        /// </summary>
        ConstantHorizontalCenter,
        
        /// <summary>
        /// 固定尺寸界面适配器，并始终保持垂直居中
        /// </summary>
        ConstantVerticalCenter,
        
        /// <summary>
        /// 固定尺寸界面适配器，并始终保持居中
        /// </summary>
        ConstantCenter,
        
        /// <summary>
        /// 全屏界面适配器, 界面尺寸随屏幕尺寸变化, 提供安全区域适配功能 节点名固定为safeArea。
        /// </summary>
        FullScreen,
        
        /// <summary>
        /// 在安全区域内全屏适配，界面尺寸始终与安全区域尺寸相同且位置始终与安全区域位置相同。
        /// </summary>
        SafeAreaFullScreen,
    }
}

