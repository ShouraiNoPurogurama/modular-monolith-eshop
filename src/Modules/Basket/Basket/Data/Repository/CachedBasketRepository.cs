using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Data.Repository;

public class CachedBasketRepository : IBasketRepository
{
    private readonly IBasketRepository _basketRepository;
    private readonly IDistributedCache _cache;

    public CachedBasketRepository(IBasketRepository basketRepository, IDistributedCache cache)
    {
        _basketRepository = basketRepository;
        _cache = cache;
    }

    public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        //If tracking is needed
        if (!asNoTracking)
        {
            return await _basketRepository.GetBasket(userName, false, cancellationToken);
        }

        //Get the serialized Basket stored in cache
        var cachedBasket = await _cache.GetStringAsync(userName, cancellationToken);

        //If the value was found
        if (!string.IsNullOrEmpty(cachedBasket))
        {
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;
        }

        //If the value was not found
        var basket = await _basketRepository.GetBasket(userName, false, cancellationToken);
        
        await _cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), cancellationToken);
        
        return basket;
    }

    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        var newBasket = await _basketRepository.CreateBasket(basket, cancellationToken);
        
        await _cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(newBasket), cancellationToken);

        return newBasket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        var result = await _basketRepository.DeleteBasket(userName, cancellationToken);
         
        await _cache.RemoveAsync(userName, cancellationToken);

        return result;
    }

    public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        var result = await _basketRepository.SaveChangesAsync(userName, cancellationToken);

        if (result == 0) return result;
        
        //Clear the cache
        if (userName is not null)
        {
            await _cache.RemoveAsync(userName, cancellationToken);
        }

        return result;
    }
}