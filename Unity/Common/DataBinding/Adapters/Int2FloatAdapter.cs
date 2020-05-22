using Hermit.Common.DataBinding.Core;

namespace Hermit.Common.DataBinding.Adapters
{
    [Adapter(typeof(int), typeof(float))]
    public class Int2FloatAdapter : AdapterBase
    {
        public override object Convert(object fromObj)
        {
            return fromObj.ToString();
        }
    }
}