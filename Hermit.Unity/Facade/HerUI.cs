using System.Threading.Tasks;
using Hermit.UIStack;

namespace Hermit
{
    public partial class Her
    {
        #region UI Manager

        public static async Task<ulong> PushUIAsync(string widgetName)
        {
            return await PushUIAsync<Widget>(widgetName);
        }

        public static async Task<ulong> PushUIAsync(string widgetName, UIMessage message)
        {
            return await PushUIAsync<Widget>(widgetName, message);
        }

        public static async Task<ulong> PushUIAsync<TWidget>(string widgetName) where TWidget : Widget
        {
            return await PushUIAsync<TWidget>(widgetName, UIMessage.Empty);
        }

        public static async Task<ulong> PushUIAsync<TWidget>(string widgetName, UIMessage message)
            where TWidget : Widget
        {
            return await Current.uiStack.PushAsync<TWidget>(widgetName, message);
        }

        public static async void PopUIAsync(bool recycle = false)
        {
            await Current.uiStack.PopAsync(recycle);
        }

        public static async void ClearUIPopupsAsync()
        {
            await Current.uiStack.ClearPopupsAsync();
        }

        public static async void ClearUIFixesAsync()
        {
            await Current.uiStack.ClearFixesAsync();
        }

        public static async void ClearUIWindowsAsync()
        {
            await Current.uiStack.ClearWindowsAsync();
        }

        public static async void ClearAllUIAsync()
        {
            await Current.uiStack.ClearAllAsync();
        }

        public static async void CloseUIAsync(ulong widgetId, bool recycle = false)
        {
            await Current.uiStack.CloseAsync(widgetId, recycle);
        }

        #endregion
    }
}