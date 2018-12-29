using Dungeons;
using System;
using System.Diagnostics;

namespace ConsoleDungeonsRunner
{
  class Program
  {

    static void Main(string[] args)
    {

      var runner = new Runner();
      runner.Run();
    }

    class Runner
    {
      Generator generator = new Generator();
      PrintInfo printInfo = new PrintInfo();

      public void Run()
      {
        ConsoleSetup.Init();
        Reload();

        bool exit = false;
        while (!exit)
        {
          var key = Console.ReadKey();
          var input = key.Key;
          if (input == ConsoleKey.Escape)
            exit = true;
          if (input == ConsoleKey.R)
            Reload();
          if (input == ConsoleKey.D)
          {
            printInfo.PrintNodeIndexes = !printInfo.PrintNodeIndexes;
            Redraw();
          }
        }
      }

      void Reload()
      {
        generator.Run();
        Redraw();
      }

      
      private void Redraw()
      {
        Console.Clear();
        PrintUsage();
        if (generator.Level != null)
          generator.Level.Print(printInfo);
        else
        {
          //var nodes = generator.CreateDungeonNodes();
          //foreach (var node in nodes)
          //  node.Print(printInfo);
        }
        
      }

      private void PrintUsage()
      {
        //Console.WriteLine(Environment.NewLine);
        //Console.WriteLine(Environment.NewLine);
        Console.WriteLine("Usage:");
        Console.WriteLine("R - reload");
        Console.WriteLine("Esc - exit");
      }
    }

  }
}
