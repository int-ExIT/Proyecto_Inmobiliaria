using System;
using System.ComponentModel.DataAnnotations.Schema;
using Inmobiliaria.Utils;

namespace Inmobiliaria.Models;

[Table("usuarios")] public class Usuario(int Id, int Dni, string Nombre, string Apellido, long Contacto, string Mail, string Rol, bool Estado)
{
  [Col("id", true)] public int Id { get; set; } = Id;
  [Col("dni", true, 1)] public int Dni { get; set; } = Dni;
  [Col("nombre")] public string? Nombre { get; set; } = Nombre;
  [Col("apellido")] public string? Apellido { get; set; } = Apellido;
  [Col("contacto", true, 2)] public long Contacto { get; set; } = Contacto;
  [Col("mail", true, 3)] public string? Mail { get; set; } = Mail;
  [Col("rol")] public string? Rol { get; set; } = Rol;
  [Col("estado")] public bool Estado { get; set; } = Estado;
}