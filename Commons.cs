using System.Collections.Generic;

namespace EnumBitSet
{
    public static class Commons
    {
        public static IEnumerable<int> EnumerateSetBits(uint mask)
        {
            var i = 0;
            while (mask != 0 && i < sizeof(uint) * 8)
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