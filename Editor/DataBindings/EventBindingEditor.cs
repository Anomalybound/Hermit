using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hermit.DataBinding;
using UnityEditor;
using UnityEngine.Events;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(EventBinding))]
    public class EventBindingEditor : PropertyBindingEditorBase
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

                Target.ViewModelActionEntry = DrawViewModelActionPopup(Target.ViewModelActionEntry, EventMemberInfo);

                EditorGUI.EndDisabledGroup();
            }
        }
    }
}