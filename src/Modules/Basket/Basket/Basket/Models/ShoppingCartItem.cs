using System.Text.Json.Serialization;

namespace Basket.Basket.Models;

public class ShoppingCartItem : Entity<Guid>
{
    public Guid ShoppingCartId { get; private set; }  = default!;
    public Guid ProductId { get; private set; } = default!;
    public int Quantity { get; internal set; } = default!;
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Color { get; private set; }
    //Will come from Catalog module
    public decimal Price { get; private set; } = default!;
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ProductName { get; private set; }

    internal ShoppingCartItem(Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
    {
        ShoppingCartId = shoppingCartId;
        ProductId = productId;
        Quantity = quantity;
        Color = color;
        Price = price;
        ProductName = productName;
    }

    [JsonConstructor]
    public ShoppingCartItem(Guid id, Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
    {
        Id = id;
        ShoppingCartId = shoppingCartId;
        ProductId = productId;
        Quantity = quantity;
        Color = color;
        Price = price;
        ProductName = productName;
    }
}