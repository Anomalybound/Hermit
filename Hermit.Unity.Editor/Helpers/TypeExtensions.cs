using System;
using System.Linq;
using System.Text;

namespace Hermit
{
    public static class TypeExtensions
    {
        private static string GetTypeString(Type type)
        {
            var t = type.AssemblyQualifiedName.Replace(type.Namespace + ".", "");
            var output = new StringBuilder();

            var iAssyBackTick = t.IndexOf('`') + 1;
            output.Append(t.Substring(0, iAssyBackTick - 1).Replace("[", string.Empty));
            var genericTypes = type.GetGenericArguments();

            var typeStrings = genericTypes
                .Select(genType => genType.IsGenericType
                    ? GetTypeString(genType)
                    : genType.ToString().Replace(genType.Namespace + ".", "")).ToList();

            output.Append($"<{string.Join(",", typeStrings)}>");
            return output.ToString();
        }

        public static string GetFullTypeString(Type type)
        {
            var t = type.AssemblyQualifiedName;
            var output = new StringBuilder();

            var iAssyBackTick = t.IndexOf('`') + 1;
            output.Append(t.Substring(0, iAssyBackTick - 1).Replace("[", string.Empty));
            var genericTypes = type.GetGenericArguments();

            var typeStrings = genericTypes
                .Select(genType => genType.IsGenericType ? GetFullTypeString(genType) : genType.ToString()).ToList();

            output.Append($"<{string.Join(",", typeStrings)}>");
            return output.ToString();
        }

        public static string PrettyName(this Type type)
        {
            var friendlyName = type.Name;
            if (type.IsGenericType) { friendlyName = GetTypeString(type); }

            return friendlyName;
        }

        public static string FullPrettyName(this Type type)
        {
            var friendlyName = type.FullName;
            if (type.IsGenericType) { friendlyName = GetFullTypeString(type); }

            return friendlyName;
        }
    }
}