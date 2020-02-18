using System;

namespace Hermit
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(() => new T());

        public static T Instance => LazyInstance.Value;
    }
}