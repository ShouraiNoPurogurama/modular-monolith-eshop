using Microsoft.AspNetCore.Mvc;

namespace Basket.Basket.Features.AddItemIntoBasket;

public record AddItemIntoBasketRequest(string UserName, ShoppingCartItemDto ShoppingCartItemDto);

public record AddItemIntoBasketResponse(Guid Id);

public class AddItemIntoBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/{userName}/items",
                async ([FromRoute] string userName, [FromBody] AddItemIntoBasketRequest request, ISender sender) =>
                {
                    var command = new AddItemIntoBasketCommand(userName, request.ShoppingCartItemDto);

                    var result = await sender.Send(command);

                    var response = result.Adapt<AddItemIntoBasketResponse>();

                    return Results.Created($"/basket/{response.Id}", response);
                })
            .Produces<AddItemIntoBasketResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Item into Basket")
            .WithDescription("Add Item into Basket");
    }
}