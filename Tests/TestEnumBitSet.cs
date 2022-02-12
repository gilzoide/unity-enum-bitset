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
        public void TestEmptyEnumBitSet()
        {
            var bitset = new EnumBitSet32<TestEnum>();
        
            Assert.IsFalse(bitset.Any());
            foreach (var enumValue in (TestEnum[]) Enum.GetValues(typeof(TestEnum)))
            {
                Assert.IsFalse(bitset[enumValue]);
                Assert.IsFalse(bitset.Contains(enumValue));
                Assert.IsFalse(bitset.Remove(enumValue));
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

            using(var enumerator = bitset.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(TestEnum.Zero, enumerator.Current);
                Assert.IsFalse(enumerator.MoveNext());
            }
        }
    }
}
