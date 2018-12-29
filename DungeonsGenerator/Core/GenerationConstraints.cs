namespace Dungeons.Core
{
  public class GenerationConstraints
  {
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
      var minXOK = !IsBorderUsed(Min.x) || point.x > Min.x;
      var minYOK = !IsBorderUsed(Min.y) || point.y > Min.y;
      var maxXOK = !IsBorderUsed(Max.x) || point.x < Max.x;
      var maxYOK = !IsBorderUsed(Max.y) || point.y < Max.y;
      return minXOK && minYOK && maxXOK && maxYOK;
    }
  }
}
