using Dungeons.Core;
using Dungeons.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeons
{
  public interface IGameGenerator
  {
    DungeonNode Generate();
  }

  public class Generator : IGameGenerator
  {
    static protected Random random;
    protected List<DungeonNode> nodes;
    int levelCounter;

    static Generator()
    {
      random = new Random();
    }

    public virtual DungeonNode Generate()
    {
      return Generate<DungeonNode>(levelCounter++);
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
      var minNodeSize = index == 0 && gi.FirstNodeSmaller ? gi.MinNodeSize - gi.MinNodeSize / 2 : gi.MinNodeSize;
      var maxNodeSize = index == 0 && gi.FirstNodeSmaller ? gi.MaxNodeSize - gi.MaxNodeSize / 2 : gi.MaxNodeSize;

      var width = random.Next(minNodeSize, maxNodeSize);
      var height = random.Next(minNodeSize, maxNodeSize);

      return CreateNode(width, height , gi, index);
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
    
    //TODO public
    public virtual List<DungeonNode> CreateDungeonNodes()
    {
      nodes = new List<DungeonNode>();
      var gi = this.CreateLevelGenerationInfo();
      //gi.GenerateOuterWalls = true;
      //for (int i = 0; i < NumberOfNodes; i++)
      for (int i = 0; i < 3; i++)
      {
        var node = CreateNode(i, gi);
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
      //gi.GenerateOuterWalls = false;

      return gi;
    }

    /// <summary>
    /// Generates a dungeon 
    /// </summary>
    /// <param name="mazeNodes"></param>
    /// <returns></returns>
    protected virtual T Generate<T>(int levelIndex) where T : DungeonNode, new()
    {
      var mazeNodes = CreateDungeonNodes();
      var layouter = new DefaultNodeLayouter();
      T level = layouter.DoLayout<T>(mazeNodes);
      return level;
    }

    
  }
}
