using Architecture.Core;
using System;

namespace Architecture.UnitTests
{
    public class SimpleHandlerFactory : IHandlerFactory
    {
        private readonly Type _service;
        private readonly Type[] _implementations;

        public SimpleHandlerFactory() { }

        public SimpleHandlerFactory(Type service, Type implementation)
        {
            _service = service;
            _implementations = new[] { implementation };
        }

        public SimpleHandlerFactory(Type service, Type[] implementations)
        {
            _service = service;
            _implementations = implementations;
        }

        public object Create(Type serviceType)
        {
            if (serviceType == _service)
            {
                if (_implementations.Length != 1)
                {
                    return null;
                }
                return Activator.CreateInstance(_implementations[0]);
            }
            else
            {
                return null;
            }
        }

        public object[] CreateMany(Type serviceType)
        {
            if (serviceType == _service)
            {
                var length = _implementations.Length;
                var instances = new object[length];
                for(var i = 0; i < length; i++)
                {
                    instances[i] = Activator.CreateInstance(_implementations[i]);
                }
                return instances;
            }
            else
            {
                return new object[0];
            }
        }
    }
}
