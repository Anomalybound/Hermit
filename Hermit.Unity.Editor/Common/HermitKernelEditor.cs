using System.Linq;
using System.Reflection;
using Hermit.Common;
using Hermit.Injection;
using UnityEditor;
using UnityEngine;

namespace Hermit
{
    [CustomEditor(typeof(HermitKernel))]
    public class HermitKernelEditor : Editor
    {
        public const string HERMIT_DOTWEEN = "HERMIT_DOTWEEN";

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
                typeof(HermitKernel), typeof(HermitKernelServiceProvider), typeof(HermitDataBindingServiceProvider)
            );
            kernel = kernelObj.GetComponent<HermitKernel>();

            var array = new MonoServiceProvider[]
            {
                kernelObj.GetComponent<HermitKernelServiceProvider>(),
                kernelObj.GetComponent<HermitDataBindingServiceProvider>()
            };
            var fieldInfo =
                typeof(HermitKernel).GetField("ServiceProviders", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo?.SetValue(kernel, array);

            EditorUtility.SetDirty(kernelObj);
        }

        private GUIStyle VersionDisplay;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (VersionDisplay == null)
            {
                VersionDisplay = new GUIStyle(EditorStyles.helpBox) {alignment = TextAnchor.MiddleCenter};
            }

            EditorGUILayout.LabelField($"Hermit Version: {Her.Version}", VersionDisplay);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("InjectSceneObjects"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ServiceProviders"), true);
                if (check.changed) { serializedObject.ApplyModifiedProperties(); }
            }
        }
    }
}