using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;

using Inmobiliaria.Models;
using Inmobiliaria.Interfaces;
using Inmobiliaria.Models.ViewModels;

namespace Inmobiliaria.Controllers;

public class InmuebleController(IRepository<Inmueble> Repository) : Controller
{
  private readonly IRepository<Inmueble> _userRepository = Repository;

  [HttpGet]
  public IActionResult Index()
  {
    var elements = _userRepository.ReadAll();

    return View(elements);
  }

  [HttpGet]
  public IActionResult GetElement(decimal Coordenadas)
  {
    var element = _userRepository.ReadOne(("coordenadas", Coordenadas)).Entity;

    if (element == null) return NotFound();

    return Json(new { Success = true, Body = element });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditElement(InmuebleEditVm vm)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var element = _userRepository.ReadOne(("coordenadas", vm.Coordenadas)).Entity;

    if (element == null) return NotFound(new { Success = false, Message = "Item not found." });

    Dictionary<string, object> newData = new()
    {
      { "coordenadas", vm.Coordenadas },
      { "id_propietario", vm.IdPropietario },
      { "direccion", vm.Direccion },
      { "tipo_de_uso", vm.TipoDeUso },
      { "tipo", vm.Tipo },
      { "numero_de_cuartos", vm.NumeroDeCuartos },
      { "precio", vm.Precio }
    };
    int affectedRows = _userRepository.Update(newData);
    Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, Body = vm });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditState(decimal Coordenadas)
  {
    var element = _userRepository.ReadOne(("coordenadas", Coordenadas)).Entity;

    if (element == null) return NotFound(new { Success = false, Message = "Item not found." });

    Dictionary<string, object> newData = new()
    {
      { "estado", !element.Estado },
      { "coordenadas", Coordenadas }
    };
    int affectedRows = _userRepository.Update(newData);
    Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, State = element.Estado });
  }
}