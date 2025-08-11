using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using HoweFramework;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 日志重定向。
    /// </summary>
    public static class EditorLogRedirection
    {
        private static readonly Regex LogRegex = new Regex(@" \(at (.+\.cs)\:(\d+)\)| in (.+\.cs)\:(\d+)");

        private static string[] IgnorePaths = new string[]
        {
            "Assets/HoweFramework/Log.cs",
        };

        [OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceId, int line)
        {
            string name = EditorUtility.InstanceIDToObject(instanceId).name;

            string msg = GetSelectedStackTrace();
            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }

            Match match = LogRegex.Match(msg);
            string path;

            do
            {
                if (!match.Success)
                {
                    return false;
                }

                if (match.Groups[1].Success)
                {
                    path = match.Groups[1].Value.Replace("\\", "/");
                    line = int.Parse(match.Groups[2].Value);
                }
                else
                {
                    path = match.Groups[3].Value.Replace("\\", "/");
                    line = int.Parse(match.Groups[4].Value);
                }

                match = match.NextMatch();
            }
            while (string.IsNullOrEmpty(path) || Array.IndexOf(IgnorePaths, path) != -1);

            InternalEditorUtility.OpenFileAtLineExternal(path, line);
            return true;
        }


        /// <summary>
        /// 获取点击Log的文本信息
        /// </summary>
        /// <returns></returns>
        private static string GetSelectedStackTrace()
        {
            Assembly editorWindowAssembly = typeof(EditorWindow).Assembly;
            if (editorWindowAssembly == null)
            {
                return null;
            }

            System.Type consoleWindowType = editorWindowAssembly.GetType("UnityEditor.ConsoleWindow");
            if (consoleWindowType == null)
            {
                return null;
            }

            FieldInfo consoleWindowFieldInfo =
                consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            if (consoleWindowFieldInfo == null)
            {
                return null;
            }

            EditorWindow consoleWindow = consoleWindowFieldInfo.GetValue(null) as EditorWindow;
            if (consoleWindow == null)
            {
                return null;
            }

            if (consoleWindow != EditorWindow.focusedWindow)
            {
                return null;
            }

            FieldInfo activeTextFieldInfo =
                consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            if (activeTextFieldInfo == null)
            {
                return null;
            }

            return (string)activeTextFieldInfo.GetValue(consoleWindow);
        }
    }
}