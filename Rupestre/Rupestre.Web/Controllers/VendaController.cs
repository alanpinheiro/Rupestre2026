using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Web.Models.DataTables;

namespace Rupestre.Web.Controllers;

public class VendaController : Controller
{
    private readonly IVendaService _service;
    private readonly ICaixaService _caixaService;
    private readonly IClienteService _clienteService;
    private readonly IVendedorService _vendedorService;
    private readonly IProdutoService _produtoService;
    private readonly IFormaPagamentoService _formaPagamentoService;

    public VendaController(IVendaService service, ICaixaService caixaService,
        IClienteService clienteService, IVendedorService vendedorService,
        IProdutoService produtoService, IFormaPagamentoService formaPagamentoService)
    {
        _service = service;
        _caixaService = caixaService;
        _clienteService = clienteService;
        _vendedorService = vendedorService;
        _produtoService = produtoService;
        _formaPagamentoService = formaPagamentoService;
    }

    public IActionResult Index() => View();

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> GetData([FromForm] DataTablesRequest request)
    {
        var result = await _service.GetPagedAsync(request.Start, request.Length, request.SearchValue,
            request.GetOrderColumn("dataVenda"), request.OrderDir);

        return Json(new DataTablesResponse<VendaDto>
        {
            Draw = request.Draw,
            RecordsTotal = result.TotalRecords,
            RecordsFiltered = result.FilteredRecords,
            Data = result.Data
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var caixa = await _caixaService.GetCaixaAbertoAsync();
        if (caixa is null)
        {
            TempData["Erro"] = "Não há caixa aberto. Abra o caixa antes de registrar uma venda.";
            return RedirectToAction("Index", "Caixa");
        }
        await PopulateFormData();
        return View(new VendaSaveDto { DataVenda = DateTime.Today });
    }

    [HttpGet]
    [Authorize(Roles = "Gerente")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto is null) return NotFound();
        await PopulateFormData();
        return View("Create", dto);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromBody] VendaSaveDto dto)
    {
        if (dto.Id != 0 && !User.IsInRole("Gerente"))
            return Json(new { success = false, message = "Acesso negado. Apenas Gerentes podem editar vendas." });

        try
        {
            if (dto.Id == 0)
                await _service.CreateAsync(dto);
            else
                await _service.UpdateAsync(dto);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Gerente")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private async Task PopulateFormData()
    {
        var clientes = (await _clienteService.GetAllAsync()).OrderBy(c => c.Nome);
        var vendedores = (await _vendedorService.GetAllAsync()).OrderBy(v => v.Nome);
        var produtos = (await _produtoService.GetAllAsync()).OrderBy(p => p.Nome);
        var formas = (await _formaPagamentoService.GetAtivosAsync()).OrderBy(f => f.Nome);

        ViewBag.Clientes = clientes.Select(c => new SelectListItem(c.Nome, c.Id.ToString()));
        ViewBag.Vendedores = vendedores.Select(v => new SelectListItem(v.Nome, v.Id.ToString()));

        ViewBag.ProdutosData = produtos
            .Select(p => new { p.Id, p.Nome, p.PrecoVenda, p.Estoque })
            .ToList();

        ViewBag.FormasPagamentoData = formas
            .Select(f => new
            {
                f.Id, f.Nome,
                Fatores = new[] { f.Fator1, f.Fator2, f.Fator3, f.Fator4, f.Fator5, f.Fator6,
                                  f.Fator7, f.Fator8, f.Fator9, f.Fator10, f.Fator11, f.Fator12 }
            })
            .ToList();
    }
}
