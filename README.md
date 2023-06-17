# EnumBitSet
[![openupm](https://img.shields.io/npm/v/com.gilzoide.enum-bitset?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.gilzoide.enum-bitset/)

Fast and memory efficient implementation of C#'s `ISet` for enums, storing data as bit masks:
- `EnumBitSet32<T>`: uses `EnumBitMask32<T>` as data, supporting enums with up to 32 values.
- `EnumBitSet64<T>`: uses `EnumBitMask64<T>` as data, supporting enums with up to 64 values.

Bit masks are readonly structs implementing `IReadOnlySet` for enums:
- `EnumBitMask32<T>`: uses `int` as data, supporting enums with up to 32 values.
- `EnumBitMask64<T>`: uses `long` as data, supporting enums with up to 64 values.

All implementations support enums both with and without `[Flags]` attributes.

Conversions between enum values and integer types are non-boxing where possible by using unsafe utilities from Unity 2018+, .NET 5+ or .NET Core 3.0+


## Installing the package
This package is available on the [openupm registry](https://openupm.com/) and can be installed using the [openupm-cli](https://github.com/openupm/openupm-cli):

```
openupm add com.gilzoide.enum-bitset
```

Otherwise, you can install directly using the [Unity Package Manager](https://docs.unity3d.com/Manual/Packages.html) with the following URL:

```
https://github.com/gilzoide/unity-enum-bitset.git#1.0.0
```


## Unity 2020+ Serialization and Property Drawer
In Unity 2020+, `EnumBitSet`s are serializable directly as generic classes.
There's also a custom property drawer for selecting the containing enums:

```cs
using System;
using UnityEngine;
using Gilzoide.EnumBitSet;

public enum TestEnum
{
    Zero, One, Two, Three
}

[Flags]
public enum TestEnumFlags
{
    Zero = 1 << 0, One = 1 << 1, Two = 1 << 2, Three = 1 << 3
}

public class ScriptWithBitSet : MonoBehaviour
{    
    public EnumBitSet32<TestEnum> aBitset;
    public EnumBitSet64<TestEnumFlags> anotherBitset;
}
```

![](Extras~/CustomDrawer.png)

## Unity 2019- Serialization and Property Drawer
In Unity 2019 and earlier, create non-generic classes that
inherit from the generic ones:

```cs
using System;
using UnityEngine;
using Gilzoide.EnumBitSet;

public enum TestEnum
{
    Zero, One, Two, Three
}

[Flags]
public enum TestEnumFlags
{
    Zero = 1 << 0, One = 1 << 1, Two = 1 << 2, Three = 1 << 3
}

[Serializable]
public class TestEnumBitSet32 : EnumBitSet32<TestEnum> {}

[Serializable]
public class TestEnumFlagsBitSet64 : EnumBitSet64<TestEnumFlags> {}

public class ScriptWithBitSet : MonoBehaviour
{
    public EnumBitSet32<TestEnum> aBitset;
    public EnumBitSet64<TestEnumFlags> anotherBitset;
}
```

For the custom Property Drawer, the same applies.
Create a class that inherits from `EnumBitSetPropertyDrawer`:

```cs
using UnityEditor;
using Gilzoide.EnumBitSet.Editor;

[CustomPropertyDrawer(typeof(TestEnumBitSet32))]
public class TestEnumBitSet32PropertyDrawer : EnumBitSetPropertyDrawer {}
```