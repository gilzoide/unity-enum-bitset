using System;
using EnumBitSet;
using NUnit.Framework;

namespace Tests
{
    public class TestEnumBitSet
    {
        private enum TestEnum
        {
            Zero,
            One,
            Two,
            Three,
        }

        [Test]
        public void TestEmptySet()
        {
            var bitset = new EnumBitSet32<TestEnum>();
        
            Assert.IsFalse(bitset.Any());
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] {}));
            foreach (var enumValue in (TestEnum[]) Enum.GetValues(typeof(TestEnum)))
            {
                Assert.IsFalse(bitset[enumValue]);
                Assert.IsFalse(bitset.Contains(enumValue));
                Assert.IsFalse(bitset.Remove(enumValue));
                Assert.IsFalse(bitset.SetEquals(new TestEnum[] { enumValue }));
            }
            Assert.IsFalse(bitset[null]);
            Assert.AreEqual(0, bitset.Count);
            
            Assert.IsFalse(bitset.GetEnumerator().MoveNext());
        }

        [Test]
        public void TestSingletonEnumBitSet()
        {
            var bitset = new EnumBitSet32<TestEnum>(TestEnum.Zero);

            Assert.AreEqual(1, bitset.Count);

            Assert.IsTrue(bitset[TestEnum.Zero]);
            Assert.IsTrue(bitset.Contains(TestEnum.Zero));
            Assert.IsFalse(bitset[TestEnum.One]);
            Assert.IsFalse(bitset.Contains(TestEnum.One));
            Assert.IsFalse(bitset[TestEnum.Two]);
            Assert.IsFalse(bitset.Contains(TestEnum.Two));
            Assert.IsFalse(bitset[TestEnum.Three]);
            Assert.IsFalse(bitset.Contains(TestEnum.Three));
            Assert.IsFalse(bitset[null]);
            
            Assert.IsFalse(bitset[TestEnum.Zero, TestEnum.One]);

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
            var bitset = new EnumBitSet32<TestEnum>();
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
            var bitset = new EnumBitSet32<TestEnum>(
                TestEnum.Zero,
                TestEnum.One,
                TestEnum.Two,
                TestEnum.Three
            );

            var array = new TestEnum[4];
            bitset.CopyTo(array, 0);
            Assert.AreEqual(new TestEnum[] {
                TestEnum.Zero,
                TestEnum.One,
                TestEnum.Two,
                TestEnum.Three,
            }, array);

            bitset.CopyTo(array, 1);
            Assert.AreEqual(new TestEnum[] {
                TestEnum.Zero,
                TestEnum.Zero,
                TestEnum.One,
                TestEnum.Two,
            }, array);

            bitset.CopyTo(array, 2);
            Assert.AreEqual(new TestEnum[] {
                TestEnum.Zero,
                TestEnum.Zero,
                TestEnum.Zero,
                TestEnum.One,
            }, array);

            bitset.CopyTo(array, 3);
            Assert.AreEqual(new TestEnum[] {
                TestEnum.Zero,
                TestEnum.Zero,
                TestEnum.Zero,
                TestEnum.Zero,
            }, array);
        }

        [Test]
        public void TestExceptWith()
        {
            var bitset = new EnumBitSet32<TestEnum>(TestEnum.One, TestEnum.Three);

            bitset.ExceptWith(new TestEnum[] { TestEnum.Zero, TestEnum.One });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Three }));

            bitset.ExceptWith(new TestEnum[] { TestEnum.Two, TestEnum.One });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Three }));


            bitset.ExceptWith(new TestEnum[] { TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] {}));
        }

        [Test]
        public void TestIntersectWith()
        {
            var bitset = new EnumBitSet32<TestEnum>(TestEnum.Zero, TestEnum.One, TestEnum.Two, TestEnum.Three);

            bitset.IntersectWith(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Two });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Two }));

            bitset.IntersectWith(new TestEnum[] { TestEnum.Zero, TestEnum.Two });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Zero, TestEnum.Two }));

            bitset.IntersectWith(new TestEnum[] { TestEnum.One, TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] {}));
        }

        [Test]
        public void TestOverlaps()
        {
            var bitset = new EnumBitSet32<TestEnum>(TestEnum.Zero, TestEnum.One);

            Assert.IsTrue(bitset.Overlaps(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Two }));
            Assert.IsTrue(bitset.Overlaps(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.Overlaps(new TestEnum[] { TestEnum.One, TestEnum.Three }));
            Assert.IsFalse(bitset.Overlaps(new TestEnum[] {}));
            Assert.IsFalse(bitset.Overlaps(new TestEnum[] { TestEnum.Two, TestEnum.Three }));
        }

        [Test]
        public void TestSymmetricExceptWith()
        {
            var bitset = new EnumBitSet32<TestEnum>(TestEnum.Zero, TestEnum.One);

            bitset.SymmetricExceptWith(new TestEnum[] { TestEnum.One, TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Zero, TestEnum.Three }));

            bitset.SymmetricExceptWith(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.One }));
        }

        [Test]
        public void TestUnionWith()
        {
            var bitset = new EnumBitSet32<TestEnum>();

            bitset.UnionWith(new TestEnum[] { TestEnum.One, TestEnum.Three });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.One, TestEnum.Three }));

            bitset.UnionWith(new TestEnum[] { TestEnum.Zero, TestEnum.One });
            Assert.IsTrue(bitset.SetEquals(new TestEnum[] { TestEnum.Zero, TestEnum.One, TestEnum.Three }));
        }

        [Test]
        public void TestSubset()
        {
            var bitset = new EnumBitSet32<TestEnum>(TestEnum.Zero);

            Assert.IsTrue(bitset.IsSubsetOf(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.IsSubsetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Three }));
            Assert.IsFalse(bitset.IsSubsetOf(new TestEnum[] { TestEnum.One }));

            Assert.IsFalse(bitset.IsProperSubsetOf(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.IsProperSubsetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Two }));
        }

        [Test]
        public void TestSuperset()
        {
            var bitset = new EnumBitSet32<TestEnum>(TestEnum.Zero, TestEnum.Two);

            Assert.IsTrue(bitset.IsSupersetOf(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.IsSupersetOf(new TestEnum[] { TestEnum.Two }));
            Assert.IsTrue(bitset.IsSupersetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Two }));
            Assert.IsFalse(bitset.IsSupersetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Three }));
            Assert.IsFalse(bitset.IsSupersetOf(new TestEnum[] { TestEnum.One }));

            Assert.IsTrue(bitset.IsProperSupersetOf(new TestEnum[] { TestEnum.Zero }));
            Assert.IsTrue(bitset.IsProperSupersetOf(new TestEnum[] { TestEnum.Two }));
            Assert.IsFalse(bitset.IsProperSupersetOf(new TestEnum[] { TestEnum.Zero, TestEnum.Two }));
        }
    }
}
