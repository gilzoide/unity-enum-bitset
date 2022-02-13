using System.Collections.Generic;

namespace Tests
{
    public class TestEnumHashSet : TestEnumSet
    {
        protected override ISet<TestEnum> CreateSet(params TestEnum[] initialValues)
        {
            return new HashSet<TestEnum>(initialValues);
        }
    }
}
