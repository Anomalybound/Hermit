using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Hermit.UIStack
{
    public class Widget : MonoBehaviour, IWidget
    {
        [SerializeField]
        private UILayer _layer = UILayer.Window;

        public int Id { get; private set; }

        public string Path { get; private set; }

        public IWidgetController Controller { get; set; }

        public virtual UILayer Layer => _layer;

        protected IUIStack IuiStack { get; private set; }

        protected UIMessage Message { get; private set; } = UIMessage.Empty;

        public void SetManagerInfo(int id, string path, IUIStack manager, UIMessage message)
        {
            Id = id;
            Path = path;
            IuiStack = manager;
            Message = message;
        }

        #region Events

        public event Action<Widget> OnDestroyEvent;

        #endregion

        public virtual async Task OnShow()
        {
            await Task.FromResult(default(object));
        }

        public virtual async Task OnHide()
        {
            await Task.FromResult(default(object));
        }

        public virtual async Task OnResume()
        {
            await Task.FromResult(default(object));
        }

        public virtual async Task OnFreeze()
        {
            await Task.FromResult(default(object));
        }

        public void DestroyWidget()
        {
            OnDestroyEvent?.Invoke(this);
        }

        public virtual void OnDestroy()
        {
            DestroyWidget();
        }
    }
}