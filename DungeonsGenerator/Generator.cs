using Dungeons.Core;
using Dungeons.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeons
{
  public class Generator
  {
    static protected Random random;
    protected List<DungeonNode> nodes;
    int levelCounter;

    static Generator()
    {
      random = new Random();
    }

    public DungeonNode Level
    {
      get;
      private set;
    }

    public virtual DungeonNode Run()
    {
      return Generate(levelCounter++);
    }

    Tile GetPossibleDoorTile(List<Tile> listOne, List<Tile> listTwo)
    {
      var common = listOne.SelectMany(x => listTwo.Where(y => y.IsAtSamePosition(x))).ToList();
      int doorIndex = random.Next(common.Count);
      if (doorIndex == 0)
        doorIndex++;
      if (doorIndex == common.Count - 1)
        doorIndex--;
      return common[doorIndex];
    }

    DungeonNode CreateNode(int index)
    {
      GenerationInfo gi = CreateNodeGenerationInfo();
      return CreateNode(index, gi);
    }

    protected virtual DungeonNode CreateNode(int index, GenerationInfo gi)
    {
      var minNodeSize = index == 0 && gi.FirstNodeSmaller ? gi.MinMazeNodeSize - gi.MinMazeNodeSize / 2 : gi.MinMazeNodeSize;
      var maxNodeSize = index == 0 && gi.FirstNodeSmaller ? gi.MaxMazeNodeSize - gi.MaxMazeNodeSize / 2 : gi.MaxMazeNodeSize;

      return CreateNode(random.Next(minNodeSize, maxNodeSize), random.Next(minNodeSize, maxNodeSize), gi, index);
    }

    protected virtual DungeonNode CreateNode(int w, int h, GenerationInfo gi, int index)
    {
      return new DungeonNode(w, h, gi, index);
    }

    public virtual int NumberOfNodes
    {
      get
      {
        return GenerationInfo.NumberOfNodes;
      }
    }

    protected virtual List<DungeonNode> CreateDungeonNodes()
    {
      nodes = new List<DungeonNode>();
      var gi = this.CreateLevelGenerationInfo();
      for (int i = 0; i < NumberOfNodes; i++)
      {
        var node = CreateNode(i);
        node.Reveal(true);
        nodes.Add(node);
      }
      return nodes;
    }

    protected virtual GenerationInfo CreateNodeGenerationInfo()
    {
      return new GenerationInfo();
    }

    protected virtual DungeonNode CreateLevel(int levelIndex, int w, int h, GenerationInfo gi)
    {
      return new DungeonNode(w, h, gi);
    }

    protected virtual GenerationInfo CreateLevelGenerationInfo()
    {
      var gi = new GenerationInfo();
      gi.GenerateOuterWalls = false;

      return gi;
    }

    /// <summary>
    /// Generates a dungeon 
    /// </summary>
    /// <param name="mazeNodes"></param>
    /// <returns></returns>
    protected virtual DungeonNode Generate(int levelIndex)
    {
      var gi = CreateLevelGenerationInfo();
      var localLevel = CreateLevel(levelIndex, 60, 60, gi);
      localLevel.Reveal(false);

      var mazeNodes = CreateDungeonNodes();

      LayoutNodes(localLevel, mazeNodes);

      var max = localLevel.GetMaxXY();
      Level = CreateLevel(levelIndex, max.First + 1, max.Second + 1, gi);
      Level.AppendMaze(localLevel, new Point(0, 0), new Point(max.First + 1, max.Second + 1));
      Level.DeleteWrongDoors();

      return Level;
    }

    protected virtual void LayoutNodes(DungeonNode localLevel, List<DungeonNode> mazeNodes)
    {
      int nextX = 0;
      int nextY = 0;
      EntranceSide side = EntranceSide.Right;
      float chanceForLevelTurn = 0.5f;
      EntranceSide? nextForcedSide = null;
      for (int i = 0; i < mazeNodes.Count; i++)
      {
        localLevel.AppendMaze(mazeNodes[i], new Point(nextX, nextY), null, false, side);
        var prevSide = side;
        if (nextForcedSide != null)
        {
          side = nextForcedSide.Value;
          nextForcedSide = null;
        }
        else
        {
          side = random.NextDouble() >= .5f ? EntranceSide.Bottom : EntranceSide.Right;
          if (i > 0 && prevSide == side)
          {
            if (random.NextDouble() >= chanceForLevelTurn)
              side = prevSide == EntranceSide.Bottom ? EntranceSide.Right : EntranceSide.Bottom;
          }
          chanceForLevelTurn -= 0.15f;
        }
        if (side == EntranceSide.Bottom)
          nextY += mazeNodes[i].Height - 1;
        else if (side == EntranceSide.Right)
          nextX += mazeNodes[i].Width - 1;
      }
    }
  }
}
