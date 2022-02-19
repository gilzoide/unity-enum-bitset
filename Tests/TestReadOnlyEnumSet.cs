using System;
using EnumBitSet;
using NUnit.Framework;

namespace Tests
{
    public abstract class TestReadOnlyEnumSet<T> where T : Enum
    {
        readonly static T[] EnumValues = (T[]) Enum.GetValues(typeof(T));
        readonly static T Zero = EnumValues[0];
        readonly static T One = EnumValues[1];
        readonly static T Two = EnumValues[2];
        readonly static T Three = EnumValues[3];

        protected abstract IReadOnlySet<T> CreateSet(params T[] initialValues);

        [Test]
        public void TestEmptySet()
        {
            var bitset = CreateSet();
        
            Assert.IsTrue(bitset.SetEquals(new T[0]));
            foreach (var enumValue in EnumValues)
            {
                Assert.IsFalse(bitset.Contains(enumValue));
                Assert.IsFalse(bitset.SetEquals(new[] { enumValue }));
            }
            Assert.AreEqual(0, bitset.Count);
            
            Assert.IsFalse(bitset.GetEnumerator().MoveNext());
        }

        [Test]
        public void TestSingletonSet()
        {
            var bitset = CreateSet(Zero);

            Assert.AreEqual(1, bitset.Count);

            Assert.IsTrue(bitset.Contains(Zero));
            Assert.IsFalse(bitset.Contains(One));
            Assert.IsFalse(bitset.Contains(Two));
            Assert.IsFalse(bitset.Contains(Three));

            using (var enumerator = bitset.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(Zero, enumerator.Current);
                Assert.IsFalse(enumerator.MoveNext());
            }
        }
        
        [Test]
        public void TestFullSet()
        {
            var bitset = CreateSet(Three, Zero, Two, One);

            Assert.AreEqual(4, bitset.Count);

            Assert.IsTrue(bitset.Contains(Zero));
            Assert.IsTrue(bitset.Contains(One));
            Assert.IsTrue(bitset.Contains(Two));
            Assert.IsTrue(bitset.Contains(Three));

            using (var enumerator = bitset.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(Enum.IsDefined(typeof(T), enumerator.Current));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(Enum.IsDefined(typeof(T), enumerator.Current));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(Enum.IsDefined(typeof(T), enumerator.Current));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(Enum.IsDefined(typeof(T), enumerator.Current));
                Assert.IsFalse(enumerator.MoveNext());
            }
        }
        
        [Test]
        public void TestOverlaps()
        {
            var bitset = CreateSet(Zero, One);

            Assert.IsTrue(bitset.Overlaps(new[] { Zero, One, Two }));
            Assert.IsTrue(bitset.Overlaps(new[] { Zero }));
            Assert.IsTrue(bitset.Overlaps(new[] { One, Three }));
            Assert.IsFalse(bitset.Overlaps(new T[0]));
            Assert.IsFalse(bitset.Overlaps(new[] { Two, Three }));

            Assert.Throws<ArgumentNullException>(() => bitset.Overlaps(null));
        }
        
        [Test]
        public void TestSubset()
        {
            var bitset = CreateSet(Zero);

            Assert.IsTrue(bitset.IsSubsetOf(new[] { Zero }));
            Assert.IsTrue(bitset.IsSubsetOf(new[] { Zero, Three }));
            Assert.IsFalse(bitset.IsSubsetOf(new[] { One }));

            Assert.IsFalse(bitset.IsProperSubsetOf(new[] { Zero }));
            Assert.IsTrue(bitset.IsProperSubsetOf(new[] { Zero, Two }));

            Assert.Throws<ArgumentNullException>(() => bitset.IsSubsetOf(null));
            Assert.Throws<ArgumentNullException>(() => bitset.IsProperSubsetOf(null));
        }

