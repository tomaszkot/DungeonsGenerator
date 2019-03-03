using Dungeons.ASCIIDisplay;
using System;

namespace Dungeons.ASCIIDisplay.Presenters
{
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
      base.Reset();
      WriteLine(Text);
    }
  }
}
