using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hermit.UIStack
{
    [Serializable]
    public class UIManagerSettings
    {
        public bool ScreenSpaceCameraMode = true;

        [Range(0, 1)]
        public float MatchWidthOrHeight = 0.5f;

        public Vector2 ReferenceResolution = new Vector2(1920, 1080);
    }

    public enum UILayer
    {
        UIHidden = -1,
        Background = 0,
        Window = 1,
        Popup = 2
    }

    public class UIStackManager : MonoBehaviour, IUIStack
    {
        private readonly Stack<Widget> StackedWindows = new Stack<Widget>();
        private readonly List<ulong> WindowsInDisplay = new List<ulong>();
        private readonly List<ulong> Popups = new List<ulong>();

        private readonly Dictionary<UILayer, GameObject> LayerLookup = new Dictionary<UILayer, GameObject>();
        private IWidgetFactory DefaultFactory = new DefaultWidgetFactory();
        private readonly Dictionary<ulong, IWidgetFactory> FactoryLookup = new Dictionary<ulong, IWidgetFactory>();

        private readonly Dictionary<string, Stack<Widget>> PoolingWidgets =
            new Dictionary<string, Stack<Widget>>();

        private IViewManager _viewManager;

        public static UIStackManager FromInstance(UIStackManager uiStackManagerPrefab)
        {
            var instance = Instantiate(uiStackManagerPrefab);

            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var layerLabel = layer.ToString();

                var child = instance.transform.Find(layerLabel);
                if (child == null)
                {
                    Debug.LogError($"Layer {layerLabel} can not be found in UI Manager.");
                    continue;
                }

                instance.LayerLookup.Add(layer, child.gameObject);
            }

            return instance;
        }

        public static UIStackManager BuildHierarchy(UIManagerSettings settings)
        {
            var manager = new GameObject("UI Stack Manager").AddComponent<UIStackManager>();

            var cam = Camera.main;
            if (settings.ScreenSpaceCameraMode)
            {
                cam = new GameObject("UI Camera").AddComponent<Camera>();
                cam.orthographic = true;
                cam.transform.SetParent(manager.transform, false);
                cam.clearFlags = CameraClearFlags.Depth;
                cam.cullingMask = LayerMask.GetMask("UI");
            }

            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var layerObj = new GameObject(layer.ToString());
                manager.LayerLookup.Add(layer, layerObj);

                var layerCanvas = layerObj.AddComponent<Canvas>();
                if (settings.ScreenSpaceCameraMode)
                {
                    layerCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    layerCanvas.worldCamera = cam;
                }
                else { layerCanvas.renderMode = RenderMode.ScreenSpaceOverlay; }

                layerCanvas.sortingOrder = (int) layer;

                var layerCanvasScaler = layerObj.AddComponent<CanvasScaler>();
                layerCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                layerCanvasScaler.referenceResolution = settings.ReferenceResolution;
                layerCanvasScaler.matchWidthOrHeight = settings.MatchWidthOrHeight;

                var layerRaycaster = layerObj.AddComponent<GraphicRaycaster>();
                layerRaycaster.name = layer.ToString();
                layerRaycaster.transform.SetParent(manager.transform);
                layerRaycaster.transform.localPosition = Vector3.zero;

                if (layer == UILayer.UIHidden)
                {
                    layerObj.layer = LayerMask.NameToLayer("UIHidden");
                    layerRaycaster.enabled = false;
                }
                else { layerObj.layer = LayerMask.NameToLayer("UI"); }
            }

            if (FindObjectOfType<EventSystem>() != null) { return manager; }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(manager.transform, false);

            return manager;
        }

        #region Push

        public async Task<ulong> PushAsync(string widgetName, IWidgetFactory factory = null)
        {
            return await PushAsync<Widget>(widgetName, factory);
        }

        public async Task<ulong> PushAsync(string widgetName, UIMessage message, IWidgetFactory factory = null)
        {
            return await PushAsync<Widget>(widgetName, message, factory);
        }

        public async Task<ulong> PushAsync<TWidget>(string widgetName, IWidgetFactory factory = null)
            where TWidget : Widget
        {
            return await PushAsync<TWidget>(widgetName, UIMessage.Empty, factory);
        }

        public async Task<ulong> PushAsync<TWidget>(string widgetName, UIMessage message, IWidgetFactory factory = null)
            where TWidget : Widget
        {
            var instance = await GetInstance<TWidget>(widgetName, message, factory);
            var parent = LayerLookup[instance.Layer];
            instance.transform.SetParent(parent.transform, false);

            switch (instance.Layer)
            {
                case UILayer.Popup:
                    Popups.Add(instance.ViewId);
                    break;
            }

            if (StackedWindows.Count > 0)
            {
                var prevWidget = StackedWindows.Peek();

                if (prevWidget != null)
                {
                    await prevWidget.OnFreeze();

                    // Window will overlay previous windows.
                    if (instance.Layer == UILayer.Window && WindowsInDisplay.Contains(prevWidget.ViewId))
                    {
                        WindowsInDisplay.Remove(prevWidget.ViewId);
                    }
                }
            }

            await instance.OnShow();

            StackedWindows.Push(instance);
            WindowsInDisplay.Add(instance.ViewId);

            return instance.ViewId;
        }

        #endregion

        #region Pop

        public async Task PopAsync(bool reuse = false)
        {
            if (StackedWindows.Count < 0)
            {
                Debug.LogWarning("Nothing to pop.");
                return;
            }

            var current = StackedWindows.Pop();

            await current.OnHide();

            if (reuse) { MoveToHidden(current); }
            else { ReturnWidget(current); }

            if (StackedWindows.Count > 0)
            {
                // resume previous window
                var resumeWindow = StackedWindows.Peek();
                await resumeWindow.OnResume();
            }
        }

        #endregion

        #region Clear

        public async Task ClearPopupsAsync(bool reuse = false)
        {
            foreach (var popup in Popups) { await CloseAsync(popup, reuse); }

            Popups.Clear();
        }

        public async Task ClearWindowsAsync(bool reuse = false)
        {
            while (StackedWindows.Count > 0)
            {
                var window = StackedWindows.Pop();
                await CloseAsync(window.ViewId, reuse);
            }
        }

        public async Task ClearAllAsync(bool reuse = false)
        {
            await ClearPopupsAsync(reuse);
            await ClearWindowsAsync(reuse);
        }

        public async Task CloseAsync(ulong widgetId, bool reuse = false)
        {
            var targetWidget = _viewManager.GetView<Widget>(widgetId);
            if (targetWidget.Layer != UILayer.Window || WindowsInDisplay.Contains(widgetId))
            {
                await targetWidget.OnHide();

                if (reuse) { MoveToHidden(targetWidget); }
                else { ReturnWidget(targetWidget); }

                if (WindowsInDisplay.Contains(widgetId)) { WindowsInDisplay.Remove(widgetId); }
            }
        }

        #endregion

        #region Helpers

        public void RegisterDefaultFactory(IWidgetFactory factory)
        {
            DefaultFactory = factory ?? throw new ArgumentNullException();
        }

        private void ReturnWidget(Widget widget)
        {
            if (FactoryLookup.TryGetValue(widget.ViewId, out var factory))
            {
                FactoryLookup.Remove(widget.ViewId);
                factory.ReturnInstance(widget);
            }
            else { DefaultFactory.ReturnInstance(widget); }
        }

        #endregion

        #region Internal Funtions

        private async Task<TWidget> GetInstance<TWidget>(string widgetPath, UIMessage message,
            IWidgetFactory factory = null)
            where TWidget : Widget
        {
            if (PoolingWidgets.ContainsKey(widgetPath))
            {
                var pool = PoolingWidgets[widgetPath];
                if (pool.Count > 0)
                {
                    var instance = pool.Pop();
                    return instance as TWidget;
                }
            }

            if (factory == null) { factory = DefaultFactory; }

            TWidget widget;
            try { widget = (TWidget) await factory.CreateInstance(this, widgetPath, message); }
            catch (Exception e)
            {
                Her.Error(e);
                throw;
            }

            // mark widget factory
            FactoryLookup.Add(widget.ViewId, factory);

            return widget;
        }

        private void MoveToHidden(Widget toHide)
        {
            var hiddenLayer = LayerLookup[UILayer.UIHidden];
            toHide.transform.SetParent(hiddenLayer.transform);

            if (PoolingWidgets.ContainsKey(toHide.Path)) { PoolingWidgets[toHide.Path].Push(toHide); }
            else
            {
                var newStack = new Stack<Widget>();
                newStack.Push(toHide);
                PoolingWidgets.Add(toHide.Path, newStack);
            }
        }

        #endregion

        #region Unity LifeTime

        private void Start()
        {
            _viewManager = Her.Resolve<IViewManager>();
        }

        #endregion
    }
}