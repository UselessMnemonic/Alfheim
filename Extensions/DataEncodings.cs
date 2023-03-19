using System;
using System.Text;
using UnityEngine;

namespace Hermodr.Extensions;

public static class DataEncodings
{
    public static ushort SwapEndian(ushort value)
    {
        return (ushort) ((value & 0x00_ff) << 8 | (value & 0xff_00) >> 8);
    }

    public static short SwapEndian(short value)
    {
        unsafe
        {
            var result = SwapEndian(*(ushort*) &value);
            return *(short*) &result;
        }
    }

    public static uint SwapEndian(uint value)
    {
        return (value & 0x00_00_00_ff) << 24 | (value & 0x00_00_ff_00) << 8 |
               (value & 0x00_ff_00_00) >> 8 | (value & 0xff_00_00_00) >> 24;
    }

    public static int SwapEndian(int value)
    {
        unsafe
        {
            var result = SwapEndian(*(uint*) &value);
            return *(int*) &result;
        }
    }

    public static ulong SwapEndian(ulong value)
    {
        return (value & 0x00_00_00_00_00_00_00_ff) << 56 | (value & 0x00_00_00_00_00_00_ff_00) << 40 |
               (value & 0x00_00_00_00_00_ff_00_00) << 24 | (value & 0x00_00_00_00_ff_00_00_00) << 8  |
               (value & 0x00_00_00_ff_00_00_00_00) >> 8  | (value & 0x00_00_ff_00_00_00_00_00) >> 24 |
               (value & 0x00_ff_00_00_00_00_00_00) >> 40 | (value & 0xff_00_00_00_00_00_00_00) >> 56;
    }

    public static long SwapEndian(long value)
    {
        unsafe
        {
            var result = SwapEndian(*(ulong*) &value);
            return *(long*) &result;
        }
    }

