using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Orders.Dtos;
using Ordering.Orders.Features.CreateOrder;
using Shared.Messaging.Events;
using Guid = System.Guid;

namespace Ordering.Orders.EventHandlers;

public class BasketCheckoutIntegrationEventHandler
    (ISender sender, ILogger<BasketCheckoutIntegrationEventHandler> logger)
    : IConsumer<BasketCheckoutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
    {
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

        //Create new order and start order fulfillment process
        var createOrderCommand = MapToCreateOrderCommand(context.Message);
        await sender.Send(createOrderCommand);
    }

    private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutIntegrationEvent message)
    {
        var addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine,
            message.Country, message.State, message.ZipCode);

        var paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration, message.Cvv,
            message.PaymentMethod);

        var orderId = Guid.NewGuid();

        var orderDto = new OrderDto(orderId, message.CustomerId, message.UserName, addressDto, addressDto, paymentDto,
        [
            new OrderItemDto(orderId, new Guid("1a4f660f-24c2-4197-bae6-ffdd06f2a674"), 1, 799.99m),
            new OrderItemDto(orderId, new Guid("958330dc-55ec-447e-87af-6749f7b48453"), 1, 749.99m)
        ]);
        
        return new CreateOrderCommand(orderDto);
    }
}