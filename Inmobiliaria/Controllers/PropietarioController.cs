using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;

using Inmobiliaria.Models;
using Inmobiliaria.Interfaces;
using Inmobiliaria.Models.ViewModels;

namespace Inmobiliaria.Controllers;

public class PropietarioController(IRepository<Propietario> Repository) : Controller
{
  private readonly IRepository<Propietario> _userRepository = Repository;

  [HttpGet]
  public IActionResult Index()
  {
    var elements = _userRepository.ReadAll();

    return View(elements);
  }

  [HttpGet]
  public IActionResult GetUser(int Dni)
  {
    var element = _userRepository.ReadOne(("dni", Dni)).Entity;

    if (element == null) return NotFound();

    return Json(new { Success = true, Body = element });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditUser(UsuarioEditVm vm)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var element = _userRepository.ReadOne(("dni", vm.Dni)).Entity;

    if (element == null) return NotFound(new { Success = false, Message = "Propietario no encontrado." });

    Dictionary<string, object> newData = new()
    {
      { "dni", vm.Dni },
      { "nombre", vm.Nombre },
      { "apellido", vm.Apellido },
      { "contacto", vm.Contacto },
      { "mail", vm.Mail },
    };
    int affectedRows = _userRepository.Update(newData);
    Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, Body = vm });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditState(int Dni)
  {
    var element = _userRepository.ReadOne(("dni", Dni)).Entity;

    if (element == null) return NotFound(new { Success = false, Message = "Propietario no encontrado" });

    Dictionary<string, object> newData = new()
    {
      { "estado", !element.Estado },
      { "dni", Dni }
    };
    int affectedRows = _userRepository.Update(newData);
    Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, State = element.Estado });
  }
}