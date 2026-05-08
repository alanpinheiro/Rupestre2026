using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface ICaixaRepository : IRepository<Caixa>
{
    Task<Caixa?> GetCaixaAbertoAsync();
}
