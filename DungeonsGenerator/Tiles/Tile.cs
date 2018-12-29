using Dungeons.Core;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace Dungeons.Tiles
{
  [XmlInclude(typeof(Wall))]
  [Serializable]
  public class Tile
  {
    public const char SymbolBackground = '.';

    //members public for speed purposes
    public Point point;
    private char symbol = SymbolBackground;
    public string name;
    public ConsoleColor color = ConsoleColor.White;
    public int dungeonNodeIndex = -1;
    public TileCorner? corner;

    bool revealed;
    public bool Revealed
    {
      get { return revealed; }
      set
      {
        revealed = value;
      }
    }
    public int ToursSinceRevealed { get; set; }
    

    public bool IsAtValidPoint
    {
      get { return point.IsValid; }
    }

    public bool IsEmpty { get { return Symbol == SymbolBackground; } }

    public Tile() : this(SymbolBackground)
    {
    }

    public Tile(char symbol) : this(Point.Invalid, symbol)
    {

    }

    public Tile(Point point, char symbol)
    {
      this.Name = GetType().Name;
      this.point = point;
      this.Symbol = symbol;
      this.revealed = false;
    }

    public string Name
    {
      get
      {
        return name;
      }
      set
      {
        name = value.Trim();//call GetCapitalized(value) ?;
      }
    }

    private static string GetCapitalized(string val)
    {
      var result = "";
      if (val.Any())
      {
        var parts = val.Split(' ');
        foreach (var part in parts.Where(i => i.Any()))
        {
          var resPart = part.First().ToString().ToUpper() + part.Substring(1);
          result += resPart + " ";
        }
      }

      return result;
    }

    public bool IsFromChildIsland
    {
      get { return dungeonNodeIndex < 0; }
    }

    public float RevealPercent { get; set; }

    public virtual char Symbol
    {
      get
      {
        return symbol;
      }

      set
      {
        symbol = value;
      }
    }

    public bool IsSamePosition(Tile other)
    {
      return point.Equals(other.point);
    }

    public override string ToString()
    {
      return Symbol + " " + dungeonNodeIndex + " [" + point.x + "," + point.y + "]" + " " + GetHashCode();
    }

    public double DistanceFrom(Tile other)
    {
      var dPowered = (Math.Pow(point.x - other.point.x, 2) + Math.Pow(point.y - other.point.y, 2));
      return Math.Sqrt(dPowered);
    }
  }
}
