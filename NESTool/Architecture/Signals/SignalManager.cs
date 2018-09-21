using System;
using System.Collections.Generic;

namespace NESTool.Architecture.Signals
{
    public static class SignalManager
    {
        public static T Get<T>() where T : ISignalBase, new()
        {
            Type className = typeof(T);

            if (!Signals.TryGetValue(className.Name, out object signal))
            {
                var s = new T();

                Signals.Add(className.Name, s);

                return s;
            }

            return (T)signal;
        }

        private static Dictionary<string, object> Signals = new Dictionary<string, object>();
    }
}
