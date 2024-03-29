using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public interface ICategoryCollection
    {
        Task InsertCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(string id);

        Task<List<Category>> GetAllCategories();

        Task<Category> GetCategoryById(string id);
    }
}
