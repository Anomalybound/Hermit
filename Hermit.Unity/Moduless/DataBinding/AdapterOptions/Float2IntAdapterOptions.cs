using System;
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

        public ParseType FloatParseType = ParseType.Round;


        public override object Convert(object fromObj)
        {
            switch (FloatParseType)
            {
                case ParseType.Ceil:
                    return Mathf.CeilToInt((float) fromObj);
                case ParseType.Floor:
                    return Mathf.FloorToInt((float) fromObj);
                case ParseType.Round:
                    return Mathf.RoundToInt((float) fromObj);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}