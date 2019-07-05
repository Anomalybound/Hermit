using System;
using System.Collections.Generic;

namespace Hermit
{
    public interface IDataRepository<out TData>
    {
        bool Initialized { get; }

        int Initialize();
        
        IEnumerable<TData> GetAll();

        TData Find(Predicate<TData> predicate);
    }
}