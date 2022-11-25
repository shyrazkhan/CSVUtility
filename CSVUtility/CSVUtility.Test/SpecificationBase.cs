using NUnit.Framework;

namespace CSVUtility.Test
{
    public class SpecificationBase
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            ForAllTests();
        }

        [SetUp]
        public void SetUp()
        {
            Given();
            When();

            AfterActing();
        }

        [TearDown]
        public void TearDown()
        {
            CleanUp();
        }

        protected virtual void ForAllTests()
        {
        }

        protected virtual void Given()
        {
        }

        protected virtual void When()
        {
        }

        protected virtual void AfterActing()
        {
        }

        protected virtual void CleanUp()
        {
        }
    }

    public class ThenAttribute : TestAttribute
    { }
}
