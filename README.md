# EnumBitSet
A memory efficient `ISet` for enums that store data using bit masks.

Currently, there is an implementation using `int` as data (`EnumBitSet32<T>`),
supporting enums with up to 32 values.

In Unity, there's a custom property drawer for selecting the containing enums.