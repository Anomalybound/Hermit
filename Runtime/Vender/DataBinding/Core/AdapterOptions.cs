using UnityEngine;

namespace Hermit.DataBinding
{
    public abstract class AdapterOptions : ScriptableObject
    {
        public abstract object Convert(object fromObj);
    }
}