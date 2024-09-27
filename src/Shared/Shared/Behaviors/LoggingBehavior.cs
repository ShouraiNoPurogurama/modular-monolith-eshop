using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Shared.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        //Notify which request is being processing
        logger.LogInformation(
            "[START] Handle request = {RequestType} - Response = {ResponseType} - RequestData = {Request}", typeof(TRequest).Name,
            typeof(TResponse).Name, request);

        //Initialize timer
        var timer = new Stopwatch();
        timer.Start();

        //Get response of next handler
        var response = await next();

        //Get execution time
        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3)
            logger.LogWarning("[PERFORMANCE] the request {RequestType} took {TimeTaken} seconds.", typeof(TRequest).Name,
                timeTaken);

        logger.LogInformation("[END] Handled {requestType} with {responseType}", typeof(TRequest).Name, typeof(TResponse).Name);
        return response;
    }
}