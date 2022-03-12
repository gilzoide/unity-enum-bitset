using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace EnumBitSet.Tests
{
    public abstract class TestEnumSet<T> where T : Enum
    {
        readonly static T[] EnumValues = (T[]) Enum.GetValues(typeof(T));
        readonly static T Zero = EnumValues[0];
        readonly static T One = EnumValues[1];
        readonly static T Two = EnumValues[2];
        readonly static T Three = EnumValues[3];

        protected abstract ISet<T> CreateSet(params T[] initialValues);

        [Test]
        public void TestEmptySet()
        {
            var bitset = CreateSet();
        
            Assert.IsTrue(bitset.SetEquals(new T[0]));
            foreach (var enumValue in EnumValues)
            {
                Assert.IsFalse(bitset.Contains(enumValue));
                Assert.IsFalse(bitset.Remove(enumValue));
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
        public void TestAddRemoveClear()
        {
            var bitset = CreateSet();
            Assert.AreEqual(0, bitset.Count);
            
            Assert.IsTrue(bitset.Add(Zero));
            Assert.AreEqual(1, bitset.Count);
            Assert.IsFalse(bitset.Add(Zero));
            Assert.AreEqual(1, bitset.Count);

            Assert.IsTrue(bitset.Add(One));
            Assert.AreEqual(2, bitset.Count);
            Assert.IsFalse(bitset.Add(One));
            Assert.AreEqual(2, bitset.Count);

            Assert.IsTrue(bitset.Add(Three));
            Assert.AreEqual(3, bitset.Count);
            Assert.IsFalse(bitset.Add(Three));
            Assert.AreEqual(3, bitset.Count);

            Assert.IsTrue(bitset.Remove(Zero));
            Assert.AreEqual(2, bitset.Count);
            Assert.IsFalse(bitset.Remove(Zero));
            Assert.AreEqual(2, bitset.Count);

            bitset.Clear();
            Assert.AreEqual(0, bitset.Count);
        }

        [Test]
        public void TestCopyTo()
        {
            var bitset = CreateSet(Zero, One, Two, Three);

            var array = new T[5];
            bitset.CopyTo(array, 0);
            Assert.AreEqual(new[] {
                Zero,
                One,
                Two,
                Three,
                Commons.IntToEnum<T>(0)
            }, array);

            bitset.CopyTo(array, 1);
            Assert.AreEqual(new[] {
                Zero,
                Zero,
                One,
                Two,
                Three,
            }, array);

            Assert.Throws<ArgumentNullException>(() => bitset.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => bitset.CopyTo(array, -1));
            Assert.Throws<ArgumentException>(() => bitset.CopyTo(array, 2));
            Assert.Throws<ArgumentException>(() => bitset.CopyTo(array, 3));
        }

        [Test]
        public void TestExceptWith()
        {
            var bitset = CreateSet(One, Three);

            bitset.ExceptWith(new[] { Zero, One });
            Assert.IsTrue(bitset.SetEquals(new[] { Three }));

            bitset.ExceptWith(new[] { Two, One });
            Assert.IsTrue(bitset.SetEquals(new[] { Three }));

            bitset.ExceptWith(new[] { Three });
            Assert.IsTrue(bitset.SetEquals(new T[0]));

            Assert.Throws<ArgumentNullException>(() => bitset.ExceptWith(null));
        }

        [Test]
        public void TestIntersectWith()
        {
            var bitset = CreateSet(Zero, One, Two, Three);

            bitset.IntersectWith(new[] { Zero, One, Two });
            Assert.IsTrue(bitset.SetEquals(new[] { Zero, One, Two }));

            bitset.IntersectWith(new[] { Zero, Two });
            Assert.IsTrue(bitset.SetEquals(new[] { Zero, Two }));

            bitset.IntersectWith(new[] { One, Three });
            Assert.IsTrue(bitset.SetEquals(new T[0]));

            Assert.Throws<ArgumentNullException>(() => bitset.IntersectWith(null));
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
        public void TestSymmetricExceptWith()
        {
            var bitset = CreateSet(Zero, One);

            bitset.SymmetricExceptWith(new[] { One, Three });
            Assert.IsTrue(bitset.SetEquals(new[] { Zero, Three }));

            bitset.SymmetricExceptWith(new[] { Zero, One, Three });
            Assert.IsTrue(bitset.SetEquals(new[] { One }));

            Assert.Throws<ArgumentNullException>(() => bitset.SymmetricExceptWith(null));
        }

        [Test]
        public void TestUnionWith()
        {
            var bitset = CreateSet();

            bitset.UnionWith(new[] { One, Three });
            Assert.IsTrue(bitset.SetEquals(new[] { One, Three }));

            bitset.UnionWith(new[] { Zero, One });
            Assert.IsTrue(bitset.SetEquals(new[] { Zero, One, Three }));

            Assert.Throws<ArgumentNullException>(() => bitset.UnionWith(null));
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
    
    public class TestEnumBitSet32<T> : TestEnumSet<T> where T : struct, Enum
    {
        protected override ISet<T> CreateSet(params T[] initialValues)
        {
            return new EnumBitSet32<T>(initialValues);
        }
    }
    public class TestEnumBitSet32_32 : TestEnumBitSet32<TestEnum32> {}
    public class TestEnumBitSet32_64 : TestEnumBitSet32<TestEnum64> {}
    public class TestEnumBitSet32_Flags32 : TestEnumBitSet32<TestEnumFlags32> {}
    public class TestEnumBitSet32_Flags64 : TestEnumBitSet32<TestEnumFlags64> {}
    
    public class TestEnumBitSet64<T> : TestEnumSet<T> where T : struct, Enum
    {
        protected override ISet<T> CreateSet(params T[] initialValues)
        {
            return new EnumBitSet64<T>(initialValues);
        }
    }
    public class TestEnumBitSet64_32 : TestEnumBitSet64<TestEnum32> {}
    public class TestEnumBitSet64_64 : TestEnumBitSet64<TestEnum64> {}
    public class TestEnumBitSet64_Flags32 : TestEnumBitSet64<TestEnumFlags32> {}
    public class TestEnumBitSet64_Flags64 : TestEnumBitSet64<TestEnumFlags64> {}
    
    public class TestEnumHashSet<T> : TestEnumSet<T> where T : struct, Enum
    {
        protected override ISet<T> CreateSet(params T[] initialValues)
        {
            return new HashSet<T>(initialValues);
        }
    }
    public class TestEnumHashSet_32 : TestEnumHashSet<TestEnum32> {}
    public class TestEnumHashSet_64 : TestEnumHashSet<TestEnum64> {}
    public class TestEnumHashSet_Flags32 : TestEnumHashSet<TestEnumFlags32> {}
    public class TestEnumHashSet_Flags64 : TestEnumHashSet<TestEnumFlags64> {}
}
