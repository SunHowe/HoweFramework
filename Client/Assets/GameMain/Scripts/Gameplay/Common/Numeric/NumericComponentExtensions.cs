using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 属性组件扩展。
    /// </summary>
    public static class NumericComponentExtensions
    {
        /// <summary>
        /// 修改属性值。
        /// </summary>
        /// <param name="numericComponent">属性组件。</param>
        /// <param name="modifyDict">修改的属性值。</param>
        /// <param name="source">来源。不能为空。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public static void ModifyByKey(this NumericComponent numericComponent, Dictionary<int, long> modifyDict, object source, bool dispatchEvent = true)
        {
            if (modifyDict == null || modifyDict.Count == 0)
            {
                return;
            }

            foreach (var (id, value) in modifyDict)
            {
                numericComponent.ModifyByKey(id, value, source, dispatchEvent);
            }
        }

        /// <summary>
        /// 逆向修改属性值。
        /// </summary>
        /// <param name="numericComponent">属性组件。</param>
        /// <param name="modifyDict">修改的属性值。</param>
        /// <param name="source">来源。不能为空。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public static void ModifyReverseByKey(this NumericComponent numericComponent, Dictionary<int, long> modifyDict, object source, bool dispatchEvent = true)
        {
            if (modifyDict == null || modifyDict.Count == 0)
            {
                return;
            }

            foreach (var (id, value) in modifyDict)
            {
                numericComponent.ModifyByKey(id, -value, source, dispatchEvent);
            }
        }

        /// <summary>
        /// 修改属性值。
        /// </summary>
        /// <param name="numericComponent">属性组件。</param>
        /// <param name="modifyDict">修改的属性值。</param>
        /// <param name="source">来源。不能为空。</param>
        /// <param name="multiplier">倍数。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public static void ModifyByKey(this NumericComponent numericComponent, Dictionary<int, long> modifyDict, object source, int multiplier, bool dispatchEvent = true)
        {
            if (modifyDict == null || modifyDict.Count == 0 || multiplier == 0)
            {
                return;
            }

            foreach (var (id, value) in modifyDict)
            {
                numericComponent.ModifyByKey(id, value * multiplier, source, dispatchEvent);
            }
        }

        /// <summary>
        /// 逆向修改属性值。
        /// </summary>
        /// <param name="numericComponent">属性组件。</param>
        /// <param name="modifyDict">修改的属性值。</param>
        /// <param name="source">来源。不能为空。</param>
        /// <param name="multiplier">倍数。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public static void ModifyReverseByKey(this NumericComponent numericComponent, Dictionary<int, long> modifyDict, object source, int multiplier, bool dispatchEvent = true)
        {
            if (modifyDict == null || modifyDict.Count == 0 || multiplier == 0)
            {
                return;
            }

            foreach (var (id, value) in modifyDict)
            {
                numericComponent.ModifyByKey(id, -value * multiplier, source, dispatchEvent);
            }
        }
    }
}
