using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rupestre.Infrastructure.Identity;

namespace Rupestre.Web.Controllers;

[AllowAnonymous]
public class LoginController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(string usuario, string senha)
    {
        var result = await _signInManager.PasswordSignInAsync(
            userName: usuario,
            password: senha,
            isPersistent: true,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            ViewBag.Erro = "Usuário ou senha incorretos.";
            return View();
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sair()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index");
    }
}
