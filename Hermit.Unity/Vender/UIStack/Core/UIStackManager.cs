using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hermit.Injection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hermit.UIStack
{
    [Serializable]
    public class UIManagerSettings
    {
        public bool IsLandscape = true;

        public Vector2 ReferenceResolution = new Vector2(1920, 1080);
    }

    public enum UILayer
    {
        UIHidden = -1,
        Background = 0,
        Window = 1,
        Fixed = 2,
        Popup = 3,
        Mask = 4
    }

    public class UIStackManager : MonoBehaviour, IUIStack
    {
        private readonly Stack<Widget> StackedWindows = new Stack<Widget>();
        private readonly List<ulong> WindowsInDisplay = new List<ulong>();
        private readonly List<ulong> Popups = new List<ulong>();
        private readonly List<ulong> Fixes = new List<ulong>();

        private readonly Dictionary<UILayer, GameObject> LayerLookup = new Dictionary<UILayer, GameObject>();
        private static readonly Dictionary<Type, IWidgetFactory> FactoryLookup = new Dictionary<Type, IWidgetFactory>();

        private readonly Dictionary<string, Stack<Widget>> PoolingWidgets =
            new Dictionary<string, Stack<Widget>>();

        private IViewManager _viewManager;

        public static UIStackManager FromInstance(UIStackManager uiStackManagerPrefab)
        {
            CollectFactories();

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
            CollectFactories();

            var manager = new GameObject("UI Stack Manager").AddComponent<UIStackManager>();

            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var layerObj = new GameObject(layer.ToString());
                manager.LayerLookup.Add(layer, layerObj);

                var layerCanvas = layerObj.AddComponent<Canvas>();
                layerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                layerCanvas.sortingOrder = (int) layer;

                var layerCanvasScaler = layerObj.AddComponent<CanvasScaler>();
                layerCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                layerCanvasScaler.referenceResolution = settings.ReferenceResolution;
                layerCanvasScaler.matchWidthOrHeight = settings.IsLandscape ? 1 : 0;

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

        public async Task<ulong> PushAsync(string widgetName)
        {
            return await PushAsync<Widget>(widgetName);
        }

        public async Task<ulong> PushAsync(string widgetName, UIMessage message)
        {
            return await PushAsync<Widget>(widgetName, message);
        }

        public async Task<ulong> PushAsync<TWidget>(string widgetName) where TWidget : Widget
        {
            return await PushAsync<TWidget>(widgetName, UIMessage.Empty);
        }

        public async Task<ulong> PushAsync<TWidget>(string widgetName, UIMessage message) where TWidget : Widget
        {
            var instance = await GetInstance<TWidget>(widgetName, message);
            var parent = LayerLookup[instance.Layer];
            instance.transform.SetParent(parent.transform, false);

            switch (instance.Layer)
            {
                case UILayer.Popup:
                    Popups.Add(instance.ViewId);
                    break;
                case UILayer.Fixed:
                    Fixes.Add(instance.ViewId);
                    break;
            }

            if (StackedWindows.Count > 0)
            {
                var prevWidget = StackedWindows.Peek();

                await prevWidget.OnFreeze();

                // Window will overlay previous windows.
                if (instance.Layer == UILayer.Window && WindowsInDisplay.Contains(prevWidget.ViewId))
                {
                    WindowsInDisplay.Remove(prevWidget.ViewId);
                }
            }

            await instance.OnShow();

            StackedWindows.Push(instance);
            WindowsInDisplay.Add(instance.ViewId);

            return instance.ViewId;
        }

        #endregion

        #region Pop

        public async Task PopAsync(bool recycle = false)
        {
            await PopAsync(null, recycle);
        }

        public async Task PopAsync(Action onDone, bool recycle = false)
        {
            if (StackedWindows.Count < 0)
            {
                Debug.LogWarning("Nothing to pop.");
                return;
            }

            var current = StackedWindows.Pop();

            if (StackedWindows.Count > 0)
            {
                await current.OnHide();

                if (recycle) { MoveToHidden(current); }
                else
                {
                    _viewManager.UnRegister(current.ViewId);
                    Destroy(current.gameObject);
                }

                current.Controller?.OnDestroy();
                onDone?.Invoke();

                // resume previous window
                var resumeWindow = StackedWindows.Peek();

                await resumeWindow.OnResume();
            }
            else
            {
                await current.OnHide();

                if (recycle) { MoveToHidden(current); }
                else
                {
                    _viewManager.UnRegister(current.ViewId);
                    Destroy(current.gameObject);
                }

                current.Controller?.OnDestroy();
                onDone?.Invoke();
            }
        }

        #endregion

        #region Clear

        public async Task ClearPopupsAsync()
        {
            foreach (var popup in Popups) { await CloseAsync(popup); }

            Popups.Clear();
        }

        public async Task ClearFixesAsync()
        {
            foreach (var fix in Fixes) { await CloseAsync(fix); }

            Fixes.Clear();
        }

        public async Task ClearWindowsAsync()
        {
            while (StackedWindows.Count > 0)
            {
                var window = StackedWindows.Pop();
                await CloseAsync(window.ViewId);
            }
        }

        public async Task ClearAllAsync()
        {
            await ClearPopupsAsync();
            await ClearFixesAsync();
            await ClearWindowsAsync();
        }

        public async Task CloseAsync(ulong widgetId, bool recycle = false)
        {
            await Close(widgetId, null, recycle);
        }

        public async Task Close(ulong widgetId, Action onClosed, bool recycle = false)
        {
            var targetWidget = _viewManager.GetView<Widget>(widgetId);
            if (targetWidget.Layer != UILayer.Window || WindowsInDisplay.Contains(widgetId))
            {
                await targetWidget.OnHide();

                if (recycle) { MoveToHidden(targetWidget); }
                else
                {
                    _viewManager.UnRegister(targetWidget.ViewId);
                    Destroy(targetWidget.gameObject);
                }

                targetWidget.Controller?.OnDestroy();
                onClosed?.Invoke();

                if (WindowsInDisplay.Contains(widgetId)) { WindowsInDisplay.Remove(widgetId); }
            }
        }

        #endregion

        #region Helpers

        public static void RegisterFactory(Type type, IWidgetFactory factory)
        {
            if (FactoryLookup.ContainsKey(type))
            {
                Debug.LogErrorFormat("Factory already registered for type: {0}.", type);
            }

            FactoryLookup[type] = factory;
        }

        #endregion

        #region Internal Funtions

        private static void CollectFactories()
        {
            if (FactoryLookup.Count > 0) { return; }

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(IWidgetFactory).IsAssignableFrom(x));

            foreach (var factoryType in types)
            {
                if (!factoryType.IsAbstract && !factoryType.IsInterface)
                {
                    var attributes = factoryType.GetCustomAttributes(typeof(CustomWidgetFactoryAttribute), true);
                    if (attributes.Length <= 0) { continue; }

                    if (!(attributes[0] is CustomWidgetFactoryAttribute attribute)) { continue; }

                    if (!(Context.GlobalContext.Create(factoryType) is IWidgetFactory factoryInstance)) { continue; }

                    RegisterFactory(attribute.WidgetType, factoryInstance);
                }
            }
        }

        private async Task<TWidget> GetInstance<TWidget>(string widgetPath, UIMessage message)
            where TWidget : Widget
        {
            var resolveType = typeof(TWidget);
            if (PoolingWidgets.ContainsKey(widgetPath))
            {
                var pool = PoolingWidgets[widgetPath];
                if (pool.Count > 0)
                {
                    var instance = pool.Pop();

                    if (instance.Controller == null) { return instance as TWidget; }

                    try
                    {
                        instance.Controller?.SetControllerInfo(instance, this, message);
                        instance.Controller?.Initialize();
                    }
                    catch (Exception ex) { Debug.LogException(ex); }

                    return instance as TWidget;
                }
            }

            var useSpecifiedFactory = false;
            if (!FactoryLookup.TryGetValue(resolveType, out var factory))
            {
                while (factory == null && resolveType.BaseType != null)
                {
                    resolveType = resolveType.BaseType;
                    FactoryLookup.TryGetValue(resolveType, out factory);
                }

                if (factory == null)
                {
                    Debug.LogError($"Widget factory not found for type: {typeof(TWidget)}, no fallback.");
                    return null;
                }
            }
            else { useSpecifiedFactory = true; }

            TWidget ret = null;

            // fallback
            if (useSpecifiedFactory)
            {
                if (factory is IWidgetFactory<TWidget> specifiedFactory)
                {
                    ret = await specifiedFactory.CreateInstance(this, widgetPath, message);
                }
            }
            else
            {
                var widgetCreated = await factory.CreateInstance(this, widgetPath, message);
                ret = widgetCreated as TWidget;
                if (ret == null)
                {
                    Debug.LogWarningFormat("Can not convert [{0}] to type: {1}", widgetCreated, typeof(TWidget));
                }
            }

            return ret;
        }

        private void RunCoroutine(IEnumerator target, Action onDone)
        {
            StartCoroutine(MonitorCoroutine(target, onDone));
        }

        private IEnumerator MonitorCoroutine(IEnumerator target, Action onDone)
        {
            yield return target;
            onDone?.Invoke();
        }

        private void MoveToHidden(Widget toHide)
        {
            var hiddenLayer = LayerLookup[UILayer.UIHidden];
            toHide.transform.SetParent(hiddenLayer.transform);

            var type = toHide.GetType();
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