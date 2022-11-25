using NUnit.Framework;
using CSVUtility.Core;

namespace CSVUtility.Test
{
    public class UtilityEngineTestSample : SpecificationBase
    {
        private UtilityEngine sut;

        protected override void Given()
        {
            sut = new UtilityEngine(null);
        }

        protected override void When()
        {
            sut.SampleProperty = true;
        }

        [Test]
        public void it_should_not_throw()
        {
            Assert.DoesNotThrow(() => sut.Action(), "Does not throw when the action is called.");
        }
    }
}
