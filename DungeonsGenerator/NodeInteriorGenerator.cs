using Dungeons.Core;
using Dungeons.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Dungeons
{
  class NodeInteriorGenerator
  {
    DungeonNode dn;
    GenerationInfo generationInfo;

    public int Width { get { return dn.Width; } }
    public int Height { get { return dn.Height; } }

    public NodeInteriorGenerator(DungeonNode dn, GenerationInfo gi)
    {
      this.dn = dn;
      this.generationInfo = gi;
    }

    public void GenerateRandomInterior()
    {
      if (!Inited())
        return;

      Interior? interior = null;
      var rand = RandHelper.GetRandomDouble();
      if (generationInfo.ChildIslandAllowed && (generationInfo.PreferChildIslandInterior || rand < .33))
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

    bool Inited()
    {
      return generationInfo != null && dn != null;
    }

    Interior? GenerateRandomSimpleInterior(bool addFinishingDecorations = false)
    {
      Interior? interior = null;
      if (Width - generationInfo.MinSimpleInteriorSize > 4
        && Height - generationInfo.MinSimpleInteriorSize > 4)
      {
        interior = RandHelper.GetRandomEnumValue<Interior>();
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

    void AddSplitWall(bool vertically, int entrancesCount = 1)
    {
      List<Point> points = new List<Point>();
      if (vertically)
      {
        int x = this.Width / 2;
        points = GenerateWallPoints(x, x + 1, 1, Height, 1);
      }
      var tiles = AddWalls(points);
      GenerateEntrance(tiles);
    }

    internal Tile GenerateEntrance(List<Wall> points)
    {
      if (!Inited())
        return null;
      int index = RandHelper.Random.Next(points.Count - 2);
      if (index == 0)
        index++;//avoid corner
      var pt = points[index].point;
      var entry = new Tile();
      dn.SetTile(entry, pt);
      return entry;
    }

    public void GenerateOuterWalls()
    {
      if (!Inited())
        return;

      var topPoints = GenerateWallPoints(0, Width, 0, 1, 0);
      var bottomPoints = GenerateWallPoints(0, Width, Height - 1, Height, 0);
      var leftPoints = GenerateWallPoints(0, 1, 0, Height, 0);
      var rightPoints = GenerateWallPoints(Width - 1, Width, 0, Height, 0);
      dn.Sides.Add(EntranceSide.Top, AddWalls(topPoints));
      dn.Sides.Add(EntranceSide.Bottom, AddWalls(bottomPoints));
      dn.Sides.Add(EntranceSide.Left, AddWalls(leftPoints));
      dn.Sides.Add(EntranceSide.Right, AddWalls(rightPoints));

      foreach (var side in dn.Sides.Values)
      {
        foreach (var si in side)
          (si as Wall).IsSide = true;
      }

      List<EntranceSide> generated = new List<EntranceSide>();
      for (int i = 0; i < generationInfo.EntrancesCount; i++)
      {
        var entr = GenerateEntranceAtRandomSide(generated.ToArray());
        if (entr.Second != null)
        {
          generated.Add(entr.First);
          dn.CreateDoor(entr.Second);
        }
      }
    }

    Tuple<EntranceSide, Tile> GenerateEntranceAtRandomSide(EntranceSide[] skip = null)
    {
      EntranceSide side = RandHelper.GetRandomEnumValue<EntranceSide>(skip);
      return GenerateEntranceAtSide(side);
    }

    Tuple<EntranceSide, Tile> GenerateEntranceAtSide(EntranceSide side)
    {
      var tile = GenerateEntrance(dn.Sides[side]);
      Tuple<EntranceSide, Tile> res = new Tuple<EntranceSide, Tile>(side, tile);
      return res;
    }

    void Split(bool vertically)
    {
      AddSplitWall(vertically);
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

    public Point GetInteriorStartingPoint(int minSizeReduce = 6, DungeonNode child = null)
    {
      if (!Inited())
        return GenerationConstraints.InvalidPoint;
      int islandWidth = child != null ? child.Width : (this.Width - minSizeReduce);

      int islandHeight = child != null ? child.Height : (this.Height - minSizeReduce);

      int xMiddle = Width / 2;
      int yMiddle = Height / 2;

      int xIsland = xMiddle - islandWidth / 2;
      int yIsland = yMiddle - islandHeight / 2;

      var sp = new Point(xIsland, yIsland);
      return sp;
    }

    void GenerateInterior(Interior interior)
    {
      List<Point> points = new List<Point>();
      var startPoint = GetInteriorStartingPoint();
      if (interior == Interior.T)
      {
        int endX = Width - startPoint.X;
        points = GenerateWallPoints(startPoint.X, endX, startPoint.Y, startPoint.Y + 1, 1);

        int legX = (startPoint.X + endX) / 2;
        var pointsY = new List<Point>();
        pointsY.AddRange(GenerateWallPoints(legX, legX + 1, startPoint.Y, Height - startPoint.Y));

        if (pointsY.Count > 6)
          pointsY.RemoveAt(pointsY.Count / 2);
        points.AddRange(pointsY);

      }
      else if (interior == Interior.L)
      {
        int legX = startPoint.X;
        //vertical
        points.AddRange(GenerateWallPoints(legX, legX + 1, startPoint.Y, Height - startPoint.Y));
        if (points.Count > 6)
          points.RemoveAt(points.Count / 2);

        int endX = Width - startPoint.X;
        int endY = Height - startPoint.Y;
        //horiz
        points.AddRange(GenerateWallPoints(legX, endX, endY - 1, endY));
      }
      AddWalls(points);
      AddFinishingDecorations();
    }

    public void AddFinishingDecorations()
    {
      if (!Inited())
        return;
      Func<Tile, bool> areAllEmpty = (Tile i) => { return dn.GetNeighborTiles(i, true).All(j => j != null && j.IsEmpty); };

      var empty = dn.GetEmptyTiles().Where(i => areAllEmpty(i)).ToList();
      if (empty.Any())
      {
        var t = empty[RandHelper.Random.Next(empty.Count())];
        var pts = new List<Point>() { t.point };

        var others = dn.GetNeighborTiles(t).Where(i => areAllEmpty(i)).ToList();
        if (others.Any())
        {
          int maxDecLen = 6;
          int max = RandHelper.Random.Next(1, maxDecLen);
          for (int i = 0; i < max && i < others.Count; i++)
            pts.Add(others[i].point);
        }

        AddWalls(pts);
      }
    }

    List<Wall> AddWalls(List<Point> points)
    {
      var tiles = new List<Wall>();
      foreach (var pt in points)
      {
        var wall = dn.CreateWall();
        dn.SetTile(wall, pt);
        tiles.Add(wall);
      }

      return tiles;
    }
    
    internal DungeonNode[] GenerateChildIslands()
    {
      if (!Inited())
        return null;

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
        islandWidth -= RandHelper.Random.Next(xRandRange);

      if (islandWidth < generationInfo.MinSubMazeNodeSize)
        islandWidth = generationInfo.MinSubMazeNodeSize;
      if (islandHeight < generationInfo.MinSubMazeNodeSize)
        islandHeight = generationInfo.MinSubMazeNodeSize;

      var generationInfoIsl = generationInfo.Clone() as GenerationInfo;//TODO
      generationInfoIsl.EntrancesCount = 4;
      generationInfoIsl.ChildIsland = true;
      Point? destStartPoint = null;
      if (generationInfo.NumberOfChildIslands > 1)
        destStartPoint = new Point(generationInfo.MinRoomLeft / 2 + 1, generationInfo.MinRoomLeft / 2);
      for (int i = 0; i < generationInfo.NumberOfChildIslands; i++)
      {
        var child = dn.CreateChildIslandInstance(islandWidth, islandHeight, generationInfoIsl, parent: dn);
        dn.AppendMaze(child, destStartPoint, childIsland: true);
        dn.ChildIslands.Add(child);
        nodes.Add(child);

        if (destStartPoint != null)
        {
          var nextPoint = new Point();
          nextPoint.X = destStartPoint.Value.X;
          nextPoint.Y = destStartPoint.Value.Y + islandHeight + 1;
          destStartPoint = nextPoint;
        }
      }

      return nodes.ToArray();
    }

    public void GenerateRandomStonesBlocks()
    {
      if (!Inited())
        return;
      if (generationInfo.GenerateRandomStonesBlocks)
      {
        int maxDec = (Width + Height) / 4;
        //int numDec = generationInfo.DecorationsCount != null ? generationInfo.DecorationsCount.Value : random.Next(3, maxDec > 3 ? maxDec : 3);
        int numDec = RandHelper.Random.Next(3, maxDec > 3 ? maxDec : 3);
        for (int i = 0; i < numDec; i++)
          AddFinishingDecorations();
      }
    }
  }
}
