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
      var minNodeSize = index == 0 && gi.FirstNodeSmaller ? gi.MinNodeSize - gi.MinNodeSize / 2 : gi.MinNodeSize;
      var maxNodeSize = index == 0 && gi.FirstNodeSmaller ? gi.MaxNodeSize - gi.MaxNodeSize / 2 : gi.MaxNodeSize;

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


    //TODO public
    public virtual List<DungeonNode> CreateDungeonNodes()
    {
      nodes = new List<DungeonNode>();
      var gi = this.CreateLevelGenerationInfo();
      //gi.GenerateOuterWalls = true;
      for (int i = 0; i < NumberOfNodes; i++)
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
    protected virtual DungeonNode Generate(int levelIndex)
    {
      var mazeNodes = CreateDungeonNodes();
      var layouter = new DefaultNodeLayouter();
      Level = layouter.DoLayout(mazeNodes);
      
      return Level;
    }

    
  }
}
