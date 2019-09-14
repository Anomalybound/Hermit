using System;

namespace Hermit.DataBinding
{
    [Adapter(typeof(string), typeof(int))]
    public class String2IntAdapter : AdapterBase
    {
        public override object Convert(object fromObj)
        {
            if (int.TryParse((string) fromObj, out var value)) { return value; }

            throw new InvalidCastException();
        }
    }
}