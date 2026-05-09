namespace Rupestre.Web.ViewModels;

public class UsuarioViewModel
{
    public string?  Id           { get; set; }
    public string   UserName     { get; set; } = "";
    public string?  NomeCompleto { get; set; }
    public string   Perfil       { get; set; } = "Vendedor";
    public string?  Senha        { get; set; }
}
