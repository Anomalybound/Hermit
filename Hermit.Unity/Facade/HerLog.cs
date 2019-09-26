namespace Hermit
{
    /// <summary>
    ///  Her.Log would be available anytime.
    /// </summary>
    public partial class Her
    {
        public static void Log(object message)
        {
            Current.Logger.Log(message);
        }

        public static void Warn(object message)
        {
            Current.Logger.Warn(message);
        }

        public static void Error(object message)
        {
            Current.Logger.Error(message);
        }

        public static void Assert(bool condition, string message)
        {
            Current.Logger.Assert(condition, message);
        }
    }
}