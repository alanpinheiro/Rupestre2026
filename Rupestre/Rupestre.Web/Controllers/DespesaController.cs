using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Web.Models.DataTables;

namespace Rupestre.Web.Controllers;

public class DespesaController : Controller
{
    private readonly IDespesaService _service;
    private readonly ICaixaService _caixaService;

    public DespesaController(IDespesaService service, ICaixaService caixaService)
    {
        _service = service;
        _caixaService = caixaService;
    }

    public async Task<IActionResult> Index()
    {
        var caixa = await _caixaService.GetCaixaAbertoAsync();
        ViewBag.CaixaAberto = caixa is not null;
        return View();
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> GetData([FromForm] DataTablesRequest request)
    {
        var result = await _service.GetPagedAsync(request.Start, request.Length, request.SearchValue,
            request.GetOrderColumn("dataDespesa"), request.OrderDir);

        return Json(new DataTablesResponse<DespesaDto>
        {
            Draw = request.Draw,
            RecordsTotal = result.TotalRecords,
            RecordsFiltered = result.FilteredRecords,
            Data = result.Data
        });
    }

    [HttpGet]
    public async Task<IActionResult> Form(int id = 0)
    {
        if (id != 0 && !User.IsInRole("Gerente"))
            return Json(new { success = false, message = "Acesso negado. Apenas Gerentes podem editar despesas." });

        if (id == 0 && await _caixaService.GetCaixaAbertoAsync() is null)
            return Json(new { success = false, message = "Não há caixa aberto. Abra o caixa antes de lançar uma despesa." });

        var dto = id == 0
            ? new DespesaDto { DataDespesa = DateTime.Today }
            : await _service.GetByIdAsync(id) ?? new DespesaDto { DataDespesa = DateTime.Today };

        return PartialView("_Form", dto);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromForm] DespesaDto dto)
    {
        if (dto.Id != 0 && !User.IsInRole("Gerente"))
            return Json(new { success = false, message = "Acesso negado. Apenas Gerentes podem editar despesas." });

        try
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Dados inválidos." });

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
}
