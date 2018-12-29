using Dungeons.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeons.Core
{
  public class EntityGenerationConstraints
  {
    public List<Tile> Tiles;

    Point min = Point.Invalid;
    Point max = Point.Invalid;

    public Point Min
    {
      get
      {
        return min;
      }

      set
      {
        min = value;
      }
    }

    public Point Max
    {
      get
      {
        return max;
      }

      set
      {
        max = value;
      }
    }

    public EntityGenerationConstraints(Point min, Point max)
    {
      this.Min = min;
      this.Max = max;
    }

    public EntityGenerationConstraints(Point min)
    {
      this.Min = min;
    }
    public EntityGenerationConstraints() { }

    public bool IsInside(Point point)
    {
      var minXOK = Min.x < 0 || point.x > Min.x;
      var minYOK = Min.y < 0 || point.y > Min.y;
      var maxXOK = Max.x < 0 || point.x < Max.x;
      var maxYOK = Max.y < 0 || point.y < Max.y;
      return minXOK && minYOK && maxXOK && maxYOK;
    }
  }
}
