namespace Hermit.DataBinding
{
    public interface IAdapter
    {
        object Covert(object fromObj, AdapterOptions options);
    }
}