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

  public class Label : Item
  {
    public ConsoleColor Color = ConsoleColor.White;
    public string Text;

    public Label(int x, int y) : base(x, y) { }
    public Label(int x, int y, string txt) : this(x, y)
    {
      Text = txt;
    }

    public virtual void Redraw(IDrawingEngine presenter)
    {
      WriteLine(Text);
    }
  }

  public class LabelContent 
  {
    public ConsoleColor Color = ConsoleColor.White;
    public string Text;

    public LabelContent() { }
    public LabelContent(string txt)
    {
      Text = txt;
    }
  }

  public class ListItem : LabelContent
  {
    public ListItem()
    {
    }

    public ListItem(string txt)
    {
      Text = txt;
    }
  }
  
  public class ListPresenter : Item
  {
    string caption;
    char border = '-';
    int width = 25;

    public List<ListItem> Items { get ; set ; } = new List<ListItem>();
    public string Caption { get => caption; set => caption = value; }

    public ListPresenter(string caption, int x, int y, int width) : base(x, y)
    {
      this.Caption = caption;
      this.width = width;
    }

    public int TotalHeight
    {
      get
      {
        return Items.Count +
               2 + //2 - borders over caption, 
               1 + //1 - caption,
               1 ; //1 - bottom border
      }

    }

    public virtual void Redraw(IDrawingEngine presenter)
    {
      Presenter = presenter;
      Reset();
      //presenter.SetCursorPosition(OriginPositionX, OriginPositionY);
      DrawBorder(presenter);
      presenter.ForegroundColor = ConsoleColor.Cyan;
      WriteLine(Caption);
      DrawBorder(presenter);

      foreach (var line in Items)
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
      for (int i = 0; i < width; i++)
        line += border;
      WriteLine(line);
    }
  }
}
