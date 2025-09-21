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
  public IActionResult GetContracts(string Entity, int Dni)
  {
    Dictionary<string, object> filter = new() { { "@value", Dni } };
    string query = $"SELECT a.*, b.nombre AS nombre_propietario, c.nombre AS nombre_inquilino, b.apellido AS apellido_propietario, c.apellido AS apellido_inquilino, d.direccion AS direccion FROM contratos AS a INNER JOIN propietarios AS b ON a.id_propietario = b.id INNER JOIN inquilinos AS c ON a.id_inquilino = c.id INNER JOIN inmuebles AS d ON a.id_inmueble = d.id WHERE {(Entity == "propietario" ? "b" : "c")}.dni LIKE @value;";

    var element = _userRepository.CustomQuery(query, filter);

    if (element == null) return NotFound();

    return Json(new { Success = true, Body = element });
  }

  [HttpGet]
  public IActionResult GetTenants(string Name)
  {
    GenericRepository<Inquilino> inquilinoRepository = new();
    (string, object) filter = ("nombre", Name);

    var element = inquilinoRepository.FindLike(filter);

    if (element == null) return NotFound();

    return Json(new { Success = true, Body = element });
  }

  [HttpGet]
  public IActionResult GetOwners(string Name)
  {
    GenericRepository<Propietario> propietarioRepository = new();
    (string, object) filter = ("nombre", Name);

    var element = propietarioRepository.FindLike(filter);

    if (element == null) return NotFound();

    return Json(new { Success = true, Body = element });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditElement([FromBody] ContratoEditVm vm)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    Dictionary<string, object> filter = new() { { "@value", vm.Id } };
    string query = $"SELECT a.*, b.nombre AS nombre_propietario, c.nombre AS nombre_inquilino, b.apellido AS apellido_propietario, c.apellido AS apellido_inquilino, d.direccion AS direccion FROM contratos AS a INNER JOIN propietarios AS b ON a.id_propietario = b.id INNER JOIN inquilinos AS c ON a.id_inquilino = c.id INNER JOIN inmuebles AS d ON a.id_inmueble = d.id WHERE a.id = @value;";

    var element = _userRepository.CustomQuery(query, filter).FirstOrDefault();

    if (element is null) return NotFound(new { Success = false, Message = "Item not found." });

    if (vm.DiaDeFinalizacion != element.DiaDeFinalizacion)
    {
      string flag = CheckEndDate(vm.DiaDeFinalizacion, vm.Id, element.IdInmueble);

      if (flag != "-" && flag != vm.DiaDeFinalizacion.ToString("dd/MM/yyyy")) return Ok(new
      {
        Success = false,
        Message = $"Invalid date: {flag}"
      });
    }

    Dictionary<string, object> newData = new()
    {
      {"id", vm.Id},
      {"dia_de_finalizacion", vm.DiaDeFinalizacion},
      {"precio_mensual", vm.PrecioMensual},
      {"dni_usuario_cierre", vm.DniUsuarioCierre},
    };
    int affectedRows = _userRepository.Update(newData);
    Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, Body = vm });
  }

  private string CheckEndDate(DateTime NuevaFecha, int IdContrato, int IdInmueble)
  {
    // Valores de Prueba:
    // "2025/11/01" Ok.
    // "2026/04/01" Ok.
    // "2026/04/02" Error.
    // "2026/09/01" Error.
    Dictionary<string, object> filter = new()
    {
      { "@value",  IdInmueble},
      { "@value1",  IdContrato},
      { "@value2",  NuevaFecha.ToString("yyyy/MM/dd")}
    };
    string query = "SELECT a.*, b.nombre AS nombre_propietario, c.nombre AS nombre_inquilino, b.apellido AS apellido_propietario, c.apellido AS apellido_inquilino, d.direccion AS direccion FROM contratos AS a INNER JOIN propietarios AS b ON a.id_propietario = b.id INNER JOIN inquilinos AS c ON a.id_inquilino = c.id INNER JOIN inmuebles AS d ON a.id_inmueble = d.id WHERE a.estado NOT IN ('finalizado', 'cancelado') AND a.id_inmueble = @value AND a.id != @value1 AND a.dia_de_inicio <= @value2 ORDER BY a.dia_de_inicio ASC LIMIT 1;";

    var element = _userRepository.CustomQuery(query, filter).FirstOrDefault();

    if (element is null) return "-";

    return element.DiaDeInicio.ToString("dd/MM/yyyy");
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