using System;
using Hermit;
using UnityEditor;

namespace Hermit
{
    [InitializeOnLoad]
    public class ScriptOrderManager
    {
        static ScriptOrderManager()
        {
            foreach (var monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (monoScript.GetClass() == null) { continue; }

                foreach (var a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptOrderAttribute)))
                {
                    var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
                    var newOrder = ((ScriptOrderAttribute) a).Order;
                    if (currentOrder != newOrder) { MonoImporter.SetExecutionOrder(monoScript, newOrder); }
                }
            }
        }
    }
}