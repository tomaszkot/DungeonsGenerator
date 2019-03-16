using System;

namespace Dungeons
{
  /// <summary>
  /// Info describing creation of the dungeon node. Members public to enhance speed.
  /// </summary>
  public class GenerationInfo : ICloneable
  {
    public const int NumberOfNodes = 2;

    /// <summary>
    /// Normally true, can be set to false for issue testing purposes
    /// </summary>
    public bool CreateDoors = true;

    public int EntrancesCount = 0;
    public bool ChildIsland;
    public bool GenerateOuterWalls = true;
    public bool GenerateRandomInterior = true;
    public bool PreferChildIslandInterior = true;
    public bool FirstNodeSmaller = false;
    public bool GenerateRandomStonesBlocks = true;

    public int MinNodeSize = 12;
    public int MaxNodeSize = 12;

    public readonly int MinSubMazeNodeSize = 5;
    public readonly int MinSimpleInteriorSize = 3;
    public int MinRoomLeft = 6;
    public int NumberOfChildIslands = 1;
    public bool ChildIslandAllowed = true;
    internal bool GenerateEmptyTiles = true;
    public bool GenerateDoors = true;
    public bool RevealTiles { get; set; } = true;

    public GenerationInfo()
    {
    }

    public object Clone()
    {
      return this.MemberwiseClone() as GenerationInfo;
    }
  }
}
