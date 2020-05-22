using UnityEngine;

namespace Hermit.Common.DataBinding.Core
{
    public abstract class AdapterOptions : ScriptableObject
    {
        public abstract object Convert(object fromObj);
    }
}