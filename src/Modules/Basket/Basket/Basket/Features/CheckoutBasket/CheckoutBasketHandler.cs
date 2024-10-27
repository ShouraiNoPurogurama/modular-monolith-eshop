using System.Text.Json;
using Basket.Basket.Models;
using Shared.Messaging.Events;

namespace Basket.Basket.Features.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout) : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketValidator()
    {
        RuleFor(x => x.BasketCheckout).NotNull().WithMessage("Basket checkout Dto can't be null");
        RuleFor(x => x.BasketCheckout.UserName).NotEmpty().WithMessage("User name is required.");
    }
}

public class CheckoutBasketHandler : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    private readonly BasketDbContext _dbContext;

    public CheckoutBasketHandler(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand request, CancellationToken cancellationToken)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var basket = await _dbContext.ShoppingCarts
                             .Include(x => x.Items)
                             .FirstOrDefaultAsync(x => x.UserName == request.BasketCheckout.UserName, cancellationToken)
                         ?? throw new BasketNotFoundException(request.BasketCheckout.UserName);

            var eventMessage = request.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
            eventMessage.TotalPrice = basket.TotalPrice;
            
            //Write a message to the outbox
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = typeof(BasketCheckoutIntegrationEvent).AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(eventMessage),
                OccuredOn = DateTime.UtcNow
            };

            _dbContext.OutboxMessages.Add(outboxMessage);
            
            //Delete the basket
            _dbContext.ShoppingCarts.Remove(basket);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            //Rollback if transaction fails
            await transaction.RollbackAsync(cancellationToken);
            return new CheckoutBasketResult(false);
        }

        return new CheckoutBasketResult(true);


        /////////////  CHECKOUT BASKET WITHOUT OUTBOX PATTERN
        //Get existing basket with total price
        // var basket = await _basketRepository.GetBasket(request.BasketCheckout.UserName, true, cancellationToken)
        //              ?? throw new NotFoundException("Basket not found.");
        //
        // var eventMessage = request.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
        //
        // //Set total price on basket checkout event message
        // eventMessage.TotalPrice = basket.TotalPrice;
        //
        // //Send basket checkout event to rabbitmq using masstransit
        // await _bus.Publish(eventMessage, cancellationToken);
        //
        // //Delete the basket
        // await _basketRepository.DeleteBasket(request.BasketCheckout.UserName, cancellationToken);
        //
        // return new CheckoutBasketResult(true);
    }
}