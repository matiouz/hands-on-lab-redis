using System.Text.Json;

public interface IProductCacheService
{
    Task<Product?> GetProductAsync(string id);
    Task SetProductAsync(Product product);
    Task<IEnumerable<Product>?> GetProductsAsync();
    Task SetProductsAsync(IEnumerable<Product> products);
}

public class ProductCacheService : IProductCacheService
{
    private readonly string prefix = "products";

    private string ProductsKey()
    {
        return prefix;
    }

    private string ProductKey(string productId)
    {
        return $"{prefix}:{productId}";
    }

    public async Task<Product?> GetProductAsync(string id)
    {
        var json = await RedisService.Get(ProductKey(id));

        if (json == null) {
            return null;
        }

        return JsonSerializer.Deserialize<Product>(json);
    }

    public async Task SetProductAsync(Product product)
    {
        var json = JsonSerializer.Serialize<Product>(product);
        await RedisService.Set(ProductKey(product.Id), json);
    }

    public async Task<IEnumerable<Product>?> GetProductsAsync()
    {
        // TODO: allow disabling cache with CACHE_DISABLE=1
        var json = await RedisService.Get(ProductsKey());

        if (json == null) {
            return null;
        }

        return JsonSerializer.Deserialize<List<Product>>(json);
    }

    public async Task SetProductsAsync(IEnumerable<Product> products)
    {
        // TODO: allow disabling cache with CACHE_DISABLE=1
        var json = JsonSerializer.Serialize<IEnumerable<Product>>(products);
        await RedisService.Set(ProductsKey(), json);
    }
}