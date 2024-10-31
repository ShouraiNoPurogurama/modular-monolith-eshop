using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.Data.Processors;

public class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    IServiceProvider serviceProvider,
    IBus bus,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private CancellationTokenSource _wakeupCancellationTokenSource = new CancellationTokenSource();


    #region Old implementation

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Create a while loop to run whether stopping still not require cancelaion

        //1. Create scope for BasketDbcontext to ensure that it will be disposed after the try catch block

        //2. Fetch the OutboxMessages with ProcessedOn = null in db

        //3. Iterate the Outbox Messages, get Event Type, deserialize the Event Message and public it to RabbitMQ

        //4. Update ProcessedOn prop after publish the Event

        //5. Save changes to db

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
                var outboxMessages = await dbContext.OutboxMessages
                    .Where(o => o.ProcessedOn == null)
                    .ToListAsync(stoppingToken);

                foreach (var message in outboxMessages)
                {
                    var eventType = Type.GetType(message.Type);
                    if (eventType is null)
                    {
                        logger.LogWarning("Could not resolve type: {Type}", message.Type);
                        continue;
                    }

                    var eventMessage = JsonSerializer.Deserialize(message.Content, eventType);
                    if (eventMessage is null)
                    {
                        logger.LogWarning("Could not deserialize message: {Content}", message.Content);
                        continue;
                    }

                    await bus.Publish(eventMessage, stoppingToken);

                    message.ProcessedOn = DateTime.UtcNow;

                    logger.LogInformation("Successfully processed outbox message with ID: {ID}", message.Id);

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error processing outbox message");
            }

            //Delay: Just check for Outbox table for each 10 seconds
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        #endregion

        #region new implementation

        // public void StartPublishingIntegrationEvents()
        // {
        //     _wakeupCancellationTokenSource.Cancel();
        // }
        //
        // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        // {
        //     try
        //     {
        //         var factory = new ConnectionFactory();
        //         var connection = factory.CreateConnection();
        //         var channel = connection.CreateModel();
        //         channel.ConfirmSelect(); //enable publisher confirms
        //         var props = channel.CreateBasicProperties();
        //         props.DeliveryMode = 2; //persist message
        //
        //         while (!stoppingToken.IsCancellationRequested)
        //         {
        //             try
        //             {
        //                 using var scope = serviceProvider.CreateScope();
        //                 var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
        //                 var outboxMessages = await dbContext.OutboxMessages
        //                     .Where(o => o.ProcessedOn == null)
        //                     .ToListAsync(stoppingToken);
        //
        //                 foreach (var message in outboxMessages)
        //                 {
        //                     var eventType = Type.GetType(message.Type);
        //                     if (eventType is null)
        //                     {
        //                         logger.LogWarning("Could not resolve type: {Type}", message.Type);
        //                         continue;
        //                     }
        //
        //                     var eventMessage = JsonSerializer.Deserialize(message.Content, eventType);
        //                     if (eventMessage is null)
        //                     {
        //                         logger.LogWarning("Could not deserialize message: {Content}", message.Content);
        //                         continue;
        //                     }
        //
        //                     channel.BasicPublish(exchange: "user",
        //                         routingKey: message.Type,
        //                         basicProperties: props,
        //                         body: Encoding.UTF8.GetBytes(message.Content)
        //                         );
        //                     
        //                     channel.WaitForConfirmsOrDie(new TimeSpan(0,0,5));
        //                     
        //                     message.ProcessedOn = DateTime.UtcNow;
        //
        //                     logger.LogInformation("Successfully processed outbox message with ID: {ID}", message.Id);
        //
        //                     await dbContext.SaveChangesAsync(stoppingToken);
        //                 }
        //             }
        //             catch (Exception e)
        //             {
        //                 logger.LogError(e, "Error processing outbox message");
        //             }
        //
        //             using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_wakeupCancellationTokenSource.Token, stoppingToken);
        //             try
        //             {
        //                 await Task.Delay(Timeout.Infinite, linkedCts.Token);
        //             }
        //             catch (OperationCanceledException)
        //             {
        //                 if (_wakeupCancellationTokenSource.Token.IsCancellationRequested)
        //                 {
        //                     logger.LogInformation("Publish requested");
        //                     var tmp = _wakeupCancellationTokenSource;
        //                     _wakeupCancellationTokenSource = new CancellationTokenSource();
        //                     tmp.Dispose();
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         throw;
        //     }

        #endregion
    }
}