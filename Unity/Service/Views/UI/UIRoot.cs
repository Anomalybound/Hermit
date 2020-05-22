using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hermit.Service.Views.UI
{
    public class UIRoot : MonoBehaviour, IUIStack
    {
        [Header("UI Layers")]
        [SerializeField] private UILayer windowLayer = null;
        [SerializeField] private UILayer modalLayer = null;

        [Header("UI Mask")]
        [SerializeField] protected GameObject maskObject = null;

        #region Runtime Variables

        public IUIView Focus { get; private set; }

        protected IViewManager ViewManager { get; private set; }

        protected IViewFactory DefaultFactory { get; private set; }

        protected Dictionary<ulong, IViewFactory> FactoryLookup { get; } = new Dictionary<ulong, IViewFactory>();

        protected Stack<IUIView> Windows { get; } = new Stack<IUIView>();

        protected Stack<IUIView> ModalWindows { get; } = new Stack<IUIView>();

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
            var parent = windowLayer;
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
            var parent = modalLayer;
            instance.gameObject.transform.SetParent(parent.transform, false);

            if (ModalWindows.Count > 0)
            {
                await FreezeFocusUIView(ModalWindows);

                ModalWindows.Push(instance);
                Focus = instance;
            } else
            {
                if (Windows.Count > 0) { await FreezeFocusUIView(Windows); }

                if (maskObject != null) { maskObject.SetActive(true); }
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
                    if (maskObject != null) { maskObject.SetActive(false); }

                    if (Windows.Count > 0) { focus = Windows.Peek(); }
                } else { focus = ModalWindows.Peek(); }

                Focus = focus;

                if (focus != null) { await focus.OnResume(); }
            }
        }

        #endregion

        #region Helpers

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
            } else { DefaultFactory.ReturnInstance(view); }
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

            if (maskObject != null) { maskObject.SetActive(false); }
        }

        #endregion
    }
}
