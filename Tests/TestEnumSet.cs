using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Tests
{
    public abstract class TestEnumSet
    {
        protected enum TestEnum
        {
            Zero,
            One,
            Two,
            Three,
        }

        protected abstract ISet<TestEnum> CreateSet(params TestEnum[] initialValues);

        [Test]
        public void TestEmptySet()
        {
            var bitset = CreateSet();
        
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] {}));
            foreach (var enumValue in (TestEnum[]) Enum.GetValues(typeof(TestEnum)))
            {
                Assert.IsFalse(bitset.Contains(enumValue));
                Assert.IsFalse(bitset.Remove(enumValue));
                Assert.IsFalse(bitset.SetEquals(new TestEnum[] { enumValue }));
            }
            Assert.AreEqual(0, bitset.Count);
            
            Assert.IsFalse(bitset.GetEnumerator().MoveNext());
        }

        [Test]
        public void TestSingletonEnumBitSet()
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
        public void TestAddRemoveClear()
        {
            var bitset = CreateSet();
            Assert.AreEqual(0, bitset.Count);
            
            Assert.IsTrue(bitset.Add(TestEnum.Zero));
            Assert.AreEqual(1, bitset.Count);
            Assert.IsFalse(bitset.Add(TestEnum.Zero));
            Assert.AreEqual(1, bitset.Count);

            Assert.IsTrue(bitset.Add(TestEnum.One));
            Assert.AreEqual(2, bitset.Count);
            Assert.IsFalse(bitset.Add(TestEnum.One));
            Assert.AreEqual(2, bitset.Count);

            Assert.IsTrue(bitset.Add(TestEnum.Three));
            Assert.AreEqual(3, bitset.Count);
            Assert.IsFalse(bitset.Add(TestEnum.Three));
            Assert.AreEqual(3, bitset.Count);

            Assert.IsTrue(bitset.Remove(TestEnum.Zero));
            Assert.AreEqual(2, bitset.Count);
            Assert.IsFalse(bitset.Remove(TestEnum.Zero));
            Assert.AreEqual(2, bitset.Count);

            bitset.Clear();
            Assert.AreEqual(0, bitset.Count);
        }

        [Test]
        public void TestCopyTo()
        {
            var bitset = CreateSet(TestEnum.Zero, TestEnum.One, TestEnum.Two, TestEnum.Three);

            var array = new TestEnum[5];
            bitset.CopyTo(array, 0);
            Assert.AreEqual(new TestEnum[] {
                TestEnum.Zero,
                TestEnum.One,
                TestEnum.Two,
                TestEnum.Three,
                0
            }, array);

            bitset.CopyTo(array, 1);
            Assert.AreEqual(new TestEnum[] {
                TestEnum.Zero,
                TestEnum.Zero,
                TestEnum.One,
                TestEnum.Two,
                TestEnum.Three,
            }, array);

            Assert.Throws<ArgumentNullException>(() => bitset.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => bitset.CopyTo(array, -1));
            Assert.Throws<ArgumentException>(() => bitset.CopyTo(array, 2));
            Assert.Throws<ArgumentException>(() => bitset.CopyTo(array, 3));
        }

        [Test]
        public void TestExceptWith()
        {
            var bitset = CreateSet(TestEnum.One, TestEnum.Three);

            bitset.ExceptWith(new TestEnum[] { TestEnum.Zero, TestEnum.One });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Three }));

            bitset.ExceptWith(new TestEnum[] { TestEnum.Two, TestEnum.One });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Three }));

            bitset.ExceptWith(new TestEnum[] { TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] {}));

            Assert.Throws<ArgumentNullException>(() => bitset.ExceptWith(null));
        }

        [Test]
        public void TestIntersectWith()
        {
            var bitset = CreateSet(TestEnum.Zero, TestEnum.One, TestEnum.Two, TestEnum.Three);

            bitset.IntersectWith(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Two });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Two }));

            bitset.IntersectWith(new TestEnum[] { TestEnum.Zero, TestEnum.Two });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Zero, TestEnum.Two }));

            bitset.IntersectWith(new TestEnum[] { TestEnum.One, TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] {}));

            Assert.Throws<ArgumentNullException>(() => bitset.IntersectWith(null));
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
        public void TestSymmetricExceptWith()
        {
            var bitset = CreateSet(TestEnum.Zero, TestEnum.One);

            bitset.SymmetricExceptWith(new TestEnum[] { TestEnum.One, TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Zero, TestEnum.Three }));

            bitset.SymmetricExceptWith(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.One }));

            Assert.Throws<ArgumentNullException>(() => bitset.SymmetricExceptWith(null));
        }

        [Test]
        public void TestUnionWith()
        {
            var bitset = CreateSet();

            bitset.UnionWith(new TestEnum[] { TestEnum.One, TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.One, TestEnum.Three }));

            bitset.UnionWith(new TestEnum[] { TestEnum.Zero, TestEnum.One });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Three }));

            Assert.Throws<ArgumentNullException>(() => bitset.UnionWith(null));
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
}
