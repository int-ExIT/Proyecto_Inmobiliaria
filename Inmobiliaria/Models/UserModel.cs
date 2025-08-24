using System;
using System.ComponentModel.DataAnnotations.Schema;
using Inmobiliaria.Utils;

namespace Inmobiliaria.Models;

[Table("users")]
public class User(Guid Id, int Dni, string Name, string LastName, long Contact, string Mail, string Rol, bool State)
{
  [Col("id", true)]
  public Guid Id { get; set; } = Id;
  [Col("dni", true, 1)]
  public int Dni { get; set; } = Dni;
  [Col("name")]
  public string? Name { get; set; } = Name;
  [Col("last_name")]
  public string? LastName { get; set; } = LastName;
  [Col("contact", true, 2)]
  public long? Contact { get; set; } = Contact;
  [Col("mail", true, 3)]
  public string? Mail { get; set; } = Mail;
  [Col("rol")]
  public string? Rol { get; set; } = Rol;
  [Col("state")]
  public bool State { get; set; } = State;
}