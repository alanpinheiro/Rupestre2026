using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface ISuprimentoRepository : IRepository<Suprimento>
{
    Task<IEnumerable<Suprimento>> GetByCaixaAsync(int caixaId);
}
