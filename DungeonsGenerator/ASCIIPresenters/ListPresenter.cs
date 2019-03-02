using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeons.ASCIIPresenters
{
  public class Presenter
  {
    int x;
    int y;

    public Presenter(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }

    public int X { get => x; set => x = value; }
    public int Y { get => y; set => y = value; }
  }

  public class ListItem
  {
    public ConsoleColor Color = ConsoleColor.White;
    public string Text;
  }


  public class ListPresenter : Presenter
  {
    List<ListItem> lines;
    string caption;
    char border = '-';
    int borderSize = 20;

    public List<ListItem> Lines { get => lines; set => lines = value; }

    public ListPresenter(string caption, int x, int y) : base(x, y)
    {
      this.caption = caption;
    }

    public virtual void Redraw(IPresenter presenter)
    {
      presenter.SetCursorPosition(X, Y);
      for (int i = 0; i < borderSize; i++)
        presenter.Write(border);

      presenter.WriteLine("");
      presenter.ForegroundColor = ConsoleColor.Cyan;
      presenter.WriteLine(caption);
      presenter.ForegroundColor = ConsoleColor.White;
      for (int i = 0; i < borderSize; i++)
        presenter.Write(border);

      presenter.WriteLine("");
      foreach (var line in Lines)
      {
        presenter.ForegroundColor = line.Color;
        presenter.WriteLine(line.Text);
      }
      for (int i = 0; i < borderSize; i++)
        presenter.Write(border);
    }
  }
}
