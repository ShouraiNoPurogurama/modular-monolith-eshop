namespace Basket.Data.Repository;

public class BasketRepository(BasketDbContext dbContext) : IBasketRepository
{
    public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.ShoppingCarts
            .Include(sc => sc.Items)
            .Where(sc => sc.UserName == userName);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        var basket = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return basket ?? throw new BasketNotFoundException(userName);
    }

    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        dbContext.ShoppingCarts.Add(basket);
        await dbContext.SaveChangesAsync(cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        var basket = await GetBasket(userName, false, cancellationToken);

        dbContext.Remove(basket);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}