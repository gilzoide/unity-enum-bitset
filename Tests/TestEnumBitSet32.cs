using System.Collections.Generic;
using EnumBitSet;

namespace Tests
{
    public class TestEnumBitSet32 : TestEnumSet
    {
        protected override ISet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new EnumBitSet32<TestEnum>(initialValues);
        }
    }
}
