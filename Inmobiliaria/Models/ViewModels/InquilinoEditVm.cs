using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.ViewModels;

public class InquilinoEditVm
{
  [Required] public int Dni { get; set; }
  [Required, StringLength(100)] public string Nombre { get; set; } = "";
  [Required, StringLength(50)] public string Apellido { get; set; } = "";
  [Required] public long Contacto { get; set; }
  [Required, EmailAddress] public string Mail { get; set; } = "";
}