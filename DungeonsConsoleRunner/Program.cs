using Dungeons;
using DungeonsConsoleRunner;
using System;
using System.Diagnostics;

namespace ConsoleDungeonsRunner
{
  class Program
  {
    static void Main(string[] args)
    {
      var controller = new GameController(new Generator());
      controller.Run();
    }
  }
}
