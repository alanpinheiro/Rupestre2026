using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rupestre.Infrastructure.Identity;
using Rupestre.Web.ViewModels;

namespace Rupestre.Web.Controllers;

[Authorize(Roles = "Gerente")]
public class UsuarioController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole>   _roleManager;

    public UsuarioController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.OrderBy(u => u.UserName).ToList();
        var list  = new List<UsuarioViewModel>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            list.Add(new UsuarioViewModel
            {
                Id           = u.Id,
                UserName     = u.UserName!,
                NomeCompleto = u.NomeCompleto,
                Perfil       = roles.FirstOrDefault() ?? "-"
            });
        }

        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Form(string? id = null)
    {
        if (id is null)
            return PartialView("_Form", new UsuarioViewModel());

        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return PartialView("_Form", new UsuarioViewModel
        {
            Id           = user.Id,
            UserName     = user.UserName!,
            NomeCompleto = user.NomeCompleto,
            Perfil       = roles.FirstOrDefault() ?? "Vendedor"
        });
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromForm] UsuarioViewModel vm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(vm.Id))
            {
                if (string.IsNullOrWhiteSpace(vm.Senha))
                    return Json(new { success = false, message = "Senha é obrigatória para novos usuários." });

                var user = new ApplicationUser
                {
                    UserName       = vm.UserName,
                    Email          = $"{vm.UserName}@rupestre.local",
                    NomeCompleto   = vm.NomeCompleto,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, vm.Senha);
                if (!result.Succeeded)
                    return Json(new { success = false, message = string.Join("; ", result.Errors.Select(e => e.Description)) });

                await _userManager.AddToRoleAsync(user, vm.Perfil);
            }
            else
            {
                var user = await _userManager.FindByIdAsync(vm.Id);
                if (user is null)
                    return Json(new { success = false, message = "Usuário não encontrado." });

                user.NomeCompleto = vm.NomeCompleto;
                await _userManager.UpdateAsync(user);

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, vm.Perfil);

                if (!string.IsNullOrWhiteSpace(vm.Senha))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, token, vm.Senha);
                    if (!resetResult.Succeeded)
                        return Json(new { success = false, message = string.Join("; ", resetResult.Errors.Select(e => e.Description)) });
                }
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            if (id == _userManager.GetUserId(User))
                return Json(new { success = false, message = "Não é possível excluir o próprio usuário." });

            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return Json(new { success = false, message = "Usuário não encontrado." });

            await _userManager.DeleteAsync(user);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
