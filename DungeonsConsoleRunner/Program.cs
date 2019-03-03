using Dungeons;
using Dungeons.ASCIIDisplay;
using DungeonsConsoleRunner;
using SimpleInjector;
using System;
using System.Diagnostics;

namespace ConsoleDungeonsRunner
{
  class Program
  {
    static Container container = new Container();

    static void Main(string[] args)
    {
      container.Register<GameController, GameController>();
      container.Register<IGameGenerator, Generator>();
      container.Register<IDrawingEngine, ConsoleDrawingEngine>();
      container.Register<Screen, Screen>();
      container.Verify();

      var controller = container.GetInstance<GameController>();
      controller.Run();
    }
  }
}
