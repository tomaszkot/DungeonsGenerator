using Dungeons.Core;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dungeons
{
  struct AppendNodeInfo
  {
    public int nextX;
    public int nextY;
    public EntranceSide side;
    public EntranceSide? nextForcedSide;
  }

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
      AppendNodeInfo info = new AppendNodeInfo();
      info.side = EntranceSide.Right;
      float chanceForLevelTurn = 0.5f;
      EntranceSide? prevEntranceSide = null;

      for (int nodeIndex = 0; nodeIndex < mazeNodes.Count; nodeIndex++)
      {
        var infoNext = CalcNextValues(mazeNodes, info, chanceForLevelTurn, nodeIndex);
        if(nodeIndex < mazeNodes.Count-1)
          mazeNodes[nodeIndex].GenerateLayoutDoors(infoNext.side);

        EntranceSide? entranceSide = null;
        if (nodeIndex > 0)
        {
          if (prevEntranceSide == EntranceSide.Right)
            entranceSide = EntranceSide.Left;
          else if (prevEntranceSide == EntranceSide.Bottom)
            entranceSide = EntranceSide.Top;
          else
            Debug.Assert(false);
        }
        localLevel.AppendMaze(mazeNodes[nodeIndex], new Point(info.nextX, info.nextY), null, false, entranceSide);

        prevEntranceSide = infoNext.side;
        info = infoNext;
      }
    }

    private AppendNodeInfo CalcNextValues(List<DungeonNode> mazeNodes, AppendNodeInfo prevInfo, float chanceForLevelTurn, int nodeIndex)
    {
      AppendNodeInfo infoNext = prevInfo;

      if (prevInfo.nextForcedSide != null)
      {
        infoNext.side = prevInfo.nextForcedSide.Value;
        //nextForcedSide = null;
      }
      else
      {
        infoNext.side = RandHelper.GetRandomDouble() >= .5f ? EntranceSide.Bottom : EntranceSide.Right;
        if (nodeIndex > 0 && prevInfo.side == infoNext.side)
        {
          if (RandHelper.GetRandomDouble() >= chanceForLevelTurn)
            infoNext.side = prevInfo.side == EntranceSide.Bottom ? EntranceSide.Right : EntranceSide.Bottom;
        }
        chanceForLevelTurn -= 0.15f;
      }
      if (infoNext.side == EntranceSide.Bottom)
      {
        infoNext.nextY += mazeNodes[nodeIndex].Height - 1;

      }
      else if (infoNext.side == EntranceSide.Right)
      {
        infoNext.nextX += mazeNodes[nodeIndex].Width - 1;
      }

      return infoNext;
    }
  }
}
