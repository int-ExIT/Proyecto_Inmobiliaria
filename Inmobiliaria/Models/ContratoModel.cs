using System;
using System.ComponentModel.DataAnnotations.Schema;
using Inmobiliaria.Utils;

namespace Inmobiliaria.Models;

[Table("contratos")] public class Contrato(
  Guid Id,
  Guid IdPropietario,
  Guid IdInquilino,
  Guid IdInmueble,
  int DniPropietario,
  int DniInquilino,
  string NombrePropietario,
  string NombreInquilino,
  string ApellidoPropietario,
  string ApellidoInquilino,
  string Direccion,
  decimal Coordenadas,
  DateTime DiaDeInicio,
  DateTime DiaDeFinalizacion,
  DateTime DiaDeCierre,
  decimal PrecioMensual,
  int DniUsuarioApertura,
  int DniUsuarioCierre,
  string Estado)
{
  [Col("id", true)] public Guid Id { get; set; } = Id;
  [Col("id_propietario")] public Guid IdPropietario { get; set; } = IdPropietario;
  [Col("id_inquilino")] public Guid IdInquilino { get; set; } = IdInquilino;
  [Col("id_inmueble")] public Guid IdInmueble { get; set; } = IdInmueble;
  [Col("dni_propietario_snapshot")] public int DniPropietario { get; set; } = DniPropietario;
  [Col("dni_inquilino_snapshot")] public int DniInquilino { get; set; } = DniInquilino;
  [Col("nombre_propietario")] public string NombrePropietario { get; set; } = NombrePropietario;
  [Col("nombre_inquilino")] public string NombreInquilino { get; set; } = NombreInquilino;
  [Col("apellido_propietario")] public string ApellidoPropietario { get; set; } = ApellidoPropietario;
  [Col("apellido_inquilino")] public string ApellidoInquilino { get; set; } = ApellidoInquilino;
  [Col("direccion")] public string Direccion { get; set; } = Direccion;
  [Col("coordenadas_snapshot")] public decimal Coordenadas { get; set; } = Coordenadas;
  [Col("dia_de_inicio")] public DateTime DiaDeInicio { get; set; } = DiaDeInicio;
  [Col("dia_de_finalizacion")] public DateTime DiaDeFinalizacion { get; set; } = DiaDeFinalizacion;
  [Col("dia_de_cierre")] public DateTime DiaDeCierre { get; set; } = DiaDeCierre;
  [Col("precio_mensual")] public decimal PrecioMensual { get; set; } = PrecioMensual;
  [Col("dni_usuario_apertura")] public int DniUsuarioApertura { get; set; } = DniUsuarioApertura;
  [Col("dni_usuario_cierre")] public int DniUsuarioCierre { get; set; } = DniUsuarioCierre;
  [Col("estado")] public string Estado { get; set; } = Estado;
}