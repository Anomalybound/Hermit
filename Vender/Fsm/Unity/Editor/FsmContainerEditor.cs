using Hermit.Fsm;
using UnityEditor;
using UnityEngine;

namespace Hermit
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
            serializedObject.Update();

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
            var active = _activeState == state;
            if (active) { DrawActiveState(stateName, state); }
            else { DrawNormalState(stateName, state); }

            if (state.Children.Count > 0) { DrawChildren(state); }
        }

        private void DrawNormalState(string stateName, IState state)
        {
            EditorGUILayout.LabelField(
                $"Name: {stateName}[{state.GetType().FullName}] - Children: [{state.Children.Count}] - Active: [{state.ActiveStates.Count}]");
        }

        private void DrawActiveState(string stateName, IState state)
        {
            var guiCol = GUI.skin.label.normal.textColor;

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(
                    $"Name: {stateName}[{state.GetType().FullName}] - Children: [{state.Children.Count}] - Active: [{state.ActiveStates.Count}]",
                    GUILayout.ExpandWidth(true));

                var rect = GUILayoutUtility.GetLastRect();
                rect.width = 20;

                GUI.skin.label.normal.textColor = new Color(0f, 0.78f, 0.2f);
                GUI.Label(rect, "\u2714");
                GUI.skin.label.normal.textColor = guiCol;
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

        private static Color GetActiveStateColor()
        {
            return Color.green;
        }
    }
}