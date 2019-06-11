using UnityEditor;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(PropertyMethodBinding))]
    public class PropertyMethodBindingEditor : DataBindingEditorBase
    {
        #region Runtime Variables

        protected SerializedProperty ViewAdapterOptions;

        protected PropertyMethodBinding Target;

        #endregion

        protected override void OnEnable()
        {
            Target = target as PropertyMethodBinding;

            base.OnEnable();

            ViewAdapterOptions = serializedObject.FindProperty("_viewAdapterOptions");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Target.ShowDeclaredMethodsOnly)));

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
                Target.ViewEntry = DrawViewMethodPopup(Target.ViewEntry, Target.ShowDeclaredMethodsOnly);

                // Draw View adapter popup
                Target.ViewAdapterType = DrawViewAdapterPopup(Target.ViewAdapterType, ViewAdapterOptions);

                EditorGUI.EndDisabledGroup();
            }
        }
    }
}