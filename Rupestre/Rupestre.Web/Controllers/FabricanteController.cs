using Microsoft.AspNetCore.Mvc;
using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Web.Models.DataTables;

namespace Rupestre.Web.Controllers;

public class FabricanteController : Controller
{
    private readonly IFabricanteService _service;

    public FabricanteController(IFabricanteService service) => _service = service;

    public IActionResult Index() => View();

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> GetData([FromForm] DataTablesRequest request)
    {
        var result = await _service.GetPagedAsync(request.Start, request.Length, request.SearchValue,
            request.GetOrderColumn("nome"), request.OrderDir);

        return Json(new DataTablesResponse<FabricanteDto>
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
        var dto = id == 0 ? new FabricanteDto() : await _service.GetByIdAsync(id) ?? new FabricanteDto();
        return PartialView("_Form", dto);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromForm] FabricanteDto dto)
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
