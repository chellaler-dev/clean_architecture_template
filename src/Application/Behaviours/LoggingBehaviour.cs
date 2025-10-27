using MediatR;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
namespace Application.Behaviours;
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : MediatR.IRequest<TResponse>
    {
        private readonly ILogger <LoggingBehaviour<TRequest, TResponse>> _logger;
        
        
        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) 
        { 
            
            this._logger = logger;
        }

        
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {

            // Pre Logic
            var requestName = request.GetType();
            _logger.LogInformation("{Request} is starting.", requestName);

            var timer = Stopwatch.StartNew();

            var response = await next();
            
            timer.Stop();

            // Post Logic
            _logger.LogInformation("{Request} has finished in {Time}ms.", requestName, timer.ElapsedMilliseconds);


            return response;
        }
    }

