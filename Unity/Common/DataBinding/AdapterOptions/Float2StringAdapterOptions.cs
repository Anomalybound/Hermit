using Hermit.Common.DataBinding.Core;
using UnityEngine;

namespace Hermit.Common.DataBinding
{
    [CreateAssetMenu(menuName = "Hermit/Adapter Options/Float => String")]
    public class Float2StringAdapterOptions : AdapterOptions
    {
        [Header("Options")]
        public string format = "{0}";

        public override object Convert(object fromObj)
        {
            return string.Format(format, fromObj);
        }
    }
}