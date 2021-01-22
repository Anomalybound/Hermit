using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hermit
{
    public class AssemblyHelper
    {
        public static IEnumerable<Type> GetInheritancesInParentAssembly(Type parentType)
        {
            var assembly = parentType.Assembly;
            return GetInheritancesInAssembly(assembly, parentType);
        }

        public static IEnumerable<Type> GetInheritancesInAppDomain(Type parentType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return assemblies.SelectMany(a => GetInheritancesInAssembly(a, parentType));
        }

        public static IEnumerable<Type> GetInheritancesInAssembly(Assembly assembly, Type parentType)
        {
            var types = assembly.GetTypes();

            return types.Where(t => t.IsClass
                                    && !t.IsAbstract
                                    && (parentType.IsInterface
                                        ? t.GetInterfaces().Contains(parentType)
                                        : t.IsSubclassOf(parentType))).ToArray();
        }

        private static readonly Dictionary<string, Type> TypeNameLookup = new Dictionary<string, Type>();

        public static Type GetTypeInAppDomain(string typeName)
        {
            if (TypeNameLookup.TryGetValue(typeName, out var type)) { return type; }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                var ret = assembly.GetType(typeName);
                if (ret == null) { continue; }

                TypeNameLookup[typeName] = ret;
                return ret;
            }

            return null;
        }
    }
}
