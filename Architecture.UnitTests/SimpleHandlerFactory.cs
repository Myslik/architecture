using Architecture.Core;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Architecture.UnitTests
{
    public class SimpleHandlerFactory : IHandlerFactory
    {
        private Dictionary<Type, List<Type>> _registry =
            new Dictionary<Type, List<Type>>();

        public SimpleHandlerFactory() { }

        public void Register(Type service, Type implementation)
        {
            if (!_registry.ContainsKey(service))
            {
                _registry.Add(service, new List<Type>());
            }
            var implementations = _registry[service];
            if (!implementations.Contains(implementation))
            {
                implementations.Add(implementation);
            }
        }

        public void Register(Type service, params Type[] implementations)
        {
            foreach (var implementation in implementations)
            {
                Register(service, implementation);
            }
        }

        public void Register<TService, TImplementation> ()
        {
            Register(typeof(TService), typeof(TImplementation));
        }

        public void Register<TService>(params Type[] implementations)
        {
            Register(typeof(TService), implementations);
        }

        private Type[] Find(Type service)
        {
            if (_registry.ContainsKey(service))
            {
                return _registry[service].ToArray();
            }
            else
            {
                return new Type[0];
            }
        }

        public object Create(Type type)
        {
            var implementations = Find(type);
            if (implementations.Length != 1)
            {
                return null;
            }
            try
            {
                object implementation = Activator.CreateInstance(implementations[0]);
                return implementation;
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        public IEnumerable<object> CreateMany(Type type)
        {
            var implementations = Find(type);
            var length = implementations.Length;
            var instances = new object[length];
            for (var i = 0; i < length; i++)
            {
                instances[i] = Activator.CreateInstance(implementations[i]);
            }
            return instances;
        }
    }
}
