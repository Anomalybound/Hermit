using UnityEngine;

namespace Hermit.DataBinding
{
    public interface IViewCollectionChangedHandler : ICollectionChangedHandler
    {
        void SetUp(GameObject viewTemplate, Transform viewContainer);
    }
}