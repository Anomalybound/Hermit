using UnityEngine;

namespace Hermit.View
{
    public interface IView
    {
        ulong ViewId { get; }

        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }

        // ReSharper disable once InconsistentNaming
        Component component { get; }

        void SetUpViewInfo();

        void CleanUpViewInfo();
    }
}