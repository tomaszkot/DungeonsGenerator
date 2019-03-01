using Dungeons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonsConsoleRunner
{
  public class GameController
  {
    IGameGenerator generator = new Generator();
    PrintInfo printInfo = new PrintInfo();

    public virtual DungeonNode Node { get; set; }

    public GameController(IGameGenerator generator)
    {
      this.generator = generator;
    }

    public void Run()
    {
      ConsoleSetup.Init();
      Generate();

      bool exit = false;
      while (!exit)
      {
        var key = Console.ReadKey();
        exit = HandleKey(key);
      }
    }

    protected virtual void Generate()
    {
      Reload();
    }

    protected virtual bool HandleKey(ConsoleKeyInfo key)
    {
      bool exit = false;
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

      return exit;
    }

    protected void Reload()
    {
      Node = generator.Generate();
      Redraw();
    }

    protected virtual void Redraw()
    {
      Console.Clear();
      PrintUsage();
      if (Node != null)
      {
        Console.WriteLine("");
        Console.WriteLine(Node.Description);
        Node.Print(printInfo);
      }
    }

    private void PrintUsage()
    {
      //Console.WriteLine(Environment.NewLine);
      //Console.WriteLine(Environment.NewLine);
      Console.WriteLine("Usage:");
      Console.WriteLine("R - reload");
      Console.WriteLine("D - toggle node_indexes/symbols");
      Console.WriteLine("Esc - exit");
    }
  }
}
