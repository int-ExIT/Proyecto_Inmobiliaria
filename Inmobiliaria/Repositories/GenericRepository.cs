using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic; // Necesario para IEnumerable<T>
using System.ComponentModel.DataAnnotations.Schema;

using MySqlConnector;

using Inmobiliaria.Helpers;
using Inmobiliaria.Interfaces;

namespace Inmobiliaria.Repositories;

public class GenericRepository<T> : IRepository<T>
{
  protected readonly string Name;
  protected readonly PropertyInfo[] PropsClass;
  protected readonly IEnumerable<string> PropsDb;
  protected readonly IEnumerable<(string Name, int Value)> PropsUnique;
  protected readonly Func<IDataRecord, T> Build;
  protected readonly MySqlConnection Connection;

  public GenericRepository()
  {
    this.Name = ClassData<T>.GetName();
    this.PropsClass = ClassData<T>.GetProperties();
    this.PropsDb = ClassData<T>.GetAttributes();
    this.PropsUnique = ClassData<T>.GetAttUnique();
    this.Build = ClassData<T>.NewBuilder();

    this.Connection = DbConnectionSinglenton.ConnectionInstance;
  }

  public virtual int Create(T Element)
  {
    try
    {
      ConnectionManager(true);
      // Crear consulta
      string attributtes = string.Join(", ", this.PropsDb);
      string values = string.Join(", ", this.PropsClass.Select(p => "@" + p.Name));
      string query = $"INSERT INTO {this.Name}({attributtes}) VALUES ({values});";
      using MySqlCommand command = new(query, this.Connection);
      foreach (var prop in this.PropsClass)
      {
        // Accedo al valor del elemento: Element.PropName
        var value = prop.GetValue(Element);
        // Este paso se implementa porque el ID en la bd se genera como un UUID
        if (value is Guid guidValue) value = guidValue.ToByteArray();
        // Reemplazo los datos para la creacion del nuevo registro
        command.Parameters.AddWithValue("@" + prop.Name, value);
      }
      // Ejecutar consulta
      int AffectedRows = command.ExecuteNonQuery();

      Console.WriteLine("Successful insertion.");

      return AffectedRows;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Failed insertion.", ex);
    }
    finally
    {
      ConnectionManager(false);
    }
  }

  public virtual (bool Status, T? Entity) ReadOne((string Key, object Value) Filter)
  {
    try
    {
      // Establecer conexion
      ConnectionManager(true);

      // Verifico que el cliente haya pasado una propiedad de tipo "unique" para garanatizar un solo resultado
      if (
        !this.PropsUnique!.Any(Prop => Prop.Name == Filter.Key)
      ) throw new DataException("Missing unique identifier for selection.");

      // Crear consulta
      string query = $"SELECT * FROM {this.Name} WHERE {Filter.Key} = @{Filter.Key}";
      using MySqlCommand command = new(query, this.Connection);
      command.Parameters.AddWithValue($"@{Filter.Key}", Filter.Value);

      // Ejecutar consulta
      using var reader = command.ExecuteReader();

      // Verificar resultados
      if (!reader.Read()) return (false, default);

      // Construir elemento
      var instance = this.Build(reader);

      Console.WriteLine("Successful selection.");

      // Retornar en formato tupla para evitar nulls
      return (true, instance); ;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Failed selection.", ex);
    }
    finally // Este bloque se ejecutara siempre INCLUSO si se ejecuta un return desde el try.
    {
      ConnectionManager(false);
    }
  }

  public virtual IEnumerable<T> ReadAll()
  {
    try
    {
      // Establecer conexion
      ConnectionManager(true);

      // Crear consulta
      string query = $"SELECT * FROM {this.Name};";
      using MySqlCommand command = new(query, this.Connection);
      // Ejecutar consulta
      using var reader = command.ExecuteReader();

      Console.WriteLine("Successful selections.");

      // Construyo de forma dinamica y retorno cada elemento que me devuelva la consulta
      return [..
        Enumerable
          .Repeat(0, 10)
          .TakeWhile(_ => reader.Read())
          .Select(_ => this.Build(reader))
      ];
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Failed selections.", ex);
    }
    finally
    {
      ConnectionManager(false);
    }
  }

  public virtual IEnumerable<T> FindBy(Dictionary<string, object> Filter)
  {
    try
    {
      if (Filter.Count == 0) throw new ArgumentException("Filter cannot be empty");

      // Establecer conexion
      ConnectionManager(true);

      // Crear consulta
      using MySqlCommand command = new() { Connection = this.Connection };
      var conditions = new List<string>();
      foreach (var kv in Filter)
      {
        string paramName = $"@{kv.Key}";
        conditions.Add($"{kv.Key} = {paramName}");

        command.Parameters.AddWithValue(paramName, kv.Value);
      }
      command.CommandText = $"SELECT * FROM {this.Name} WHERE {string.Join(" AND ", conditions)};";

      // Ejecutar consulta
      using var reader = command.ExecuteReader();

      Console.WriteLine("Successful advanced selection/s.");

      return [..
        Enumerable
          .Repeat(0, 10)
          .TakeWhile(_ => reader.Read())
          .Select(_ => this.Build(reader))
      ];
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Advanced selection/s failed.", ex);
    }
    finally
    {
      ConnectionManager(false);
    }
  }

