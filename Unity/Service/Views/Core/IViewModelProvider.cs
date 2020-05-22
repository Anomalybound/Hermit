using System;

namespace Hermit.Service.Views
{
    public interface IViewModelProvider
    {
        event Action OnDataReady;
        
        void SetViewModel(object context);

        ViewModel GetViewModel();

        string GetViewModelTypeName { get; }
    }
}