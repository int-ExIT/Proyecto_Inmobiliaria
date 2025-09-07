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
    Console.WriteLine("Ejecutando el endpoint...");
    if (!ModelState.IsValid) return BadRequest(ModelState);
    Console.WriteLine("This is coordenadas -> " + vm.Coordenadas);

    var element = _userRepository.ReadOne(("coordenadas", vm.Coordenadas)).Entity;
    Console.WriteLine("This is element -> " + element?.Coordenadas ?? "--N/A--");

    if (element == null) return NotFound(new { Success = false, Message = "Item not found." });
    Console.WriteLine("Entrando al mapa...");

    Dictionary<string, object> newData = new()
    {
      { "coordenadas", vm.Coordenadas },
      { "direccion", vm.Direccion },
      { "tipo_de_uso", vm.TipoDeUso },
      { "tipo", vm.Tipo },
      { "numero_de_cuartos", vm.NumeroDeCuartos },
      { "precio", vm.Precio }
    };
    Console.WriteLine("Saliendo del mapa...");
    Console.WriteLine("Entrando al update...");
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