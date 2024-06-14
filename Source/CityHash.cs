using System.Collections;
using System.Text;

namespace AIItems;

internal readonly struct UInt128(ulong low, ulong high)
{
    public ulong Low { get; } = low;
    public ulong High { get; } = high;
    public UInt128(ulong low)
        : this(low, 0UL) { }
}

public static class CityHash
{
    private const ulong K0 = 0xc3a5c85c97cb3127;
    private const ulong K1 = 0xb492b66fbe98f273;
    private const ulong K2 = 0x9ae16a3b2f90404f;

    private const uint C1 = 0xcc9e2d51;
    private const uint C2 = 0x1b873593;


    public static HashValue ComputeHash(
        ArraySegment<byte> data,
        int hashSizeInBits,
        CancellationToken cancellationToken = default
    )
    {
        return hashSizeInBits switch
        {
            32 => CityHash.ComputeHash32(data, cancellationToken),
            64 => CityHash.ComputeHash64(data, cancellationToken),
            128 => CityHash.ComputeHash128(data, cancellationToken),
            _ => throw new NotImplementedException(),
        };
    }


    #region ComputeHash32

    private static HashValue ComputeHash32(ArraySegment<byte> data, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dataCount = data.Count;

        uint hashValue = dataCount > 24
            ? CityHash.Hash32Len25Plus(data, cancellationToken)
            : dataCount > 12 ? CityHash.Hash32Len13to24(data) : dataCount > 4 ? CityHash.Hash32Len5to12(data) : CityHash.Hash32Len0to4(data);
        return new HashValue(BitConverter.GetBytes(hashValue), 32);
    }

    private static uint Hash32Len0to4(ArraySegment<byte> data)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        var endOffset = dataOffset + dataCount;

        uint b = 0;
        uint c = 9;

        for (var currentOffset = dataOffset; currentOffset < endOffset; currentOffset += 1)
        {
            b = (b * C1) + dataArray[currentOffset];
            c ^= b;
        }

