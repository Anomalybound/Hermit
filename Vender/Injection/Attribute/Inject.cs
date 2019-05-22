using System;

namespace Hermit
{
    public class Inject : Attribute
    {
        public string BindingId { get; }

        public Inject() { }

        public Inject(string bindingId)
        {
            BindingId = bindingId;
        }
    }
}