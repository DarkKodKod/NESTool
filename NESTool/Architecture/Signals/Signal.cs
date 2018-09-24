namespace NESTool.Architecture.Signals
{
    public class Signal : ISignal
    {
        private Dispatcher methods;

        public void AddListener(Dispatcher method) => methods += method;

        public void Dispatch() => methods?.Invoke();

        public void RemoveListener(Dispatcher method) => methods -= method;
    }

    public class Signal<T1> : ISignal1<T1>
    {
        private Dispatcher<T1> methods;

        public void AddListener(Dispatcher<T1> method) => methods += method;

        public void Dispatch(T1 t1) => methods?.Invoke(t1);

        public void RemoveListener(Dispatcher<T1> method) => methods -= method;
    }

    public class Signal<T1, T2> : ISignal2<T1, T2>
    {
        private Dispatcher<T1, T2> methods;

        public void AddListener(Dispatcher<T1, T2> method) => methods += method;

        public void Dispatch(T1 t1, T2 t2) => methods?.Invoke(t1, t2);

        public void RemoveListener(Dispatcher<T1, T2> method) => methods -= method;
    }

    public class Signal<T1, T2, T3> : ISignal3<T1, T2, T3>
    {
        private Dispatcher<T1, T2, T3> methods;

        public void AddListener(Dispatcher<T1, T2, T3> method) => methods += method;

        public void Dispatch(T1 t1, T2 t2, T3 t3) => methods?.Invoke(t1, t2, t3);

        public void RemoveListener(Dispatcher<T1, T2, T3> method) => methods -= method;
    }

    public class Signal<T1, T2, T3, T4> : ISignal4<T1, T2, T3, T4>
    {
        private Dispatcher<T1, T2, T3, T4> methods;

        public void AddListener(Dispatcher<T1, T2, T3, T4> method) => methods += method;

        public void Dispatch(T1 t1, T2 t2, T3 t3, T4 t4) => methods?.Invoke(t1, t2, t3, t4);

        public void RemoveListener(Dispatcher<T1, T2, T3, T4> method) => methods -= method;
    }

    public class Signal<T1, T2, T3, T4, T5> : ISignal5<T1, T2, T3, T4, T5>
    {
        private Dispatcher<T1, T2, T3, T4, T5> methods;

        public void AddListener(Dispatcher<T1, T2, T3, T4, T5> method) => methods += method;

        public void Dispatch(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => methods?.Invoke(t1, t2, t3, t4, t5);

        public void RemoveListener(Dispatcher<T1, T2, T3, T4, T5> method) => methods -= method;
    }
}
