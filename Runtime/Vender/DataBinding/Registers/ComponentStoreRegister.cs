using UnityEngine;

namespace Hermit.DataBinding
{
    [ScriptOrder(-5000)]
    public sealed class ComponentStoreRegister : MonoBehaviour
    {
        [SerializeField]
        private string storeId = "Global";

        [SerializeField]
        private string storeKey = "";

        [SerializeField]
        private Component storeValue = null;

        private void Start()
        {
            if (storeValue == null || string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(storeKey)) { return; }

            var store = Her.GetStore(storeId);
            store.Set(storeKey, storeValue);
        }
    }
}