using Dungeons.Core;
using System;

namespace Dungeons.Tiles
{
  public class Door : Tile
  {
    public const char SymbolDoor = '+';

    public Door(Point point) : base(point, SymbolDoor)
    {
      color = ConsoleColor.Yellow;
    }

    public Door() : this(Point.Invalid)
    {

    }
  }
}
