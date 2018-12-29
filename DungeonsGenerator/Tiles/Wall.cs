using Dungeons.Core;

namespace Dungeons.Tiles
{
  public interface IObstacleTile
  {

  }

  public class Wall : Tile, IObstacleTile
  {
    public const char SymbolWall = '#';
    public bool IsSide { get; set; }

    public Wall(Point point) : base(point, SymbolWall)
    {
    }

    public Wall() : base(SymbolWall) { }

    public override string ToString()
    {
      return base.ToString() + " DNI: " + dungeonNodeIndex;
    }
  }
}
