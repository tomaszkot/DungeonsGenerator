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
    public int OriginX = 2;
    public int OriginY = 2;

    ListPresenter usagePresenter;

    public virtual DungeonNode Dungeon { get; set; }
    public DungeonPresenter DungeonPresenter { get; set; }
    public int DungeonX { get ; set ; }
    public int DungeonY { get; set; } = 10;
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
      DungeonPresenter = new DungeonPresenter(Dungeon, Presenter, OriginX+ DungeonX, OriginY + DungeonY);
      usagePresenter = new ListPresenter("Usage", OriginX, OriginY);
           
      usagePresenter.Lines.Add(new ListItem("R - reload"));
      usagePresenter.Lines.Add(new ListItem("D - toggle node_indexes/symbols"));
      usagePresenter.Lines.Add(new ListItem("Esc - exit"));
      Redraw();
    }

    protected virtual void Redraw()
    {
      Console.Clear();
      usagePresenter.Redraw(Presenter);
      if (Dungeon != null)
      {
        PrintDungeonDesc();
        DungeonPresenter.Redraw(printInfo);
      }
    }

    private void PrintDungeonDesc()
    {
      //Presenter.WriteLine("");
      Presenter.SetCursorPosition(OriginX, Presenter.GetCursorPosition().Item2);
      Presenter.WriteLine(Dungeon.Description);
      Presenter.WriteLine("");
    }

  }
}
