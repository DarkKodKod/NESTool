using System;

namespace ArchitectureLibrary.Signals
{
    public class Signal : ISignal
    {
        public Action Listener = null;

        public void Dispatch()
        {
            Action handler = Listener;
            handler?.Invoke();
        }
    }

    public class Signal<T1> : ISignal1<T1>
    {
        public Action<T1> Listener = null;

        public void Dispatch(T1 t1)
        {
            Action<T1> handler = Listener;
            handler?.Invoke(t1);
        }
    }

    public class Signal<T1, T2> : ISignal2<T1, T2>
    {
        public Action<T1, T2> Listener = null;

        public void Dispatch(T1 t1, T2 t2)
        {
            Action<T1, T2> handler = Listener;
            handler?.Invoke(t1, t2);
        }
    }

    public class Signal<T1, T2, T3> : ISignal3<T1, T2, T3>
    {
        public Action<T1, T2, T3> Listener = null;

        public void Dispatch(T1 t1, T2 t2, T3 t3)
        {
            Action<T1, T2, T3> handler = Listener;
            handler?.Invoke(t1, t2, t3);
        }
    }

    public class Signal<T1, T2, T3, T4> : ISignal4<T1, T2, T3, T4>
    {
        public Action<T1, T2, T3, T4> Listener = null;

        public void Dispatch(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            Action<T1, T2, T3, T4> handler = Listener;
            handler?.Invoke(t1, t2, t3, t4);
        }
    }

    public class Signal<T1, T2, T3, T4, T5> : ISignal5<T1, T2, T3, T4, T5>
    {
        public Action<T1, T2, T3, T4, T5> Listener = null;

        public void Dispatch(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            Action<T1, T2, T3, T4, T5> handler = Listener;
            handler?.Invoke(t1, t2, t3, t4, t5);
        }
    }
}
