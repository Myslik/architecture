using Architecture.Core;
using System;
using System.Reflection;
using System.Collections.Generic;

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

        public object Create(Type type)
        {
            if (type == _service)
            {
                if (_implementations.Length != 1)
                {
                    return null;
                }
                try
                {
                    object implementation = Activator.CreateInstance(_implementations[0]);
                    return implementation;
                }
                catch (TargetInvocationException ex) when (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<object> CreateMany(Type type)
        {
            if (type == _service)
            {
                var length = _implementations.Length;
                var instances = new object[length];
                for (var i = 0; i < length; i++)
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
