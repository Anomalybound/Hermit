using Hermit.Common.DataBinding.Core;
using UnityEngine;

namespace Hermit.Common.DataBinding.Adapters
{
    [Adapter(typeof(float), typeof(int))]
    public class Float2IntAdapter : AdapterBase
    {
        public override object Convert(object fromObj)
        {
            return Mathf.RoundToInt((float) fromObj).ToString();
        }
    }
}