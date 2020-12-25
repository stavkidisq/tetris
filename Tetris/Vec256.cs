using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Tetris
{
    public unsafe struct Vec256
    {
        private readonly Vector256<byte> data;

        private static readonly Vec256 ZERO = 0;
        public static readonly Vec256 ONE = 1;
        private static readonly Vec256 V_255 = 0xFF;

        public Vec256(Vector256<byte> _data) => data = _data;

        public Vec256(params byte[] bytes) : this()
        {
            if (bytes.Length < 32)
            {
                Array.Resize(ref bytes, 32);
            }

            fixed (byte* b = bytes)
                data = Avx2.LoadVector256(b);
        }

        static Vec256()
        {
            if (!Avx2.IsSupported)
            {
                throw new Exception();
            }
        }

        public static implicit operator Vec256(Vector256<byte> Data) => new Vec256(Data);

        public static Vec256 operator &(Vec256 x, Vec256 y) => Avx2.And(x.data, y.data);

        public static implicit operator Vec256(byte b) => FromByteToVec256(b);

        public static Vec256 operator >>(Vec256 value, int count)
        {
            Vector256<ulong> _data = Avx2.ShiftRightLogical(value.data.AsUInt64(), (byte)count);
            Vector256<ulong> _carry = Avx2.ShiftLeftLogical(value.data.AsUInt64(), (byte)(64 - count));

            _carry = Avx2.Permute4x64(_carry, 0x39);

            _carry = Avx2.Blend(ZERO.data.AsUInt32(), _carry.AsUInt32(), 0x3F).AsUInt64();

            _data = Avx2.Or(_data, _carry);

            return _data.AsByte();
        }

        public static Vec256 operator <<(Vec256 value, int count)
        {
            Vector256<ulong> _carry = Avx2.ShiftRightLogical(value.data.AsUInt64(), (byte)(64 - count));
            Vector256<ulong> _data = Avx2.ShiftLeftLogical(value.data.AsUInt64(), (byte)count);

            _carry = Avx2.Permute4x64(_carry, 0x93);
            _carry = Avx2.Blend(ZERO.data.AsUInt32(), _carry.AsUInt32(), 0xFC).AsUInt64();

            _data = Avx2.Or(_data, _carry);

            return _data.AsByte();
        }
        public static bool operator ==(Vec256 first, Vec256 second) => -1 == Avx2.MoveMask(Avx2.CompareEqual(first.data, second.data));

        public static bool operator !=(Vec256 first, Vec256 second) => -1 != Avx2.MoveMask(Avx2.CompareEqual(first.data, second.data));

        public static Vec256 operator ~(Vec256 value) => Avx2.AndNot(value.data, Vec256.V_255.data);

        public static Vec256 operator |(Vec256 first, Vec256 second) => Avx2.Or(first.data, second.data);

        public static Vec256 FromByteToVec256(byte b) => Avx2.BroadcastScalarToVector256(&b);

        public Vec256 Shuffle(Vec256 value) => Avx2.Shuffle(data, value.data);

        public Vec256 Min(Vec256 other) => Avx2.Min(data, other.data);

        public override bool Equals(object obj) => this == (Vec256)obj;

        public T[] ToArray<T>() where T : unmanaged
        {
            T[] array = new T[32 / sizeof(T)];

            fixed (T* item = array)
            {
                Avx2.Store((byte*)item, data);
            }

            return array;
        }

        public override string ToString()
        {
            byte* bytes = stackalloc byte[32];
            Avx2.Store(bytes, data);

            return string.Join(" ", Enumerable.Range(0, 32).Select(item => bytes[item].ToString("X2")));
        }

        public override int GetHashCode()
        {
            byte* buffer = stackalloc byte[32];
            Avx2.Store(buffer, data);
            ulong* ubuf = (ulong*)buffer;
            ulong longHC = ((ubuf[0] * 31 + ubuf[1]) * 31 + ubuf[2]) * 31 + ubuf[3];
            return (int)(longHC ^ (longHC >> 32));
        }
    }
}
