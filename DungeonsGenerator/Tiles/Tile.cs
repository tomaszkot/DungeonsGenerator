using Dungeons.Core;
using System;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;

namespace Dungeons.Tiles
{
  public class Constants
  {
    public const int MinNormalNodeIndex = 0;

    public const char SymbolBackground = '.';
    public const char SymbolDoor = '+';
    public const char SymbolWall = '#';
  }

  [XmlInclude(typeof(Wall))]
  [Serializable]
  public class Tile
  {
    //members public for speed purposes
    public Point Point;

    private char symbol = Constants.SymbolBackground;
    public string name;
    public ConsoleColor color = ConsoleColor.White;

    /// <summary>
    /// The index of the node (room) the tile belongs to
    /// </summary>
    public int dungeonNodeIndex = Constants.MinNormalNodeIndex - 1;

    /// <summary>
    /// If the tile is at node's corner this member says which corner it is.
    /// </summary>
    public TileCorner? corner;

    /// <summary>
    /// If false the tile is not visible. The revealed flag shall be typically set to true when a door leading to room are opened.
    /// </summary>
    bool revealed;


    public Tile() : this(Constants.SymbolBackground)
    {
    }

    public Tile(char symbol) : this(GenerationConstraints.InvalidPoint, symbol)
    {

    }

    public Tile(Point point, char symbol)
    {
      this.Name = GetType().Name;
      this.Point = point;
      this.Symbol = symbol;
      this.revealed = true;
    }

    public bool Revealed
    {
      get { return revealed; }
      set
      {
        revealed = value;
      }
    }

    public bool IsAtValidPoint
    {
      get { return Point != GenerationConstraints.InvalidPoint; }
    }

    public bool IsEmpty { get { return Symbol == Constants.SymbolBackground; } }

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
      get { return dungeonNodeIndex < Constants.MinNormalNodeIndex; }
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

    public ConsoleColor Color { get => color; set => color = value; }

    public bool IsAtSamePosition(Tile other)
    {
      return Point.Equals(other.Point);
    }

    public override string ToString()
    {
      return Symbol + " " + dungeonNodeIndex + " [" + Point.X + "," + Point.Y + "]" + " " + GetHashCode();
    }

    public double DistanceFrom(Tile other)
    {
      var dPowered = (Math.Pow(Point.X - other.Point.X, 2) + Math.Pow(Point.Y - other.Point.Y, 2));
      return Math.Sqrt(dPowered);
    }
  }
}
