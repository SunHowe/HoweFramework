namespace HoweFramework
{
    /// <summary>
    /// 自定义销毁接口。实现该接口的GameObject会在调用XX.Destroy时触发CustomDestroy，未实现则调用Object.Destroy。
    /// </summary>
    public interface ICustomDestroy
    {
        /// <summary>
        /// 自定义销毁。
        /// </summary>
        void CustomDestroy();
    }
}