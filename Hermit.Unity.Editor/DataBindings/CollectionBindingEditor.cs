using UnityEditor;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(CollectionBinding))]
    public class CollectionBindingEditor : DataBindingEditorBase
    {
        protected CollectionBinding Target;

        protected override void OnEnable()
        {
            base.OnEnable();

            Target = target as CollectionBinding;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                Target.CollectionHandlerTypeName = DrawCollectionHandlerPopup(Target.CollectionHandlerTypeName);

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    DrawBindingLabel("View Model");

                    Target.ViewModelCollectionEntry = DrawViewModelCollectionPopup(Target.ViewModelCollectionEntry);
                }

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    DrawBindingLabel("View");

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_viewTemplate"));

                    if (Target.ViewTemplate != null &&
                        Target.ViewTemplate.GetComponent<IViewModelProvider>() == null)
                    {
                        EditorGUILayout.HelpBox("View Template must be a IViewModelProvider.", MessageType.Error);
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_viewContainer"));
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