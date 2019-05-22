using System.Threading.Tasks;
using UnityEngine;

namespace Hermit
{
    public interface IViewLoader
    {
        Task<GameObject> LoadView(string key);

        Task<TView> LoadView<TView>(string key) where TView : IView;
    }
}