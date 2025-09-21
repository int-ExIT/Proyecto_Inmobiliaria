/// IMPORTANTE: 
///  El metodo a "NewBuilder()" No retorna la instancia de un elemento si no
/// un "Delegate", el cual es una referencia a un metodo, en este caso al 
/// constructor del elemento con el que se este trabajando (T). Quien sera  
/// encargara de instanciar el elemento sera el metodo "Create()" por medio 
/// de la varibale Builder, la cual cacheara el delegate construido para 
/// garantizar una unica compilacion y poder usarlo cada vez que sea 
/// necesario sin afectar el rendimiento del programa.

using System;
using System.Linq;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations.Schema;

using Inmobiliaria.Utils;

namespace Inmobiliaria.Helpers;

public class ClassData<T>
{
  ///  "Func<IDataRecord, T>" es un "delegate generico" de C# que sirve para 
  /// crear referencias a metodos, en este caso <IDataRecord, _> es lo que 
  /// necesita para parametrizar el metodo, y <_, T> es lo que retornara al 
  /// ejecutarse exitosamente. El n° de params que necesite el metodo ira en 
  /// funcion del n° de argumentos pasados a Func<>, (siendo siempre el 
  /// ultimo el valor de retorno)???
  public static Func<IDataRecord, T> NewBuilder() // Construccion del delegate
  {
    // 1. Seleccionar el ctor y obtener sus params:
    var ctor = typeof(T).GetConstructors()
      .OrderByDescending(ctor => ctor.GetParameters().Length) // Ordena los ctores Segun el n° de params que estos acepten 
      .First();
    var ctorParams = ctor.GetParameters();
    // 2. Crear Input de Entrada: Indicamos que la lambda va a recibir un elemento llamado "record" de tipo IDataRecord como param
    var paramLambda = Expression.Parameter(typeof(IDataRecord), "record");
    // 3. Se define un Arreglo de Expreciones: El n° de posiciones de este ira en funcion del n° atributos que el constructor reciba
    var argsExpr = new Expression[ctorParams.Length];

    // 4. Cargar la exprecion del constructor con sus respectivos parametros:
    for (int i = 0; i < ctorParams.Length; i++)
    {
      // 4.1. Obtiener uno de los atributos:
      var p = ctorParams[i];
      // 4.2. Corroborar que el nombre de las propiedades coincidan con los nombres de los parametros: ".FirstOrDefault()" obtiene el primer elemento que cumpla con la condicion que se establezca (=>), si no encuentra ninguno devuelve NULL
      var prop = typeof(T).GetProperties().FirstOrDefault(x =>
        string.Equals(x.Name, p.Name, StringComparison.OrdinalIgnoreCase) // "string.Equals()" se usa SOLO para ignorar mayusculas
      ) ?? throw new InvalidOperationException($"No property found for parameter [{p.Name}]"); // throw en caso de NULL
      // 4.3. Acceder record[prop.Name]:
      var columnName = prop.GetCustomAttribute<ColAttribute>()?.Name ?? prop.Name;
      var columnNameExpr = Expression.Constant(columnName);
      // 4.4. Crear la Exprecion de Acceso a la Propiedad: Esto representa hacer "record['NameCol1']"
      var indexerExpr = Expression.Property(paramLambda, "Item", columnNameExpr);

      // 4.5. Setear el tipo de la Exprecion de Acceso:
      Expression isDBNull = Expression.Equal(indexerExpr, Expression.Constant(DBNull.Value));
      Expression ifFalse = Expression.Convert(indexerExpr, p.ParameterType); //Determino el tipo de la exprecion
      Expression valueExpr = Expression.Condition(isDBNull, Expression.Default(p.ParameterType), ifFalse);
      argsExpr[i] = valueExpr;
    }

    // 5. Crear la Exprecion del Constructor: Esto equivale a "New T(record['NameCol1'], ..., record['NameColN'])"
    var newExpr = Expression.New(ctor, argsExpr);
    // 6. Armar el "Metodo Anonimo" completo: Esto equivale a "T Method(IDataRecord record) => new T(record['NameCol1'], ..., record['NameColN']);"
    var lambda = Expression.Lambda<Func<IDataRecord, T>>(newExpr, paramLambda);

    // 7. Retornar el ejecutable del constructor:
    return lambda.Compile();
  }

  public static string GetName()
  {
    return typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;
  }

  public static PropertyInfo[] GetProperties()
  {
    return typeof(T).GetProperties();
  }

  public static IEnumerable<string> GetAttributes()
  {
    return typeof(T).GetProperties()
      .Select(Prop => Prop.GetCustomAttribute<ColAttribute>()!.Name);
  }

  public static IEnumerable<(string Name, int Value)> GetAttUnique()
  {
    return typeof(T).GetProperties()
      .Select(Prop => Prop.GetCustomAttribute<ColAttribute>()!)
      .Where(Prop => Prop.IsUnique)
      .Select(Prop => (Prop.Name, Prop.Priority));
  }
}