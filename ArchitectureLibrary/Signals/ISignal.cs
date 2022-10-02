namespace ArchitectureLibrary.Signals
{
    public interface ISignalBase { }

    public interface ISignal : ISignalBase
    {
        void Dispatch();
    }

    public interface ISignal1<T1> : ISignalBase
    {
        void Dispatch(T1 t1);
    }

    public interface ISignal2<T1, T2> : ISignalBase
    {
        void Dispatch(T1 t1, T2 t2);
    }

    public interface ISignal3<T1, T2, T3> : ISignalBase
    {
        void Dispatch(T1 t1, T2 t2, T3 t3);
    }

    public interface ISignal4<T1, T2, T3, T4> : ISignalBase
    {
        void Dispatch(T1 t1, T2 t2, T3 t3, T4 t4);
    }

    public interface ISignal5<T1, T2, T3, T4, T5> : ISignalBase
    {
        void Dispatch(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    }
}
