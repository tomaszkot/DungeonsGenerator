using Dungeons;
using System;

namespace ConsoleDungeonsRunner
{
  class Program
  {
    static void Main(string[] args)
    {
      ConsoleSetup.Init();

      var generator = new Generator();
      var level = generator.Run();
      level.Print();

      Console.ReadKey();
    }
  }
}
