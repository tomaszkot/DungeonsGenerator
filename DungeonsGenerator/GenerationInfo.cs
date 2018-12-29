using System;

namespace Dungeons
{
  /// <summary>
  /// Info describing creation of the dungeon node. Members public to enhance speed.
  /// </summary>
  public class GenerationInfo : ICloneable
  {
    public const int NumberOfNodes = 5;

    /// <summary>
    /// Normally true, can be set to false for issue testing purposes
    /// </summary>
    public bool CreateDoors = true;

    public int EntrancesCount = 0;
    public bool ChildIsland;
    public bool GenerateOuterWalls = true;
    public bool GenerateRandomInterior = true;
    public bool PreferChildIslandInterior = false;
    public bool FirstNodeSmaller = false;
    public bool GenerateRandomStonesBlocks = true;

    public int MinNodeSize = 15;
    public int MaxNodeSize = 15;

    public readonly int MinSubMazeNodeSize = 5;
    public readonly int MinSimpleInteriorSize = 3;
    public int MinRoomLeft = 6;
    public int NumberOfChildIslands = 1;
    public bool ChildIslandAllowed = true;

    public GenerationInfo()
    {
    }

    public object Clone()
    {
      return this.MemberwiseClone() as GenerationInfo;
    }
  }
}
