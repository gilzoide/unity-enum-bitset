using System.Collections.Generic;
using EnumBitSet;

namespace Tests
{
    public class TestEnumBitSet64 : TestEnumSet
    {
        protected override ISet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new EnumBitSet64<TestEnum>(initialValues);
        }
    }
}
