using System.Collections.Specialized;

namespace Hermit.DataBinding
{
    public interface ICollectionChangedHandler
    {
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);
    }
}