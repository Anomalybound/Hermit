using System;
using System.Collections.Generic;

namespace Hermit.View
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ViewAttribute : Attribute
    {
        public string Path { get; }

        public ViewAttribute(string path)
        {
            Path = path;
        }

        #region static methods

        private static readonly Dictionary<Type, ViewAttribute> Attributes = new Dictionary<Type, ViewAttribute>();

        public static ViewAttribute Find<TView>() where TView : IView
        {
            return Find(typeof(TView));
        }

        public static ViewAttribute Find(Type type)
        {
            if (Attributes.TryGetValue(type, out var attribute)) { return attribute; }

            var attributes = type.GetCustomAttributes(typeof(ViewAttribute), false);
            if (attributes.Length > 0)
            {
                var viewAttribute = attributes[0] as ViewAttribute;
                Attributes[type] = viewAttribute;
                return viewAttribute;
            }

            Her.Error($"View attribute not found on [{type.FullName}].");

            return null;
        }

        #endregion
    }
}
