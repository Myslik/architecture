using System;
using System.Collections.Generic;

namespace Architecture.Core
{
    public interface IHandlerFactory
    {
        object Create(Type type);
        IEnumerable<object> CreateMany(Type type);
    }
}
