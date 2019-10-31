using System.Threading.Tasks;
using Hermit.UIStack;

namespace Hermit
{
    public partial class Her
    {
        #region UI Manager

        public static async Task<IView> PushUIAsync(string widgetName)
        {
            return await PushUIAsync(widgetName, UIMessage.Empty);
        }

        public static async Task<IView> PushUIAsync(string widgetName, UIMessage message)
        {
            var id = await Current.UiStack.PushAsync(widgetName, message);
            return Current._viewManager.GetView(id);
        }

        public static async Task<IView> PushUIAsync<TWidget>(string widgetName) where TWidget : Widget
        {
            return await PushUIAsync<TWidget>(widgetName, UIMessage.Empty);
        }

        public static async Task<IView> PushUIAsync<TWidget>(string widgetName, UIMessage message)
            where TWidget : Widget
        {
            var id = await Current.UiStack.PushAsync<TWidget>(widgetName, message);
            return Current._viewManager.GetView(id);
        }

        public static async void PopUIAsync(bool reuse = false)
        {
            await Current.UiStack.PopAsync(reuse);
        }

        public static async void ClearUIPopupsAsync(bool reuse = false)
        {
            await Current.UiStack.ClearPopupsAsync(reuse);
        }

        public static async void ClearUIWindowsAsync(bool reuse = false)
        {
            await Current.UiStack.ClearWindowsAsync(reuse);
        }

        public static async void ClearAllUIAsync(bool reuse = false)
        {
            await Current.UiStack.ClearAllAsync(reuse);
        }

        public static async void CloseUIAsync(ulong widgetId, bool reuse = false)
        {
            await Current.UiStack.CloseAsync(widgetId, reuse);
        }

        #endregion
    }
}