using System;

namespace Dungeons
{
  public class GenerationInfo : ICloneable
  {
    //members public to enhance speed
    public int entrancesCount = 0;
    public bool generateOuterWalls = true;
    public bool generateRandomInterior = true;
    public bool preferChildIslandInterior = false;
    public bool firstNodeSmaller = false;
    public bool GenerateRandomStonesBlocks = true;

    public int MinMazeNodeSize = 15;
    public int MaxMazeNodeSize = 15;

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