    public static void PutBytesBE(ushort value, byte[] buffer, int offset)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if ((long) (uint) offset >= (long) buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (offset > buffer.Length - 2)
            throw new ArgumentException("buffer offset is too large");
        if (offset % 2 == 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                value = DataEncodings.SwapEndian(value);
            }
            unsafe
            {
                fixed (byte* numPtr = &buffer[offset])
                {
                    *(ushort*) numPtr = value;
                }
            }
        }
        else
        {
            buffer[offset] = (byte) (value >> 8);
            buffer[offset + 1] = (byte) value;
        }
    }

    public static void PutBytesBE(uint value, byte[] buffer, int offset)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if ((long) (uint) offset >= (long) buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (offset > buffer.Length - 4)
            throw new ArgumentException("buffer offset is too large");
        if (offset % 4 == 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                value = DataEncodings.SwapEndian(value);
            }
            unsafe
            {
                fixed (byte* numPtr = &buffer[offset])
                {
                    *(uint*) numPtr = value;
                }
            }
        }
        else
        {
            buffer[offset] = (byte) (value >> 24);
            buffer[offset + 1] = (byte) (value >> 16);
            buffer[offset + 2] = (byte) (value >> 8);
            buffer[offset + 3] = (byte) value;
        }
    }

    public static void PutBytesBE(ulong value, byte[] buffer, int offset)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if ((long) (uint) offset >= (long) buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (offset > buffer.Length - 8)
            throw new ArgumentException("buffer offset is too large");
        if (offset % 8 == 0)
        {
            if (BitConverter.IsLittleEndian)
            {
                value = DataEncodings.SwapEndian(value);
            }
            unsafe
            {
                fixed (byte* numPtr = &buffer[offset])
                {
                    *(ulong*) numPtr = value;
                }
            }
        }
        else
        {
            buffer[offset] = (byte) (value >> 56);
            buffer[offset + 1] = (byte) (value >> 48);
            buffer[offset + 2] = (byte) (value >> 40);
            buffer[offset + 3] = (byte) (value >> 32);
            buffer[offset + 4] = (byte) (value >> 24);
            buffer[offset + 5] = (byte) (value >> 16);
            buffer[offset + 6] = (byte) (value >> 8);
            buffer[offset + 7] = (byte) value;
        }
    }

    public static void PutBytesBE(short value, byte[] buffer, int offset)
    {
        unsafe
        {
            PutBytesBE(*(ushort*) &value, buffer, offset);
        }
    }

    public static void PutBytesBE(int value, byte[] buffer, int offset)
    {
        unsafe
        {
            PutBytesBE(*(uint*) &value, buffer, offset);
        }
    }

    public static void PutBytesBE(long value, byte[] buffer, int offset)
    {
        unsafe
        {
            PutBytesBE(*(ulong*) &value, buffer, offset);
        }
    }

    public static void PutBytesBE(float value, byte[] buffer, int offset)
    {
        unsafe
        {
            PutBytesBE(*(uint*) &value, buffer, offset);
        }
    }

    public static void PutBytesBE(double value, byte[] buffer, int offset)
    {
        unsafe
        {
            PutBytesBE(*(ulong*) &value, buffer, offset);
        }
    }

    public static ushort GetUShortBE(byte[] buffer, int offset)
    {
        ushort value;
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if ((long) (uint) offset >= (long) buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (offset > buffer.Length - 2)
            throw new ArgumentException("buffer offset is too large");
        if (offset % 2 == 0)
        {
            unsafe
            {
                fixed (byte* numPtr = &buffer[offset])
                {
                    value = *(ushort*) numPtr;
                }
            }
            if (BitConverter.IsLittleEndian)
            {
                value = DataEncodings.SwapEndian(value);
            }
        }
        else
        {
            value = (ushort) (buffer[offset] << 8);
            value |= buffer[offset + 1];
        }
        return value;
    }

    public static uint GetUIntBE(byte[] buffer, int offset)
    {
        uint value;
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if ((long) (uint) offset >= (long) buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (offset > buffer.Length - 4)
            throw new ArgumentException("buffer offset is too large");
        if (offset % 4 == 0)
        {
            unsafe
            {
                fixed (byte* numPtr = &buffer[offset])
                {
                    value = *(uint*) numPtr;
                }
            }
            if (BitConverter.IsLittleEndian)
            {
                value = DataEncodings.SwapEndian(value);
            }
        }
        else
        {
            value = (uint) buffer[offset] << 24;
            value |= (uint) buffer[offset + 1] << 16;
            value |= (uint) buffer[offset + 2] << 8;
            value |= buffer[offset + 3];
        }
        return value;
    }

    public static ulong GetULongBE(byte[] buffer, int offset)
    {
        ulong value;
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));
        if ((long) (uint) offset >= (long) buffer.Length)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (offset > buffer.Length - 8)
            throw new ArgumentException("buffer offset is too large");
        if (offset % 8 == 0)
        {
            unsafe
            {
                fixed (byte* numPtr = &buffer[offset])
                {
                    value = *(ulong*) numPtr;
                }
            }
            if (BitConverter.IsLittleEndian)
            {
                value = DataEncodings.SwapEndian(value);
            }
        }
        else
        {
            value = (ulong) buffer[offset] << 56;
            value |= (ulong) buffer[offset + 1] << 48;
            value |= (ulong) buffer[offset + 2] << 40;
            value |= (ulong) buffer[offset + 3] << 32;
            value |= (ulong) buffer[offset + 4] << 24;
            value |= (ulong) buffer[offset + 5] << 16;
            value |= (ulong) buffer[offset + 6] << 8;
            value |= buffer[offset + 7];
        }
        return value;
    }

    public static short GetShortBE(byte[] buffer, int offset)
    {
        unsafe
        {
            var result = GetUShortBE(buffer, offset);
            return *(short*) &result;
        }
    }

    public static int GetIntBE(byte[] buffer, int offset)
    {
        unsafe
        {
            var result = GetUIntBE(buffer, offset);
            return *(int*) &result;
        }
    }

    public static long GetLongBE(byte[] buffer, int offset)
    {
        unsafe
        {
            var result = GetULongBE(buffer, offset);
            return *(long*) &result;
        }
    }

    public static float GetFloatBE(byte[] buffer, int offset)
    {
        unsafe
        {
            var result = GetIntBE(buffer, offset);
            return *(float*) &result;
        }
    }

    public static double GetDoubleBE(byte[] buffer, int offset)
    {
        unsafe
        {
            var result = GetLongBE(buffer, offset);
            return *(double*) &result;
        }
    }

    public static void PutZDOIDBE(ZDOID value, byte[] buffer, int offset)
    {
        PutBytesBE(value.userID, buffer, offset);
        PutBytesBE(value.id, buffer, offset + 8);
    }

    public static ZDOID GetZDOIDBE(byte[] buffer, int offset)
    {
        var userID = GetLongBE(buffer, offset);
        var id = GetUIntBE(buffer, offset + 8);
        return new ZDOID(userID, id);
    }

    public static void PutVector3BE(Vector3 value, byte[] buffer, int offset)
    {
        PutBytesBE(value.x, buffer, offset);
        PutBytesBE(value.y, buffer, offset + 4);
        PutBytesBE(value.z, buffer, offset + 8);
    }

    public static Vector3 GetVector3BE(byte[] buffer, int offset)
    {
        var x = GetFloatBE(buffer, offset);
        var y = GetFloatBE(buffer, offset + 4);
        var z = GetFloatBE(buffer, offset + 8);
        return new Vector3(x, y, z);
    }

    public static void PutVector2iBE(Vector2i value, byte[] buffer, int offset)
    {
        PutBytesBE(value.x, buffer, offset);
        PutBytesBE(value.y, buffer, offset + 4);
    }

    public static Vector2i GetVector2iBE(byte[] buffer, int offset)
    {
        var x = GetIntBE(buffer, offset);
        var y = GetIntBE(buffer, offset + 4);
        return new Vector2i(x, y);
    }

    public static void PutQuaternionBE(Quaternion value, byte[] buffer, int offset)
    {
        PutBytesBE(value.x, buffer, offset);
        PutBytesBE(value.y, buffer, offset + 4);
        PutBytesBE(value.z, buffer, offset + 8);
        PutBytesBE(value.w, buffer, offset + 12);
    }

    public static Quaternion GetQuaternionBE(byte[] buffer, int offset)
    {
        var x = GetFloatBE(buffer, offset);
        var y = GetFloatBE(buffer, offset + 4);
        var z = GetFloatBE(buffer, offset + 8);
        var w = GetFloatBE(buffer, offset + 12);
        return new Quaternion(x, y, z, w);
    }
}
