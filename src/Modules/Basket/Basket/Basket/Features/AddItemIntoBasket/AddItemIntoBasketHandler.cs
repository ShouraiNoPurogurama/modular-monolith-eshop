namespace Basket.Basket.Features.AddItemIntoBasket;

public record AddItemIntoBasketCommand(string UserName, ShoppingCartItemDto ShoppingCartItemDto)
    : ICommand<AddItemIntoBasketResult>;

public record AddItemIntoBasketResult(Guid Id);

public class AddItemIntoBasketValidator : AbstractValidator<AddItemIntoBasketCommand>
{
    public AddItemIntoBasketValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required.");
        RuleFor(x => x.ShoppingCartItemDto.ProductId).NotEmpty().WithMessage("Product Id is required.");
        RuleFor(x => x.ShoppingCartItemDto.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}

public class AddItemIntoBasketHandler : ICommandHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
{
    private readonly BasketDbContext _dbContext;

    public AddItemIntoBasketHandler(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await _dbContext.ShoppingCarts 
            .SingleOrDefaultAsync(s => s.UserName == command.UserName, cancellationToken: cancellationToken)
            ?? throw new BasketNotFoundException(command.UserName);

        var shoppingCartItemDto = command.ShoppingCartItemDto;
        
        basket.AddItem(
            shoppingCartItemDto.ProductId,
            shoppingCartItemDto.Quantity,
            shoppingCartItemDto.Color,
            shoppingCartItemDto.Price,
            shoppingCartItemDto.ProductName
            );

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AddItemIntoBasketResult(basket.Id);
    }
}