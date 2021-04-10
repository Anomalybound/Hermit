namespace Hermit
{
    /// <summary>
    ///  Her.Log would be available anytime.
    /// </summary>
    public partial class App
    {
        public static void Log(object message)
        {
            I.Logger.Log(message);
        }

        public static void Warn(object message)
        {
            I.Logger.Warn(message);
        }

        public static void Error(object message)
        {
            I.Logger.Error(message);
        }

        public static void Assert(bool condition, string message)
        {
            I.Logger.Assert(condition, message);
        }
    }
}