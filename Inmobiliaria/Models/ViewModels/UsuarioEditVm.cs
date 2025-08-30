using System;
using System.ComponentModel.DataAnnotations;

///  Los ViewModels son herramientas que proporcionan un major control a la hora 
/// de realizar modificaciones (UPDATEs) a los elementos de un base de datos.
///  En ellos se pueden establecer los atributos que si se puedan modificar y 
/// las respectivas validaciones de cada uno de estos. 
///  Recomendada para proyectos medianos/grandes. 
namespace Inmobiliaria.Models.ViewModels;

public class UsuarioEditVm
{
  [Required] public int Dni { get; set; }
  [Required, StringLength(100)] public string Nombre { get; set; } = "";
  [Required, StringLength(50)] public string Apellido { get; set; } = "";
  [Required] public long Contacto { get; set; }
  [Required, EmailAddress] public string Mail { get; set; } = "";
  [Required, StringLength(50)] public string Rol { get; set; } = "empleado";
}