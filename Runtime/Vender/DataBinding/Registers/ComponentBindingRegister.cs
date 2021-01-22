using System;
using Hermit.Injection;
using UnityEngine;

namespace Hermit.DataBinding
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
        private Component target = null;

        [SerializeField]
        private AsType asScope = AsType.Singleton;

        [SerializeField]
        private BindingRegisterType bindingType = BindingRegisterType.BindSelf;

        [SerializeField]
        private string bindingId = null;

        private void Awake()
        {
            var container = Contexts.GlobalContext.Container;

            if (target == null) { return; }

            IBindingInfo bindingInfo;

            switch (bindingType)
            {
                case BindingRegisterType.BindSelf:
                    bindingInfo = container.Bind(target.GetType()).FromInstance(target);
                    break;
                case BindingRegisterType.BindAll:
                    bindingInfo = container.BindAll(target.GetType()).FromInstance(target);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (asScope)
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

            bindingInfo.WithId(bindingId.Trim());

            (container as DiContainer)?.Build(bindingInfo);
        }
    }
}