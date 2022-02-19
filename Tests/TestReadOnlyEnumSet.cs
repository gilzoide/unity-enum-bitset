using System;
using System.Collections.Generic;
using EnumBitSet;
using NUnit.Framework;

namespace Tests
{
    public abstract class TestReadOnlyEnumSet
    {
        protected enum TestEnum
        {
            Zero,
            One,
            Two,
            Three,
        }

        protected abstract IReadOnlySet<TestEnum> CreateSet(params TestEnum[] initialValues);

        [Test]
        public void TestEmptySet()
        {
            var bitset = CreateSet();
        
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] {}));
            foreach (var enumValue in (TestEnum[]) Enum.GetValues(typeof(TestEnum)))
            {
                Assert.IsFalse(bitset.Contains(enumValue));
                Assert.IsFalse(bitset.SetEquals(new TestEnum[] { enumValue }));
            }
            Assert.AreEqual(0, bitset.Count);
            
            Assert.IsFalse(bitset.GetEnumerator().MoveNext());
        }

        [Test]
        public void TestSingletonSet()
        {
            var bitset = CreateSet(TestEnum.Zero);

            Assert.AreEqual(1, bitset.Count);

            Assert.IsTrue(bitset.Contains(TestEnum.Zero));
            Assert.IsFalse(bitset.Contains(TestEnum.One));
            Assert.IsFalse(bitset.Contains(TestEnum.Two));
            Assert.IsFalse(bitset.Contains(TestEnum.Three));

            using (var enumerator = bitset.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(TestEnum.Zero, enumerator.Current);
                Assert.IsFalse(enumerator.MoveNext());
            }
        }
        
        [Test]
        public void TestFullSet()
        {
            var bitset = CreateSet(TestEnum.Three, TestEnum.Zero, TestEnum.Two, TestEnum.One);

            Assert.AreEqual(4, bitset.Count);

            Assert.IsTrue(bitset.Contains(TestEnum.Zero));
            Assert.IsTrue(bitset.Contains(TestEnum.One));
            Assert.IsTrue(bitset.Contains(TestEnum.Two));
            Assert.IsTrue(bitset.Contains(TestEnum.Three));

            using (var enumerator = bitset.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(Enum.IsDefined(typeof(TestEnum), enumerator.Current));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(Enum.IsDefined(typeof(TestEnum), enumerator.Current));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(Enum.IsDefined(typeof(TestEnum), enumerator.Current));
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsTrue(Enum.IsDefined(typeof(TestEnum), enumerator.Current));
                Assert.IsFalse(enumerator.MoveNext());
            }
        }
        
        [Test]
        public void TestOverlaps()
        {
            var bitset = CreateSet(TestEnum.Zero, TestEnum.One);

            Assert.IsTrue(bitset.Overlaps(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Two }));
            Assert.IsTrue(bitset.Overlaps(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.Overlaps(new TestEnum[] { TestEnum.One, TestEnum.Three }));
            Assert.IsFalse(bitset.Overlaps(new TestEnum[] {}));
            Assert.IsFalse(bitset.Overlaps(new TestEnum[] { TestEnum.Two, TestEnum.Three }));

            Assert.Throws<ArgumentNullException>(() => bitset.Overlaps(null));
        }
        
        [Test]
        public void TestSubset()
        {
            var bitset = CreateSet(TestEnum.Zero);

            Assert.IsTrue(bitset.IsSubsetOf(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.IsSubsetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Three }));
            Assert.IsFalse(bitset.IsSubsetOf(new TestEnum[] { TestEnum.One }));

            Assert.IsFalse(bitset.IsProperSubsetOf(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.IsProperSubsetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Two }));

            Assert.Throws<ArgumentNullException>(() => bitset.IsSubsetOf(null));
            Assert.Throws<ArgumentNullException>(() => bitset.IsProperSubsetOf(null));
        }

        [Test]
        public void TestSuperset()
        {
            var bitset = CreateSet(TestEnum.Zero, TestEnum.Two);

            Assert.IsTrue(bitset.IsSupersetOf(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.IsSupersetOf(new TestEnum[] { TestEnum.Two }));
            Assert.IsTrue(bitset.IsSupersetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Two }));
            Assert.IsFalse(bitset.IsSupersetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Three }));
            Assert.IsFalse(bitset.IsSupersetOf(new TestEnum[] { TestEnum.One }));

            Assert.IsTrue(bitset.IsProperSupersetOf(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.IsProperSupersetOf(new TestEnum[] { TestEnum.Two }));
            Assert.IsFalse(bitset.IsProperSupersetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Two }));

            Assert.Throws<ArgumentNullException>(() => bitset.IsSupersetOf(null));
            Assert.Throws<ArgumentNullException>(() => bitset.IsProperSupersetOf(null));
        }
    }
    
    public class TestReadOnlyEnumBitSet32 : TestReadOnlyEnumSet
    {
        protected override IReadOnlySet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new EnumBitSet32<TestEnum>(initialValues);
        }
    }
    
    public class TestReadOnlyEnumBitSet64 : TestReadOnlyEnumSet
    {
        protected override IReadOnlySet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new EnumBitSet64<TestEnum>(initialValues);
        }
    }
    
    public class TestReadOnlyEnumBitMask32 : TestReadOnlyEnumSet
    {
        protected override IReadOnlySet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new EnumBitMask32<TestEnum>(initialValues);
        }
    }
    
    public class TestReadOnlyEnumBitMask64 : TestReadOnlyEnumSet
    {
        protected override IReadOnlySet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new EnumBitMask64<TestEnum>(initialValues);
        }
    }
}
