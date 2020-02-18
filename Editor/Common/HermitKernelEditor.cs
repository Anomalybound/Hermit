using System.Reflection;
using Hermit.Common;
using Hermit.Injection;
using Hermit.View;
using UnityEditor;
using UnityEngine;

namespace Hermit
{
    [CustomEditor(typeof(HermitKernel))]
    public class HermitKernelEditor : Editor
    {
        [MenuItem("Hermit/Quick Scene Setup %#k")]
        public static void Setup()
        {
            var kernel = FindObjectOfType<HermitKernel>();
            if (kernel != null)
            {
                Debug.LogWarning("Kernel already exists.");
                return;
            }

            var kernelObj = new GameObject("Hermit Kernel",
                typeof(HermitKernel), typeof(HermitKernelServiceProvider), typeof(HermitUIServiceProvider)
            );
            kernel = kernelObj.GetComponent<HermitKernel>();
            var kernelServiceProvider = kernelObj.GetComponent<HermitKernelServiceProvider>();
            var uiServiceProvider = kernelObj.GetComponent<HermitUIServiceProvider>();
            uiServiceProvider.uiRootPrefab = Resources.Load<UIRoot>("UI Root");

            var array = new MonoServiceProvider[]
            {
                kernelServiceProvider, uiServiceProvider
            };
            var fieldInfo = typeof(HermitKernel).GetField("serviceProviders",
                BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo?.SetValue(kernel, array);

            EditorUtility.SetDirty(kernelObj);
        }

        private GUIStyle _versionDisplay;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (_versionDisplay == null)
            {
                _versionDisplay = new GUIStyle(EditorStyles.helpBox) {alignment = TextAnchor.MiddleCenter};
            }

            EditorGUILayout.LabelField($"Hermit Version: {Her.Version}", _versionDisplay);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("injectSceneObjects"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("serviceProviders"), true);
                if (check.changed) { serializedObject.ApplyModifiedProperties(); }
            }
        }
    }
}
