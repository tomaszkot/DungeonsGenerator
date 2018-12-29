using Dungeons.Core;
using Dungeons.Tiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Dungeons
{
  public interface INodePrinter
  {
    void Print(Tile tile);
    void PrintNewLine();
  }

  public enum EntranceSide { Left, Right, Top, Bottom };
  public enum TileCorner { NorthWest, SouthWest, NorthEast, SouthEast }
  public enum TileNeighborhood { North, South, East, West }

  [XmlRoot("Node", Namespace = "DungeonNode")]
  [XmlInclude(typeof(Wall))]
  public class DungeonNode : INodePrinter
  {
    [XmlIgnore]
    [JsonIgnore]
    protected Tile[,] tiles;
    protected GenerationInfo generationInfo;
    protected static Random random;
    public enum Interior { T, L };
    [XmlIgnore]
    [JsonIgnore]
    List<DungeonNode> childIslands = new List<DungeonNode>();

    //sides are borders of the dungeon
    [XmlIgnore]
    [JsonIgnore]
    Dictionary<EntranceSide, List<Tile>> sides = new Dictionary<EntranceSide, List<Tile>>();

    //dungeons appended as child of this one
    private List<DungeonNode> parts = new List<DungeonNode>();
    List<Door> doors = new List<Door>();

    [XmlIgnore]
    [JsonIgnore]
    public Tile[,] Tiles { get { return tiles; } }
    public int Width { get { return tiles.GetLength(1); } }
    public int Height { get { return tiles.GetLength(0); } }
    [XmlIgnore]
    [JsonIgnore]
    internal Dictionary<EntranceSide, List<Tile>> Sides { get { return sides; } }


    protected List<TileNeighborhood> allNeighborhoods = new List<TileNeighborhood> { TileNeighborhood.East, TileNeighborhood.West, TileNeighborhood.North, TileNeighborhood.South };
    public const int DefaultNodeIndex = -100;
    public const int ChildIslandNodeIndex = -2;
    int nodeIndex = DefaultNodeIndex;
    DungeonNode parent;
    public static int ChildIslandNodeIndexCounter;

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


    static DungeonNode()
    {
      random = new Random();
      ChildIslandNodeIndexCounter = ChildIslandNodeIndex;
    }

    public DungeonNode() : this(10, 10, null, -1)
    {
    }
    public DungeonNode(int width = 10, int height = 10, GenerationInfo gi = null, int nodeIndex = -1, DungeonNode parent = null)
    {
      this.Parent = parent;
      if (parent != null)
        this.NodeIndex = parent.NodeIndex * 10;
      //Console.WriteLine("DungeonNode ctor width ="+ width);
      this.NodeIndex = nodeIndex;
      if (gi != null)
        this.generationInfo = gi;

      tiles = new Tile[height, width];
      //var wi = tiles.GetLength(0);
      //var he = tiles.GetLength(1);
      if (generationInfo != null)
      {
        GenerateContent();
      }
    }

    protected virtual void GenerateContent()
    {
      if (generationInfo.generateOuterWalls)
        GenerateOuterWalls();

      //GenerateDragonJar();
      if (generationInfo.generateRandomInterior)
        GenerateRandomInterior();

      GenerateRandomStonesBlocks();
    }

    protected void GenerateRandomStonesBlocks()
    {
      if (generationInfo.GenerateRandomStonesBlocks)
      {
        int maxDec = (Width + Height) / 4;
        //int numDec = generationInfo.DecorationsCount != null ? generationInfo.DecorationsCount.Value : random.Next(3, maxDec > 3 ? maxDec : 3);
        int numDec = random.Next(3, maxDec > 3 ? maxDec : 3);
        for (int i = 0; i < numDec; i++)
          AddFinishingDecorations();
      }
    }

    public DungeonNode(Tile[,] tiles, GenerationInfo gi = null, int nodeIndex = -1, DungeonNode parent = null)
    {
      this.Parent = parent;
      if (parent != null)
        this.NodeIndex = parent.NodeIndex * 10;
      //Console.WriteLine("DungeonNode ctor width ="+ width);
      this.NodeIndex = nodeIndex;
      if (gi != null)
        this.generationInfo = gi;
      this.tiles = tiles;
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
      //allNeighborhoods.ForEach(
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
        return false;
      });
      return emptyTiles;
    }

    private void GenerateRandomInterior()
    {
      Interior? interior = null;
      var rand = random.NextDouble();
      if (generationInfo.ChildIslandAllowed && (generationInfo.preferChildIslandInterior || rand < .33))
      {
        var island = GenerateChildIslands();
        if (island == null)
        {
          interior = GenerateRandomSimpleInterior(true);
        }
      }
      else
        interior = GenerateRandomSimpleInterior();
    }

    Interior? GenerateRandomSimpleInterior(bool addFinishingDecorations = false)
    {
      Interior? interior = null;
      if (Width - generationInfo.MinSimpleInteriorSize > 4
        && Height - generationInfo.MinSimpleInteriorSize > 4)
      {
        interior = Interior.T;//CommonRandHelper.GetRandomEnumValue<Interior>();
        GenerateInterior(interior.Value);
      }
      else
        addFinishingDecorations = true;
      if (addFinishingDecorations)
      {
        AddFinishingDecorations();
      }

      return interior;
    }

    private void AddFinishingDecorations()
    {
      Func<Tile, bool> areAllEmpty = (Tile i) => { return GetNeighborTiles(i, true).All(j => j != null && j.IsEmpty); };

      var empty = GetEmptyTiles().Where(i => areAllEmpty(i)).ToList();
      if (empty.Any())
      {
        var t = empty[random.Next(empty.Count())];
        var pts = new List<Point>() { t.point };

        var others = GetNeighborTiles(t).Where(i => areAllEmpty(i)).ToList();
        if (others.Any())
        {
          int maxDecLen = 6;
          int max = random.Next(1, maxDecLen);
          for (int i = 0; i < max && i < others.Count; i++)
            pts.Add(others[i].point);
        }

        AddTiles(pts);
      }
    }

    internal void GenerateInterior(Interior interior)
    {
      List<Point> points = new List<Point>();
      var startPoint = GetInteriorStartingPoint();
      if (interior == Interior.T)
      {
        int endX = Width - startPoint.x;
        points = GenerateWallPoints(startPoint.x, endX, startPoint.y, startPoint.y + 1, 1);

        int legX = (startPoint.x + endX) / 2;
        var pointsY = new List<Point>();
        pointsY.AddRange(GenerateWallPoints(legX, legX + 1, startPoint.y, Height - startPoint.y));

        if (pointsY.Count > 6)
          pointsY.RemoveAt(pointsY.Count / 2);
        points.AddRange(pointsY);

      }
      else if (interior == Interior.L)
      {
        int legX = startPoint.x;
        //vertical
        points.AddRange(GenerateWallPoints(legX, legX + 1, startPoint.y, Height - startPoint.y));
        if (points.Count > 6)
          points.RemoveAt(points.Count / 2);

        int endX = Width - startPoint.x;
        int endY = Height - startPoint.y;
        //horiz
        points.AddRange(GenerateWallPoints(legX, endX, endY - 1, endY));
      }
      AddTiles(points);
      AddFinishingDecorations();
    }


    public void Split(bool vertically)
    {
      AddSplitWall(vertically);
    }

    List<Tile> AddTiles(List<Point> points)
    {
      List<Tile> tiles = new List<Tile>();
      foreach (var pt in points)
      {
        var wall = new Wall();
        SetTile(wall, pt);
        tiles.Add(wall);
      }

      return tiles;
    }

    private void AddSplitWall(bool vertically, int entrancesCount = 1)
    {
      List<Point> points = new List<Point>();
      if (vertically)
      {
        int x = this.Width / 2;
        points = GenerateWallPoints(x, x + 1, 1, Height, 1);
      }
      var tiles = AddTiles(points);
      GenerateEntrance(tiles);
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
    public event EventHandler<GenericEventArgs<Tile>> OnTileRevealed;
    protected virtual void SetDungeonNodeIndex(Tile tile)
    {
      tile.dungeonNodeIndex = this.NodeIndex;
    }

    List<Point> GenerateWallPoints(int startX, int endX, int startY, int endY, int entrancesCount = 0)
    {
      var wall = new List<Point>();
      for (int row = startY; row < endY; row++)
      {
        for (int col = startX; col < endX; col++)
        {
          wall.Add(new Point(col, row));
        }
      }
      return wall;
    }

    Point GetInteriorStartingPoint(int minSizeReduce = 6, DungeonNode child = null)
    {

      int islandWidth = child != null ? child.Width : (this.Width - minSizeReduce);

      int islandHeight = child != null ? child.Height : (this.Height - minSizeReduce);

      int xMiddle = Width / 2;
      int yMiddle = Height / 2;

      int xIsland = xMiddle - islandWidth / 2;
      int yIsland = yMiddle - islandHeight / 2;

      var sp = new Point(xIsland, yIsland);
      return sp;
    }

    public virtual Tile CreateDoor(Tile original)
    {
      var door = CreateDoorInstance();
      SetTile(door, original.point);
      door.dungeonNodeIndex = original.dungeonNodeIndex;
      Doors.Add(door);
      return door;
    }

    protected virtual Door CreateDoorInstance()
    {
      return new Door();
    }

    internal DungeonNode[] GenerateChildIslands()
    {
      List<DungeonNode> nodes = new List<DungeonNode>();
      var roomLeft = Width - generationInfo.MinSubMazeNodeSize * generationInfo.NumberOfChildIslands;
      if (roomLeft < generationInfo.MinRoomLeft)
        return null;
      roomLeft = Height - generationInfo.MinSubMazeNodeSize * generationInfo.NumberOfChildIslands;
      if (roomLeft < generationInfo.MinRoomLeft)
        return null;
      int islandWidth = this.Width - generationInfo.MinRoomLeft;// * generationInfo.NumberOfChildIslands;
      if (generationInfo.NumberOfChildIslands > 1)
        islandWidth -= 2;//TODO
      int islandHeight = this.Height / generationInfo.NumberOfChildIslands - generationInfo.MinRoomLeft;// * generationInfo.NumberOfChildIslands;

      var xRandRange = islandWidth - generationInfo.MinSubMazeNodeSize;
      if (xRandRange > 0)
        islandWidth -= random.Next(xRandRange);

      if (islandWidth < generationInfo.MinSubMazeNodeSize)
        islandWidth = generationInfo.MinSubMazeNodeSize;
      if (islandHeight < generationInfo.MinSubMazeNodeSize)
        islandHeight = generationInfo.MinSubMazeNodeSize;

      var generationInfoIsl = generationInfo.Clone() as GenerationInfo;//TODO
      generationInfoIsl.entrancesCount = 4;
      Point? destStartPoint = null;
      if (generationInfo.NumberOfChildIslands > 1)
        destStartPoint = new Point(generationInfo.MinRoomLeft / 2 + 1, generationInfo.MinRoomLeft / 2);
      for (int i = 0; i < generationInfo.NumberOfChildIslands; i++)
      {
        var child = CreateChildIslandInstance(islandWidth, islandHeight, generationInfoIsl, parent: this);
        AppendMaze(child, destStartPoint, childIsland: true);
        ChildIslands.Add(child);
        nodes.Add(child);

        if (destStartPoint != null)
        {
          var nextPoint = new Point();
          nextPoint.x = destStartPoint.Value.x;// + islandWidth + 1;
          nextPoint.y = destStartPoint.Value.y + islandHeight + 1;
          destStartPoint = nextPoint;
        }
      }

      return nodes.ToArray();
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
    /// <param name="entranceSide"></param>
    /// <param name="maxDoors">can be used to limit doors number, used for secret room</param>
    public virtual void AppendMaze(DungeonNode childMaze, Point? destStartPoint = null, Point? maxSize = null, bool childIsland = false,
      EntranceSide? entranceSide = null, int maxDoors = -1)
    {
      childMaze.AppendedSide = entranceSide;
      Parts.Add(childMaze);

      var start = destStartPoint ?? GetInteriorStartingPoint(4, childMaze);
      if (maxSize == null)
        maxSize = new Point(childMaze.Width, childMaze.Height);

      childMaze.AppendMazeStartPoint = start;
      SetAppendedNodeIndex(childMaze);
      int createdDoorsNumber = 0;
      for (int row = 0; row < maxSize.Value.y; row++)
      {
        for (int col = 0; col < maxSize.Value.x; col++)
        {
          var tile = childMaze.tiles[row, col];
          if (tile == null)
            continue;
          SetCorner(maxSize, row, col, tile);
          int destCol = col + start.x;
          int destRow = row + start.y;
          tile.point = new Point(destCol, destRow);

          if (childIsland)
            tile.dungeonNodeIndex = ChildIslandNodeIndexCounter;
          var tileToSet = tile;

          var createDoors = (maxDoors < 0 || maxDoors > createdDoorsNumber);
          if (createDoors && entranceSide != null)
          {
            var prevTile = this.tiles[destRow, destCol];
            var door = CreateDoors(maxSize, entranceSide, row, col, prevTile, tileToSet);
            if (door != null)
            {
              tileToSet = door;
              createdDoorsNumber++;
            }
          }
          this.tiles[destRow, destCol] = tileToSet;
        }
      }

      if (childIsland)
        ChildIslandNodeIndexCounter--;
    }

    protected virtual void SetAppendedNodeIndex(DungeonNode childMaze)
    {
      childMaze.NodeIndex = ChildIslandNodeIndexCounter;
    }

    protected virtual Tile CreateDoors(Point? maxSize, EntranceSide? entranceSide, int row, int col, Tile prevTile, Tile tileToSet)
    {
      if (prevTile is Wall && tileToSet is Wall)
      {
        //GameManager.Instance.Assert((prevTile as Wall).IsSide && (tileToSet as Wall).IsSide);
        ////Debug.WriteLine("tileToSet side = "+ tileToSet);
        if (
          ((col % 2 == 0 && col > 1 && col < maxSize.Value.x - 1 && entranceSide.Value == EntranceSide.Bottom) ||
          (row % 2 == 0 && row > 1 && row < maxSize.Value.y - 1 && entranceSide.Value == EntranceSide.Right))
          )
        {
          return CreateDoor(tileToSet);
        }
      }

      return null;
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

    internal Tuple<EntranceSide, Tile> GenerateEntranceAtRandomSide(EntranceSide[] skip = null)
    {
      EntranceSide side = RandHelper.GetRandomEnumValue<EntranceSide>(skip);
      return GenerateEntranceAtSide(side);
    }

    internal Tuple<EntranceSide, Tile> GenerateEntranceAtSide(EntranceSide side)
    {
      var tile = GenerateEntrance(Sides[side]);
      Tuple<EntranceSide, Tile> res = new Tuple<EntranceSide, Tile>(side, tile);
      return res;
    }

    protected void GenerateOuterWalls()
    {
      var topPoints = GenerateWallPoints(0, Width, 0, 1, 0);
      var bottomPoints = GenerateWallPoints(0, Width, Height - 1, Height, 0);
      var leftPoints = GenerateWallPoints(0, 1, 0, Height, 0);
      var rightPoints = GenerateWallPoints(Width - 1, Width, 0, Height, 0);
      Sides.Add(EntranceSide.Top, AddTiles(topPoints));
      Sides.Add(EntranceSide.Bottom, AddTiles(bottomPoints));
      Sides.Add(EntranceSide.Left, AddTiles(leftPoints));
      Sides.Add(EntranceSide.Right, AddTiles(rightPoints));

      foreach (var side in Sides.Values)
      {
        foreach (var si in side)
          (si as Wall).IsSide = true;
      }

      DoGridAction((int col, int row) =>
      {
        if (tiles[row, col] == null)
          tiles[row, col] = GenerateEmptyTile(new Point(col, row));
        return false;
      });

      List<EntranceSide> generated = new List<EntranceSide>();
      for (int i = 0; i < generationInfo.entrancesCount; i++)
      {
        var entr = GenerateEntranceAtRandomSide(generated.ToArray());
        if (entr.Second != null)
        {
          generated.Add(entr.First);
          CreateDoor(entr.Second);
        }
      }
    }

    public bool HasTile(Tile tile)
    {
      bool has = false;
      DoGridAction((int col, int row) =>
      {
        if (tiles[row, col] == tile)
        {
          has = true;
          return true;
        }
        return false;
      });

      return has;
    }

    public void DoGridAction(Func<int, int, bool> ac)
    {
      bool cancel = false;
      for (int row = 0; row < Height; row++)
      {
        if (cancel)
          break;
        for (int col = 0; col < Width; col++)
        {
          cancel = ac(col, row);
          if (cancel)
            break;
        }
      }
    }

    internal Tile GenerateEntrance(List<Tile> points)
    {
      int index = random.Next(points.Count - 2);
      if (index == 0)
        index++;//avoid corner
      var pt = points[index].point;
      var entry = new Tile();
      SetTile(entry, pt);
      return entry;
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

    public void PrintNewLine()
    {
      Console.Write('\n');
    }

    public static bool PrintNodeIndexes { get; set; }

    public List<Door> Doors
    {
      get
      {
        return doors;
      }
    }

    public EntranceSide? AppendedSide { get; private set; }

    public void Print(Tile tile)
    {
      var color = ConsoleColor.White;
      var symbol = ' ';
      if (tile != null)
      {
        color = tile.color;
        if (PrintNodeIndexes)
        {
          Console.ForegroundColor = color;
          Console.Write(tile.dungeonNodeIndex);
          return;
        }
        if (tile.Revealed)
        {
          symbol = tile.Symbol;

        }
      }
      Console.ForegroundColor = color;
      Console.Write(symbol);
    }

    //public Point consoleStartPos = new Point(0 , 0);
    public void Print(INodePrinter p)
    {
      //consoleStartPos.x = Console.CursorLeft;
      //consoleStartPos.y = Console.CursorTop;


      for (int row = 0; row < Height; row++)
      {
        p.PrintNewLine();
        for (int col = 0; col < Width; col++)
        {
          var tile = tiles[row, col];
          p.Print(tile);
        }
      }
    }

    public virtual void Print()
    {
      Print(this);
    }

    internal void DeleteWrongDoors()
    {
      List<Tile> toDel = new List<Tile>();
      foreach (Tile tile in Tiles)
      {
        if (tile is Door)
        {
          var neibs = GetNeighborTiles(tile);
          if (neibs.Where(i => i is Wall).Count() == 3)
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
        return false;
      });
    }

  }
}
