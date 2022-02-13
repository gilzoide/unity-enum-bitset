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
    }
}