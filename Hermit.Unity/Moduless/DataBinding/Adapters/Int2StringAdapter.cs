namespace Hermit.DataBinding
{
    [Adapter(typeof(int), typeof(string), typeof(Int2StringAdapterOptions))]
    public class Int2StringAdapter : IAdapter
    {
        public object Covert(object fromObj, AdapterOptions options)
        {
            var format = ((Float2StringAdapterOptions) options).Format;
            return ((int) fromObj).ToString(format);
        }
    }
}