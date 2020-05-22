using Hermit.Common.DataBinding;
using UnityEditor;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(MethodBinding))]
    public class MethodBindingEditor : DataBindingEditorBase
    {
        #region Runtime Variables

        protected SerializedProperty ViewAdapterOptions;

        protected MethodBinding Target;

        #endregion

        protected override void OnEnable()
        {
            Target = target as MethodBinding;

            base.OnEnable();

            ViewAdapterOptions = serializedObject.FindProperty("viewAdapterOptions");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Target.showDeclaredMethodsOnly)));

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
                    Target.ViewEntry = DrawViewMethodPopup(Target.ViewEntry, Target.showDeclaredMethodsOnly);

                    // Draw View adapter popup
                    Target.ViewAdapterType = DrawViewAdapterPopup(Target.ViewAdapterType, ViewAdapterOptions);

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
}