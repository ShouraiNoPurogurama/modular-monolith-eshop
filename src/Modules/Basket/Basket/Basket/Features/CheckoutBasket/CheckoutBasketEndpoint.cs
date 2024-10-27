namespace Basket.Basket.Features.CheckoutBasket;

public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckout);

public record CheckoutBasketResponse(bool IsSuccess);

public class CheckoutBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/checkout", async (CheckoutBasketRequest request, ISender sender) =>
            {
                var command = request.Adapt<CheckoutBasketCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<CheckoutBasketResponse>();

                return Results.Ok(response);
            })
            .WithName("CheckoutBasket")
            .Produces<CheckoutBasketResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription("Checkout Basket")
            .WithSummary("Checkout Basket")
            .RequireAuthorization();
    }
}