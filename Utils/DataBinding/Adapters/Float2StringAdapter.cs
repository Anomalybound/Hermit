namespace Hermit.DataBinding
{
    [Adapter(typeof(float), typeof(string), typeof(Float2StringAdapterOptions))]
    public class Float2StringAdapter : IAdapter
    {
        public object Covert(object fromObj, AdapterOptions options)
        {
            var format = ((Float2StringAdapterOptions) options).Format;
            return ((float) fromObj).ToString(format);
        }
    }
}