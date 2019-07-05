namespace Hermit.DataBinding
{
    [Adapter(typeof(int), typeof(string))]
    public class Int2FloatAdapter : IAdapter
    {
        public object Covert(object fromObj, AdapterOptions options)
        {
            return (float) fromObj;
        }
    }
}