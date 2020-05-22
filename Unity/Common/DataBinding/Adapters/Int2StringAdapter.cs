using Hermit.Common.DataBinding.Core;

namespace Hermit.Common.DataBinding.Adapters
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