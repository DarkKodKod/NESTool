using System;
using System.Collections.Generic;

namespace ArchitectureLibrary.Managers
{
    public abstract class AManager<TInterface> where TInterface : class
    {
        public static T Get<T>() where T : TInterface, new()
        {
            Type className = typeof(T);

            if (!Interfaces.TryGetValue(className.Name, out object interfaceObject))
            {
                T m = new T();

                Interfaces.Add(className.Name, m);

                return m;
            }

            return (T)interfaceObject;
        }

        private static readonly Dictionary<string, object> Interfaces = new Dictionary<string, object>();
    }
}
