using Catalog.Contracts.Products.Features.GetProductById;

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

public class AddItemIntoBasketHandler(IBasketRepository basketRepository, ISender sender)
    : ICommandHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
{
    public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetBasket(command.UserName, false, cancellationToken);

        var shoppingCartItemDto = command.ShoppingCartItemDto;

        //Before add item into SC, we should call Catalog module GetProductById method
        //Get latest product information and set Price and ProductName when adding item 
        var result = await sender.Send(new GetProductByIdQuery(command.ShoppingCartItemDto.ProductId), cancellationToken);

        basket.AddItem(
            shoppingCartItemDto.ProductId,
            shoppingCartItemDto.Quantity,
            shoppingCartItemDto.Color,
            result.Product.Price,
            result.Product.Name
        );

        await basketRepository.SaveChangesAsync(command.UserName, cancellationToken);

        return new AddItemIntoBasketResult(basket.Id);
    }
}