using System;
using HeatKeeper.Server.WebApi.Tests;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Server.WebApi
{
    public class TestLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            TestOutputHelper.Current.WriteLine($"{logLevel} {formatter(state, exception)}");
            if (exception != null)
            {
                 TestOutputHelper.Current.WriteLine(exception.ToString());
            }
        }
    }


    public class TestLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger();
        }

        public void Dispose()
        {

        }
    }


    public static class TestLoggerFactoryExtensions
    {
       public static ILoggerFactory AddTestLogger(this ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new TestLoggerProvider());
            return loggerFactory;
        }
    }
}