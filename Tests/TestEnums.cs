using System;

namespace EnumBitSet.Tests
{
    public enum TestEnum32
    {
        Zero, One, Two, Three,
    }

    public enum TestEnum64 : long
    {
        Zero, One, Two, Three,
    }

    [Flags]
    public enum TestEnumFlags32
    {
        Zero = 1 << 0, One = 1 << 1, Two = 1 << 2, Three = 1 << 3,
    }

    [Flags]
    public enum TestEnumFlags64 : long
    {
        Zero = 1 << 0, One = 1 << 1, Two = 1 << 2, Three = 1 << 3,
    }
}