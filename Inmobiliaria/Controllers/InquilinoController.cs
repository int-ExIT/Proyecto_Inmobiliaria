using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;

using Inmobiliaria.Models;
using Inmobiliaria.Interfaces;
using Inmobiliaria.Models.ViewModels;

namespace Inmobiliaria.Controllers;

public class InquilinoController(IRepository<Inquilino> Repository) : Controller
{
  private readonly IRepository<Inquilino> _userRepository = Repository;

  [HttpGet]
  public IActionResult Index()
  {
    var elements = _userRepository.ReadAll();

    return View(elements);
  }

  [HttpGet] // VER COMO MANEJAR EL TEMA DE LA MULTI INSTANCIA DE LOS CONTROLADORES
  public IActionResult GetUser(int Dni)
  {
    var element = _userRepository.ReadOne(("dni", Dni)).Entity;

    if (element == null) return NotFound();

    return Json(new { Success = true, Body = element });
  }

  // Esta accion recolecta los datos de la modal y los usa para la actualizacion
  [HttpPost] /* Indica que tal metodo responde a una solicitud POST */
  [ValidateAntiForgeryToken] /* Para evitar ataques CSRF (Corss-Site Request Forgery) */
  public IActionResult EditUser(UsuarioEditVm vm) /* Cubre peticiones POST */
  {
    // "ModelState.IsValid" cheque que todos los campos requeridos cumplan sus respectivas condiciones
    // "BadRequest" devuelve un 404 con body 
    if (!ModelState.IsValid) return BadRequest(ModelState);

    var element = _userRepository.ReadOne(("dni", vm.Dni)).Entity;

    // "NotFound" devuelve un 404 son body 
    if (element == null) return NotFound(new { Success = false, Message = "Inquilino no encontrado." });

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

    if (element == null) return NotFound(new { Success = false, Message = "Inquilino no encontrado." });

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