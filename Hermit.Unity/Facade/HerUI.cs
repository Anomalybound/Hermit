using System.Threading.Tasks;
using Hermit.UIStack;
using Hermit.View;

namespace Hermit
{
    public partial class Her
    {
        #region UI Manager

        public static async Task<IViewModelProvider> PushUIAsync(string widgetName)
        {
            return await PushUIAsync(widgetName, UIMessage.Empty);
        }

        public static async Task<IViewModelProvider> PushUIAsync(string widgetName, UIMessage message)
        {
            var id = await Current._uiStack.PushAsync(widgetName, message);
            return Current._viewManager.GetView(id) as IViewModelProvider;
        }

        public static async Task<IViewModelProvider> PushUIAsync<TWidget>(string widgetName) where TWidget : Widget
        {
            return await PushUIAsync<TWidget>(widgetName, UIMessage.Empty);
        }

        public static async Task<IViewModelProvider> PushUIAsync<TWidget>(string widgetName, UIMessage message)
            where TWidget : Widget
        {
            var id = await Current._uiStack.PushAsync<TWidget>(widgetName, message);
            return Current._viewManager.GetView(id) as IViewModelProvider;
        }

        public static async void PopUIAsync(bool reuse = false)
        {
            await Current._uiStack.PopAsync(reuse);
        }

        public static async void ClearUIPopupsAsync(bool reuse = false)
        {
            await Current._uiStack.ClearPopupsAsync(reuse);
        }

        public static async void ClearUIWindowsAsync(bool reuse = false)
        {
            await Current._uiStack.ClearWindowsAsync(reuse);
        }

        public static async void ClearAllUIAsync(bool reuse = false)
        {
            await Current._uiStack.ClearAllAsync(reuse);
        }

        public static async void CloseUIAsync(ulong widgetId, bool reuse = false)
        {
            await Current._uiStack.CloseAsync(widgetId, reuse);
        }

        #endregion
    }
}