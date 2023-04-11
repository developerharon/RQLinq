using RQLinq.UnitTests.Infrastructure;

namespace RQLinq.UnitTests
{
    public class RqlEvaluatorTests
    {
        [Fact]
        public void Can_Filter_Given_Valid_RQL()
        {
            var testData = TestData.Orders;

            string rql = "eq(id,4)";
            var filterFunc = RqlEvaluator.Evaluate<Order>(rql);

            var result = testData.AsQueryable().Where(filterFunc).ToList();

            Assert.Single(result);
            var order = result[0];
            Assert.Equal(4, order.Id);
        }
    }
}