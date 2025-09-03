using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;

using Inmobiliaria.Models;
using Inmobiliaria.Interfaces;
using Inmobiliaria.Models.ViewModels;

namespace Inmobiliaria.Controllers;

public class ContratoController(IRepository<Contrato> Repository) : Controller
{
  private readonly IRepository<Contrato> _userRepository = Repository;

  [HttpGet]
  public IActionResult Index()
  {
    var elements = _userRepository.ReadAll();

    return View(elements);
  }

  [HttpGet]
  public IActionResult GetElement(Guid Id)
  {
    var element = _userRepository.ReadOne(("id", Id)).Entity;

    if (element == null) return NotFound();

    return Json(new { Success = true, Body = element });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditElement(ContratoEditVm vm)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var element = _userRepository.ReadOne(("id", vm.Id)).Entity;

    if (element == null) return NotFound(new { Success = false, Message = "Item not found." });

    Dictionary<string, object> newData = new()
    {
      { "dia_de_finalizacion", vm.DiaDeFinalizacion },
      { "precio_mensual", vm.PrecioMensual },
    };
    int affectedRows = _userRepository.Update(newData);
    Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, Body = vm });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditState(Guid Id, int DniUsuarioCierre, string Estado)
  {
    var element = _userRepository.ReadOne(("id", Id)).Entity;

    if (element == null) return NotFound(new { Success = false, Message = "Item not found." });

    Dictionary<string, object> newData = new()
    {
      { "dni_usuario_cierre", DniUsuarioCierre },
      { "estado", Estado },
      { "id", Id }
    };
    int affectedRows = _userRepository.Update(newData);
    Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, State = element.Estado });
  }
}