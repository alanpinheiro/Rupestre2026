using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Web.Models.DataTables;

namespace Rupestre.Web.Controllers;

[Authorize(Roles = "Gerente")]
public class ProdutoController : Controller
{
    private readonly IProdutoService _service;
    private readonly IFabricanteService _fabricanteService;

    public ProdutoController(IProdutoService service, IFabricanteService fabricanteService)
    {
        _service = service;
        _fabricanteService = fabricanteService;
    }

    public IActionResult Index() => View();

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> GetData([FromForm] DataTablesRequest request)
    {
        var result = await _service.GetPagedAsync(request.Start, request.Length, request.SearchValue,
            request.GetOrderColumn("nome"), request.OrderDir);

        return Json(new DataTablesResponse<ProdutoDto>
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
        var fabricantes = await _fabricanteService.GetAllAsync();
        ViewBag.Fabricantes = new SelectList(fabricantes, "Id", "Nome");
        var dto = id == 0 ? new ProdutoDto() : await _service.GetByIdAsync(id) ?? new ProdutoDto();
        return PartialView("_Form", dto);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromForm] ProdutoDto dto)
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
