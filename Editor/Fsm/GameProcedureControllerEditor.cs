using UnityEditor;
using Hermit.Procedure;

namespace Hermit
{
    [CustomEditor(typeof(GameProcedureController<,>), true)]
    public class GameProcedureControllerEditor : FsmContainerEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("initState"));
                if (check.changed) { serializedObject.ApplyModifiedProperties(); }
            }
        }
    }
}