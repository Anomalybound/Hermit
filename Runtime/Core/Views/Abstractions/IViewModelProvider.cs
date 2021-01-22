using System;
using System.Collections.Generic;
using Hermit.DataBinding;

namespace Hermit.Views
{
    public interface IViewModelProvider
    {
        event Action OnDataReady;
        
        void SetViewModel(object context);

        ViewModel GetViewModel();

        string ViewModelTypeName { get; }
        
        List<DataBindingBase> DataBindings { get; }
    }
}