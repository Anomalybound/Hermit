using System;
using System.Collections.Specialized;
using System.Reflection;
using Hermit.DataBinding;
using UnityEngine;
using UnityEngine.Assertions;

namespace Hermit
{
    public class CollectionBinding : DataBindingBase
    {
        [SerializeField]
        private string _viewModelCollectionEntry;

        [SerializeField]
        private string _collectionHandlerTypeName;

        [SerializeField]
        private GameObject _viewTemplate;

        [SerializeField]
        private Transform _viewContainer;


        #region Propertyies

        public string ViewModelCollectionEntry
        {
            get => _viewModelCollectionEntry;
            set
            {
#if UNITY_EDITOR
                if (_viewModelCollectionEntry != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                _viewModelCollectionEntry = value;
            }
        }

        public string CollectionHandlerTypeName
        {
            get => _collectionHandlerTypeName;
            set
            {
#if UNITY_EDITOR
                if (_collectionHandlerTypeName != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                _collectionHandlerTypeName = value;
            }
        }

        public GameObject ViewTemplate
        {
            get => _viewTemplate;
            set
            {
#if UNITY_EDITOR
                if (_viewTemplate != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                _viewTemplate = value;
            }
        }


        public Transform ViewContainer
        {
            get
            {
                if (_viewContainer == null) { return _viewTemplate != null ? _viewTemplate.transform.parent : null; }

                return _viewContainer;
            }
            set
            {
#if UNITY_EDITOR
                if (_viewContainer != value) { UnityEditor.EditorUtility.SetDirty(this); }
#endif
                _viewContainer = value;
            }
        }

        #endregion

        #region Runtime Variables

        protected INotifyCollectionChanged CollectionValue;

        protected IViewCollectionChangedHandler ChangedHandler;

        #endregion

        public override void SetupBinding()
        {
            base.SetupBinding();

            GetCollectionChangedHandlerInstance();

            BindViewModelCollections();
        }

        private void GetCollectionChangedHandlerInstance()
        {
            if (string.IsNullOrEmpty(CollectionHandlerTypeName)) { return; }

            ChangedHandler = Her.Resolve<IViewCollectionChangedHandler>(CollectionHandlerTypeName);
            ChangedHandler.SetUp(ViewTemplate, _viewContainer);
        }

        public override void Connect()
        {
            if (CollectionValue == null) { return; }

            Assert.IsNotNull(ChangedHandler);
            CollectionValue.CollectionChanged += ChangedHandler.OnCollectionChanged;
        }

        public override void Disconnect()
        {
            if (CollectionValue == null) { return; }

            Assert.IsNotNull(ChangedHandler);
            CollectionValue.CollectionChanged -= ChangedHandler.OnCollectionChanged;
        }

        public override void UpdateBinding() { }

        private void BindViewModelCollections()
        {
            #region View Model

            var memberInfo = ParseViewModelEntry(ViewModel, ViewModelCollectionEntry);

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo) memberInfo;
                    CollectionValue = fieldInfo.GetValue(ViewModel) as INotifyCollectionChanged;
                    break;
                case MemberTypes.Property:
                    var propertyInfo = (PropertyInfo) memberInfo;
                    CollectionValue = propertyInfo.GetValue(ViewModel) as INotifyCollectionChanged;
                    break;
                default:
                    throw new Exception(
                        $"MemberType: {memberInfo.MemberType} is not supported in one way property binding.");
            }

            #endregion
        }
    }
}