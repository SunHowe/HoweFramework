using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HoweFramework.Editor
{
    [CustomEditor(typeof(ObjectCollector))]
    internal sealed class ObjectCollectorInspector : InspectorBase
    {
        /// <summary>
        /// 每页最大的item数量。
        /// </summary>
        private const int PAGE_ITEM_LIMIT = 10;

        private int m_PageIndex = 0;

        private string m_AddKey = string.Empty;
        private Object m_AddValue = null;

        [MenuItem("GameObject/HoweFramework/@(Alt+A)Add Object Collector &a", false, 0)]
        static void AddObjectCollector()
        {
            var gameObject = Selection.objects.FirstOrDefault() as GameObject;
            if (gameObject == null)
            {
                return;
            }

            if (gameObject.GetComponent<ObjectCollector>() != null)
            {
                return;
            }
            
            gameObject.AddComponent<ObjectCollector>();
        }

        public static bool UpdateObjectCollector(ObjectCollector t)
        {
            var isModify = false;
            
            var objectExports = t.GetComponentsInChildren<ObjectExport>();
            foreach (var objectExport in objectExports)
            {
                var exportName = !string.IsNullOrEmpty(objectExport.ExportName) ? objectExport.ExportName : objectExport.name;
                var exportType = System.Type.GetType(objectExport.ExportTypeName);
                if (exportType == null)
                {
                    Debug.LogError($"无效的组件类型在节点{objectExport.name}上: {objectExport.ExportTypeName}");
                    continue;
                }

                Object exportObject;

                if (exportType == typeof(GameObject))
                {
                    exportObject = objectExport.gameObject;
                }
                else if (typeof(Component).IsAssignableFrom(exportType))
                {
                    exportObject = objectExport.GetComponent(exportType);
                }
                else
                {
                    Debug.LogError($"无效的组件类型在节点{objectExport.name}上: {objectExport.ExportTypeName}");
                    continue;
                }

                if (exportObject == null)
                {
                    Debug.LogError($"无效的组件类型在节点{objectExport.name}上: {objectExport.ExportTypeName}");
                    continue;
                }

                var index = t.ObjectNameList.IndexOf(exportName);
                if (index >= 0)
                {
                    if (t.ObjectList[index] != exportObject)
                    {
                        Debug.LogError($"重复的挂件名称: {exportName}");
                    }
                                    
                    continue;
                }
                                
                t.ObjectNameList.Add(exportName);
                t.ObjectList.Add(exportObject);

                isModify = true;
            }
            
            return isModify;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            var t = (ObjectCollector)target;
            t.ObjectList ??= new List<Object>();
            t.ObjectNameList ??= new List<string>();

            var count = Math.Min(t.ObjectNameList.Count, t.ObjectList.Count);
            var pageCount = Mathf.CeilToInt((float)count / PAGE_ITEM_LIMIT);
            m_PageIndex = pageCount > 0 ? Mathf.Clamp(m_PageIndex, 0, pageCount - 1) : 0;

            var isModify = false;

            using (EditorGUIUtility.MakeDisabledGroupScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                using (EditorGUIUtility.MakeVerticalScope("box"))
                {
                    // 绘制添加按钮。
                    using (EditorGUIUtility.MakeHorizontalScope("box"))
                    {
                        // key框, value框, +
                        m_AddKey = EditorGUILayout.TextField(m_AddKey);

                        var value = EditorGUILayout.ObjectField(m_AddValue, typeof(Object), true);
                        if (value != m_AddValue)
                        {
                            m_AddValue = value;
                            if (value != null && string.IsNullOrEmpty(m_AddKey))
                            {
                                // 默认传入节点的名字。
                                m_AddKey = value.name;
                            }
                        }

                        // key value有一个为空时禁用+按钮
                        using (EditorGUIUtility.MakeDisabledGroupScope(string.IsNullOrEmpty(m_AddKey) || m_AddValue == null))
                        {
                            if (GUILayout.Button("+", GUILayout.Width(20)))
                            {
                                t.ObjectNameList.Add(m_AddKey);
                                t.ObjectList.Add(m_AddValue);

                                m_AddKey = string.Empty;
                                m_AddValue = null;

                                ++count;

                                isModify = true;
                            }
                        }
                    }

                    // 绘制组件字典对。
                    using (EditorGUIUtility.MakeVerticalScope("box"))
                    {
                        // 限制每页的显示数量。
                        var startIndex = m_PageIndex * PAGE_ITEM_LIMIT;

                        for (var offset = 0; offset < PAGE_ITEM_LIMIT; ++offset)
                        {
                            var index = startIndex + offset;
                            if (index >= count)
                            {
                                break;
                            }

                            var key = t.ObjectNameList[index];
                            var value = t.ObjectList[index];

                            // 绘制Key、value对的字典。
                            using (EditorGUIUtility.MakeHorizontalScope())
                            {
                                t.ObjectNameList[index] = EditorGUILayout.TextField(key);
                                t.ObjectList[index] = EditorGUILayout.ObjectField(value, typeof(Object), true);

                                if (GUILayout.Button("X", GUILayout.Width(20)))
                                {
                                    t.ObjectNameList.RemoveAt(index);
                                    t.ObjectList.RemoveAt(index);

                                    --count;

                                    isModify = true;
                                }
                            }
                        }
                    }

                    // 绘制分页按钮。
                    using (EditorGUIUtility.MakeHorizontalScope("box"))
                    {
                        GUILayout.FlexibleSpace();
                        // EditorGUILayout.Space();
                        
                        // 绘制上一页按钮。在index已经为0时禁用。
                        using (EditorGUIUtility.MakeDisabledGroupScope(m_PageIndex <= 0))
                        {
                            if (GUILayout.Button("<<", GUILayout.Width(50)))
                            {
                                --m_PageIndex;
                            }
                        }
                        
                        GUILayout.FlexibleSpace();
                        
                        // 绘制 当前页数/总页数 的Label
                        EditorGUILayout.LabelField($"{m_PageIndex + 1}/{pageCount}", GUILayout.MaxWidth(60));
                        
                        GUILayout.FlexibleSpace();

                        // 绘制下一页按钮。在index已经为最后一页时禁用。
                        using (EditorGUIUtility.MakeDisabledGroupScope(m_PageIndex >= pageCount - 1))
                        {
                            if (GUILayout.Button(">>", GUILayout.Width(50)))
                            {
                                ++m_PageIndex;
                            }
                        }
                        
                        GUILayout.FlexibleSpace();
                    }

                    // 绘制功能按钮区
                    using (EditorGUIUtility.MakeVerticalScope("box"))
                    {
                        if (GUILayout.Button("清空收集器"))
                        {
                            t.ObjectNameList.Clear();
                            t.ObjectList.Clear();

                            count = 0;
                            m_PageIndex = 0;

                            isModify = true;
                        }

                        if (GUILayout.Button("收集所有Export组件(自己和子节点)"))
                        {
                            if (UpdateObjectCollector(t))
                            {
                                isModify = true;
                            }
                        }
                    }
                }
            }

            if (isModify)
            {
                EditorUtility.SetDirty(t);
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }
    }
}