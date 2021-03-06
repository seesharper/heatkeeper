using System;

namespace HeatKeeper.Abstractions.Logging
{
    public delegate Logger LogFactory(Type type);

    public delegate void Logger(LogLevel level, string message, Exception ex = null);

    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    public static class LogExtensions
    {
        public static Logger CreateLogger<T>(this LogFactory logFactory) => logFactory(typeof(T));

        public static void Trace(this Logger logger, string message) => logger(LogLevel.Trace, message);
        public static void Debug(this Logger logger, string message) => logger(LogLevel.Debug, message);
        public static void Info(this Logger logger, string message) => logger(LogLevel.Info, message);
        public static void Warning(this Logger logger, string message, Exception exception = null) => logger(LogLevel.Warning, message, exception);
        public static void Error(this Logger logger, string message, Exception exception = null) => logger(LogLevel.Error, message, exception);
        public static void Critical(this Logger logger, string message, Exception exception = null) => logger(LogLevel.Critical, message, exception);
    }
}