        return Mix(CityHash.Mur(b, CityHash.Mur((uint)dataCount, c)));
    }

    private static uint Hash32Len5to12(ArraySegment<byte> data)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        uint a = (uint)dataCount;
        uint b = (uint)dataCount * 5;

        uint c = 9;
        uint d = b;

        a += BitConverter.ToUInt32(dataArray, dataOffset);
        b += BitConverter.ToUInt32(dataArray, dataOffset + dataCount - 4);
        c += BitConverter.ToUInt32(dataArray, dataOffset + ((dataCount >> 1) & 4));

        return Mix(CityHash.Mur(c, CityHash.Mur(b, CityHash.Mur(a, d))));
    }

    private static uint Hash32Len13to24(ArraySegment<byte> data)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        uint a = BitConverter.ToUInt32(dataArray, dataOffset + (dataCount >> 1) - 4);
        uint b = BitConverter.ToUInt32(dataArray, dataOffset + 4);
        uint c = BitConverter.ToUInt32(dataArray, dataOffset + dataCount - 8);
        uint d = BitConverter.ToUInt32(dataArray, dataOffset + (dataCount >> 1));
        uint e = BitConverter.ToUInt32(dataArray, dataOffset);
        uint f = BitConverter.ToUInt32(dataArray, dataOffset + dataCount - 4);
        uint h = (uint)dataCount;

        return Mix(CityHash.Mur(f, CityHash.Mur(e, CityHash.Mur(d, CityHash.Mur(c, CityHash.Mur(b, CityHash.Mur(a, h)))))));
    }

    private static uint Hash32Len25Plus(ArraySegment<byte> data, CancellationToken cancellationToken)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        var endOffset = dataOffset + dataCount;

        cancellationToken.ThrowIfCancellationRequested();

        // dataCount > 24
        uint h = (uint)dataCount;
        uint g = (uint)dataCount * C1;
        uint f = g;
        {
            uint a0 = RotateRight(BitConverter.ToUInt32(dataArray, endOffset - 4) * C1, 17) * C2;
            uint a1 = RotateRight(BitConverter.ToUInt32(dataArray, endOffset - 8) * C1, 17) * C2;
            uint a2 = RotateRight(BitConverter.ToUInt32(dataArray, endOffset - 16) * C1, 17) * C2;
            uint a3 = RotateRight(BitConverter.ToUInt32(dataArray, endOffset - 12) * C1, 17) * C2;
            uint a4 = RotateRight(BitConverter.ToUInt32(dataArray, endOffset - 20) * C1, 17) * C2;

            h ^= a0;
            h = RotateRight(h, 19);
            h = (h * 5) + 0xe6546b64;
            h ^= a2;
            h = RotateRight(h, 19);
            h = (h * 5) + 0xe6546b64;

            g ^= a1;
            g = RotateRight(g, 19);
            g = (g * 5) + 0xe6546b64;
            g ^= a3;
            g = RotateRight(g, 19);
            g = (g * 5) + 0xe6546b64;

            f += a4;
            f = RotateRight(f, 19);
            f = (f * 5) + 0xe6546b64;
        }

        var groupsToProcess = (dataCount - 1) / 20;
        var groupEndOffset = dataOffset + (groupsToProcess * 20);

        for (int groupOffset = dataOffset; groupOffset < groupEndOffset; groupOffset += 20)
        {
            cancellationToken.ThrowIfCancellationRequested();

            uint a0 =
                RotateRight(BitConverter.ToUInt32(dataArray, groupOffset + 0) * C1, 17) * C2;
            uint a1 = BitConverter.ToUInt32(dataArray, groupOffset + 4);
            uint a2 =
                RotateRight(BitConverter.ToUInt32(dataArray, groupOffset + 8) * C1, 17) * C2;
            uint a3 =
                RotateRight(BitConverter.ToUInt32(dataArray, groupOffset + 12) * C1, 17) * C2;
            uint a4 = BitConverter.ToUInt32(dataArray, groupOffset + 16);

            h ^= a0;
            h = RotateRight(h, 18);
            h = (h * 5) + 0xe6546b64;

            f += a1;
            f = RotateRight(f, 19);
            f *= C1;

            g += a2;
            g = RotateRight(g, 18);
            g = (g * 5) + 0xe6546b64;

            h ^= a3 + a1;
            h = RotateRight(h, 19);
            h = (h * 5) + 0xe6546b64;

            g ^= a4;
            g = ReverseByteOrder(g) * 5;

            h += a4 * 5;
            h = ReverseByteOrder(h);

            f += a0;

            Permute3(ref f, ref h, ref g);
        }

        cancellationToken.ThrowIfCancellationRequested();

        g = RotateRight(g, 11) * C1;
        g = RotateRight(g, 17) * C1;

        f = RotateRight(f, 11) * C1;
        f = RotateRight(f, 17) * C1;

        h = RotateRight(h + g, 19);
        h = (h * 5) + 0xe6546b64;
        h = RotateRight(h, 17) * C1;
        h = RotateRight(h + f, 19);
        h = (h * 5) + 0xe6546b64;
        h = RotateRight(h, 17) * C1;

        return h;
    }

    #endregion

    #region ComputeHash64

    private static HashValue ComputeHash64(ArraySegment<byte> data, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dataCount = data.Count;
        ulong hashValue = dataCount > 64
            ? CityHash.Hash64Len65Plus(data, cancellationToken)
            : dataCount > 32 ? CityHash.Hash64Len33to64(data) : dataCount > 16 ? CityHash.Hash64Len17to32(data) : CityHash.Hash64Len0to16(data);
        return new HashValue(BitConverter.GetBytes(hashValue), 64);
    }

    private static ulong Hash64Len16(ulong u, ulong v)
    {
        return Hash128to64(new UInt128(u, v));
    }

    private static ulong Hash64Len16(ulong u, ulong v, ulong mul)
    {
        ulong a = (u ^ v) * mul;
        a ^= a >> 47;

        ulong b = (v ^ a) * mul;
        b ^= b >> 47;
        b *= mul;

        return b;
    }

    private static ulong Hash64Len0to16(ArraySegment<byte> data)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        var endOffset = dataOffset + dataCount;

        if (dataCount >= 8)
        {
            ulong mul = K2 + ((ulong)dataCount * 2);
            ulong a = BitConverter.ToUInt64(dataArray, dataOffset) + K2;
            ulong b = BitConverter.ToUInt64(dataArray, endOffset - 8);
            ulong c = (RotateRight(b, 37) * mul) + a;
            ulong d = (RotateRight(a, 25) + b) * mul;

            return Hash64Len16(c, d, mul);
        }

        if (dataCount >= 4)
        {
            ulong mul = K2 + ((ulong)dataCount * 2);
            ulong a = BitConverter.ToUInt32(dataArray, dataOffset);
            return Hash64Len16(
                (ulong)dataCount + (a << 3),
                BitConverter.ToUInt32(dataArray, endOffset - 4),
                mul
            );
        }

        if (dataCount > 0)
        {
            byte a = dataArray[dataOffset];
            byte b = dataArray[dataOffset + (dataCount >> 1)];
            byte c = dataArray[endOffset - 1];

            uint y = a + ((uint)b << 8);
            uint z = (uint)dataCount + ((uint)c << 2);

            return Mix((y * K2) ^ (z * K0)) * K2;
        }

        return K2;
    }

    // This probably works well for 16-byte strings as well, but it may be overkill
    // in that case.
    private static ulong Hash64Len17to32(ArraySegment<byte> data)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        var endOffset = dataOffset + dataCount;

        ulong mul = K2 + ((ulong)dataCount * 2);
        ulong a = BitConverter.ToUInt64(dataArray, dataOffset) * K1;
        ulong b = BitConverter.ToUInt64(dataArray, dataOffset + 8);
        ulong c = BitConverter.ToUInt64(dataArray, endOffset - 8) * mul;
        ulong d = BitConverter.ToUInt64(dataArray, endOffset - 16) * K2;

        return Hash64Len16(
            RotateRight(a + b, 43) + RotateRight(c, 30) + d,
            a + RotateRight(b + K2, 18) + c,
            mul
        );
    }

    // Return a 16-byte hash for 48 bytes.  Quick and dirty.
    // Callers do best to use "random-looking" values for a and b.
    private static UInt128 WeakHashLen32WithSeeds(
        ulong w,
        ulong x,
        ulong y,
        ulong z,
        ulong a,
        ulong b
    )
    {
        a += w;
        b = RotateRight(b + a + z, 21);

        ulong c = a;
        a += x;
        a += y;

        b += RotateRight(a, 44);

        return new UInt128(a + z, b + c);
    }

    // Return a 16-byte hash for s[0] ... s[31], a, and b.  Quick and dirty.
    private static UInt128 WeakHashLen32WithSeeds(byte[] data, int startIndex, ulong a, ulong b)
    {
        return CityHash.WeakHashLen32WithSeeds(
            BitConverter.ToUInt64(data, startIndex),
            BitConverter.ToUInt64(data, startIndex + 8),
            BitConverter.ToUInt64(data, startIndex + 16),
            BitConverter.ToUInt64(data, startIndex + 24),
            a,
            b
        );
    }

    // Return an 8-byte hash for 33 to 64 bytes.
    private static ulong Hash64Len33to64(ArraySegment<byte> data)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        var endOffset = dataOffset + dataCount;

        ulong mul = K2 + ((ulong)dataCount * 2);
        ulong a = BitConverter.ToUInt64(dataArray, dataOffset) * K2;
        ulong b = BitConverter.ToUInt64(dataArray, dataOffset + 8);
        ulong c = BitConverter.ToUInt64(dataArray, endOffset - 24);
        ulong d = BitConverter.ToUInt64(dataArray, endOffset - 32);
        ulong e = BitConverter.ToUInt64(dataArray, dataOffset + 16) * K2;
        ulong f = BitConverter.ToUInt64(dataArray, dataOffset + 24) * 9;
        ulong g = BitConverter.ToUInt64(dataArray, endOffset - 8);
        ulong h = BitConverter.ToUInt64(dataArray, endOffset - 16) * mul;

        ulong u = RotateRight(a + g, 43) + ((RotateRight(b, 30) + c) * 9);
        ulong v = ((a + g) ^ d) + f + 1;
        ulong w = ReverseByteOrder((u + v) * mul) + h;
        ulong x = RotateRight(e + f, 42) + c;
        ulong y = (ReverseByteOrder((v + w) * mul) + g) * mul;
        ulong z = e + f + c;

        a = ReverseByteOrder(((x + z) * mul) + y) + b;
        b = Mix(((z + a) * mul) + d + h) * mul;
        return b + x;
    }

    private static ulong Hash64Len65Plus(ArraySegment<byte> data, CancellationToken cancellationToken)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        var endOffset = dataOffset + dataCount;

        // For strings over 64 bytes we hash the end first, and then as we
        // loop we keep 56 bytes of state: v, w, x, y, and z.
        ulong x = BitConverter.ToUInt64(dataArray, endOffset - 40);
        ulong y =
            BitConverter.ToUInt64(dataArray, endOffset - 16)
            + BitConverter.ToUInt64(dataArray, endOffset - 56);
        ulong z = CityHash.Hash64Len16(
            BitConverter.ToUInt64(dataArray, endOffset - 48) + (ulong)dataCount,
            BitConverter.ToUInt64(dataArray, endOffset - 24)
        );

        UInt128 v = CityHash.WeakHashLen32WithSeeds(dataArray, endOffset - 64, (ulong)dataCount, z);
        UInt128 w = CityHash.WeakHashLen32WithSeeds(dataArray, endOffset - 32, y + K1, x);

        x = (x * K1) + BitConverter.ToUInt64(dataArray, 0);

        // For each 64-byte chunk
        var groupEndOffset = dataOffset + (dataCount - (dataCount % 64));

        for (var currentOffset = dataOffset; currentOffset < groupEndOffset; currentOffset += 64)
        {
            cancellationToken.ThrowIfCancellationRequested();

            x =
                RotateRight(x + y + v.Low + BitConverter.ToUInt64(dataArray, currentOffset + 8), 37)
                * K1;
            y =
                RotateRight(y + v.High + BitConverter.ToUInt64(dataArray, currentOffset + 48), 42)
                * K1;
            x ^= w.High;
            y += v.Low + BitConverter.ToUInt64(dataArray, currentOffset + 40);
            z = RotateRight(z + w.Low, 33) * K1;
            v = CityHash.WeakHashLen32WithSeeds(dataArray, currentOffset, v.High * K1, x + w.Low);
            w = CityHash.WeakHashLen32WithSeeds(
                dataArray,
                currentOffset + 32,
                z + w.High,
                y + BitConverter.ToUInt64(dataArray, currentOffset + 16)
            );

            (z, x) = (x, z);
        }

        return CityHash.Hash64Len16(
            CityHash.Hash64Len16(v.Low, w.Low) + (Mix(y) * K1) + z,
            CityHash.Hash64Len16(v.High, w.High) + x
        );
    }

    #endregion

    #region ComputeHash128

    private static HashValue ComputeHash128(ArraySegment<byte> data, CancellationToken cancellationToken)
    {
        var dataCount = data.Count;

        UInt128 hashValue;

        if (dataCount >= 16)
        {
            var dataArray = data.Array;
            var dataOffset = data.Offset;

            hashValue = CityHash.CityHash128WithSeed(
                new ArraySegment<byte>(dataArray, dataOffset + 16, dataCount - 16),
                new UInt128(
                    BitConverter.ToUInt64(dataArray, dataOffset),
                    BitConverter.ToUInt64(dataArray, dataOffset + 8) + K0
                ),
                cancellationToken
            );
        }
        else
        {
            hashValue = CityHash.CityHash128WithSeed(data, new UInt128(K0, K1), cancellationToken);
        }

        var hashValueBytes = BitConverter
            .GetBytes(hashValue.Low)
            .Concat(BitConverter.GetBytes(hashValue.High));

        return new HashValue(hashValueBytes, 128);
    }

    private static UInt128 CityHash128WithSeed(
        ArraySegment<byte> data,
        UInt128 seed,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var dataCount = data.Count;

        if (dataCount < 128)
            return CityHash.CityMurmur(data, seed);

        var dataArray = data.Array;
        var dataOffset = data.Offset;

        var endOffset = dataOffset + dataCount;

        // We expect len >= 128 to be the common case.  Keep 56 bytes of state:
        // v, w, x, y, and z.
        UInt128 v;
        {
            var vLow =
                (RotateRight(seed.High ^ K1, 49) * K1) + BitConverter.ToUInt64(dataArray, dataOffset);
            v = new UInt128(
                vLow,
                (RotateRight(vLow, 42) * K1) + BitConverter.ToUInt64(dataArray, dataOffset + 8)
            );
        }

        UInt128 w = new(
            (RotateRight(seed.High + ((ulong)dataCount * K1), 35) * K1) + seed.Low,
            RotateRight(seed.Low + BitConverter.ToUInt64(dataArray, dataOffset + 88), 53) * K1
        );

        ulong x = seed.Low;
        ulong y = seed.High;
        ulong z = (ulong)dataCount * K1;

        // This is the same inner loop as CityHash64()
        int lastGroupEndOffset;
        {
            var groupEndOffset = dataOffset + (dataCount - (dataCount % 128));

            for (
                var groupCurrentOffset = dataOffset;
                groupCurrentOffset < groupEndOffset;
                groupCurrentOffset += 128
            )
            {
                cancellationToken.ThrowIfCancellationRequested();

                x =
                    RotateRight(
                        x + y + v.Low + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 8),
                        37
                    ) * K1;
                y =
                    RotateRight(
                        y + v.High + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 48),
                        42
                    ) * K1;
                x ^= w.High;
                y += v.Low + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 40);
                z = RotateRight(z + w.Low, 33) * K1;
                v = CityHash.WeakHashLen32WithSeeds(dataArray, groupCurrentOffset, v.High * K1, x + w.Low);
                w = CityHash.WeakHashLen32WithSeeds(
                    dataArray,
                    groupCurrentOffset + 32,
                    z + w.High,
                    y + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 16)
                );

                {
                    (x, z) = (z, x);
                }

                x =
                    RotateRight(
                        x + y + v.Low + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 72),
                        37
                    ) * K1;
                y =
                    RotateRight(
                        y + v.High + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 112),
                        42
                    ) * K1;
                x ^= w.High;
                y += v.Low + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 104);
                z = RotateRight(z + w.Low, 33) * K1;
                v = CityHash.WeakHashLen32WithSeeds(
                    dataArray,
                    groupCurrentOffset + 64,
                    v.High * K1,
                    x + w.Low
                );
                w = CityHash.WeakHashLen32WithSeeds(
                    dataArray,
                    groupCurrentOffset + 96,
                    z + w.High,
                    y + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 80)
                );

                {
                    (x, z) = (z, x);
                }
            }

            lastGroupEndOffset = groupEndOffset;
        }

        cancellationToken.ThrowIfCancellationRequested();

        x += RotateRight(v.Low + z, 49) * K0;
        y = (y * K0) + RotateRight(w.High, 37);
        z = (z * K0) + RotateRight(w.Low, 27);
        w = new UInt128(w.Low * 9, w.High);
        v = new UInt128(v.Low * K0, v.High);

        // Hash up to 4 chunks of 32 bytes each from the end of data.
        {
            var groupEndOffset = lastGroupEndOffset - 32;

            for (
                var groupCurrentOffset = endOffset - 32;
                groupCurrentOffset > groupEndOffset;
                groupCurrentOffset -= 32
            )
            {
                cancellationToken.ThrowIfCancellationRequested();

                y = (RotateRight(x + y, 42) * K0) + v.High;
                w = new UInt128(
                    w.Low + BitConverter.ToUInt64(dataArray, groupCurrentOffset + 16),
                    w.High
                );
                x = (x * K0) + w.Low;
                z += w.High + BitConverter.ToUInt64(dataArray, groupCurrentOffset);
                w = new UInt128(w.Low, w.High + v.Low);
                v = CityHash.WeakHashLen32WithSeeds(dataArray, groupCurrentOffset, v.Low + z, v.High);
                v = new UInt128(v.Low * K0, v.High);
            }
        }

        // At this point our 56 bytes of state should contain more than
        // enough information for a strong 128-bit hash.  We use two
        // different 56-byte-to-8-byte hashes to get a 16-byte final result.
        x = CityHash.Hash64Len16(x, v.Low);
        y = CityHash.Hash64Len16(y + z, w.Low);

        return new UInt128(
            CityHash.Hash64Len16(x + v.High, w.High) + y,
            CityHash.Hash64Len16(x + w.High, y + v.High)
        );
    }

    // A subroutine for CityHash128().  Returns a decent 128-bit hash for strings
    // of any length representable in signed long.  Based on City and Murmur.
    private static UInt128 CityMurmur(ArraySegment<byte> data, UInt128 seed)
    {
        var dataArray = data.Array;
        var dataOffset = data.Offset;
        var dataCount = data.Count;

        var endOffset = dataOffset + dataCount;

        ulong a = seed.Low;
        ulong b = seed.High;
        ulong c;
        ulong d;

        if (dataCount <= 16)
        {
            // len <= 16
            a = Mix(a * K1) * K1;
            c = (b * K1) + CityHash.Hash64Len0to16(data);
            d = Mix(a + (dataCount >= 8 ? BitConverter.ToUInt64(dataArray, dataOffset) : c));
        }
        else
        {
            // len > 16
            c = CityHash.Hash64Len16(BitConverter.ToUInt64(dataArray, endOffset - 8) + K1, a);
            d = CityHash.Hash64Len16(
                b + (ulong)dataCount,
                c + BitConverter.ToUInt64(dataArray, endOffset - 16)
            );
            a += d;

            var groupEndOffset = dataOffset + dataCount - 16;

            for (
                var groupCurrentOffset = dataOffset;
                groupCurrentOffset < groupEndOffset;
                groupCurrentOffset += 16
            )
            {
                a ^= Mix(BitConverter.ToUInt64(dataArray, groupCurrentOffset) * K1) * K1;
                a *= K1;
                b ^= a;
                c ^= Mix(BitConverter.ToUInt64(dataArray, groupCurrentOffset + 8) * K1) * K1;
                c *= K1;
                d ^= c;
            }
        }

        a = CityHash.Hash64Len16(a, c);
        b = CityHash.Hash64Len16(d, b);
        return new UInt128(a ^ b, CityHash.Hash64Len16(b, a));
    }

    #endregion

    #region Shared Utilities

    private static uint Mix(uint h)
    {
        h ^= h >> 16;
        h *= 0x85ebca6b;
        h ^= h >> 13;
        h *= 0xc2b2ae35;
        h ^= h >> 16;
        return h;
    }

    private static ulong Mix(ulong value) => value ^ (value >> 47);

    private static uint Mur(uint a, uint h)
    {
        // Helper from Murmur3 for combining two 32-bit values.
        a *= C1;
        a = RotateRight(a, 17);
        a *= C2;
        h ^= a;
        h = RotateRight(h, 19);
        return (h * 5) + 0xe6546b64;
    }

    private static void Permute3(ref uint a, ref uint b, ref uint c)
    {
        uint temp = a;

        a = c;
        c = b;
        b = temp;
    }

    private static ulong Hash128to64(UInt128 x)
    {
        const ulong kMul = 0x9ddfea08eb382d69;

        ulong a = (x.Low ^ x.High) * kMul;
        a ^= a >> 47;

        ulong b = (x.High ^ a) * kMul;
        b ^= b >> 47;
        b *= kMul;

        return b;
    }

    private static uint RotateRight(uint operand, int shiftCount)
    {
        shiftCount &= 0x1f;

        return (operand >> shiftCount) | (operand << (32 - shiftCount));
    }

    private static ulong RotateRight(ulong operand, int shiftCount)
    {
        shiftCount &= 0x3f;

        return (operand >> shiftCount) | (operand << (64 - shiftCount));
    }

    private static uint ReverseByteOrder(uint operand)
    {
        return (operand >> 24)
            | ((operand & 0x00ff0000) >> 8)
            | ((operand & 0x0000ff00) << 8)
            | (operand << 24);
    }

    private static ulong ReverseByteOrder(ulong operand)
    {
        return (operand >> 56)
            | ((operand & 0x00ff000000000000) >> 40)
            | ((operand & 0x0000ff0000000000) >> 24)
            | ((operand & 0x000000ff00000000) >> 8)
            | ((operand & 0x00000000ff000000) << 8)
            | ((operand & 0x0000000000ff0000) << 24)
            | ((operand & 0x000000000000ff00) << 40)
            | (operand << 56);
    }

    #endregion
}


