namespace Hermit.DataBinding
{
    public interface IAdapter
    {
        object Convert(object fromObj);

        object Convert(object fromObj, AdapterOptions options);
    }
}