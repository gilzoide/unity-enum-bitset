using System;
using System.Collections.Generic;
using EnumBitSet;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Tests.Performance
{
    public abstract class PerformanceEnumSet<T> where T : Enum
    {
        readonly static T[] EnumValues = (T[]) Enum.GetValues(typeof(T));
        readonly static T Zero = EnumValues[0];
        readonly static T One = EnumValues[1];
        readonly static T Two = EnumValues[2];
        readonly static T Three = EnumValues[3];

        protected abstract ISet<T> CreateSet(params T[] initialValues);
        
        [Test, Performance]
        public void TestAddPerformance()
        {
            Measure.Method(() =>
                {
                    var bitset = CreateSet();

                    bitset.Add(Zero);
                    bitset.Add(One);
                    bitset.Add(Two);
                    bitset.Add(Three);
                })
                .SampleGroup("Add")
                .IterationsPerMeasurement(10000)
                .MeasurementCount(20)
                .GC()
                .Run();
        }
        
        [Test, Performance]
        public void TestRemovePerformance()
        {
            Measure.Method(() =>
                {
                    var bitset = CreateSet(Zero, One, Two, Three);

                    bitset.Remove(Zero);
                    bitset.Remove(One);
                    bitset.Remove(Two);
                    bitset.Remove(Three);
                })
                .SampleGroup("Remove")
                .IterationsPerMeasurement(10000)
                .MeasurementCount(20)
                .GC()
                .Run();
        }
        
        [Test, Performance]
        public void TestUnionPerformance()
        {
            Measure.Method(() =>
                {
                    var bitset = CreateSet();

                    bitset.UnionWith(new[] {Zero, One, Two, Three});
                })
                .SampleGroup("UnionWith")
                .IterationsPerMeasurement(10000)
                .MeasurementCount(20)
                .GC()
                .Run();
        }
    }
    
    public class PerformanceEnumBitSet32<T> : PerformanceEnumSet<T> where T : struct, Enum
    {
        protected override ISet<T> CreateSet(params T[] initialValues)
        {
            return new EnumBitSet32<T>(initialValues);
        }
    }
    public class PerformanceEnumBitSet32_32 : PerformanceEnumBitSet32<TestEnum32> {}
    public class PerformanceEnumBitSet32_64 : PerformanceEnumBitSet32<TestEnum64> {}
    public class PerformanceEnumBitSet32_Flags32 : PerformanceEnumBitSet32<TestEnumFlags32> {}
    public class PerformanceEnumBitSet32_Flags64 : PerformanceEnumBitSet32<TestEnumFlags64> {}
    
    public class PerformanceEnumBitSet64<T> : PerformanceEnumSet<T> where T : struct, Enum
    {
        protected override ISet<T> CreateSet(params T[] initialValues)
        {
            return new EnumBitSet64<T>(initialValues);
        }
    }
    public class PerformanceEnumBitSet64_32 : PerformanceEnumBitSet64<TestEnum32> {}
    public class PerformanceEnumBitSet64_64 : PerformanceEnumBitSet64<TestEnum64> {}
    public class PerformanceEnumBitSet64_Flags32 : PerformanceEnumBitSet64<TestEnumFlags32> {}
    public class PerformanceEnumBitSet64_Flags64 : PerformanceEnumBitSet64<TestEnumFlags64> {}
    
    public class PerformanceEnumHashSet<T> : PerformanceEnumSet<T> where T : struct, Enum
    {
        protected override ISet<T> CreateSet(params T[] initialValues)
        {
            return new HashSet<T>(initialValues);
        }
    }
    public class PerformanceEnumHashSet_32 : PerformanceEnumHashSet<TestEnum32> {}
    public class PerformanceEnumHashSet_64 : PerformanceEnumHashSet<TestEnum64> {}
    public class PerformanceEnumHashSet_Flags32 : PerformanceEnumHashSet<TestEnumFlags32> {}
    public class PerformanceEnumHashSet_Flags64 : PerformanceEnumHashSet<TestEnumFlags64> {}
}