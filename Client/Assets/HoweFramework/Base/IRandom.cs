namespace HoweFramework
{
    /// <summary>
    /// 随机数接口。
    /// </summary>
    public interface IRandom
    {
        /// <summary>
        /// 获取随机数[min, max)。
        /// </summary>
        /// <param name="min">最小值。</param>
        /// <param name="max">最大值。</param>
        /// <returns>随机数。</returns>
        int GetRandom(int min, int max);

        /// <summary>
        /// 获取随机数[0, max)。
        /// </summary>
        /// <param name="max">最大值。</param>
        /// <returns>随机数。</returns>
        int GetRandom(int max);
    }
}
