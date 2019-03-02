using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeons.ASCIIPresenters
{
  public class Item
  {
    int x;
    int y;

    public Item(int x, int y)
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


  public class ListPresenter : Item
  {
    string caption;
    char border = '-';
    int borderSize = 20;

    public List<ListItem> Lines { get ; set ; } = new List<ListItem>();

    public ListPresenter(string caption, int x, int y) : base(x, y)
    {
      this.caption = caption;
    }

    public virtual void Redraw(IDrawingEngine presenter)
    {
      presenter.SetCursorPosition(X, Y);
      DrawBorder(presenter);

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

    private void DrawBorder(IDrawingEngine presenter)
    {
      for (int i = 0; i < borderSize; i++)
        presenter.Write(border);
    }
  }
}
