using Hermit.DataBinding;
using UnityEditor;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(OneWayPropertyBinding))]
    public class OneWayDataBindingEditor : DataBindingEditorBase
    {
        #region Runtime Variables

        protected SerializedProperty ViewAdapterOptions;

        protected OneWayPropertyBinding Target;

        #endregion

        protected override void OnEnable()
        {
            Target = target as OneWayPropertyBinding;

            base.OnEnable();

            ViewAdapterOptions = serializedObject.FindProperty("viewAdapterOptions");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using var check = new EditorGUI.ChangeCheckScope();
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawBindingLabel("View Model", ViewModelSource);

                // Draw View Model Popup
                Target.ViewModelEntry = DrawViewModelPopup(Target.ViewModelEntry);
            }

            DrawBindingTypeInfo(true);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawBindingLabel("View", ViewSource);

                EditorGUI.BeginDisabledGroup(Target.ViewModelEntry == null);

                // Draw View Popup
                Target.ViewEntry = DrawViewPropertyPopup(Target.ViewEntry);

                // Draw View adapter popup
                Target.ViewAdapterTypeString = DrawViewAdapterPopup(Target.ViewAdapterTypeString, ViewAdapterOptions);

                EditorGUI.EndDisabledGroup();
            }

            if (check.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}