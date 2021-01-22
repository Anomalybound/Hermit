using System;

namespace Hermit.DataBinding.Adapters
{
    [Adapter(typeof(string), typeof(float))]
    public class String2FloatAdapter : AdapterBase
    {
        public override object Convert(object fromObj)
        {
            if (float.TryParse((string) fromObj, out var value)) { return value; }

            throw new InvalidCastException();
        }
    }
}