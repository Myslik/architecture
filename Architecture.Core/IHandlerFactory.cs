﻿using System;

namespace Architecture.Core
{
    public interface IHandlerFactory
    {
        object Create(Type serviceType);
        object[] CreateMany(Type serviceType);
    }
}
