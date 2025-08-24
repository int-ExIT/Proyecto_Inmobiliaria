using System;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Inmobiliaria.Interfaces;

namespace Inmobiliaria.Controllers;

public class UserController(IRepository<User> Repository) : Controller
{
  private readonly IRepository<User> _userRepository = Repository;

  public IActionResult Index(string rol)
  {
    Dictionary<string, object> Filter = new() { { "rol", rol } };
    var Models = _userRepository.FindBy(Filter);
    return View(Models);
  }
}