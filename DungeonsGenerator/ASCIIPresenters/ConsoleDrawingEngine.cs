using System;

namespace Dungeons.ASCIIPresenters
{
  public class ConsoleDrawingEngine : IDrawingEngine
  {
    public bool CursorVisible { set { Console.CursorVisible = value; } }
    public ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }

    public void SetCursorPosition(int x, int y)
    {
      Console.SetCursorPosition(x, y);
    }

    public void Write(char v)
    {
      Console.Write(v);
    }

    public void Write(int v)
    {
      Console.Write(v);
    }

    public void Write(string line)
    {
      Console.Write(line);
    }

    public void WriteLine(string line)
    {
      Console.WriteLine(line);
    }
  }
}
