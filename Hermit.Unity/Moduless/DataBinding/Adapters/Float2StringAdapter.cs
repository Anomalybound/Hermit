namespace Hermit.DataBinding
{
    [Adapter(typeof(float), typeof(string))]
    public class Float2StringAdapter : AdapterBase
    {
        public override object Convert(object fromObj)
        {
            return fromObj.ToString();
        }
    }
}