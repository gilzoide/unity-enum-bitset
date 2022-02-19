using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
#if UNITY_5_3_OR_NEWER
using Unity.Collections.LowLevel.Unsafe;
#endif

namespace EnumBitSet
{
    public static class Commons
    {
        public static IEnumerable<int> EnumerateSetBits(int mask)
        {
            var i = 0;
            while (mask != 0 && i < sizeof(int) * 8)
            {
                if ((mask & 1) != 0)
                {
                    yield return i;
                }

                i++;
                mask >>= 1;
            }
        }

        public static IEnumerable<int> EnumerateSetBits(long mask)
        {
            var i = 0;
            while (mask != 0 && i < sizeof(long) * 8)
            {
                if ((mask & 1) != 0)
                {
                    yield return i;
                }

                i++;
                mask >>= 1;
            }
        }

        public static int CountSetBits(int mask)
        {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            return BitOperations.PopCount(mask);
#else
            var count = 0;
            foreach (var _ in EnumerateSetBits(mask))
            {
                count++;
            }
            return count;
#endif
        }
        
        public static int CountSetBits(long mask)
        {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            return BitOperations.PopCount(mask);
#else
            var count = 0;
            foreach (var _ in EnumerateSetBits(mask))
            {
                count++;
            }
            return count;
#endif
        }

        public static int EnumToInt<T>(T value) where T : struct, Enum
        {
#if UNITY_5_3_OR_NEWER
            return UnsafeUtility.EnumToInt(value);
#elif NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            if (EnumMetadata<T>.SizeOf == sizeof(long))
            {
                return (int) Unsafe.As<T, long>(ref value);
            }
            return Unsafe.As<T, int>(ref value);
#else
            return Convert.ToInt32(value);
#endif
        }

        public static T IntToEnum<T>(int value) where T : Enum
        {
#if UNITY_5_3_OR_NEWER
            if (EnumMetadata<T>.SizeOf == sizeof(long))
            {
                var longValue = (long) value;
                return UnsafeUtility.As<long, T>(ref longValue);
            }
            return UnsafeUtility.As<int, T>(ref value);
#elif NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            if (EnumMetadata<T>.SizeOf == sizeof(long))
            {
                var longValue = (long) value;
                return Unsafe.As<long, T>(ref longValue);
            }
            return Unsafe.As<int, T>(ref value);
#else
            return (T) Enum.ToObject(typeof(T), value);
#endif
        }

        public static long EnumToLong<T>(T value) where T : struct, Enum
        {
#if UNITY_5_3_OR_NEWER
            if (EnumMetadata<T>.SizeOf == sizeof(long))
            {
                return UnsafeUtility.As<T, long>(ref value);
            }
            return UnsafeUtility.EnumToInt(value);
#elif NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            if (EnumMetadata<T>.SizeOf == sizeof(long))
            {
                return Unsafe.As<T, long>(ref value);
            }
            return Unsafe.As<T, int>(ref value);
#else
            return Convert.ToInt64(value);
#endif
        }

        public static T LongToEnum<T>(long value) where T : Enum
        {
#if UNITY_5_3_OR_NEWER
            if (EnumMetadata<T>.SizeOf == sizeof(long))
            {
                return UnsafeUtility.As<long, T>(ref value);
            }
            var intValue = (int) value;
            return UnsafeUtility.As<int, T>(ref intValue);
#elif NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            if (EnumMetadata<T>.SizeOf == sizeof(long))
            {
                return Unsafe.As<long, T>(ref value);
            }
            var intValue = (int) value;
            return Unsafe.As<int, T>(ref intValue);
#else
            return (T) Enum.ToObject(typeof(T), value);
#endif
        }
    }

    public static class EnumMetadata<T> where T : Enum
    {
        public static int SizeOf = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(T)));
        public static bool HasFlags = typeof(T).IsDefined(typeof(FlagsAttribute));
    }
}