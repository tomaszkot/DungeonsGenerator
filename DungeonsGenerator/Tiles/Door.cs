using Dungeons.Core;
using System;

namespace Dungeons.Tiles
{
  public class Door : Tile
  {
    public Door(Point point) : base(point, Constants.SymbolDoor)
    {
      color = ConsoleColor.Yellow;
    }

    public Door() : this(Point.Invalid)
    {

    }
  }
}