        [Test]
        public void TestSuperset()
        {
            var bitset = CreateSet(Zero, Two);

            Assert.IsTrue(bitset.IsSupersetOf(new[] { Zero }));
            Assert.IsTrue(bitset.IsSupersetOf(new[] { Two }));
            Assert.IsTrue(bitset.IsSupersetOf(new[] { Zero, Two }));
            Assert.IsFalse(bitset.IsSupersetOf(new[] { Zero, Three }));
            Assert.IsFalse(bitset.IsSupersetOf(new[] { One }));

            Assert.IsTrue(bitset.IsProperSupersetOf(new[] { Zero }));
            Assert.IsTrue(bitset.IsProperSupersetOf(new[] { Two }));
            Assert.IsFalse(bitset.IsProperSupersetOf(new[] { Zero, Two }));

            Assert.Throws<ArgumentNullException>(() => bitset.IsSupersetOf(null));
            Assert.Throws<ArgumentNullException>(() => bitset.IsProperSupersetOf(null));
        }
    }
    
    public class TestReadOnlyEnumBitSet32<T> : TestReadOnlyEnumSet<T> where T : struct, Enum
    {
        protected override IReadOnlySet<T> CreateSet(params T[] initialValues)
        {
            return new EnumBitSet32<T>(initialValues);
        }
    }
    public class TestReadOnlyEnumBitSet32_32 : TestReadOnlyEnumBitSet32<TestEnum32> {}
    public class TestReadOnlyEnumBitSet32_64 : TestReadOnlyEnumBitSet32<TestEnum64> {}
    public class TestReadOnlyEnumBitSet32_Flags32 : TestReadOnlyEnumBitSet32<TestEnumFlags32> {}
    public class TestReadOnlyEnumBitSet32_Flags64 : TestReadOnlyEnumBitSet32<TestEnumFlags64> {}
    
    public class TestReadOnlyEnumBitSet64<T> : TestReadOnlyEnumSet<T> where T : struct, Enum
    {
        protected override IReadOnlySet<T> CreateSet(params T[] initialValues)
        {
            return new EnumBitSet64<T>(initialValues);
        }
    }
    public class TestReadOnlyEnumBitSet64_32 : TestReadOnlyEnumBitSet64<TestEnum32> {}
    public class TestReadOnlyEnumBitSet64_64 : TestReadOnlyEnumBitSet64<TestEnum64> {}
    public class TestReadOnlyEnumBitSet64_Flags32 : TestReadOnlyEnumBitSet64<TestEnumFlags32> {}
    public class TestReadOnlyEnumBitSet64_Flags64 : TestReadOnlyEnumBitSet64<TestEnumFlags64> {}
    
    public class TestReadOnlyEnumBitMask32<T> : TestReadOnlyEnumSet<T> where T : struct, Enum
    {
        protected override IReadOnlySet<T> CreateSet(params T[] initialValues)
        {
            return new EnumBitMask32<T>(initialValues);
        }
    }
    public class TestReadOnlyEnumBitMask32_32 : TestReadOnlyEnumBitMask32<TestEnum32> {}
    public class TestReadOnlyEnumBitMask32_64 : TestReadOnlyEnumBitMask32<TestEnum64> {}
    public class TestReadOnlyEnumBitMask32_Flags32 : TestReadOnlyEnumBitMask32<TestEnumFlags32> {}
    public class TestReadOnlyEnumBitMask32_Flags64 : TestReadOnlyEnumBitMask32<TestEnumFlags64> {}
    
    public class TestReadOnlyEnumBitMask64<T> : TestReadOnlyEnumSet<T> where T : struct, Enum
    {
        protected override IReadOnlySet<T> CreateSet(params T[] initialValues)
        {
            return new EnumBitMask64<T>(initialValues);
        }
    }
    public class TestReadOnlyEnumBitMask64_32 : TestReadOnlyEnumBitMask64<TestEnum32> {}
    public class TestReadOnlyEnumBitMask64_64 : TestReadOnlyEnumBitMask64<TestEnum64> {}
    public class TestReadOnlyEnumBitMask64_Flags32 : TestReadOnlyEnumBitMask64<TestEnumFlags32> {}
    public class TestReadOnlyEnumBitMask64_Flags64 : TestReadOnlyEnumBitMask64<TestEnumFlags64> {}
}
