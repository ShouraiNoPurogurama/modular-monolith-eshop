using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace Basket.Basket.EventHandlers;

public class ProductPriceChangedIntegrationEventHandler(ILogger<ProductPriceChangedIntegrationEventHandler> logger) 
    : IConsumer<ProductPriceChangedIntegrationEvent>
{
    public Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
    {
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);
        
        //Find basket items with product id and update item price
        
        //MediatR new command and handler to find products on basket and update price

        return Task.CompletedTask;
    }
}