/// <summary>
/// Implementation of <see cref="IHashValue"/>
/// </summary>
public sealed class HashValue
    : IHashValue
{
    /// <summary>
    /// Gets the length of the hash value in bits.
    /// </summary>
    /// <value>
    /// The length of the hash value bit.
    /// </value>
#pragma warning disable CA1819 // Properties should not return arrays
    public byte[] Hash { get; }
#pragma warning restore CA1819 // Properties should not return arrays

    /// <summary>
    /// Gets resulting byte array.
    /// </summary>
    /// <value>
    /// The hash value.
    /// </value>
    /// <remarks>
    /// Implementations should coerce the input hash value to be <see cref="BitLength"/> size in bits.
    /// </remarks>
    public int BitLength { get; }


    public int AsInt32()
    {
        return BitLength != 32
            ? throw new InvalidOperationException("Hash does not contain enough data to form a 32-bit integer.")
            : BitConverter.ToInt32(Hash, 0);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="HashValue"/>.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="bitLength">Length of the hash, in bits.</param>
    /// <exception cref="ArgumentNullException"><paramref name="hash"/></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitLength"/>;bitLength must be greater than or equal to 1.</exception>
    public HashValue(IEnumerable<byte> hash, int bitLength)
    {
        if (hash == null)
            throw new ArgumentNullException(nameof(hash));

        if (bitLength < 1)
            throw new ArgumentOutOfRangeException(nameof(bitLength), $"{nameof(bitLength)} must be greater than or equal to 1.");

        Hash = CoerceToArray(hash, bitLength);
        BitLength = bitLength;
    }


    /// <summary>
    /// Converts the hash value to a the base64 string.
    /// </summary>
    /// <returns>
    /// A base64 string representing this hash value.
    /// </returns>
    public string AsBase64String()
    {
        return Convert.ToBase64String(Hash);
    }

#pragma warning disable CA1200 // Properties should not return arrays
    /// <summary>
    /// Converts the hash value to a bit array.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.BitArray" /> instance to represent this hash value.
    /// </returns>
#pragma warning restore CA1200 // Properties should not return arrays
    public BitArray AsBitArray()
    {
        return new BitArray(Hash)
        {
            Length = BitLength
        };
    }

    /// <summary>
    /// Converts the hash value to a hexadecimal string.
    /// </summary>
    /// <returns>
    /// A hex string representing this hash value.
    /// </returns>
    public string AsHexString() => AsHexString(false);

    /// <summary>
    /// Converts the hash value to a hexadecimal string.
    /// </summary>
    /// <param name="uppercase"><c>true</c> if the result should use uppercase hex values; otherwise <c>false</c>.</param>
    /// <returns>
    /// A hex string representing this hash value.
    /// </returns>
    public string AsHexString(bool uppercase)
    {
        var stringBuilder = new StringBuilder(Hash.Length);
        var formatString = uppercase ? "X2" : "x2";

        foreach (var byteValue in Hash)
            _ = stringBuilder.Append(byteValue.ToString(formatString));

        return stringBuilder.ToString();
    }


    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = 17;

            hashCode = (hashCode * 31) ^ BitLength.GetHashCode();

            foreach (var value in Hash)
                hashCode = (hashCode * 31) ^ value.GetHashCode();

            return hashCode;
        }
    }

    /// <summary>
    /// Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        return Equals(obj as IHashValue);
