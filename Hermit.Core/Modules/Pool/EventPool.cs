using System;
using Hermits;

namespace Hermit
{
    public static class EventPool<T> where T : EventData, new()
    {
        private static ObjectPool<T> InternalPool { get; }

        static EventPool()
        {
            InternalPool = new ObjectPool<T>(new DefaultPooledObjectPolicy<T>(), 512);
        }

        public static T Rent()
        {
            var instance = InternalPool.Rent();
            instance.SentTime = DateTime.Now;
            return instance;
        }

        public static void Return(T obj)
        {
            InternalPool.Return(obj);
        }
    }
}