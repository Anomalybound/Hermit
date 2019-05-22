using System.Linq;
using System.Reflection;
using Hermit.Common;
using Hermit.Injection;
using UnityEditor;
using UnityEngine;

namespace Hermit
{
    public class HermitKernelEditor
    {
        public const string HERMIT_DOTWEEN = "HERMIT_DOTWEEN";

        [MenuItem("Hermit/Quick Scene Setup %#k")]
        public static void Setup()
        {
            var kernel = Object.FindObjectOfType<HermitKernel>();
            if (kernel != null)
            {
                Debug.LogWarning("Kernel already exists.");
                return;
            }

            var kernelObj = new GameObject("Hermit Kernel",
                typeof(HermitKernel), typeof(HermitKernelModule), typeof(HermitBindingAdapterModule)
            );
            kernel = kernelObj.GetComponent<HermitKernel>();

            var array = new MonoModule[]
            {
                kernelObj.GetComponent<HermitKernelModule>(),
                kernelObj.GetComponent<HermitBindingAdapterModule>()
            };
            var fieldInfo = typeof(HermitKernel).GetField("Modules", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo?.SetValue(kernel, array);
        }

#if HERMIT_DOTWEEN
        [MenuItem("Hermit/Extensions/Disable DoTween Extensions")]
#else
        [MenuItem("Hermit/Extensions/Enable DoTween Extensions")]
#endif
        public static void ToggleDoTweenSupport()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var defines = defineString.Split(';').ToList();
#if HERMIT_DOTWEEN
            if (defines.Contains(HERMIT_DOTWEEN)) { defines.Remove(HERMIT_DOTWEEN); }
#else
            if (!defines.Contains(HERMIT_DOTWEEN)) { defines.Add(HERMIT_DOTWEEN); }
#endif
            defineString = string.Join(";", defines);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineString);
        }
    }
}