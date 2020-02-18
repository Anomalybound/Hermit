using System.Reflection;
using UnityEditor;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(ActionBinding))]
    public class ActionBindingEditor : DataBindingEditorBase
    {
        protected ActionBinding Target;

        protected MemberInfo MethodMemberInfo;

        protected override void OnEnable()
        {
            base.OnEnable();

            Target = target as ActionBinding;
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

                    var actionEntry = Target.ViewActionEntry;
                    var onlyDeclaredMethods = Target.showDeclaredMethodsOnly;

                    // Draw View Action Popup
                    (Target.ViewActionEntry, MethodMemberInfo) = DrawViewMethodsPopup(actionEntry, onlyDeclaredMethods);
                }

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    DrawBindingLabel("View Model");

                    EditorGUI.BeginDisabledGroup(Target.ViewModelEventEntry == null);

                    Target.ViewModelEventEntry = DrawViewModelEventsPopup(Target.ViewModelEventEntry, MethodMemberInfo);

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