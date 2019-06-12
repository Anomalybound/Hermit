using UnityEngine;

namespace Hermit
{
    public interface IView
    {
        ulong ViewId { get; }

        GameObject ViewObject { get; }

        Component ViewComponent { get; }
    }
}