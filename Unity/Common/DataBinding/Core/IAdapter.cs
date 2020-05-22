namespace Hermit.Common.DataBinding.Core
{
    public interface IAdapter
    {
        object Convert(object fromObj);

        object Convert(object fromObj, AdapterOptions options);
    }
}