using System;

namespace Dungeons.ASCIIPresenters
{
  //basic interface for ASCII displaying
  public interface IDrawingEngine
  {
    void WriteLine(string line);
    void Write(char v);
    void Write(int v);
    void Write(string line);

    ConsoleColor ForegroundColor { get; set; }
    void SetCursorPosition(int x, int y);
    bool CursorVisible { set; }
  }
}
