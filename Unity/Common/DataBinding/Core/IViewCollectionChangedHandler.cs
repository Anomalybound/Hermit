using UnityEngine;

namespace Hermit.Common.DataBinding.Core
{
    public interface IViewCollectionChangedHandler : ICollectionChangedHandler
    {
        void SetUp(GameObject viewTemplate, Transform viewContainer);
    }
}