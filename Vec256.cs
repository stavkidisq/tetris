using System;
using System.Collections.Specialized;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

public unsafe struct Vec256
{
    private readonly Vector256<256> data;
}
