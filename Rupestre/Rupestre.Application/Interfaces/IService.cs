namespace Rupestre.Application.Interfaces;

public interface IService<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    Task<TDto?> GetByIdAsync(int id);
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<int> CreateAsync(TDto dto);
    Task UpdateAsync(TDto dto);
    Task DeleteAsync(int id);
}
