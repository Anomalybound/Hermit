using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace Hermit.View
{
    [Serializable]
    public class UIControllerSettings
    {
        public bool screenSpaceCameraMode = true;

        public RectTransform maskObject;

        [Range(0, 1)]
        public float matchWidthOrHeight = 0.5f;

        public Vector2 referenceResolution = new Vector2(1920, 1080);
    }

    public class UIController : MonoBehaviour, IUIStack
    {
        #region Runtme Variables

        public IUIView Focus { get; private set; }

        protected Canvas WindowRoot { get; private set; }

        protected Canvas MaskRoot { get; private set; }

        protected Canvas ModalRoot { get; private set; }

        protected IViewFactory DefaultFactory { get; private set; }

        protected IViewManager ViewManager { get; private set; }

        protected Dictionary<ulong, IViewFactory> FactoryLookup { get; } = new Dictionary<ulong, IViewFactory>();

        protected Stack<IUIView> Windows { get; } = new Stack<IUIView>();

        protected Stack<IUIView> ModalWindows { get; } = new Stack<IUIView>();

        protected RectTransform MaskObject { get; private set; }

        #endregion

        #region IUIController

        public void RegisterDefaultFactory(IViewFactory factory)
        {
            DefaultFactory = factory ?? throw new ArgumentNullException();
        }

        public async Task CloseAsync(ulong viewId)
        {
            var targetWidget = ViewManager.GetView<IUIView>(viewId);
            if (targetWidget == null) { return; }

            await targetWidget.OnHide();
            ReturnInstance(targetWidget);
        }

        #endregion

        #region IUIStack

        public async Task<IUIView> PushAsync<TUIView>(IViewFactory factory = null) where TUIView : IUIView
        {
            var instance = await GetInstance<TUIView>(factory);
            var parent = WindowRoot;
            instance.gameObject.transform.SetParent(parent.transform, false);

            if (Windows.Count > 0)
            {
                await FreezeFocusUIView(Windows);

                Windows.Push(instance);
                Focus = instance;
            }

            await ShowNewUIView(instance);

            return instance;
        }

        public async Task<IUIView> ModalAsync<TUIView>(IViewFactory factory = null) where TUIView : IUIView
        {
            var instance = await GetInstance<TUIView>(factory);
            var parent = ModalRoot;
            instance.gameObject.transform.SetParent(parent.transform, false);

            if (ModalWindows.Count > 0)
            {
                await FreezeFocusUIView(ModalWindows);

                ModalWindows.Push(instance);
                Focus = instance;
            }
            else
            {
                if (Windows.Count > 0) { await FreezeFocusUIView(Windows); }

                MaskRoot.enabled = true;
            }

            await ShowNewUIView(instance);

            return instance;
        }

        public async Task PopAsync()
        {
            if (Focus == null)
            {
                Debug.LogWarning("Popping nothing.");
                return;
            }

            await Focus.OnHide();

            var isModal = ModalWindows.Contains(Focus);
            //Return focus view
            ReturnInstance(Focus);

            if (isModal)
            {
                ModalWindows.Pop();
                IUIView focus = null;

                // Disable mask layer
                if (ModalWindows.Count <= 0)
                {
                    MaskRoot.enabled = false;
                    if (Windows.Count > 0) { focus = Windows.Peek(); }
                }
                else { focus = ModalWindows.Peek(); }

                Focus = focus;

                if (focus != null) { await focus.OnResume(); }
            }
        }

        #endregion

        #region Common

        public static IUIStack Build(UIControllerSettings settings)
        {
            var controller = new GameObject("UI Root").AddComponent<UIController>();

            var cam = Camera.main;
            if (settings.screenSpaceCameraMode)
            {
                cam = new GameObject("UI Camera").AddComponent<Camera>();
                cam.orthographic = true;
                cam.transform.SetParent(controller.transform, false);
                cam.clearFlags = CameraClearFlags.Depth;
                cam.cullingMask = LayerMask.GetMask("UI");
            }

            controller.WindowRoot = new GameObject("Windows").AddComponent<Canvas>();
            controller.MaskRoot = new GameObject("Mask").AddComponent<Canvas>();
            controller.ModalRoot = new GameObject("Modals").AddComponent<Canvas>();

            // default mask off
            controller.MaskRoot.enabled = false;

            // set mask object
            if (settings.maskObject != null)
            {
                controller.MaskObject = Instantiate(settings.maskObject, controller.MaskRoot.transform, false);
            }
            else
            {
                // create default mask
                var maskTransform = new GameObject("Mask Object").AddComponent<RectTransform>();
                var maskImage = maskTransform.gameObject.AddComponent<RawImage>();
                maskTransform.SetParent(controller.MaskRoot.transform, false);
                maskTransform.anchorMin = Vector2.zero;
                maskTransform.anchorMax = Vector2.one;
                maskImage.color = new Color32(40, 42, 53, 220);
                controller.MaskObject = maskTransform;
            }

            var roots = new List<Transform>
                {controller.WindowRoot.transform, controller.MaskRoot.transform, controller.ModalRoot.transform};

            foreach (var root in roots)
            {
                var obj = root.gameObject;
                var layerCanvas = obj.GetComponent<Canvas>();

                if (settings.screenSpaceCameraMode)
                {
                    layerCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                    layerCanvas.worldCamera = cam;
                }
                else { layerCanvas.renderMode = RenderMode.ScreenSpaceOverlay; }

                var canvasScaler = obj.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = settings.referenceResolution;
                canvasScaler.matchWidthOrHeight = settings.matchWidthOrHeight;

                var rayCaster = obj.AddComponent<GraphicRaycaster>();
                var rayCasterTransform = rayCaster.transform;
                rayCasterTransform.SetParent(controller.transform);
                rayCasterTransform.localPosition = Vector3.zero;
            }

            if (FindObjectOfType<EventSystem>() != null) { return controller; }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(controller.transform, false);

            return controller;
        }

        private async Task<TView> GetInstance<TView>(IViewFactory factory = null) where TView : IUIView
        {
            if (factory == null) { factory = DefaultFactory; }

            TView widget;
            try { widget = (TView) await factory.CreateInstance<TView>(); }
            catch (Exception e)
            {
                Her.Error(e);
                throw;
            }

            // mark widget factory
            FactoryLookup.Add(widget.ViewId, factory);
            return widget;
        }

        private void ReturnInstance(IView view)
        {
            if (FactoryLookup.TryGetValue(view.ViewId, out var factory))
            {
                FactoryLookup.Remove(view.ViewId);
                factory.ReturnInstance(view);
            }
            else { DefaultFactory.ReturnInstance(view); }
        }

        private static async Task FreezeFocusUIView(Stack<IUIView> targetStack)
        {
            var previousWindow = targetStack.Peek();

            if (previousWindow != null)
            {
                try { await previousWindow.OnFreeze(); }
                catch (Exception e)
                {
                    Her.Error(e);
                    throw;
                }
            }
        }

        private static async Task ShowNewUIView(IUIView view)
        {
            try { await view.OnShow(); }
            catch (Exception e)
            {
                Her.Error(e);
                throw;
            }
        }

        #endregion

        #region Unity Lifetime

        public void Start()
        {
            ViewManager = Her.Resolve<IViewManager>();

            if (DefaultFactory == null) { DefaultFactory = new DefaultViewFactory(); }
        }

        #endregion
    }
}