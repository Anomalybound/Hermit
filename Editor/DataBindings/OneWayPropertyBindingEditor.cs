using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hermit.DataBinding;
using UnityEditor;
using UnityEngine;

namespace Hermit.DataBindings
{
    [CustomEditor(typeof(OneWayPropertyBinding))]
    public class OneWayPropertyBindingEditor : Editor
    {
        protected OneWayPropertyBinding Target;

        #region Runtime Variables

        protected Type BindingDataType;
        protected Type ConvertedDataType;

        protected static readonly Dictionary<Type, List<Type>> AdapterLookup = new Dictionary<Type, List<Type>>();

        protected static readonly Dictionary<Type, AdapterAttribute> AdapterAttributeLookup =
            new Dictionary<Type, AdapterAttribute>();

        private readonly Func<Type, string> AdapterFromName = type => AdapterAttributeLookup[type].FromType.Name;
        private readonly Func<Type, string> AdapterToName = type => AdapterAttributeLookup[type].ToType.Name;

        #endregion

        private void OnEnable()
        {
            Target = target as OneWayPropertyBinding;

            if (Target == null) { return; }

            if (AdapterLookup.Count <= 0) { CollectAdapters(); }
        }

        private static void CollectAdapters()
        {
            var adapters = AssemblyHelper.GetInheritancesInParentAssembly(typeof(IAdapter)).ToList();
            foreach (var adapter in adapters)
            {
                var adapterAttribute = adapter.GetCustomAttribute<AdapterAttribute>();
                if (adapterAttribute == null)
                {
                    Debug.LogError($"{adapter} doesn't decorated with [Adapter] attribute.");
                    continue;
                }

                AdapterAttributeLookup.Add(adapter, adapterAttribute);
                AdapterLookup.AddToList(adapterAttribute.FromType, adapter);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var available = DrawViewModelPropertiesPopup();

            using (new EditorGUI.DisabledScope(!available))
            {
                DrawAdapterPopup();
                DrawViewPropertiesPopup();
            }
        }

        private bool DrawViewModelPropertiesPopup()
        {
            var dataProviders = Target.transform.GetComponentsInParent<IViewModelProvider>(true);

            if (dataProviders.Length <= 0)
            {
                var oriCol = EditorStyles.label.normal.textColor;
                EditorStyles.label.normal.textColor = Color.red;

                EditorGUILayout.LabelField("ViewModel not found in context.");

                EditorStyles.label.normal.textColor = oriCol;
                return false;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var viewModelTypes = new List<Type>();
            var providerLookup = new Dictionary<Type, IViewModelProvider>();

            foreach (var dataProvider in dataProviders)
            {
                foreach (var assembly in assemblies)
                {
                    var viewModelType = assembly.GetType(dataProvider.GetViewModelTypeName);
                    if (viewModelType == null) { continue; }

                    viewModelTypes.Add(viewModelType);
                    providerLookup.Add(viewModelType, dataProvider);
                }
            }

            var viewModelPropertyInfos =
                viewModelTypes.ToDictionary(key => key, value => value.GetProperties().ToList());

            var options = new List<string>();
            var lookup = new List<string>();
            var viewModelTypeLookup = new List<Type>();
            var propertyTypeLookup = new List<Type>();

            foreach (var info in viewModelPropertyInfos)
            {
                var properties = info.Value.Select(p =>
                {
                    var provider = (Component) providerLookup[info.Key];
                    return $"{provider.name}/{info.Key.Name}/{p} - [{p.PropertyType.Name}]";
                });
                var items = info.Value.Select(p => $"{info.Key.FullName}.{p.Name}");
                var vmType = info.Value.Select(p => info.Key);
                var pis = info.Value.Select(p => p.PropertyType);

                options.AddRange(properties);
                lookup.AddRange(items);
                viewModelTypeLookup.AddRange(vmType);
                propertyTypeLookup.AddRange(pis);
            }

            var selection = lookup.IndexOf(Target.ViewModelEntry);

            // Select View Model
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                selection = EditorGUILayout.Popup("ViewModel Properties", selection, options.ToArray());

                if (BindingDataType == null && selection >= 0) { BindingDataType = propertyTypeLookup[selection]; }

                if (check.changed)
                {
                    Target.ViewModelEntry = lookup[selection];
                    Target.DataProvider = providerLookup[viewModelTypeLookup[selection]] as Component;

                    BindingDataType = propertyTypeLookup[selection];
                    ConvertedDataType = null;
                }
            }

            var bindingTypeHint = BindingDataType != null ? BindingDataType.Name : "Undefined";
            var convertedTypeHint = ConvertedDataType != null ? $" => {ConvertedDataType.Name}" : "";

            EditorGUILayout.LabelField(
                $"Binding Data Type: {bindingTypeHint}{convertedTypeHint}",
                EditorStyles.centeredGreyMiniLabel);

            return true;
        }

        private void DrawAdapterPopup()
        {
            var data = BindingDataType != null && AdapterLookup.ContainsKey(BindingDataType)
                ? AdapterLookup[BindingDataType]
                : new List<Type>();

            var options = data
                .Select(a => $"[{AdapterFromName(a)}]->[{AdapterToName(a)}] : {a.Name}").ToList();
            options.Insert(0, "None");

            var lookup = data.Select(a => a.FullName).ToList();
            var selection = lookup.IndexOf(Target.AdapterType);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                selection++;
                selection = EditorGUILayout.Popup("Available Adapters", selection, options.ToArray());
                selection--;

                if (lookup.Count <= selection || selection < 0)
                {
                    Target.AdapterType = null;
                    ConvertedDataType = null;
                    return;
                }

                var adapter = data[selection];
                if (AdapterAttributeLookup.TryGetValue(adapter, out var adapterAttribute))
                {
                    if (adapterAttribute.OptionType != null)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_adapterOptions"));
                    }

                    ConvertedDataType = adapterAttribute.ToType;
                }

                if (!check.changed) { return; }

                Target.AdapterType = lookup[selection];
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawViewPropertiesPopup()
        {
            var components = Target.gameObject.GetComponents<Component>().ToList();
            var options = new List<string>();
            var lookup = new List<string>();

            var targetType = ConvertedDataType ?? BindingDataType;

            // Fill data
            foreach (var component in components)
            {
                if (component == Target) { continue; }

                var properties = component.GetType().GetProperties();

                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.PropertyType != targetType) { continue; }

                    lookup.Add($"{component.GetType()}.{propertyInfo.Name}");
                    options.Add($"{component.GetType().Name}/{propertyInfo.Name} - [{targetType.Name}]");
                }
            }

            var selection = lookup.IndexOf(Target.ViewEntry);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                selection = EditorGUILayout.Popup("View Properties",
                    selection, options.ToArray());

                if (!check.changed) { return; }

                Target.ViewEntry = lookup[selection];
            }
        }
    }
}