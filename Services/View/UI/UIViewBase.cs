using Hermit.UIStack;

namespace Hermit.View
{
    public abstract class UIViewBase : Widget, IUIView
    {
        public abstract void SetViewModel(object context);
    }
}