using System;

namespace Hermit.Common
{
    public class ScriptOrderAttribute : Attribute
    {
        public int Order { get; }

        public ScriptOrderAttribute(int order)
        {
            Order = order;
        }
    }
}