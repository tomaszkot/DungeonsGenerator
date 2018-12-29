﻿using Dungeons.Core;

namespace Dungeons.Tiles
{
  public interface IObstacleTile
  {

  }

  public class Wall : Tile, IObstacleTile
  {
    
    public bool IsSide { get; set; }

    public Wall(Point point) : base(point, Constants.SymbolWall)
    {
    }

    public Wall() : this(Point.Invalid) { }


  }
}
