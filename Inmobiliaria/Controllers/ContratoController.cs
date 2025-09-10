using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;

using Inmobiliaria.Models;
using Inmobiliaria.Interfaces;
using Inmobiliaria.Models.ViewModels;
using Inmobiliaria.Repositories;

namespace Inmobiliaria.Controllers;

public class ContratoController(IRepository<Contrato> Repository) : Controller
{
  private readonly IRepository<Contrato> _userRepository = Repository;

  [HttpGet]
  public IActionResult Index()
  {
    return View();
  }

  [HttpGet]
  public IActionResult GetContracts(string Entity, string? Nombre, int? Dni)
  {
    object value = (Nombre is not null ? Nombre : Dni)!;
    string query = $"SELECT a.id, a.id_propietario, id_inquilino, id_inmueble, dni_propietario_snapshot, dni_inquilino_snapshot, b.nombre AS nombre_propietario, c.nombre AS nombre_inquilino, b.apellido AS apellido_propietario, c.apellido AS apellido_inquilino, d.direccion AS direccion, coordenadas_snapshot, dia_de_inicio, dia_de_finalizacion, dia_de_cierre, precio_mensual, dni_usuario_apertura, dni_usuario_cierre, a.estado FROM contratos AS a INNER JOIN propietarios AS b ON a.id_propietario = b.id INNER JOIN inquilinos AS c ON a.id_inquilino = c.id INNER JOIN inmuebles AS d ON a.id_inmueble = d.id WHERE {(Entity == "propietario" ? "b" : "c")}.{(Nombre is not null ? "nombre" : "dni")} LIKE @value;";

    var element = _userRepository.CustomQuery(query, value);

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