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
    private readonly IBasketRepository _basketRepository;

    public RemoveItemFromBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<RemoveItemFromBasketResult> Handle(RemoveItemFromBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasket(command.UserName, false, cancellationToken);

        basket.RemoveItem(command.ProductId);

        await _basketRepository.SaveChangesAsync(command.UserName, cancellationToken);

        return new RemoveItemFromBasketResult(basket.Id);
    }
}