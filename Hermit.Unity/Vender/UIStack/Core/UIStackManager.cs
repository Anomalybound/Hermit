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
        private readonly List<int> WindowsInDisplay = new List<int>();
        private readonly List<int> Popups = new List<int>();
        private readonly List<int> Fixes = new List<int>();

        private readonly Dictionary<int, Widget> WidgetLookup = new Dictionary<int, Widget>();
        private readonly Dictionary<UILayer, GameObject> LayerLookup = new Dictionary<UILayer, GameObject>();
        private static readonly Dictionary<Type, IWidgetFactory> FactoryLookup = new Dictionary<Type, IWidgetFactory>();

        private readonly Dictionary<string, Stack<Widget>> PoolingWidgets =
            new Dictionary<string, Stack<Widget>>();

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

        public static UIStackManager BuildHierarchy(bool landscapeOrientation = true, Vector2? refResolution = null)
        {
            CollectFactories();

            var manager = new GameObject("UI Stack Manager").AddComponent<UIStackManager>();

            var uiCam = new GameObject("UI Camera", typeof(Camera)).GetComponent<Camera>();
            uiCam.clearFlags = CameraClearFlags.Depth;
            uiCam.cullingMask = 1 << LayerMask.NameToLayer("UI");
            uiCam.orthographic = true;
            uiCam.depth = 10;
            uiCam.transform.SetParent(manager.transform);
            uiCam.transform.localPosition = Vector3.zero;

            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var layerObj = new GameObject(layer.ToString());
                manager.LayerLookup.Add(layer, layerObj);

                var layerCanvas = layerObj.AddComponent<Canvas>();
                layerCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                layerCanvas.worldCamera = uiCam;
                layerCanvas.sortingOrder = (int) layer;

                var layerCanvasScaler = layerObj.AddComponent<CanvasScaler>();
                layerCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                layerCanvasScaler.referenceResolution = refResolution ?? (landscapeOrientation
                                                            ? new Vector2(1920, 1080)
                                                            : new Vector2(1080, 1920));
                layerCanvasScaler.matchWidthOrHeight = landscapeOrientation ? 1 : 0;

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

        #region Widget Manager

        private int _componentId;

        #region Push

        public async Task<int> Push(string widgetName)
        {
            return await Push<Widget>(widgetName);
        }

        public async Task<int> Push(string widgetName, UIMessage message)
        {
            return await Push<Widget>(widgetName, message);
        }

        public async Task<int> Push<TWidget>(string widgetName) where TWidget : Widget
        {
            return await Push<TWidget>(widgetName, UIMessage.Empty);
        }

        public async Task<int> Push<TWidget>(string widgetName, UIMessage message) where TWidget : Widget
        {
            var id = GetId();
            var instance = await GetInstance<TWidget>(widgetName, id, message);
            var parent = LayerLookup[instance.Layer];
            instance.transform.SetParent(parent.transform, false);
//                instance.transform.SetAsFirstSibling();

            switch (instance.Layer)
            {
                case UILayer.Popup:
                    Popups.Add(instance.Id);
                    break;
                case UILayer.Fixed:
                    Fixes.Add(instance.Id);
                    break;
            }

            if (StackedWindows.Count > 0)
            {
                var prevWidget = StackedWindows.Peek();

                await prevWidget.OnFreeze();

                // Window will overlay previous windows.
                if (instance.Layer == UILayer.Window && WindowsInDisplay.Contains(prevWidget.Id))
                {
                    WindowsInDisplay.Remove(prevWidget.Id);
                }
            }

            await instance.OnShow();

            StackedWindows.Push(instance);
            WidgetLookup.Add(id, instance);
            WindowsInDisplay.Add(id);

            return id;
        }

        #endregion

        #region Pop

        public async Task Pop(bool recycle = false)
        {
            await Pop(null, recycle);
        }

        public async Task Pop(Action onDone, bool recycle = false)
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
                    WidgetLookup.Remove(current.Id);
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
                    WidgetLookup.Remove(current.Id);
                    Destroy(current.gameObject);
                }

                current.Controller?.OnDestroy();
                onDone?.Invoke();
            }
        }

        #endregion

        #region Clear

        public async Task ClearPopups()
        {
            foreach (var popup in Popups) { await Close(popup); }

            Popups.Clear();
        }

        public async Task ClearFixes()
        {
            foreach (var fix in Fixes) { await Close(fix); }

            Fixes.Clear();
        }

        public async Task ClearWindows()
        {
            while (StackedWindows.Count > 0)
            {
                var window = StackedWindows.Pop();
                await Close(window.Id);
            }
        }

        public async Task ClearAll()
        {
            await ClearPopups();
            await ClearFixes();
            await ClearWindows();
        }

        public async Task Close(int widgetId, bool recycle = false)
        {
            await Close(widgetId, null, recycle);
        }

        public async Task Close(int widgetId, Action onClosed, bool recycle = false)
        {
            var targetWidget = Get(widgetId);
            if (targetWidget.Layer != UILayer.Window || !WindowsInDisplay.Contains(widgetId))
            {
                await targetWidget.OnHide();

                if (recycle) { MoveToHidden(targetWidget); }
                else
                {
                    WidgetLookup.Remove(targetWidget.Id);
                    Destroy(targetWidget.gameObject);
                }

                targetWidget.Controller?.OnDestroy();
                onClosed?.Invoke();

                if (WindowsInDisplay.Contains(widgetId)) { WindowsInDisplay.Remove(widgetId); }
            }
        }

        #endregion

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

        private async Task<TWidget> GetInstance<TWidget>(string widgetPath, int assignedId, UIMessage message)
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
                    ret = await specifiedFactory.CreateInstance(this, widgetPath, assignedId, message);
                }
            }
            else
            {
                var widgetCreated = await factory.CreateInstance(this, widgetPath, assignedId, message);
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

        private int GetId()
        {
            return _componentId++;
        }

        public Widget Get(int id)
        {
            if (!WidgetLookup.TryGetValue(id, out var targetComp))
            {
                Debug.LogWarningFormat("Can't load widget of id: {0}.", id);
            }

            return targetComp;
        }

        public TUiComponent Get<TUiComponent>(int id) where TUiComponent : Widget
        {
            if (!WidgetLookup.TryGetValue(id, out var targetComp))
            {
                Debug.LogWarningFormat("Can't load widget of id: {0}.", id);
            }
            else
            {
                var resultComponent = targetComp as TUiComponent;
                if (resultComponent != null) { return resultComponent; }
            }

            return null;
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
    }
}