using System;

namespace Dungeons.ASCIIDisplay
{
  public class ConsoleDrawingEngine : IDrawingEngine
  {
    public bool CursorVisible { set { Console.CursorVisible = value; } }
    public ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }

    public int WindowWidth => Console.WindowWidth;

    public void SetCursorPosition(int x, int y)
    {
      Console.SetCursorPosition(x, y);
    }

    public Tuple<int, int> GetCursorPosition()
    {
      return new Tuple<int, int>(Console.CursorLeft, Console.CursorTop);
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

    public void Clear()
    {
      Console.Clear();
    }
  }
}