  public virtual IEnumerable<T> FindLike((string Key, object Value) Filter)
  {
    try
    {
      // Establecer conexion
      ConnectionManager(true);

      // Crear consulta
      string query = $"SELECT * FROM {this.Name} WHERE {Filter.Key} LIKE @{Filter.Key};";
      using MySqlCommand command = new(query, this.Connection);
      command.Parameters.AddWithValue($"@{Filter.Key}", $"{Filter.Value}%");

      // Ejecutar consulta
      using var reader = command.ExecuteReader();

      Console.WriteLine("Successful advanced selection/s (like).");

      return [..
        Enumerable
          .Repeat(0, 10)
          .TakeWhile(_ => reader.Read())
          .Select(_ => this.Build(reader))
      ];
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Advanced selection/s (like) failed.", ex);
    }
    finally
    {
      ConnectionManager(false);
    }
  }

  public virtual IEnumerable<T> CustomQuery(string Query, object Value)
  {
    try
    {
      // Establecer conexion
      ConnectionManager(true);

      // Crear consulta
      using MySqlCommand command = new(Query, this.Connection);
      command.Parameters.AddWithValue("@value", $"{Value}%");
      // Ejecutar consulta
      using var reader = command.ExecuteReader();

      Console.WriteLine("Successful selections.");

      // Construyo de forma dinamica y retorno cada elemento que me devuelva la consulta
      return [..
        Enumerable
          .Repeat(0, 10)
          .TakeWhile(_ => reader.Read())
          .Select(_ => this.Build(reader))
      ];
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Failed selections.", ex);
    }
    finally
    {
      ConnectionManager(false);
    }
  }

  public virtual int Update(Dictionary<string, object> NewData)
  {
    try
    {
      // Establecer conexion
      ConnectionManager(true);

      /// Verifico que posea un atributo de tipo "unique" de prioridad 0 o 1, y de existir 
      /// lo aislo para utilizarlo como condicion para setear los datos
      (string Attr, object? Value) = this.PropsUnique!
        .Where(Prop => (Prop.Value == 0 || Prop.Value == 1) && NewData.ContainsKey(Prop.Name))
        .Select(Prop => (Prop.Name, NewData.GetValueOrDefault(Prop.Name)))
        .FirstOrDefault();
      if (Value is not null && !(Value is string v && string.IsNullOrWhiteSpace(v))) NewData.Remove(Attr);
      else throw new DataException("Missing unique identifier for update.");

      // Corroboro que el mapa no quede vacio
      if (NewData.Count == 0) throw new DataException("No data provided to update.");

      // Crear consulta
      using MySqlCommand command = new() { Connection = this.Connection };
      var elementData = new List<string>();
      foreach (var kv in NewData)
      {
        string paramName = $"@{kv.Key}";
        elementData.Add($"{kv.Key} = {paramName}");

        command.Parameters.AddWithValue(paramName, kv.Value);
      }
      command.CommandText = $"UPDATE {this.Name} SET {string.Join(", ", elementData)} WHERE {Attr} = @{Attr};";
      command.Parameters.AddWithValue($"@{Attr}", Value);

      // Ejecutar consulta
      int AffectedRows = command.ExecuteNonQuery();

      Console.WriteLine("Successful update.");

      return AffectedRows;
    }
    catch (Exception ex)
    {
      /// OJO: Esto no retorna el mensaje y concatenado la excepcion, InvalidOperationException(_, _) 
      /// recibe 2 parametros. --O al menos en este caso esta recibiendo 2.--
      throw new InvalidOperationException("Failed update.", ex);
    }
    finally
    {
      ConnectionManager(false);
    }
  }

  public virtual int Delete((string Key, object Value) Filter)
  {
    try
    {
      // Establecer conexion
      ConnectionManager(true);

      if (!this.PropsUnique!.Any(Prop =>
        (Prop.Value == 0 || Prop.Value == 1) && Prop.Name == Filter.Key
      )) throw new DataException("Missing unique identifier for deletion.");

      // Crear consulta
      var (Key, Value) = Filter;
      string query = $"DELETE FROM {this.Name} WHERE {Key} = @{Key}";
      using MySqlCommand command = new(query, this.Connection);
      command.Parameters.AddWithValue($"@{Key}", Value);

      // Ejecutar consulta
      int AffectedRows = command.ExecuteNonQuery();

      Console.WriteLine("Successful delete.");

      return AffectedRows;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Failed deletion.", ex);
    }
    finally
    {
      ConnectionManager(false);
    }
  }

  private void ConnectionManager(bool OnOff)
  {
    try
    {
      if (OnOff) this.Connection.Open();
      else this.Connection.Close();
    }
    catch (Exception ex)
    {
      this.Connection.Close();
      throw new InvalidOperationException("Connection failed.", ex);
    }
    finally
    {
      Console.WriteLine($"Connection {(OnOff ? "opened" : "closed")} successfully.");
    }
  }
}