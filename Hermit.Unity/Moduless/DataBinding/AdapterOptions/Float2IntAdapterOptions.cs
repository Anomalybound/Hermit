using UnityEngine;

namespace Hermit.DataBinding
{
    [CreateAssetMenu(menuName = "Hermit/Adapter Options/Float => Integer")]
    public class Float2IntAdapterOptions : AdapterOptions
    {
        public enum ParseType
        {
            Ceil,
            Floor,
            Round
        }

        public ParseType parseType = ParseType.Round;
    }
}