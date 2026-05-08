using Microsoft.AspNetCore.Mvc;
using Rupestre.Application.Interfaces;

namespace Rupestre.Web.Controllers;

public class CaixaController : Controller
{
    private readonly ICaixaService _service;

    public CaixaController(ICaixaService service) => _service = service;

    public async Task<IActionResult> Index()
    {
        var caixa = await _service.GetCaixaAbertoAsync();
        return View(caixa);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Abrir(decimal valorAbertura)
    {
        try
        {
            await _service.AbrirCaixaAsync(valorAbertura);
            TempData["Sucesso"] = "Caixa aberto com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Fechar(int id)
    {
        try
        {
            await _service.FecharCaixaAsync(id);
            TempData["Sucesso"] = "Caixa fechado com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
