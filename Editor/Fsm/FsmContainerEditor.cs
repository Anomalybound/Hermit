using UnityEditor;
using UnityEngine;

namespace Hermit.Fsm
{
    [CustomEditor(typeof(FsmContainer), true)]
    public class FsmContainerEditor : Editor
    {
        private FsmContainer _fsm;

        private IState _activeState;

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

            if (_fsm.Root == null)
            {
                EditorGUILayout.LabelField("Root state not initialized.");
                return;
            }

            _activeState = null;

            DrawState("Root", _fsm.Root);
        }

        private void DrawState(string stateName, IState state)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawState(stateName, state, _activeState == state);

                if (state.Children.Count > 0) { DrawChildren(state); }
            }
        }

        private void DrawState(string stateName, IState state, bool active)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(
                    $"{stateName}[{state.GetType().FullName}] - Children: [{state.Children.Count}] - Active: [{state.ActiveStates.Count}]",
                    GUILayout.ExpandWidth(true));

                var rect = GUILayoutUtility.GetLastRect();
                rect.width = 20;

                if (_fsm.Root != state) { GUI.Toggle(rect, active, ""); }
            }
        }

        private void DrawChildren(IState state)
        {
            EditorGUI.indentLevel++;

            if (state.ActiveStates.Count > 0)
            {
                var activeState = state.ActiveStates.Peek();
                if (_activeState != activeState)
                {
                    Repaint();
                    _activeState = activeState;
                }
            }

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