using System;

namespace Hermit.DataBinding
{
    [Adapter(typeof(string), typeof(int))]
    public class String2IntAdapter : IAdapter
    {
        public object Covert(object fromObj, AdapterOptions options)
        {
            if (int.TryParse((string) fromObj, out var value)) { return value; }

            throw new InvalidCastException();
        }
    }
}