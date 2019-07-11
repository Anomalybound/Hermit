namespace Hermit.DataBinding
{
    [Adapter(typeof(int), typeof(float))]
    public class Int2FloatAdapter : IAdapter
    {
        public object Covert(object fromObj, AdapterOptions options)
        {
            return (float) fromObj;
        }
    }
}