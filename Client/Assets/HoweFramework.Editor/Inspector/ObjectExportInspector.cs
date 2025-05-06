using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HoweFramework.Editor
{
    [CustomEditor(typeof(ObjectExport))]
    internal sealed class ObjectExportInspector : InspectorBase
    {
        private readonly List<Component> m_ComponentBuffer = new List<Component>();
        private readonly HashSet<System.Type> m_ComponentTypeBuffer = new HashSet<System.Type>();
        private readonly List<System.Type> m_IgnoreTypes = new List<System.Type>()
        {
            typeof(ObjectExport),
        };
        
        private readonly List<string> m_TypeNames = new List<string>();
        private readonly List<string> m_FullTypeNames = new List<string>();
        private readonly List<System.Type> m_Types = new List<System.Type>();

        [MenuItem("GameObject/HoweFramework/@(Alt+S)Add Object Export &s", false, 0)]
        static void AddObjectExport()
        {
            var gameObject = Selection.objects.FirstOrDefault() as GameObject;
            if (gameObject == null)
            {
                return;
            }
            
            gameObject.AddComponent<ObjectExport>();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();
            
            var t = (ObjectExport)target;
            var isModify = false;

            using (EditorGUIUtility.MakeDisabledGroupScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                using var _ = EditorGUIUtility.MakeVerticalScope("box");

                if (t.ExportWithTypeNameSuffix)
                {
                    EditorGUILayout.LabelField("导出键值", t.ExportName);
                }
                else
                {
                    var exportName = EditorGUILayout.TextField("导出键值", t.ExportName);
                    if (exportName != t.ExportName)
                    {
                        t.ExportName = exportName;
                        isModify = true;
                    }
                }

                var index = m_FullTypeNames.IndexOf(t.ExportTypeName);
                var newIndex = EditorGUILayout.Popup("导出组件类型", index >= 0 ? index : 0, m_TypeNames.ToArray());
                if (newIndex != index)
                {
                    t.ExportTypeName = m_FullTypeNames[newIndex];
                    isModify = true;
                }
                
                var exportSuffix = EditorGUILayout.Toggle("使用导出类型名作为后缀", t.ExportWithTypeNameSuffix);
                if (exportSuffix != t.ExportWithTypeNameSuffix)
                {
                    t.ExportWithTypeNameSuffix = exportSuffix;
                    isModify = true;
                }

                if (exportSuffix && string.IsNullOrEmpty(t.ExportName))
                {
                    t.ExportName = t.gameObject.name + m_TypeNames[newIndex];
                    isModify = true;
                }

                var gather = t.gameObject.GetComponentInSelfOrParent<ObjectCollector>();
                if (gather == null)
                {
                    EditorGUILayout.HelpBox("父节点不存在ObjectCollector组件，请检查是否是个无用的导出配置", MessageType.Warning);
                }
                else
                {
                    if (GUILayout.Button("更新ObjectCollector"))
                    {
                        if (ObjectCollectorInspector.UpdateObjectCollector(gather))
                        {
                            EditorUtility.SetDirty(gather);
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

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void OnEnable()
        {
            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_ComponentBuffer.Clear();
            m_ComponentTypeBuffer.Clear();
            
            var t = (ObjectExport)target;
            t.GetComponents(m_ComponentBuffer);
            
            m_Types.Clear();
            m_FullTypeNames.Clear();
            m_TypeNames.Clear();
            
            foreach (var component in m_ComponentBuffer)
            {
                var componentType = component.GetType();
                if (m_IgnoreTypes.Contains(componentType))
                {
                    continue;
                }
                
                if (!m_ComponentTypeBuffer.Add(componentType))
                {
                    continue;
                }

                m_Types.Add(componentType);
            }
            
            m_Types.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
            m_Types.Insert(0, typeof(GameObject));
            foreach (var type in m_Types)
            {
                m_TypeNames.Add(type.Name);
                m_FullTypeNames.Add(type.AssemblyQualifiedName);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}