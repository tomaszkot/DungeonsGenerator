using Dungeons.Core;
using System.Drawing;

namespace Dungeons.Tiles
{
  public class Wall : Tile, IObstacle
  {
    
    public bool IsSide { get; set; }

    public Wall(Point point) : base(point, Constants.SymbolWall)
    {
    }

    public Wall() : this(new Point().Invalid()) { }


  }
}
