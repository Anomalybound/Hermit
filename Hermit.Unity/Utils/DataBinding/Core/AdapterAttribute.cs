using System;

namespace Hermit.DataBinding
{
    public class AdapterAttribute : Attribute
    {
        public Type FromType { get; }

        public Type ToType { get; }

        public Type OptionType { get; }

        public AdapterAttribute(Type fromType, Type type)
        {
            FromType = fromType;
            ToType = type;
        }

        public AdapterAttribute(Type fromType, Type type, Type optionType)
        {
            FromType = fromType;
            ToType = type;
            OptionType = optionType;
        }
    }
}