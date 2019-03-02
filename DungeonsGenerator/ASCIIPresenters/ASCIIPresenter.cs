using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeons.ASCIIPresenters
{
  public interface IPresenter
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
