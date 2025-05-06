using System;
using UnityEditor;
using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// EditorGUI 工具类。
    /// </summary>
    public static class EditorGUIUtility
    {
        /// <summary>
        /// 创建缩进级别改变范围。
        /// </summary>
        public static IndentLevelChangedScope MakeIndentLevelChangedScope(int modify)
        {
            return new IndentLevelChangedScope(modify);
        }

        /// <summary>
        /// 创建垂直布局的分组。
        /// </summary>
        public static VerticalGroupScope MakeVerticalScope(GUIStyle style, params GUILayoutOption[] options)
        {
            return new VerticalGroupScope(style, options);
        }

        /// <summary>
        /// 创建垂直布局的分组。
        /// </summary>
        public static VerticalGroupScope MakeVerticalScope(params GUILayoutOption[] options)
        {
            return new VerticalGroupScope(options);
        }

        /// <summary>
        /// 创建水平布局的分组。
        /// </summary>
        public static HorizontalGroupScope MakeHorizontalScope(GUIStyle style, params GUILayoutOption[] options)
        {
            return new HorizontalGroupScope(style, options);
        }

        /// <summary>
        /// 创建水平布局的分组。
        /// </summary>
        public static HorizontalGroupScope MakeHorizontalScope(params GUILayoutOption[] options)
        {
            return new HorizontalGroupScope(options);
        }

        /// <summary>
        /// 创建禁用范围。
        /// </summary>
        public static DisabledGroupScope MakeDisabledGroupScope(bool disable)
        {
            return new DisabledGroupScope(disable);
        }

        public readonly struct IndentLevelChangedScope : IDisposable
        {
            private readonly int m_OriginIndentLevel;
            
            public IndentLevelChangedScope(int modify)
            {
                m_OriginIndentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel += modify;
            }

            public void Dispose()
            {
                EditorGUI.indentLevel = m_OriginIndentLevel;
            }
        }

        public struct VerticalGroupScope : IDisposable
        {
            public VerticalGroupScope(GUIStyle style, params GUILayoutOption[] options)
            {
                EditorGUILayout.BeginVertical(style, options);
            }

            public VerticalGroupScope(params GUILayoutOption[] options)
            {
                EditorGUILayout.BeginVertical(options);
            }
            
            public void Dispose()
            {
                EditorGUILayout.EndVertical();
            }
        }

        public struct HorizontalGroupScope : IDisposable
        {
            public HorizontalGroupScope(GUIStyle style, params GUILayoutOption[] options)
            {
                EditorGUILayout.BeginHorizontal(style, options);
            }

            public HorizontalGroupScope(params GUILayoutOption[] options)
            {
                EditorGUILayout.BeginHorizontal(options);
            }

            public void Dispose()
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        public struct DisabledGroupScope : IDisposable
        {
            public DisabledGroupScope(bool disable)
            {
                EditorGUI.BeginDisabledGroup(disable);
            }

            public void Dispose()
            {
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}