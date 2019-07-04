namespace Hermits
{
    public interface IPooledObjectPolicy<T> where T : class
    {
        T Rent();

        bool Return(T instance);
    }
}