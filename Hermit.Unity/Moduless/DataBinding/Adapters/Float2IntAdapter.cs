using System;
using UnityEngine;

namespace Hermit.DataBinding
{
    [Adapter(typeof(float), typeof(int), typeof(Float2IntAdapterOptions))]
    public class Float2IntAdapter : IAdapter
    {
        public object Covert(object fromObj, AdapterOptions options)
        {
            if (options == null) { return Mathf.RoundToInt((float) fromObj); }

            var type = ((Float2IntAdapterOptions) options).parseType;
            switch (type)
            {
                case Float2IntAdapterOptions.ParseType.Ceil:
                    return Mathf.CeilToInt((float) fromObj);
                case Float2IntAdapterOptions.ParseType.Floor:
                    return Mathf.FloorToInt((float) fromObj);
                case Float2IntAdapterOptions.ParseType.Round:
                    return Mathf.RoundToInt((float) fromObj);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}