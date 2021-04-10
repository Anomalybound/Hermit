using UnityEditor;
using UnityEngine;

namespace Hermit.Fsm
{
    [CustomEditor(typeof(FsmContainer), true)]
    public class FsmContainerEditor : Editor
    {
        private FsmContainer _fsm;

        private void OnEnable()
        {
            _fsm = target as FsmContainer;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
                
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Options", EditorStyles.boldLabel); 

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useFixedUpdate"));
                if (check.changed) { serializedObject.ApplyModifiedProperties(); }
            }

            if (_fsm.RootNode == null)
            {
                EditorGUILayout.LabelField("Root state not initialized.");
                return;
            }

            DrawState("Root", _fsm.RootNode);
        }

        private void DrawState(string stateName, IState state)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawState(stateName, state, state.Active);

                if (state.Children.Count > 0) { DrawChildren(state); }
            }
        }

        private void DrawState(string stateName, IState state, bool active)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(
                    $"{stateName}[{state.GetType().FullName}] - Children: [{state.Children.Count}] - Active: [{state.ActiveChildrenStates.Count}]",
                    GUILayout.ExpandWidth(true));

                var rect = GUILayoutUtility.GetLastRect();
                rect.width = 20;

                if (_fsm.RootNode != state) { GUI.Toggle(rect, active, ""); }
            }
        }

        private void DrawChildren(IState state)
        {
            EditorGUI.indentLevel++;
            
            Repaint();

            if (state.Children.Count > 0)
            {
                foreach (var pair in state.Children)
                {
                    var stateName = pair.Key;
                    var childState = pair.Value;

                    DrawState(stateName, childState);
                }
            }

            EditorGUI.indentLevel--;
        }
    }
}