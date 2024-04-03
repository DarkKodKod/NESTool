using System.Runtime.InteropServices;

namespace ArchitectureLibrary.Utils;

[StructLayout(LayoutKind.Explicit)]
public struct ValueUnion
{
    [FieldOffset(0)] public int integer;
    [FieldOffset(4)] public bool boolean;
}
