namespace Hermit.Service.ObjectPool
{
    public class DefaultPooledObjectPolicy<T> : IPooledObjectPolicy<T> where T : class, new()
    {
        public T Rent()
        {
            return new T();
        }

        public bool Return(T obj)
        {
            return true;
        }
    }
}