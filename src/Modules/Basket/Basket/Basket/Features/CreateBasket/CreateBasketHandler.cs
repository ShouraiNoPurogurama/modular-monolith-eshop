namespace Basket.Basket.Features.CreateBasket;

public record CreateBasketCommand(ShoppingCartDto ShoppingCart) : ICommand<CreateBasketResult>;

public record CreateBasketResult(Guid Id);

public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
{
    public CreateBasketCommandValidator()
    {
        RuleFor(c => c.ShoppingCart.UserName).NotEmpty().WithMessage("Username is required.");
    }
}

public class CreateBasketHandler : ICommandHandler<CreateBasketCommand, CreateBasketResult>
{
    private readonly BasketDbContext _dbContext;

    public CreateBasketHandler(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
    {
        //Create Basket entity from command object
        //Save to DB
        //Return result
        var shoppingCart = CreateNewBasket(command.ShoppingCart);

        _dbContext.ShoppingCarts.Add(shoppingCart);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CreateBasketResult(shoppingCart.Id);
    }

    private ShoppingCart CreateNewBasket(ShoppingCartDto shoppingCartDto)
    {
        var newBasket = ShoppingCart.Create(
            Guid.NewGuid(),
            shoppingCartDto.UserName
        );

        shoppingCartDto.Items.ForEach(item =>
        {
            newBasket.AddItem(
                item.ProductId,
                item.Quantity,
                item.Color,
                item.Price,
                item.ProductName
            );
        });

        return newBasket;
    }
}