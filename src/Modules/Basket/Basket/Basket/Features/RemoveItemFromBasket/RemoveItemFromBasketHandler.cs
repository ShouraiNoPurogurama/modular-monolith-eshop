namespace Basket.Basket.Features.RemoveItemFromBasket;

public record RemoveItemFromBasketCommand(string UserName, Guid ProductId)
    : ICommand<RemoveItemFromBasketResult>;

public record RemoveItemFromBasketResult(Guid Id);

public class RemoveItemFromBasketValidator : AbstractValidator<RemoveItemFromBasketCommand>
{
    public RemoveItemFromBasketValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product Id is required.");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.");
    }
}

public class RemoveItemFromBasketHandler : ICommandHandler<RemoveItemFromBasketCommand, RemoveItemFromBasketResult>
{
    private readonly BasketDbContext _dbContext;

    public RemoveItemFromBasketHandler(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RemoveItemFromBasketResult> Handle(RemoveItemFromBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await _dbContext.ShoppingCarts
                         .Include(s => s.Items)
                         .SingleOrDefaultAsync(s => s.UserName == command.UserName, cancellationToken: cancellationToken)
                     ?? throw new BasketNotFoundException(command.UserName);

        basket.RemoveItem(command.ProductId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RemoveItemFromBasketResult(basket.Id);
    }
}