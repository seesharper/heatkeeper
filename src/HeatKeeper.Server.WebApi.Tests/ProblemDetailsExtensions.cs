using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class ProblemDetailsExtensions
    {
        public static void ShouldHaveConflictStatus(this ProblemDetails problemDetails)
        {
            ((HttpStatusCode)problemDetails.Status).ShouldBeConflict();
        }

        public static void ShouldHaveUnauthorizedStatus(this ProblemDetails problemDetails)
        {
            ((HttpStatusCode)problemDetails.Status).Should().Be(HttpStatusCode.Unauthorized);
        }

        public static void ShouldHaveBadRequestStatus(this ProblemDetails problemDetails)
        {
            ((HttpStatusCode)problemDetails.Status).Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
