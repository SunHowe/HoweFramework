using System.Text;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 代码生成辅助类。
    /// </summary>
    public static class CodeGenerateHelper
    {
        /// <summary>
        /// 添加自动生成的头注释。
        /// </summary>
        /// <param name="stringBuilder">字符串构建器。</param>
        /// <returns>字符串构建器。</returns>
        public static StringBuilder AppendAutoGeneratedHeaderComment(this StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine("//------------------------------------------------------------------------------");
            stringBuilder.AppendLine("// <auto-generated>");
            stringBuilder.AppendLine("//     This code was generated by a tool.");
            stringBuilder.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            stringBuilder.AppendLine("//     the code is regenerated.");
            stringBuilder.AppendLine("// </auto-generated>");
            stringBuilder.AppendLine("//------------------------------------------------------------------------------");
            stringBuilder.AppendLine();
            return stringBuilder;
        }
    }
}
