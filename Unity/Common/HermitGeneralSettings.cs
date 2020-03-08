using System;
using UnityEngine;

namespace Hermit.Common
{
    [Serializable]
    public class HermitGeneralSettings
    {
        [SerializeField]
        protected bool autoRegisterEvents = true;

        public bool AutoRegisterEvents => autoRegisterEvents;
    }
}