using Dungeons.Core;
using System;
using System.Drawing;

namespace Dungeons.Tiles
{
  public class Door : Tile
  {
    public Door(Point point) : base(point, Constants.SymbolDoor)
    {
      Color = ConsoleColor.Yellow;
    }

    public Door() : this(GenerationConstraints.InvalidPoint)
    {

    }
  }
}
