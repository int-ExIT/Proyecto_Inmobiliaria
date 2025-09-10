using System;
using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models.ViewModels;

public class ContratoEditVm
{
  [Required] public Guid Id { get; set; }
  [Required] public DateTime DiaDeFinalizacion { get; set; }
  [Required] public DateTime DiaDeCierre { get; set; }
  [Required] public decimal PrecioMensual { get; set; }
}