using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeons.Core
{
  public static class Extensions
  {
    //ForEch was not compiling for Win8.1 phone :/
    public static void Each<T>(this IEnumerable<T> source, Action<T> action)
    {
      foreach (T element in source)
        action(element);
    }

    public static string FirstCharToUpper(string input)
    {
      switch (input)
      {
        case null: throw new ArgumentNullException(nameof(input));
        case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
        default: return input.First().ToString().ToUpper() + input.Substring(1);
      }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = RandHelper.Random.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }
  }
}
