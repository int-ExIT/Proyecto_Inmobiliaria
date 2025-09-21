using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.ViewModels;

public class ContratoEditVm
{
  [Required] public int Id { get; set; }
  [Required] public DateTime DiaDeFinalizacion { get; set; }
  [Required] public decimal PrecioMensual { get; set; }
  [Required] public int DniUsuarioCierre { get; set; }
}