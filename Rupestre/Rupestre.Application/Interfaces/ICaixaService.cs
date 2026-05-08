using Rupestre.Application.DTOs;

namespace Rupestre.Application.Interfaces;

public interface ICaixaService
{
    Task<CaixaDto?> GetByIdAsync(int id);
    Task<CaixaDto?> GetCaixaAbertoAsync();
    Task<int> AbrirCaixaAsync(decimal valorAbertura);
    Task FecharCaixaAsync(int id);
}
