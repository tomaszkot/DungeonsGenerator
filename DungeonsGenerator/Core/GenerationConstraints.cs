using System.Drawing;

namespace Dungeons.Core
{

  public class GenerationConstraints
  {
    public static Point InvalidPoint = new Point(-1, -1);

    Point min = InvalidPoint;
    Point max = InvalidPoint;

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

    public GenerationConstraints(Point min, Point max)
    {
      this.Min = min;
      this.Max = max;
    }

    public GenerationConstraints(Point min)
    {
      this.Min = min;
    }

    public GenerationConstraints() { }

    bool IsBorderUsed(int border)
    {
      return border >= 0;
    }

    public bool IsInside(Point point)
    {
      var minXOK = !IsBorderUsed(Min.X) || point.X > Min.X;
      var minYOK = !IsBorderUsed(Min.Y) || point.Y > Min.Y;
      var maxXOK = !IsBorderUsed(Max.X) || point.X < Max.X;
      var maxYOK = !IsBorderUsed(Max.Y) || point.Y < Max.Y;
      return minXOK && minYOK && maxXOK && maxYOK;
    }
  }
}
