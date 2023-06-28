using Microsoft.Extensions.Logging;
using Sentry;

namespace Libro.Infrastructure.Logging
{
    public class SentryErrorHandler : ILogger
    {
        private readonly SentryClient _sentryClient;

        //public SentryErrorHandler(string dsn)
        //{
        //    _sentryClient = new SentryClient(dsn);
        //}

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel >= LogLevel.Error)
            {
                _sentryClient.CaptureException(exception);
            }
        }
    }
}
