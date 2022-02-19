using System.Collections.Generic;
using EnumBitSet;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Tests.Performance
{
    public abstract class PerformanceEnumSet
    {
        protected enum TestEnum : long
        {
            Zero,
            One,
            Two,
            Three,
        }
        
        protected abstract ISet<TestEnum> CreateSet(params TestEnum[] initialValues);
        
        [Test, Performance]
        public void TestAddPerformance()
        {
            Measure.Method(() =>
            {
                var bitset = CreateSet();

                bitset.Add(TestEnum.Zero);
                bitset.Add(TestEnum.One);
                bitset.Add(TestEnum.Two);
                bitset.Add(TestEnum.Three);
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
                    var bitset = CreateSet(TestEnum.Zero, TestEnum.One, TestEnum.Two, TestEnum.Three);

                    bitset.Remove(TestEnum.Zero);
                    bitset.Remove(TestEnum.One);
                    bitset.Remove(TestEnum.Two);
                    bitset.Remove(TestEnum.Three);
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

                    bitset.UnionWith(new[] {TestEnum.Zero, TestEnum.One, TestEnum.Two, TestEnum.Three});
                })
                .SampleGroup("UnionWith")
                .IterationsPerMeasurement(10000)
                .MeasurementCount(20)
                .GC()
                .Run();
        }
    }
    
    public class PerformanceEnumBitSet32 : PerformanceEnumSet
    {
        protected override ISet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new EnumBitSet32<TestEnum>(initialValues);
        }
    }
    
    public class PerformanceEnumBitSet64 : PerformanceEnumSet
    {
        protected override ISet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new EnumBitSet64<TestEnum>(initialValues);
        }
    }
    
    public class PerformanceEnumHashSet : PerformanceEnumSet
    {
        protected override ISet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new HashSet<TestEnum>(initialValues);
        }
    }
}