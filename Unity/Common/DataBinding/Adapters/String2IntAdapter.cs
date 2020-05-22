using System;
using Hermit.Common.DataBinding.Core;

namespace Hermit.Common.DataBinding.Adapters
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