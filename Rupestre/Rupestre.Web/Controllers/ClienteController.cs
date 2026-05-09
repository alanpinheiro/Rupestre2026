using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Web.Models.DataTables;

namespace Rupestre.Web.Controllers;

[Authorize(Roles = "Gerente")]
public class ClienteController : Controller
{
    private readonly IClienteService _service;

    public ClienteController(IClienteService service) => _service = service;

    public IActionResult Index() => View();

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> GetData([FromForm] DataTablesRequest request)
    {
        var result = await _service.GetPagedAsync(request.Start, request.Length, request.SearchValue,
            request.GetOrderColumn("nome"), request.OrderDir);

        return Json(new DataTablesResponse<ClienteDto>
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
        var dto = id == 0 ? new ClienteDto() : await _service.GetByIdAsync(id) ?? new ClienteDto();
        return PartialView("_Form", dto);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromForm] ClienteDto dto)
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
