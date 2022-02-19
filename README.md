# EnumBitSet
Fast and memory efficient implementation of C#'s `ISet` for enums, storing data
as bit masks:
- `EnumBitSet32<T>`: uses `EnumBitMask32<T>` as data, supporting enums with up to 32 values.
- `EnumBitSet64<T>`: uses `EnumBitMask64<T>` as data, supporting enums with up to 64 values.

Bit masks are readonly structs implementing `IReadOnlySet` for enums:
- `EnumBitMask32<T>`: uses `int` as data, supporting enums with up to 32 values.
- `EnumBitMask64<T>`: uses `long` as data, supporting enums with up to 64 values.

Conversions between enum values and integer types are non-boxing where possible
by using unsafe utilities from Unity, .NET 5+ or .NET Core 3.0+

## Unity Property Drawer
In Unity, there's a custom property drawer for selecting the containing enums:

```cs
using EnumBitSet;
using UnityEngine;

public class ScriptWithEnumSet : MonoBehaviour
{
    public enum TestEnum
    {
        Zero, One, Two, Three
    }

    public EnumBitSet32<TestEnum> aBitset;
    public EnumBitSet64<TestEnum> anotherBitset;
}
```

![](Extras~/CustomDrawer.png)