using System;

namespace Inmobiliaria.Utils;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ColAttribute(string Name, bool IsUnique = false, int Priority = 0) : Attribute
{
  public string Name { get; } = Name;
  public bool IsUnique { get; } = IsUnique;
  public int Priority { get; } = Priority;
}