using System;
using UnityEngine;

namespace Hermit.Common
{
    [Serializable]
    public class HermitGeneralSettings
    {
        [SerializeField]
        protected bool registerEventWhileInjection = true;

        public bool RegisterEventWhileInjection => registerEventWhileInjection;
    }
}