namespace Basket.Models;

public class ShoppingCart : Aggregate<Guid>
{
    public string UserName { get; set; } = default!;

    private readonly List<ShoppingCartItem> _items = [];
    public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();

    public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

    public static ShoppingCart Create(Guid id, string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        var shoppingCart = new ShoppingCart
        {
            Id = id,
            UserName = userName
        };

        return shoppingCart;
    }

    public void AddItem(Guid productId, int quantity, string color, decimal price, string productName)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        var exitingItem = Items.FirstOrDefault(x => x.ProductId == productId);

        if (exitingItem is not null)
        {
            exitingItem.Quantity += quantity;
        }
        else
        {
            var newItem = new ShoppingCartItem(Id, productId, quantity, color, price, productName);
            _items.Add(newItem);
        }
    }
}