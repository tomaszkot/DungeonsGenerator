using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dungeons.Core
{
  public static class Extensions
  {
    static Point InvalidPoint = GenerationConstraints.InvalidPoint;

    public static bool IsValid(this Point pt)
    {
      return pt != InvalidPoint;
    }

    public static Point Invalid(this Point pt)
    {
      return InvalidPoint;
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
