using System;

namespace Architecture.Core
{
    public interface IHandlerFactory
    {
        object Create(Type type);
        object[] CreateMany(Type type);
    }
}
