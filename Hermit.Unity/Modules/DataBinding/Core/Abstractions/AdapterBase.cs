using System;

namespace Hermit.DataBinding
{
    public abstract class AdapterBase : IAdapter
    {
        public abstract object Convert(object fromObj);

        public object Convert(object fromObj, AdapterOptions options)
        {
            try { return options.Convert(fromObj); }
            catch (Exception e)
            {
                Her.Error(e);
                throw;
            }
        }
    }
}