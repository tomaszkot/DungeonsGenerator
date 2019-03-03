using Dungeons;
using Dungeons.ASCIIPresenters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonsConsoleRunner
{
  public class GameController
  {
    IGameGenerator generator = new Generator();

    public virtual DungeonNode Dungeon { get; set; }
    public IDrawingEngine Presenter { get; set; } = new ConsoleDrawingEngine();
    protected Screen screen;

    public GameController(IGameGenerator generator)
    {
      this.generator = generator;
    }

    protected virtual Screen CreateScreen()
    {
      return new Screen(Presenter);
    }

    public void Run()
    {
      ConsoleSetup.Init();
      Generate();
      Redraw();

      bool exit = false;
      while (!exit)
      {
        var key = Console.ReadKey(true);
        exit = HandleKey(key);
      }
    }

    protected virtual void Generate()
    {
      Dungeon = generator.Generate();
      
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
        screen.PrintInfo.PrintNodeIndexes = !screen.PrintInfo.PrintNodeIndexes;
        Redraw();
      }

      return exit;
    }

    protected void Reload()
    {
      Generate();
      Redraw();
    }

    protected virtual void Redraw()
    {
      if (screen == null)
      {
        screen = CreateScreen();
      }
      screen.Redraw(Dungeon);
    }
  }
}
