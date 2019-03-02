using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeons.ASCIIPresenters
{
  public class Item
  {
    IDrawingEngine presenter;
    public int CurrentX { get; set; }
    public int CurrentY { get; set; }

    public Item(int x, int y)
    {
      this.OriginPositionX = x;
      this.OriginPositionY = y;
      CurrentX = OriginPositionX;
      CurrentY = OriginPositionY;
    }

    public int OriginPositionX { get; set; }
    public int OriginPositionY { get; set; }
    public IDrawingEngine Presenter { get => presenter; set => presenter = value; }

    protected void Reset()
    {
      CurrentX = OriginPositionX;
      CurrentY = OriginPositionY;
      UpdatePresenterPos();
    }

    protected void WriteLine(string line)
    {
      UpdatePresenterPos();
      var debug = "";// " (at " + CurrentX + ", " + CurrentY;
      Presenter.WriteLine(line + " " + debug);
      CurrentX = OriginPositionX;
      CurrentY++;
      UpdatePresenterPos();
    }

    private void UpdatePresenterPos()
    {
      Presenter.SetCursorPosition(CurrentX, CurrentY);
    }
  }

  public class ListItem
  {
    public ConsoleColor Color = ConsoleColor.White;
    public string Text;

    public ListItem() { }
    public ListItem(string txt)
    {
      Text = txt;
    }
  }


  public class ListPresenter : Item
  {
    string caption;
    char border = '-';
    int borderSize = 25;

    public List<ListItem> Lines { get ; set ; } = new List<ListItem>();

    public ListPresenter(string caption, int x, int y) : base(x, y)
    {
      this.caption = caption;
    }

    public virtual void Redraw(IDrawingEngine presenter)
    {
      Presenter = presenter;
      Reset();
      //presenter.SetCursorPosition(OriginPositionX, OriginPositionY);
      DrawBorder(presenter);
      presenter.ForegroundColor = ConsoleColor.Cyan;
      WriteLine(caption);
      DrawBorder(presenter);

      foreach (var line in Lines)
      {
        presenter.ForegroundColor = line.Color;
        WriteLine(line.Text);
      }

      DrawBorder(presenter);

      //WriteLine("-");
    }

    private void DrawBorder(IDrawingEngine presenter)
    {
      presenter.ForegroundColor = ConsoleColor.White;
      var line = "";
      for (int i = 0; i < borderSize; i++)
        line += border;
      WriteLine(line);
    }
  }
}
