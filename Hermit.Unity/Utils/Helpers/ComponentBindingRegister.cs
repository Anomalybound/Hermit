using System;
using Hermit.Injection;
using UnityEngine;

namespace Hermit.Unity
{
    public enum BindingRegisterType
    {
        BindSelf,
        BindAll,
    }

    [ScriptOrder(-8000)]
    public sealed class ComponentBindingRegister : MonoBehaviour
    {
        [SerializeField]
        private Component _target = null;

        [SerializeField]
        private AsType _asScope = AsType.Singleton;

        [SerializeField]
        private BindingRegisterType _bindingType = BindingRegisterType.BindSelf;

        [SerializeField]
        private string _bindingId = null;

        private void Awake()
        {
            var container = Context.GlobalContext.Container;

            if (_target == null) { return; }

            IBindingInfo bindingInfo;

            switch (_bindingType)
            {
                case BindingRegisterType.BindSelf:
                    bindingInfo = container.Bind(_target.GetType()).FromInstance(_target).WithId(_bindingId);
                    break;
                case BindingRegisterType.BindAll:
                    bindingInfo = container.BindAll(_target.GetType()).FromInstance(_target).WithId(_bindingId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (_asScope)
            {
                case AsType.Singleton:
                    bindingInfo.AsSingleton();
                    break;
                case AsType.Transient:
                    bindingInfo.AsTransient();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            (container as DiContainer)?.Build(bindingInfo);
        }
    }
}