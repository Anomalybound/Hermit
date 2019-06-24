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

        public static async void PopUIAsync(bool reuse = false)
        {
            await Current.uiStack.PopAsync(reuse);
        }

        public static async void ClearUIPopupsAsync(bool reuse = false)
        {
            await Current.uiStack.ClearPopupsAsync(reuse);
        }

        public static async void ClearUIFixesAsync(bool reuse = false)
        {
            await Current.uiStack.ClearFixesAsync(reuse);
        }

        public static async void ClearUIWindowsAsync(bool reuse = false)
        {
            await Current.uiStack.ClearWindowsAsync(reuse);
        }

        public static async void ClearAllUIAsync(bool reuse = false)
        {
            await Current.uiStack.ClearAllAsync(reuse);
        }

        public static async void CloseUIAsync(ulong widgetId, bool reuse = false)
        {
            await Current.uiStack.CloseAsync(widgetId, reuse);
        }

        #endregion
    }
}