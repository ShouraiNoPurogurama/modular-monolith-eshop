namespace Basket.Basket.Features.GetBasket;

public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;

public record GetBasketResult(ShoppingCartDto ShoppingCart);

public class GetBasketHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    private readonly IBasketRepository _basketRepository;

    public GetBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasket(query.UserName, false, cancellationToken);

        var basketDto = basket.Adapt<ShoppingCartDto>();

        return new GetBasketResult(basketDto);
    }
}