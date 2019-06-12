using System.Threading.Tasks;
using Hermit.UIStack;

namespace Hermit
{
    public partial class Her
    {
        #region UI Manager

        public static async Task<ulong> PushAsync(string widgetName)
        {
            return await PushAsync<Widget>(widgetName);
        }

        public static async Task<ulong> PushAsync(string widgetName, UIMessage message)
        {
            return await PushAsync<Widget>(widgetName, message);
        }

        public static async Task<ulong> PushAsync<TWidget>(string widgetName) where TWidget : Widget
        {
            return await PushAsync<TWidget>(widgetName, UIMessage.Empty);
        }

        public static async Task<ulong> PushAsync<TWidget>(string widgetName, UIMessage message)
            where TWidget : Widget
        {
            return await Current.uiStack.PushAsync<TWidget>(widgetName, message);
        }

        public static async void Pop(bool recycle = false)
        {
            await Current.uiStack.PopAsync(recycle);
        }

        public static async void ClearPopups()
        {
            await Current.uiStack.ClearPopups();
        }

        public static async void ClearFixes()
        {
            await Current.uiStack.ClearFixes();
        }

        public static async void ClearWindows()
        {
            await Current.uiStack.ClearWindows();
        }

        public static async void ClearAll()
        {
            await Current.uiStack.ClearAll();
        }

        public static async void Close(ulong widgetId, bool recycle = false)
        {
            await Current.uiStack.Close(widgetId, recycle);
        }

        #endregion
    }
}