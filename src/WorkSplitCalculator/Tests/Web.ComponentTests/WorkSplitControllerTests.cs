using FluentAssertions;
using FluentAssertions.Execution;
using System.Net;
using System.Threading.Tasks;
using SystemTestingTools;
using Xunit;

namespace Web.ComponentTests
{
    [Collection("SharedServer collection")]
    public class WorkSplitControllerTests
    {
        private readonly TestServerFixture Fixture;

        public WorkSplitControllerTests(TestServerFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task BasicTest()
        {
            // arrange
            var client = Fixture.Server.CreateClient();
            client.CreateSession();

            // act
            var httpResponse = await client.GetAsync("/WorkSplit/testing");

            using (new AssertionScope())
            {
                // assert logs
                //var logs = client.GetSessionLogs();
                //logs.Should().HaveCount(1);
                //logs[0].ToString().Should().Be("INFORMATION: testing");

                // assert outgoing
                client.GetSessionOutgoingRequests().Should().BeEmpty();

                // assert return
                httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                var response = await httpResponse.ReadBody();

                response.Should().Be("TESTING");
            }
        }
    }
}
