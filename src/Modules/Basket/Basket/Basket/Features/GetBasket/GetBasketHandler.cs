namespace Basket.Basket.Features.GetBasket;

public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;

public record GetBasketResult(ShoppingCartDto ShoppingCart);

public class GetBasketHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    private readonly BasketDbContext _dbContext;

    public GetBasketHandler(BasketDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        var basket = await _dbContext.ShoppingCarts
                         .AsNoTracking()
                         .Include(b => b.Items)
                         .SingleOrDefaultAsync(b => b.UserName == query.UserName, cancellationToken: cancellationToken)
                     ?? throw new BasketNotFoundException(query.UserName);

        var basketDto = basket.Adapt<ShoppingCartDto>();

        return new GetBasketResult(basketDto);
    }
}