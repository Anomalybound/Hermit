using Hermit.Views;

namespace Hermit
{
    public partial class App
    {
        public static IView GetView(ulong id)
        {
            return I.ViewManager.GetView(id);
        }

        public static TView GetView<TView>(ulong id) where TView : IView
        {
            return I.ViewManager.GetView<TView>(id);
        }

        public static ulong Register<TView>(TView view) where TView : IView
        {
            return I.ViewManager.Register<TView>(view);
        }

        public static void UnRegister(ulong id)
        {
            I.ViewManager.UnRegister(id);
        }
    }
}
