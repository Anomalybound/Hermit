using UnityEditor;
using UnityEngine;

namespace Hermit.Helpers
{
    public static class EditorUtil
    {
        public const string Undefined = "Undefined";
        
        public static GUIStyle GetStyle(string styleName)
        {
            var guiStyle = GUI.skin.FindStyle(styleName) ??
                           EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            
            if (guiStyle == null) { Debug.LogError((object) ("Missing built-in GuiSTyle " + styleName)); }

            return guiStyle;
        }
    }
}