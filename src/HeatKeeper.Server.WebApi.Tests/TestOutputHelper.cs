using System.Threading;
using Xunit.Abstractions;

namespace HeatKeeper.Server.WebApi.Tests
{
        public static class TestOutputHelper
    {
        private static readonly AsyncLocal<ITestOutputHelper> CurrentTestOutputHelper
            = new AsyncLocal<ITestOutputHelper>();

        public static void Capture(this ITestOutputHelper outputHelper)
        {
            CurrentTestOutputHelper.Value = outputHelper;
        }

        public static ITestOutputHelper Current => CurrentTestOutputHelper.Value;
    }
}