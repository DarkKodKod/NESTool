using System;
using System.Collections.Generic;

namespace NESTool.Architecture.Model
{
    public static class ModelManager
    {
        public static T Get<T>() where T : IModel, new()
        {
            Type className = typeof(T);

            if (!Models.TryGetValue(className.Name, out object model))
            {
                var m = new T();

                Models.Add(className.Name, m);

                return m;
            }

            return (T)model;
        }

        private static Dictionary<string, object> Models = new Dictionary<string, object>();
    }
}
