using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LayerUtility
{
    [InitializeOnLoadMethod]
    public static void CheckUIStackLayer()
    {
        CheckLayers(new[] {"UI", "UIHidden"});
    }

    public static void CheckLayers(IEnumerable<string> layerNames)
    {
        var manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layersProp = manager.FindProperty("layers");

        foreach (var name in layerNames)
        {
            // check if layer is present
            var found = false;
            for (var i = 0; i <= 31; i++)
            {
                var sp = layersProp.GetArrayElementAtIndex(i);
                if (sp != null && name.Equals(sp.stringValue))
                {
                    found = true;
                    break;
                }
            }

            // not found, add into 1st open slot
            if (!found)
            {
                SerializedProperty slot = null;
                for (int i = 8; i <= 31; i++)
                {
                    SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                    if (sp != null && string.IsNullOrEmpty(sp.stringValue))
                    {
                        slot = sp;
                        break;
                    }
                }

                if (slot != null) { slot.stringValue = name; }
                else { Debug.LogError("Could not find an open Layer Slot for: " + name); }
            }
        }

        // save
        manager.ApplyModifiedProperties();
    }
}