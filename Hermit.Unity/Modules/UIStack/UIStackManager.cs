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
        public bool screenSpaceCameraMode = true;

        [Range(0, 1)]
        public float matchWidthOrHeight = 0.5f;

        public Vector2 referenceResolution = new Vector2(1920, 1080);
    }

    public enum UILayer
    {
        Hidden = -1,
        Background = 0,
        Window = 1,
        Popup = 2
    }

    public class UIStackManager : MonoBehaviour, IUIStack
    {
        private readonly Stack<Widget> _stackedWindows = new Stack<Widget>();
        private readonly List<ulong> _windowsInDisplay = new List<ulong>();
        private readonly List<ulong> _popups = new List<ulong>();

        private readonly Dictionary<UILayer, GameObject> _layerLookup = new Dictionary<UILayer, GameObject>();
        private IWidgetFactory _defaultFactory = new DefaultWidgetFactory();
        private readonly Dictionary<ulong, IWidgetFactory> _factoryLookup = new Dictionary<ulong, IWidgetFactory>();

        private readonly Dictionary<string, Stack<Widget>> _poolingWidgets =
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

                instance._layerLookup.Add(layer, child.gameObject);
            }

            return instance;
        }

        public static UIStackManager BuildHierarchy(UIManagerSettings settings)
        {
            var manager = new GameObject("UI Stack Manager").AddComponent<UIStackManager>();

            var cam = Camera.main;
            if (settings.screenSpaceCameraMode)
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
                manager._layerLookup.Add(layer, layerObj);

                var layerCanvas = layerObj.AddComponent<Canvas>();
                if (settings.screenSpaceCameraMode)
                {
                    layerCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    layerCanvas.worldCamera = cam;
                }
                else { layerCanvas.renderMode = RenderMode.ScreenSpaceOverlay; }

                layerCanvas.sortingOrder = (int) layer;

                var layerCanvasScaler = layerObj.AddComponent<CanvasScaler>();
                layerCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                layerCanvasScaler.referenceResolution = settings.referenceResolution;
                layerCanvasScaler.matchWidthOrHeight = settings.matchWidthOrHeight;

                var layerRaycaster = layerObj.AddComponent<GraphicRaycaster>();
                layerRaycaster.name = layer.ToString();
                layerRaycaster.transform.SetParent(manager.transform);
                layerRaycaster.transform.localPosition = Vector3.zero;

                if (layer == UILayer.Hidden)
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

        public async Task<ulong> PushAsync(string widgetName, UIMessage message,
            IWidgetFactory factory = null)
        {
            return await PushAsync<Widget>(widgetName, message, factory);
        }

        public async Task<ulong> PushAsync<TWidget>(string widgetName, UIMessage message,
            IWidgetFactory factory = null)
            where TWidget : Widget
        {
            var instance = await GetInstance<TWidget>(widgetName, message, factory);
            var parent = _layerLookup[instance.Layer];
            instance.transform.SetParent(parent.transform, false);

            switch (instance.Layer)
            {
                case UILayer.Popup:
                    _popups.Add(instance.ViewId);
                    break;
            }

            if (_stackedWindows.Count > 0)
            {
                var prevWidget = _stackedWindows.Peek();

                if (prevWidget != null)
                {
                    await prevWidget.OnFreeze();

                    // Window will overlay previous windows.
                    if (instance.Layer == UILayer.Window && _windowsInDisplay.Contains(prevWidget.ViewId))
                    {
                        _windowsInDisplay.Remove(prevWidget.ViewId);
                    }
                }
            }

            await instance.OnShow();

            _stackedWindows.Push(instance);
            _windowsInDisplay.Add(instance.ViewId);

            return instance.ViewId;
        }

        #endregion

        #region Pop

        public async Task PopAsync(bool reuse = false)
        {
            if (_stackedWindows.Count < 0)
            {
                Debug.LogWarning("Nothing to pop.");
                return;
            }

            var current = _stackedWindows.Pop();

            await current.OnHide();

            if (reuse) { MoveToHidden(current); }
            else { ReturnWidget(current); }

            if (_stackedWindows.Count > 0)
            {
                // resume previous window
                var resumeWindow = _stackedWindows.Peek();
                await resumeWindow.OnResume();
            }
        }

        #endregion

        #region Clear

        public async Task ClearPopupsAsync(bool reuse = false)
        {
            foreach (var popup in _popups) { await CloseAsync(popup, reuse); }

            _popups.Clear();
        }

        public async Task ClearWindowsAsync(bool reuse = false)
        {
            while (_stackedWindows.Count > 0)
            {
                var window = _stackedWindows.Pop();
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
            if (targetWidget == null) { return; }

            if (targetWidget.Layer != UILayer.Window || _windowsInDisplay.Contains(widgetId))
            {
                await targetWidget.OnHide();

                if (reuse) { MoveToHidden(targetWidget); }
                else { ReturnWidget(targetWidget); }

                if (_windowsInDisplay.Contains(widgetId)) { _windowsInDisplay.Remove(widgetId); }
            }
        }

        #endregion

        #region Helpers

        public void RegisterDefaultFactory(IWidgetFactory factory)
        {
            _defaultFactory = factory ?? throw new ArgumentNullException();
        }

        private void ReturnWidget(Widget widget)
        {
            if (_factoryLookup.TryGetValue(widget.ViewId, out var factory))
            {
                _factoryLookup.Remove(widget.ViewId);
                factory.ReturnInstance(widget);
            }
            else { _defaultFactory.ReturnInstance(widget); }
        }

        #endregion

        #region Internal Funtions

        private async Task<TWidget> GetInstance<TWidget>(string widgetPath, UIMessage message,
            IWidgetFactory factory = null)
            where TWidget : Widget
        {
            if (_poolingWidgets.ContainsKey(widgetPath))
            {
                var pool = _poolingWidgets[widgetPath];
                if (pool.Count > 0)
                {
                    var instance = pool.Pop();
                    return instance as TWidget;
                }
            }

            if (factory == null) { factory = _defaultFactory; }

            TWidget widget;
            try { widget = (TWidget) await factory.CreateInstance(this, widgetPath, message); }
            catch (Exception e)
            {
                Her.Error(e);
                throw;
            }

            // mark widget factory
            _factoryLookup.Add(widget.ViewId, factory);

            return widget;
        }

        private void MoveToHidden(Widget toHide)
        {
            var hiddenLayer = _layerLookup[UILayer.Hidden];
            toHide.transform.SetParent(hiddenLayer.transform);

            if (_poolingWidgets.ContainsKey(toHide.Path)) { _poolingWidgets[toHide.Path].Push(toHide); }
            else
            {
                var newStack = new Stack<Widget>();
                newStack.Push(toHide);
                _poolingWidgets.Add(toHide.Path, newStack);
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