# EnumBitSet
Fast and memory efficient implementation of C#'s `ISet` for enums, storing data
as bit masks:
- `EnumBitSet32<T>`: uses `EnumBitMask32<T>` as data, supporting enums with up to 32 values.
- `EnumBitSet64<T>`: uses `EnumBitMask64<T>` as data, supporting enums with up to 64 values.

Bit masks are readonly structs implementing `IReadOnlySet` for enums:
- `EnumBitMask32<T>`: uses `int` as data, supporting enums with up to 32 values.
- `EnumBitMask64<T>`: uses `long` as data, supporting enums with up to 64 values.

All implementations support enums both with and without `[Flags]` attributes.

Conversions between enum values and integer types are non-boxing where possible
by using unsafe utilities from Unity, .NET 5+ or .NET Core 3.0+

## Unity Property Drawer
In Unity, there's a custom property drawer for selecting the containing enums:

```cs
using System;
using UnityEngine;
using EnumBitSet;

public class ScriptWithBitSet : MonoBehaviour
{
    public enum TesteEnum
    {
        Zero, One, Two, Three
    }

    [Flags]
    public enum TesteEnumFlags
    {
        Zero = 1 << 0, One = 1 << 1, Two = 1 << 2, Three = 1 << 3
    }
    
    public EnumBitSet32<TesteEnum> aBitset;
    public EnumBitSet64<TesteEnumFlags> anotherBitset;
}
```

![](Extras~/CustomDrawer.png)