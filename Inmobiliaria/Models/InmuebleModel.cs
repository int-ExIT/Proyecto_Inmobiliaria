using System;
using System.ComponentModel.DataAnnotations.Schema;
using Inmobiliaria.Utils;

namespace Inmobiliaria.Models;

[Table("inmuebles")] public class Inmueble(Guid Id, Guid IdPropietario, string Direccion, string TipoDeUso, string Tipo, int NumeroDeCuartos, decimal Coordenadas, decimal Precio, bool Estado)
{
  [Col("id", true)] public Guid Id { get; set; } = Id;
  [Col("id_propietario")] public Guid IdPropietario { get; set; } = IdPropietario;
  [Col("direccion")] public string Direccion { get; set; } = Direccion;
  [Col("tipo_de_uso")] public string TipoDeUso { get; set; } = TipoDeUso;
  [Col("tipo")] public string Tipo { get; set; } = Tipo;
  [Col("numero_de_cuartos")] public int NumeroDeCuartos { get; set; } = NumeroDeCuartos;
  [Col("coordenadas", true, 1)] public decimal Coordenadas { get; set; } = Coordenadas;
  [Col("precio")] public decimal Precio { get; set; } = Precio;
  [Col("estado")] public bool Estado { get; set; } = Estado;
}