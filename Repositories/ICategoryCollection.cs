using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public interface ICategoryCollection
    {
        Task<List<Category>> GetCategories();
        Task<Category> GetCategoryById(string id);
        Task<Category> CreateCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task<Category> DeleteCategory(string id);
    }
}
