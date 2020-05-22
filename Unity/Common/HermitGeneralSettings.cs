using System;
using UnityEngine;

namespace Hermit.Common
{
    [Serializable]
    public class HermitGeneralSettings
    {
        [SerializeField] protected bool autoRegisterEvents = true;
        [SerializeField] protected bool registerGlobalMessageHandler = true;

        public bool AutoRegisterEvents => autoRegisterEvents;
        public bool RegisterGlobalMessageHandler => registerGlobalMessageHandler;
    }
}