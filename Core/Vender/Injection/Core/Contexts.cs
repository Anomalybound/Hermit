using System;

namespace Hermit.Injection.Core
{
    public static class Contexts
    {
        public static IContext GlobalContext { get; private set; }

        public static void SetCurrentContext(IContext context)
        {
            if (GlobalContext != null) { throw new Exception("Global context already exists."); }

            GlobalContext = context;
        }
    }
}