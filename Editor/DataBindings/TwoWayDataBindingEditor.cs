using Hermit.DataBinding;
using UnityEditor;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(TwoWayPropertyBinding))]
    public class TwoWayDataBindingEditor : DataBindingEditorBase
    {
        #region Runtime Variables

        protected SerializedProperty ViewAdapterOptions;
        protected SerializedProperty ViewModelAdapterOptions;

        protected TwoWayPropertyBinding Target;

        #endregion

        protected override void OnEnable()
        {
            Target = target as TwoWayPropertyBinding;

            base.OnEnable();

            ViewAdapterOptions = serializedObject.FindProperty("viewAdapterOptions");
            ViewModelAdapterOptions = serializedObject.FindProperty("viewModelAdapterOptions");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            using var check = new EditorGUI.ChangeCheckScope();
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawBindingLabel("View Model", ViewModelSource);

                // Draw View Model Popup
                Target.ViewModelEntry = DrawViewModelPopup(Target.ViewModelEntry);

                EditorGUI.BeginDisabledGroup(Target.ViewModelEntry == null);

                // Draw View Model Adapter
                Target.ViewModelAdapterTypeString =
                    DrawViewModelAdapterPopup(Target.ViewModelAdapterTypeString, ViewModelAdapterOptions);
            }

            DrawBindingTypeInfo(false);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawBindingLabel("View", ViewSource);

                // Draw View Popup
                Target.ViewEntry = DrawViewPropertyPopup(Target.ViewEntry);

                // Draw View Adapter
                Target.ViewAdapterTypeString = DrawViewAdapterPopup(Target.ViewAdapterTypeString, ViewAdapterOptions);

                // Draw View value-changed notification Event popup
                (Target.ViewEventEntry, _) = DrawViewEventPopup(Target.ViewEventEntry);
            }

            EditorGUI.EndDisabledGroup();

            if (check.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}