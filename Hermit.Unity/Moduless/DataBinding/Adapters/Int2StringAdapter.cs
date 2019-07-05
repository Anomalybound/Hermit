namespace Hermit.DataBinding
{
    [Adapter(typeof(int), typeof(string))]
    public class Int2StringAdapter : IAdapter
    {
        public object Covert(object fromObj, AdapterOptions options)
        {
            return fromObj.ToString();
        }
    }
}