using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace Catalog.Products.EventHandlers;

//Convert the domain event into Integration event
public class ProductPriceChangedHandler(IBus bus,ILogger<ProductPriceChangedEvent> logger)
    : INotificationHandler<ProductPriceChangedEvent>
{
    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);

        //Public product price changed integration event for update basket prices
        var integrationEvent = new ProductPriceChangedIntegrationEvent
        {
            ProductId = notification.Product.Id,
            Name = notification.Product.Name,
            Category = notification.Product.Category,
            Description = notification.Product.Description,
            ImageFile = notification.Product.ImageFile,
            Price = notification.Product.Price
        };

        //Publish the event for the integration event handler to handle it
        await bus.Publish(integrationEvent, cancellationToken);
    }
}