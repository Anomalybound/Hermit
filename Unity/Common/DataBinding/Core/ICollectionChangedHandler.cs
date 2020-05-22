using System.Collections.Specialized;

namespace Hermit.Common.DataBinding.Core
{
    public interface ICollectionChangedHandler
    {
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
    }
}