#pragma warning restore CS8604 // Possible null reference argument.
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(IHashValue other)
    {
        return other != null && other.BitLength == BitLength && Hash.SequenceEqual(other.Hash);
    }


    /// <summary>
    /// Coerces the given <paramref name="hash"/> to a byte array with <paramref name="bitLength"/> significant bits.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="bitLength">Length of the hash, in bits.</param>
    /// <returns>A byte array that has been coerced to the proper length.</returns>
    private static byte[] CoerceToArray(IEnumerable<byte> hash, int bitLength)
    {
        var byteLength = (bitLength + 7) / 8;

        if ((bitLength % 8) == 0)
        {
            if (hash is IReadOnlyCollection<byte> hashByteCollection)
            {
                if (hashByteCollection.Count == byteLength)
                    return hash.ToArray();
            }

            if (hash is byte[] hashByteArray)
            {
                var newHashArray = new byte[byteLength];
                {
                    Array.Copy(hashByteArray, newHashArray, Math.Min(byteLength, hashByteArray.Length));
                }

                return newHashArray;
            }
        }


        byte finalByteMask = (byte)((1 << (bitLength % 8)) - 1);
        {
            if (finalByteMask == 0)
                finalByteMask = 255;
        }


        var coercedArray = new byte[byteLength];

        var currentIndex = 0;
        var hashEnumerator = hash.GetEnumerator();

        while (currentIndex < byteLength && hashEnumerator.MoveNext())
        {
            coercedArray[currentIndex] = currentIndex == (byteLength - 1) ? (byte)(hashEnumerator.Current & finalByteMask) : hashEnumerator.Current;
            currentIndex += 1;
        }

        return coercedArray;
    }

}

