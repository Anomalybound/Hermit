using UnityEditor;
using UnityEngine;

namespace Hermit
{
    public class EditorUtil
    {
        public static GUIStyle GetStyle(string styleName)
        {
            var guiStyle = GUI.skin.FindStyle(styleName) ??
                           EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            
            if (guiStyle == null) { Debug.LogError((object) ("Missing built-in GuiSTyle " + styleName)); }

            return guiStyle;
        }
    }
}