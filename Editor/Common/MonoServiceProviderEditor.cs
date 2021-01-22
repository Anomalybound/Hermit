using System.Linq;
using System.Reflection;
using Hermit.Injection;
using UnityEditor;

namespace Hermit.Common
{
    [CustomEditor(typeof(MonoServiceRegistry), true)]
    public class MonoServiceProviderEditor : Editor
    {
        private void OnEnable()
        {
            var serviceProvider = target as MonoServiceRegistry;
            if (serviceProvider == null) { return; }

            var sceneContext = serviceProvider.GetComponentInParent<SceneContext>();
            if (sceneContext == null) { return; }

            var type = sceneContext.GetType();
            var field = type.GetField("ServiceProviders", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) { return; }

            var providers = field.GetValue(sceneContext) as MonoServiceRegistry[];
            for (var i = 0; i < providers?.Length; i++)
            {
                var provider = providers[i];
                if (provider == serviceProvider) { return; }
            }

            if (providers == null) { return; }

            var list = providers.ToList();
            list.Add(serviceProvider);
            field.SetValue(sceneContext, list.ToArray());
        }
    }
}