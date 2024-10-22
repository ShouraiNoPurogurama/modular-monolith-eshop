using System.Text.Json;
using System.Text.Json.Serialization;
using Basket.Basket.Models;
using Basket.Data.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Data.Repository;

public class CachedBasketRepository : IBasketRepository
{
    private readonly IBasketRepository _basketRepository;
    private readonly IDistributedCache _cache;

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new ShoppingCartConverter(), new ShoppingCartItemConverter() }
    };
    
    public CachedBasketRepository(IBasketRepository basketRepository, IDistributedCache cache)
    {
        _basketRepository = basketRepository;
        _cache = cache;
    }

    public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        //If tracking is not needed
        if (!asNoTracking)
        {
            return await _basketRepository.GetBasket(userName, false, cancellationToken);
        } 

        //Get the serialized Basket stored in cache
        var cachedBasket = await _cache.GetStringAsync(userName, cancellationToken);

        //If the value was found
        if (!string.IsNullOrEmpty(cachedBasket))
        {
            
            //Deserialize
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket, _options)!;
        }

        //If the value was not found
        var basket = await _basketRepository.GetBasket(userName, asNoTracking, cancellationToken);
        
        await _cache.SetStringAsync(userName, JsonSerializer.Serialize(basket, _options), cancellationToken);
        
        return basket;
    }

    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        var newBasket = await _basketRepository.CreateBasket(basket, cancellationToken);
        
        await _cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(newBasket, _options), cancellationToken);

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