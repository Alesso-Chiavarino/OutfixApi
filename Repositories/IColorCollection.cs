using OutfixApi.Models;

namespace OutfixApi.Repositories
{
    public interface IColorCollection
    {
        Task AddColor(Color color);
        Task UpdateColor(Color color);
        Task DeleteColor(string id);
        Task<List<Color>> GetAllColors();
        Task<Color> GetColorById(string id);
    }
}