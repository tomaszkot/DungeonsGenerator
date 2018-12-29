using Dungeons.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeons
{
  class DefaultNodeLayouter
  {
    public DungeonNode DoLayout(List<DungeonNode> nodes)
    {
      var gi = new GenerationInfo();
      var localLevel = new DungeonNode(60, 60, gi);
      localLevel.Reveal(false);

      LayoutNodes(localLevel, nodes);

      var max = localLevel.GetMaxXY();
      var Level = new DungeonNode(max.First + 1, max.Second + 1, gi);
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
          side = RandHelper.GetRandomDouble() >= .5f ? EntranceSide.Bottom : EntranceSide.Right;
          if (i > 0 && prevSide == side)
          {
            if (RandHelper.GetRandomDouble() >= chanceForLevelTurn)
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
