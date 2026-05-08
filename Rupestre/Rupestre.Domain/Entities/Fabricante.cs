namespace Rupestre.Domain.Entities;

public class Fabricante
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Deletado { get; set; }
}
