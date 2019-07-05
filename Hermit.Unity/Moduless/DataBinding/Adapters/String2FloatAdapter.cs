using System;

namespace Hermit.DataBinding
{
    [Adapter(typeof(string), typeof(float))]
    public class String2FloatAdapter : IAdapter
    {
        public object Covert(object fromObj, AdapterOptions options)
        {
            if (float.TryParse((string) fromObj, out var value)) { return value; }

            throw new InvalidCastException();
        }
    }
}