namespace Basket.Basket.Features.DeleteBasket;

public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;

public record DeleteBasketResult(bool IsSuccess);

public class DeleteBasketHandler : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    private readonly BasketDbContext _context;

    public DeleteBasketHandler(BasketDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await _context.ShoppingCarts
                         .SingleOrDefaultAsync(sc => sc.UserName == command.UserName, cancellationToken: cancellationToken)
                     ?? throw new BasketNotFoundException(command.UserName);

        _context.ShoppingCarts.Remove(basket);
        await _context.SaveChangesAsync(cancellationToken);
        return new DeleteBasketResult(true);
    }
}