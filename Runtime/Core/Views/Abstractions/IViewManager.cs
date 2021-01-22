namespace Hermit.Views
{
    public interface IViewManager
    {
        IView GetView(ulong id);

        TView GetView<TView>(ulong id) where TView : IView;

        ulong Register<TView>(TView view) where TView : IView;

        void UnRegister(ulong id);
    }
}