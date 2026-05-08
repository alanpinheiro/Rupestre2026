namespace Rupestre.Application.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cpf { get; set; }
    public string? Rg { get; set; }
    public string? Endereco { get; set; }
    public string? Telefone1 { get; set; }
    public string? Telefone2 { get; set; }
}
