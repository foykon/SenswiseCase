using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Behaviors
{
    public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

        // Sürümünde beklenen imza: (request, CancellationToken, next)
        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("Handling {Req}: {@Payload}", typeof(TRequest).Name, request);
                var resp = await next();
                sw.Stop();
                _logger.LogInformation("Handled {Req} in {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
                return resp;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Error in {Req} after {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds);
                throw;
            }
        }

        // Yeni imza ile de uyumlu olmak için (opsiyonel)
        public Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
            => Handle(request, cancellationToken, next);
    }
}
