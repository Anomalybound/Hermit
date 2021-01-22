namespace Hermit.Service.Log
{
    public interface ILog
    {
        void Log(object obj);

        void Warn(object warning);

        void Error(object error);

        void Assert(bool condition, string message);
    }
}