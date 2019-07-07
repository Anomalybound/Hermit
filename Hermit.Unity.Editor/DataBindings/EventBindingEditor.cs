using System.Reflection;
using UnityEditor;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(EventBinding))]
    public class EventBindingEditor : DataBindingEditorBase
    {
        protected EventBinding Target;

        protected MemberInfo EventMemberInfo;

        protected override void OnEnable()
        {
            base.OnEnable();

            Target = target as EventBinding;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    DrawBindingLabel("View");

                    // Draw View Event Popup
                    (Target.ViewEventEntry, EventMemberInfo) = DrawViewEventPopup(Target.ViewEventEntry);
                }

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    DrawBindingLabel("View Model");

                    EditorGUI.BeginDisabledGroup(Target.ViewEventEntry == null);

                    Target.ViewModelActionEntry =
                        DrawViewModelActionPopup(Target.ViewModelActionEntry, EventMemberInfo);

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