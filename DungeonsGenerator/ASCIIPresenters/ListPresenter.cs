using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeons.ASCIIPresenters
{
  public abstract class Item
  {
    IDrawingEngine drawingEngine;
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
    public IDrawingEngine DrawingEngine { get => drawingEngine; set => drawingEngine = value; }

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
      DrawingEngine.WriteLine(line + " " + debug);
      CurrentX = OriginPositionX;
      CurrentY++;
      UpdatePresenterPos();
    }

    private void UpdatePresenterPos()
    {
      DrawingEngine.SetCursorPosition(CurrentX, CurrentY);
    }

    public abstract void Redraw(IDrawingEngine drawingEngine);

    public abstract int TotalHeight
    {
      get;
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

    public override int TotalHeight => 1;

    public override void Redraw(IDrawingEngine drawingEngine)
    {
      this.DrawingEngine = drawingEngine;
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

    public override int TotalHeight
    {
      get
      {
        return Items.Count +
               2 + //2 - borders over caption, 
               1 + //1 - caption,
               1 ; //1 - bottom border
      }

    }

    public override void Redraw(IDrawingEngine drawingEngine)
    {
      DrawingEngine = drawingEngine;
      Reset();
      DrawBorder(drawingEngine);
      drawingEngine.ForegroundColor = ConsoleColor.Cyan;
      WriteLine(Caption);
      DrawBorder(drawingEngine);

      foreach (var line in Items)
      {
        drawingEngine.ForegroundColor = line.Color;
        WriteLine(line.Text);
      }

      DrawBorder(drawingEngine);

      //WriteLine("-");
    }

    private void DrawBorder(IDrawingEngine drawingEngine)
    {
      drawingEngine.ForegroundColor = ConsoleColor.White;
      var line = "";
      for (int i = 0; i < width; i++)
        line += border;
      WriteLine(line);
    }
  }
}
