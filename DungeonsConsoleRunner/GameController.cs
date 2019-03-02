using Dungeons;
using Dungeons.ASCIIPresenters;
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
    int dungeonX;
    int dungeonY;

    public virtual DungeonNode Dungeon { get; set; }
    public DungeonPresenter DungeonPresenter { get; set; }
    public int DungeonX { get => dungeonX; set => dungeonX = value; }
    public int DungeonY { get => dungeonY; set => dungeonY = value; }
    public IDrawingEngine Presenter { get; set; } = new ConsoleDrawingEngine();

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
        var key = Console.ReadKey(true);
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
      Dungeon = generator.Generate();
      DungeonPresenter = new DungeonPresenter(Dungeon, Presenter, DungeonX, DungeonY = 10);
      Redraw();
    }

    protected virtual void Redraw()
    {
      Console.Clear();
      PrintUsage();
      if (Dungeon != null)
      {
        PrintDungeonDesc();
        DungeonPresenter.Redraw(printInfo);
      }
    }

    private void PrintDungeonDesc()
    {
      Presenter.WriteLine("");
      Presenter.WriteLine(Dungeon.Description);
    }

    protected void PrintUsage()
    {
      Presenter.WriteLine("--");
      Presenter.WriteLine("Usage:");
      Presenter.WriteLine("R - reload");
      Presenter.WriteLine("D - toggle node_indexes/symbols");
      Presenter.WriteLine("Esc - exit");
      Presenter.WriteLine("--");
    }
  }
}
