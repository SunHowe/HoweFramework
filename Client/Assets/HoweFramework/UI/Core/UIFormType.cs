namespace HoweFramework
{
    /// <summary>
    /// 界面类型。
    /// </summary>
    public enum UIFormType
    {
        /// <summary>
        /// 主界面。同时只能存在一个主界面，当打开一个主界面时，会关闭前面的所有界面。
        /// </summary>
        Main = 0,

        /// <summary>
        /// 普通界面。打开一个界面时，会隐藏前面的所有界面。
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 弹窗。
        /// </summary>
        Popup = 2,

        /// <summary>
        /// 固定界面，不受其他界面打开逻辑影响。
        /// </summary>
        Fixed = 4,
    }
}

