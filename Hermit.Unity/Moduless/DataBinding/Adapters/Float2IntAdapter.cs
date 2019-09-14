using UnityEngine;

namespace Hermit.DataBinding
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