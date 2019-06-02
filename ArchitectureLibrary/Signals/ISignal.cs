namespace ArchitectureLibrary.Signals
{
    public delegate void Dispatcher();
    public delegate void Dispatcher<T1>(T1 t1);
    public delegate void Dispatcher<T1, T2>(T1 t1, T2 t2);
    public delegate void Dispatcher<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
    public delegate void Dispatcher<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate void Dispatcher<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    public interface ISignalBase { }

    interface ISignal : ISignalBase
    {
        void Dispatch();
        void AddListener(Dispatcher method);
        void RemoveListener(Dispatcher method);
    }

    interface ISignal1<T1> : ISignalBase
    {
        void Dispatch(T1 t1);
        void AddListener(Dispatcher<T1> method);
        void RemoveListener(Dispatcher<T1> method);
    }

    interface ISignal2<T1, T2> : ISignalBase
    {
        void Dispatch(T1 t1, T2 t2);
        void AddListener(Dispatcher<T1, T2> method);
        void RemoveListener(Dispatcher<T1, T2> method);
    }

    interface ISignal3<T1, T2, T3> : ISignalBase
    {
        void Dispatch(T1 t1, T2 t2, T3 t3);
        void AddListener(Dispatcher<T1, T2, T3> method);
        void RemoveListener(Dispatcher<T1, T2, T3> method);
    }

    interface ISignal4<T1, T2, T3, T4> : ISignalBase
    {
        void Dispatch(T1 t1, T2 t2, T3 t3, T4 t4);
        void AddListener(Dispatcher<T1, T2, T3, T4> method);
        void RemoveListener(Dispatcher<T1, T2, T3, T4> method);
    }

    interface ISignal5<T1, T2, T3, T4, T5> : ISignalBase
    {
        void Dispatch(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
        void AddListener(Dispatcher<T1, T2, T3, T4, T5> method);
        void RemoveListener(Dispatcher<T1, T2, T3, T4, T5> method);
    }
}
