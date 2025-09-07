using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.ViewModels;

public class InmuebleEditVm
{
  [Required] public decimal Coordenadas { get; set; }
  [Required] public string Direccion { get; set; } = "";
  [Required] public string TipoDeUso { get; set; } = ""; // Es un enum
  [Required] public string Tipo { get; set; } = ""; // Es un enum
  [Required] public int NumeroDeCuartos { get; set; }
  [Required] public decimal Precio { get; set; }
}