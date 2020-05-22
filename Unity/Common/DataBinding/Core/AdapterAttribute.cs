using System;

namespace Hermit.Common.DataBinding.Core
{
    public class AdapterAttribute : Attribute
    {
        public Type FromType { get; }

        public Type ToType { get; }

        public AdapterAttribute(Type fromType, Type type)
        {
            FromType = fromType;
            ToType = type;
        }
    }
}