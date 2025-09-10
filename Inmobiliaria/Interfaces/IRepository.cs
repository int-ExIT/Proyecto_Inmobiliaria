using System;

namespace Inmobiliaria.Interfaces;

public interface IRepository<T>
{
  int Create(T Element);

  (bool Status, T? Entity) ReadOne((string Key, object Value) Filter);

  IEnumerable<T> ReadAll();

  IEnumerable<T> FindBy(Dictionary<string, object> Filter);

  IEnumerable<T> FindLike((string Key, object Value) Filter);

  IEnumerable<T> CustomQuery(string Query, object Value);

  int Update(Dictionary<string, object> NewData);

  int Delete((string Key, object Value) Filter);
}