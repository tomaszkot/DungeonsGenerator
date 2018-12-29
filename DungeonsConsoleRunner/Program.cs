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

      public void Run()
      {
        ConsoleSetup.Init();
        Relaod();

        bool exit = false;
        while (!exit)
        {
          var key = Console.ReadKey();
          var input = key.Key;
          if (input == ConsoleKey.Escape)
            exit = true;
          if (input == ConsoleKey.R)
            Relaod();
        }
      }

      void Relaod()
      {
        generator.Run();
        Redraw();
      }

      private void Redraw()
      {
        Console.Clear();
        PrintUsage();
        generator.Level.Print();
        
      }

      private void PrintUsage()
      {
        Console.WriteLine(Environment.NewLine);
        Console.WriteLine(Environment.NewLine);
        Console.WriteLine("Usage:");
        Console.WriteLine("R - reload");
        Console.WriteLine("Esc - exit");
      }
    }

  }
}
