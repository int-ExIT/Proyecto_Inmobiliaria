/// Singlenton thread-safe (No Lazy)
/// Se denomina 'No Lazy' porque el objeto 'MySqlConnection' se instanciara aunque no sea necesario.

using System;
using MySqlConnector;

namespace Inmobiliaria.Repositories;

class DbConnectionSinglenton
{
  private static readonly MySqlConnection _connect = new("Server=localhost;Database=testing_db;User=root;Password=123456;");

  private DbConnectionSinglenton() { }

  public static MySqlConnection ConnectionInstance => _connect;
}