public interface IHashValue
        : IEquatable<IHashValue>
{
    /// <summary>
    /// Gets the length of the hash value in bits.
    /// </summary>
    /// <value>
    /// The length of the hash value in bits.
    /// </value>
    int BitLength { get; }

    /// <summary>
    /// Gets resulting byte array.
    /// </summary>
    /// <value>
    /// The hash value.
    /// </value>
    /// <remarks>
    /// Implementations should coerce the input hash value to be <see cref="BitLength"/> size in bits.
    /// </remarks>
#pragma warning disable CA1819 // Properties should not return arrays
    byte[] Hash { get; }
#pragma warning restore CA1819 // Properties should not return arrays


    /// <summary>
    /// Converts the hash value to a bit array.
    /// </summary>
    /// <returns>A <see cref="BitArray"/> instance to represent this hash value.</returns>
    BitArray AsBitArray();

    /// <summary>
    /// Converts the hash value to a hexadecimal string.
    /// </summary>
    /// <returns>A hex string representing this hash value.</returns>
    string AsHexString();

    /// <summary>
    /// Converts the hash value to a hexadecimal string.
    /// </summary>
    /// <param name="uppercase"><c>true</c> if the result should use uppercase hex values; otherwise <c>false</c>.</param>
    /// <returns>A hex string representing this hash value.</returns>
    string AsHexString(bool uppercase);

    /// <summary>
    /// Converts the hash value to a the base64 string.
    /// </summary>
    /// <returns>A base64 string representing this hash value.</returns>
    string AsBase64String();

}