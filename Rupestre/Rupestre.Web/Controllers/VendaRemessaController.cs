using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Web.Models.DataTables;

namespace Rupestre.Web.Controllers;

[Authorize(Roles = "Gerente")]
public class VendaRemessaController : Controller
{
    private readonly IVendaRemessaService _service;
    private readonly ICaixaService _caixaService;

    public VendaRemessaController(IVendaRemessaService service, ICaixaService caixaService)
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
        var result = await _service.GetPagedAsync(request.Start, request.Length,
            request.GetOrderColumn("id"), request.OrderDir);

        return Json(new DataTablesResponse<VendaRemessaDto>
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
        if (id == 0 && await _caixaService.GetCaixaAbertoAsync() is null)
            return Json(new { success = false, message = "Não há caixa aberto. Abra o caixa antes de lançar uma remessa." });

        var dto = id == 0
            ? new VendaRemessaDto()
            : await _service.GetByIdAsync(id) ?? new VendaRemessaDto();

        return PartialView("_Form", dto);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromForm] VendaRemessaDto dto)
    {
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
