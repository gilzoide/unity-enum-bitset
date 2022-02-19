using System.Collections.Generic;
using System.Runtime.InteropServices;

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
    }
}