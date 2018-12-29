
namespace Dungeons.Core
{
  public struct Point
  {
    const int InvalidCoord = -1;
    public int x;
    public int y;
    static Point invalid;

    static Point()
    {
      var point = new Point();
      point.x = InvalidCoord;
      point.y = InvalidCoord;
      invalid = point;
    }

    public static Point Invalid
    {
      get
      {
        return invalid;
      }
    }

    public bool IsValid { get { return x != Invalid.x && y != Invalid.y; }}

    public Point(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public Point(Point other)
    {
      this.x = other.x;
      this.y = other.y;
    }

    public override string ToString()
    {
      return "["+x+ ","+ y +"]";
    }

    public override bool Equals(object obj)
    {
      Point p = (Point)obj;
      return this == p ;
    }

    static public bool operator==(Point p1, Point p2)
    {
      return p1.x == p2.x && p1.y == p2.y;
    }

    static public bool operator !=(Point p1, Point p2)
    {
      return !(p1 == p2);
    }


    //TODO use GUID member and return here mGetHashCode.GetHashCode()
    public override int GetHashCode()
    {
      unchecked // Overflow is fine, just wrap
      {
        int hash = 17;
        // Suitable nullity checks etc, of course :)
        hash = hash * 23 + x.GetHashCode();
        hash = hash * 23 + y.GetHashCode();
        return hash;
      }
    }
  }
}
