using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public interface IProductCollection
    {
        Task InsertProduct(Product product);
        Task UpdateProduct(Product product);
        Task DeleteProduct(string id);

        Task<List<Product>> GetAllProducts(int? limit, string? category, int page = 1);
        Task<List<Product>> GetUserProducts(string owner, int? limit, string? category, int page = 1);

        Task<Product> GetProductById(string id);
    }
}
