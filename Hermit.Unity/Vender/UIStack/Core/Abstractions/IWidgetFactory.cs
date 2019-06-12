using System.Threading.Tasks;

namespace Hermit.UIStack
{
    public interface IWidgetFactory
    {
        Task<IWidget> CreateInstance(IUIStack manager, string name, UIMessage message);
    }

    public interface IWidgetFactory<TWidget> : IWidgetFactory where TWidget : Widget
    {
        new Task<TWidget> CreateInstance(IUIStack manager, string name, UIMessage message);

        void ReturnInstance(TWidget widget);
    }
}