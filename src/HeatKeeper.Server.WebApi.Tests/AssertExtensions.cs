using System.Net;
using FluentAssertions;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class AssertExtensions
    {
        public static void ShouldBeConflict(this HttpStatusCode httpStatusCode)
        {
            httpStatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        public static void ShouldBeBadRequest(this HttpStatusCode httpStatusCode)
        {
            httpStatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
