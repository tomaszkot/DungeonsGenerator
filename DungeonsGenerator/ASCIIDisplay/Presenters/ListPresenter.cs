using Dungeons.ASCIIDisplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeons.ASCIIDisplay.Presenters
{
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
    char border = '-';
    int width = 25;

    public List<ListItem> Items { get ; set ; } = new List<ListItem>();
    public string Caption { get; set; }

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
