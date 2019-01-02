using Dungeons.Core;
using Dungeons.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace Dungeons
{
  public interface INodePrinter
  {
    void Print(Tile tile, PrintInfo pi);
    void PrintNewLine();
  }

  public class PrintInfo
  {
    public bool PrintNodeIndexes = false;
  }

  public enum EntranceSide { Left, Right, Top, Bottom };
  public enum TileCorner { NorthWest, SouthWest, NorthEast, SouthEast }
  public enum TileNeighborhood { North, South, East, West }
  public enum Interior { T, L };//shape of the interior

  [XmlRoot("Node", Namespace = "DungeonNode")]
  [XmlInclude(typeof(Wall))]
  public class DungeonNode 
  {
    [XmlIgnore]
    [JsonIgnore]
    protected Tile[,] tiles;
    protected GenerationInfo generationInfo;
    protected static Random random;
    
    [XmlIgnore]
    [JsonIgnore]
    List<DungeonNode> childIslands = new List<DungeonNode>();

    //sides are borders of the dungeon node
    [XmlIgnore]
    [JsonIgnore]
    Dictionary<EntranceSide, List<Wall>> sides = new Dictionary<EntranceSide, List<Wall>>();

    /// <summary>
    /// Dungeons appended as child of this one.
    /// </summary>
    private List<DungeonNode> parts = new List<DungeonNode>();

    List<Door> doors = new List<Door>();

    [XmlIgnore]
    [JsonIgnore]
    public Tile[,] Tiles { get { return tiles; } }
    public int Width { get { return tiles.GetLength(1); } }
    public int Height { get { return tiles.GetLength(0); } }
    [XmlIgnore]
    [JsonIgnore]
    internal Dictionary<EntranceSide, List<Wall>> Sides { get { return sides; } }
    protected List<TileNeighborhood> allNeighborhoods = new List<TileNeighborhood> { TileNeighborhood.East, TileNeighborhood.West, TileNeighborhood.North, TileNeighborhood.South };
    public const int DefaultNodeIndex = 100;
    public const int ChildIslandNodeIndex = -1;
    int nodeIndex;
    DungeonNode parent;
    
    public event EventHandler<GenericEventArgs<Tile>> OnTileRevealed;
    NodeInteriorGenerator interiorGenerator;

    //ctors
    static DungeonNode()
    {
      random = new Random();
      
    }

    public DungeonNode() : this(10, 10, null, -100)
    {
    }

    public DungeonNode(int width = 10, int height = 10, GenerationInfo gi = null, 
                       int nodeIndex = DefaultNodeIndex, DungeonNode parent = null, bool generateContent = true)
      :this(null, gi, nodeIndex, parent)
    {
      tiles = new Tile[height, width];
      if (generateContent && generationInfo != null)
      {
        GenerateContent();
      }
    }

    public DungeonNode(Tile[,] tiles, GenerationInfo gi = null, int nodeIndex = DefaultNodeIndex, DungeonNode parent = null)
    {
      this.Parent = parent;
      this.NodeIndex = nodeIndex;

      this.generationInfo = gi;
      this.interiorGenerator = new NodeInteriorGenerator(this, generationInfo);
      this.tiles = tiles;
    }

    public Point? AppendMazeStartPoint { get; set; }

    public int NodeIndex
    {
      get
      {
        return nodeIndex;
      }

      set
      {
        nodeIndex = value;
      }
    }

    [XmlIgnore]
    [JsonIgnore]
    public DungeonNode Parent
    {
      get
      {
        return parent;
      }

      set
      {
        parent = value;
      }
    }

    [XmlIgnore]
    [JsonIgnore]
    public virtual List<DungeonNode> Parts
    {
      get
      {
        return parts;
      }
    }

    [XmlIgnore]
    [JsonIgnore]
    public List<DungeonNode> ChildIslands
    {
      get
      {
        return childIslands;
      }
    }

    //methods

    internal void GenerateLayoutDoors(EntranceSide side)
    {
      List<Wall> wall = sides[side];
      for (int i = 0; i < wall.Count; i++)
      {
        if (i % 2 == 0)
          CreateDoor(wall[i]);
      }
    }

    protected virtual void GenerateContent()
    {
      if(generationInfo.GenerateEmptyTiles)
        PlaceEmptyTiles();
      if (generationInfo.GenerateOuterWalls)
        GenerateOuterWalls();

      if (generationInfo.GenerateRandomInterior)
        interiorGenerator.GenerateRandomInterior();

      GenerateRandomStonesBlocks();
    }

    protected void GenerateRandomStonesBlocks()
    {
      interiorGenerator.GenerateRandomStonesBlocks();
    }
    
    public bool IsCornerWall(Wall wall)
    {
      var neibs = GetNeighborTiles(wall).Where(i => i is Wall).ToList();
      if (neibs.Count >= 3)
        return true;
      if (neibs.Count != 2)
        return false;
      if (neibs.Count(i => i.point.x == wall.point.x) == 2 ||
        neibs.Count(i => i.point.y == wall.point.y) == 2)
        return false;
      return true;

    }

    public List<T> GetNeighborTiles<T>(Tile tile) where T : Tile
    {
      return GetNeighborTiles(tile).Where(i => i != null && i.GetType() == typeof(T)).Cast<T>().ToList();
    }

    public List<Tile> GetNeighborTiles(Tile tile, bool incDiagonal = false)
    {
      var neibs = new List<Tile>();
      foreach (var i in allNeighborhoods)
      {
        var neib = GetNeighborTile(tile, i);
        neibs.Add(neib);
        if (incDiagonal && neib != null)
        {
          if (i == TileNeighborhood.North || i == TileNeighborhood.South)
          {
            neibs.Add(GetNeighborTile(neib, TileNeighborhood.East));
            neibs.Add(GetNeighborTile(neib, TileNeighborhood.West));
          }
          else
          {
            neibs.Add(GetNeighborTile(neib, TileNeighborhood.North));
            neibs.Add(GetNeighborTile(neib, TileNeighborhood.South));
          }
        }
      }


      return neibs;
    }


    public override string ToString()
    {
      return NodeIndex + " [" + Width + "," + Height + "]";
    }

    public void SetTilesNodeIndex()
    {
      //GameManager.Instance.Assert(NodeIndex >= 0); this crashes win 10 store app
      foreach (var tile in Tiles)
      {
        if (tile != null)
          tile.dungeonNodeIndex = NodeIndex;
      }
    }

    protected Tile GetNeighborTile(Tile tile, TileNeighborhood neighborhood)
    {
      Point pt = tile.point;
      switch (neighborhood)
      {
        case TileNeighborhood.North:
          pt.y -= 1;
          break;
        case TileNeighborhood.South:
          pt.y += 1;
          break;
        case TileNeighborhood.East:
          pt.x += 1;
          break;
        case TileNeighborhood.West:
          pt.x -= 1;
          break;
        default:
          break;
      }

      return GetTile(pt);
    }

    public List<Tile> GetEmptyTiles(GenerationConstraints constraints = null, bool lookInsidechildIslands = false)
    {
      var emptyTiles = new List<Tile>();
      DoGridAction((int col, int row) =>
      {
        if (tiles[row, col] != null && tiles[row, col].IsEmpty  //null can be outside the walls
        )
        {
          var pt = new Point(col, row);

          if (constraints == null || (constraints.IsInside(pt)))
            emptyTiles.Add(tiles[row, col]);
        }
      });
      return emptyTiles;
    }

    public Tile GetTile(Point point)
    {
      if (point.x < 0 || point.y < 0)
        return null;
      if (point.x >= Width || point.y >= Height)
        return null;
      return tiles[point.y, point.x];
    }

    public virtual bool SetTile(Tile tile, Point point, bool resetOldTile = true, bool revealReseted = true)
    {
      if (point.x < 0 || point.y < 0)
        return false;
      if (AppendMazeStartPoint != null)
      {
        point.x -= AppendMazeStartPoint.Value.x;
        point.y -= AppendMazeStartPoint.Value.y;
      }
      if (point.x >= Width || point.y >= Height)
        return false;

      if (tiles[point.y, point.x] == tile && tile.point == point)
        return true;

      if (tiles[point.y, point.x] != null)
      {
        var prev = tiles[point.y, point.x];
        if (tile != null && tile.dungeonNodeIndex < 0)
          tile.dungeonNodeIndex = prev.dungeonNodeIndex;
      }

      tiles[point.y, point.x] = tile;

      if (tile != null)
      {
        if (tile.dungeonNodeIndex > DungeonNode.ChildIslandNodeIndex)//do not touch islands
          SetDungeonNodeIndex(tile);
        if (resetOldTile)
        {
          //reset old tile
          if (tile.IsAtValidPoint && (tile.point != point) && Width > tile.point.x && Height > tile.point.y)
          {
            var emp = GenerateEmptyTile();
            emp.dungeonNodeIndex = tile.dungeonNodeIndex;//preserve;
            SetTile(emp, tile.point);
            if (revealReseted)
              emp.Revealed = true;//if hero goes out of the tile it must be revealed
            if (OnTileRevealed != null)
              OnTileRevealed(this, new GenericEventArgs<Tile>(emp));
          }
        }

        tile.point = point;
        return true;
        //if (OnTileRevealed != null)
        //  OnTileRevealed(this, new GenericEventArgs<Tile>(tile));
      }

      return false;
    }
    
    protected virtual void SetDungeonNodeIndex(Tile tile)
    {
      tile.dungeonNodeIndex = this.NodeIndex;
    }
    
    Point GetInteriorStartingPoint(int minSizeReduce = 6, DungeonNode child = null)
    {
      return interiorGenerator.GetInteriorStartingPoint(minSizeReduce, child);
    }

    public virtual Tile CreateDoor(Tile original)
    {
      if (generationInfo.ChildIsland)
      {
        Debug.Assert(generationInfo.EntrancesCount > 0);
      }
      else
        Debug.Assert(generationInfo.EntrancesCount == 0);
      var door = CreateDoorInstance();
      bool doorSet = SetTile(door, original.point);
      Debug.Assert(doorSet);
      door.dungeonNodeIndex = original.dungeonNodeIndex;
      Doors.Add(door);
      return door;
    }

    protected virtual Door CreateDoorInstance()
    {
      return new Door();
    }
    
    public virtual DungeonNode CreateChildIslandInstance(int w, int h, GenerationInfo gi, DungeonNode parent)
    {
      return new DungeonNode(w, h, gi, parent: this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="childMaze"></param>
    /// <param name="destStartPoint"></param>
    /// <param name="maxSize"></param>
    /// <param name="childIsland"></param>
    /// <param name="entranceSideToSkip"></param>
    public virtual void AppendMaze(DungeonNode childMaze, Point? destStartPoint = null, Point? maxSize = null, bool childIsland = false,
      EntranceSide? entranceSideToSkip = null)
    {
      childMaze.AppendedSide = entranceSideToSkip;
      Parts.Add(childMaze);

      var start = destStartPoint ?? GetInteriorStartingPoint(4, childMaze);
      if (maxSize == null)
        maxSize = new Point(childMaze.Width, childMaze.Height);

      childMaze.AppendMazeStartPoint = start;
      if (childIsland)
      {
        childMaze.NodeIndex = ChildIslands.Count * -1;
      }
      for (int row = 0; row < maxSize.Value.y; row++)
      {
        for (int col = 0; col < maxSize.Value.x; col++)
        {
          var tile = childMaze.tiles[row, col];
          if (tile == null)
            continue;
          if (entranceSideToSkip != null && childMaze.Sides[entranceSideToSkip.Value].Contains(tile as Wall))
            continue;
          SetCorner(maxSize, row, col, tile);
          int destCol = col + start.x;
          int destRow = row + start.y;
          tile.point = new Point(destCol, destRow);

          if (childIsland)
            tile.dungeonNodeIndex = childMaze.NodeIndex;

          this.tiles[destRow, destCol] = tile;
        }
      }
    }

 
    private static void SetCorner(Point? maxSize, int row, int col, Tile tile)
    {
      if ((col == 1 && row == 1)
          || (col == maxSize.Value.x - 2 && (row == 1 || row == maxSize.Value.y - 2))
          || (col == 1 && row == maxSize.Value.y - 2)
      )
      {
        if (col == 1 && row == 1)
          tile.corner = TileCorner.SouthWest;
        else if (col == 1 && row == maxSize.Value.y - 2)
          tile.corner = TileCorner.NorthWest;
        else if (col == maxSize.Value.x - 2 && row == maxSize.Value.y - 2)
          tile.corner = TileCorner.NorthEast;
        else
          tile.corner = TileCorner.SouthEast;
      }
    }

    public Tuple<int, int> GetMaxXY()
    {
      int maxX = 0;
      int maxY = 0;
      for (int row = 0; row < Height; row++)
      {
        for (int col = 0; col < Width; col++)
        {
          var tile = tiles[row, col];
          if (tile != null && tile.point.x > maxX)
            maxX = col;
          if (tile != null && tile.point.y > maxY)
            maxY = row;
        }
      }
      return new Tuple<int, int>(maxX, maxY);
    }

    protected void GenerateOuterWalls()
    {
      interiorGenerator.GenerateOuterWalls();
    }

    private void PlaceEmptyTiles()
    {
      DoGridAction((int col, int row) =>
      {
        if (tiles[row, col] == null)
          tiles[row, col] = GenerateEmptyTile(new Point(col, row));
      });
    }

    public bool HasTile(Tile tile)
    {
      bool has = false;
      DoGridFunc((int col, int row) =>
      {
        if (tiles[row, col] == tile)
        {
          has = true;
          return true;//break loop
        }
        return false;
      });

      return has;
    }

    public void DoGridFunc(Func<int, int, bool> func)
    {
      bool cancel = false;
      for (int row = 0; row < Height; row++)
      {
        if (cancel)
          break;
        for (int col = 0; col < Width; col++)
        {
          cancel = func(col, row);
          if (cancel)
            break;
        }
      }
    }
    public void DoGridAction(Action<int, int> ac)
    {
      for (int row = 0; row < Height; row++)
      {
        for (int col = 0; col < Width; col++)
        {
          ac(col, row);
        }
      }
    }
    
    internal Tile GenerateEntrance(List<Wall> points)
    {
      return interiorGenerator.GenerateEntrance(points);
    }

    public T Generate<T>() where T : class, new()
    {
      var instance = new T();
      return instance;
    }

    public T GenerateAtPosition<T>(Point pt) where T : class, new()
    {
      var instance = Generate<T>();
      SetTile(instance as Tile, pt);
      return instance;
    }

    public void SetEmptyTile(Point pt)
    {
      SetTile(GenerateEmptyTile(pt), pt);
    }

    public Tile GenerateEmptyTile()
    {
      return GenerateEmptyTile(Point.Invalid);
    }

    public Tile GenerateEmptyTile(Point pt)
    {
      var tile = new Tile(pt, Constants.SymbolBackground);
      tile.dungeonNodeIndex = NodeIndex;
      return tile;
    }

    public List<Door> Doors
    {
      get
      {
        return doors;
      }
    }

    public EntranceSide? AppendedSide { get; private set; }

    public virtual void Print(PrintInfo pi = null)
    {
      if (pi == null)
        pi = new PrintInfo();
      var ap = new AsciiPrinter(this);
      ap.Print(ap, pi);
    }

    /// <summary>
    /// Delete unreachable doors 
    /// </summary>
    internal void DeleteWrongDoors()
    {
      List<Tile> toDel = new List<Tile>();
      foreach (Tile tile in Tiles)
      {
        if (tile is Door)
        {
          var neibs = GetNeighborTiles(tile);
          if (neibs.Where(i => i is Wall).Count() >= 3)
            toDel.Add(tile);
        }
      }
      if (toDel.Any())
      {
        for (int i = 0; i < toDel.Count; i++)
        {
          this.SetTile(new Wall(), toDel[i].point);
        }
      }
    }

    public void Reveal(bool reveal)
    {
      DoGridAction((int col, int row) =>
      {
        if (tiles[row, col] != null)
        {
          tiles[row, col].Revealed = reveal;
        }
      });
    }

  }
}
