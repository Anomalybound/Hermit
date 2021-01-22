using System.Linq;
using Hermit.Views;
using UnityEditor;
using UnityEngine;

namespace Hermit.Common
{
    [CustomEditor(typeof(ViewModelProvider), true)]
    public class ViewModelProviderEditor : Editor
    {
        private ViewModelProvider provider;

        private void Awake()
        {
            provider = target as ViewModelProvider;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using var check = new EditorGUI.ChangeCheckScope();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("provideType"));
            
            var typeName = provider.ViewModelTypeName;
            var viewModelTypes = AssemblyHelper.GetInheritancesInAppDomain(typeof(ViewModel));
            var viewModelTypeNames = viewModelTypes.Select(t => t.FullName).ToArray();
            var targetIndex = viewModelTypeNames.ToList().IndexOf(typeName);
            targetIndex = EditorGUILayout.Popup(new GUIContent("View Model Type"), targetIndex, viewModelTypeNames);
            if (targetIndex > -1) { provider.ViewModelTypeName = viewModelTypeNames[targetIndex]; }

            if (check.changed) { serializedObject.ApplyModifiedProperties(); }
        }
    }
}
