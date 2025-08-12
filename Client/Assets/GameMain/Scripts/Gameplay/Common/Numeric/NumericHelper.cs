namespace GameMain
{
    /// <summary>
    /// 数值辅助类。
    /// </summary>
    public static class NumericHelper
    {
        /// <summary>
        /// 编码属性键值。
        /// </summary>
        public static int EncodeNumericKey(int id, NumericSubType subType)
        {
            return id << 4 | (int)subType;
        }

        /// <summary>
        /// 解码属性键值。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <returns>属性id和属性子类型。</returns>
        public static (int, NumericSubType) DecodeNumericKey(int key)
        {
            return (GetNumericId(key), GetNumericSubType(key));
        }

        /// <summary>
        /// 获取属性id。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <returns>属性id。</returns>
        public static int GetNumericId(int key)
        {
            return key >> 4;
        }

        /// <summary>
        /// 获取属性子类型。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <returns>属性子类型。</returns>
        public static NumericSubType GetNumericSubType(int key)
        {
            return (NumericSubType)(key & 0x0F);
        }
    }
}