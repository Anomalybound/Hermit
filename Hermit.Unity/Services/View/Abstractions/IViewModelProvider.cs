using System;

namespace Hermit.View
{
    public interface IViewModelProvider
    {
        event Action OnDataReady;
        
        void SetViewModel(object context);

        ViewModel GetViewModel();

        string GetViewModelTypeName { get; }
    }
}