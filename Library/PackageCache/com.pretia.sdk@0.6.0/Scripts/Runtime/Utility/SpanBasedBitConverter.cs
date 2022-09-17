using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class SpanBasedBitConverter
{
    public static bool ToBoolean(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(byte))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<byte>(ref MemoryMarshal.GetReference(value)) != 0;
    }

    public static char ToChar(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(char))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<char>(ref MemoryMarshal.GetReference(value));
    }

    public static double ToDouble(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(double))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference(value));
    }

    public static float ToFloat(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(float))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<float>(ref MemoryMarshal.GetReference(value));
    }

    public static short ToInt16(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(short))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<short>(ref MemoryMarshal.GetReference(value));
    }

    public static int ToInt32(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(int))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(value));
    }

    public static long ToInt64(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(long))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<long>(ref MemoryMarshal.GetReference(value));
    }

    public static ushort ToUInt16(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(ushort))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<ushort>(ref MemoryMarshal.GetReference(value));
    }

    public static uint ToUInt32(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(uint))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<uint>(ref MemoryMarshal.GetReference(value));
    }

    public static ulong ToUInt64(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(ulong))
            throw new ArgumentOutOfRangeException();
        return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(value));
    }

    public static bool TryWriteBytes(Span<byte> destination, bool value)
    {
        if (destination.Length < sizeof(byte))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value ? (byte)1 : (byte)0);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, char value)
    {
        if (destination.Length < sizeof(char))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, double value)
    {
        if (destination.Length < sizeof(double))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, short value)
    {
        if (destination.Length < sizeof(short))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, int value)
    {
        if (destination.Length < sizeof(int))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, long value)
    {
        if (destination.Length < sizeof(long))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, float value)
    {
        if (destination.Length < sizeof(float))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, ushort value)
    {
        if (destination.Length < sizeof(ushort))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, uint value)
    {
        if (destination.Length < sizeof(uint))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }

    public static bool TryWriteBytes(Span<byte> destination, ulong value)
    {
        if (destination.Length < sizeof(ulong))
            return false;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        return true;
    }
}
