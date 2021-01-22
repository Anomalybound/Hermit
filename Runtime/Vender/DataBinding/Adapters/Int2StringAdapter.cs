namespace Hermit.DataBinding.Adapters
{
    [Adapter(typeof(int), typeof(string))]
    public class Int2StringAdapter : AdapterBase
    {
        public override object Convert(object fromObj)
        {
            return fromObj.ToString();
        }
    }
}