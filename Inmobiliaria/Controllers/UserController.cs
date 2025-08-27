using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;

using Inmobiliaria.Models;
using Inmobiliaria.Interfaces;
using Inmobiliaria.Models.ViewModels;

namespace Inmobiliaria.Controllers;

public class UserController(IRepository<User> Repository) : Controller
{
  private readonly IRepository<User> _userRepository = Repository;

  [HttpGet]
  public IActionResult Index(string Rol)
  {
    var users = _userRepository.FindBy(new Dictionary<string, object>() {
      { "rol", Rol },
      // { "state", 1 }
    });

    return View(users);
  }

  [HttpGet] // VER COMO MANEJAR EL TEMA DE LA MULTI INSTANCIA DE LOS CONTROLADORES
  public IActionResult GetUser(int Dni)
  {
    User? user = _userRepository.ReadOne(("dni", Dni)).Entity;

    if (user == null) return NotFound();

    return Json(user);
  }

  // Esta accion recolecta los datos de la modal y los usa para la actualizacion
  [HttpPost] /* Indica que tal metodo responde a una solicitud POST */
  [ValidateAntiForgeryToken] /* Para evitar ataques CSRF (Corss-Site Request Forgery) */
  public IActionResult EditUser(UserEditVm vm)  /* Cubre peticiones POST */
  {
    // "ModelState.IsValid" cheque que todos los campos requeridos cumplan sus respectivas condiciones
    // "BadRequest" devuelve un 404 con body 
    if (!ModelState.IsValid) return BadRequest(ModelState);

    User? user = _userRepository.ReadOne(("dni", vm.Dni)).Entity;

    // "NotFound" devuelve un 404 son body 
    if (user == null) return NotFound(new { Success = false, Message = "Usuario no encontrado" });

    Dictionary<string, object> newData = new()
    {
      { "dni", vm.Dni },
      { "name", vm.Name },
      { "last_name", vm.LastName },
      { "contact", vm.Contact },
      { "mail", vm.Mail },
      { "rol", vm.Rol }
    };
    int affectedRows = _userRepository.Update(newData);
    // Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, User = vm });
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult EditState(int Dni)
  {
    User? user = _userRepository.ReadOne(("dni", Dni)).Entity;

    if (user == null) return NotFound(new { Success = false, Message = "Usuario no encontrado" });

    Dictionary<string, object> newData = new()
    {
      { "state", !user.State },
      { "dni", Dni }
    };
    int affectedRows = _userRepository.Update(newData);
    // Console.WriteLine($"Rows affected: {affectedRows}");

    return Ok(new { Success = true, State = user.State });
  }
}