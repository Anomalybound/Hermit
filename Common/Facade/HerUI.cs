using System.Threading.Tasks;
using Hermit.UIStack;

namespace Hermit
{
    public partial class Her
    {
        #region UI Manager

        public static async Task<int> Push(string widgetName)
        {
            return await Push<Widget>(widgetName);
        }

        public static async Task<int> Push(string widgetName, UIMessage message)
        {
            return await Push<Widget>(widgetName, message);
        }

        public static async Task<int> Push<TWidget>(string widgetName) where TWidget : Widget
        {
            return await Push<TWidget>(widgetName, UIMessage.Empty);
        }

        public static async Task<int> Push<TWidget>(string widgetName, UIMessage message)
            where TWidget : Widget
        {
            return await Current._iuiStack.Push<TWidget>(widgetName, message);
        }

        public static async void Pop(bool recycle = false)
        {
            await Current._iuiStack.Pop(recycle);
        }

        public static async void ClearPopups()
        {
            await Current._iuiStack.ClearPopups();
        }

        public static async void ClearFixes()
        {
            await Current._iuiStack.ClearFixes();
        }

        public static async void ClearWindows()
        {
            await Current._iuiStack.ClearWindows();
        }

        public static async void ClearAll()
        {
            await Current._iuiStack.ClearAll();
        }

        public static async void Close(int widgetId, bool recycle = false)
        {
            await Current._iuiStack.Close(widgetId, recycle);
        }

        #endregion
    }
}